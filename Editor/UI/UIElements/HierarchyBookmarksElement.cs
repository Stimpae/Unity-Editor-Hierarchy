using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using PopupWindow = UnityEditor.PopupWindow;

namespace Pastime.Hierarchy.GUI
{
    public class HierarchyBookmarksElement : VisualElement, IDisposable
    {
        // UI elements
        private ScrollView m_scrollView;
        private GameObject m_currentHighlighted;
        
        // Style classes
        private const string k_bookmarkItemClass = "hierarchy-bookmark-item";
        private const string k_bookmarkItemSelectedClass = "hierarchy-bookmark-item-selected";
        private const string k_bookmarksPanelClass = "hierarchy-bookmark-panel";
        
        // Callback for when a bookmark is selected
        public event Action<GameObject> OnBookmarkSelected;

        public HierarchyBookmarksElement()
        {
            // Load the stylesheet
            styleSheets.Add(Resources.Load<StyleSheet>("HierarchyBookmarks"));
            
            // Setup styles
            AddToClassList(k_bookmarksPanelClass);
            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 1;
            
            // Create scroll view for bookmarks with horizontal scrolling only
            m_scrollView = new ScrollView(ScrollViewMode.Horizontal);
            m_scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            m_scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            m_scrollView.AddToClassList("hierarchy-bookmark-panel");
            m_scrollView.style.flexGrow = 1;
            m_scrollView.contentContainer.style.flexDirection = FlexDirection.Row;
            m_scrollView.contentContainer.style.flexWrap = Wrap.NoWrap;
            
            Add(m_scrollView);
            
            // Register callbacks for drag and drop
            RegisterCallback<DragEnterEvent>(OnDragEnter);
            RegisterCallback<DragLeaveEvent>(OnDragLeave);
            RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
            
            // Register mouse wheel event for horizontal scrolling
            RegisterCallback<WheelEvent>(OnMouseWheel);
            
            // Refresh the UI
            RefreshBookmarksList();
            
            // Register for scene changes to clean up deleted objects and update UI
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosed += OnSceneClosed;
        }

        // Handle mouse wheel for horizontal scrolling
        private void OnMouseWheel(WheelEvent evt)
        {
            // Convert vertical scroll to horizontal
            m_scrollView.scrollOffset = new Vector2(
                m_scrollView.scrollOffset.x + evt.delta.y * 20f,
                m_scrollView.scrollOffset.y);
            
            // Prevent the event from bubbling up
            evt.StopPropagation();
        }

        // Called when the hierarchy changes - refreshes the UI
        private void OnHierarchyChanged()
        {
            // Cleanup missing bookmarks
            HierarchyBookmarksData.instance.CleanupMissingBookmarks();
            
            // Refresh the UI
            RefreshBookmarksList();
        }
        
        // Called when a scene is opened
        private void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            // Delay the refresh until next frame to avoid issues during scene transitions
            EditorApplication.delayCall += () => {
                if (this != null) {
                    RefreshBookmarksList();
                }
            };
        }
        
        // Called when a scene is closed
        private void OnSceneClosed(UnityEngine.SceneManagement.Scene scene)
        {
            // Delay the refresh until next frame to avoid issues during scene transitions
            EditorApplication.delayCall += () => {
                if (this != null) {
                    RefreshBookmarksList();
                }
            };
        }

        // Handle drag enter
        private void OnDragEnter(DragEnterEvent evt)
        {
            AddToClassList("hierarchy-bookmarks-drop-active");
        }

        // Handle drag leave
        private void OnDragLeave(DragLeaveEvent evt)
        {
            RemoveFromClassList("hierarchy-bookmarks-drop-active");
        }

        // Handle drag updated
        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            // Only accept GameObjects from the hierarchy
            bool isValidDrag = DragAndDrop.objectReferences
                .Any(obj => obj is GameObject go && !EditorUtility.IsPersistent(go));
            
            DragAndDrop.visualMode = isValidDrag ? 
                DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
            
            evt.StopPropagation();
        }

        // Handle drop
        private void OnDragPerform(DragPerformEvent evt)
        {
            RemoveFromClassList("hierarchy-bookmarks-drop-active");
            
            // Accept the drag
            DragAndDrop.AcceptDrag();
            
            // Process dropped objects
            foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
            {
                // Only accept scene objects (not prefabs or assets)
                if (draggedObject is GameObject go && !EditorUtility.IsPersistent(go))
                {
                    // Add to bookmarks
                    HierarchyBookmarksData.instance.AddBookmark(go);
                }
            }
            
            // Refresh the visual list
            RefreshBookmarksList();
            
            evt.StopPropagation();
        }

        // Create a visual element for a bookmark
        private VisualElement CreateBookmarkItem(HierarchyBookmarksData.BookmarkData bookmark)
        {
            // Resolve the bookmark to a GameObject
            GameObject gameObject = HierarchyBookmarksData.instance.ResolveBookmark(bookmark);
            if (gameObject == null) return null;
            
            // Create the bookmark item using the dedicated class
            var item = new HierarchyBookmarkItem(bookmark, gameObject);
            
            // Set selected state if this is the current highlighted object
            if (m_currentHighlighted == gameObject)
            {
                item.SetSelected(true);
            }
            
            // Register event handlers
            item.OnSelected += SelectBookmark;
            item.OnContextMenu += ShowContextMenu;
            
            return item;
        }

        // Show context menu for a bookmark item
        private void ShowContextMenu(HierarchyBookmarksData.BookmarkData bookmark)
        {
            GenericMenu menu = new GenericMenu();
            
            // Add menu item to rename bookmark
            menu.AddItem(new GUIContent("Rename..."), false, () => {
                RenameBookmarkDialog(bookmark);
            });
            
            // Add menu item to remove from bookmarks
            menu.AddItem(new GUIContent("Remove from Bookmarks"), false, () => {
                RemoveBookmark(bookmark);
            });
            
            // Add menu item to ping/find in hierarchy
            menu.AddItem(new GUIContent("Find in Hierarchy"), false, () => {
                GameObject go = HierarchyBookmarksData.instance.ResolveBookmark(bookmark);
                if (go != null)
                {
                    EditorGUIUtility.PingObject(go);
                }
            });
            
            menu.ShowAsContext();
        }
        
        // Show rename dialog
        private void RenameBookmarkDialog(HierarchyBookmarksData.BookmarkData bookmark)
        {
            // Create popup field for renaming
            string currentName = bookmark.customName;
            PopupWindowContent content = new BookmarkRenamePopup(currentName, (newName) => {
                if (!string.IsNullOrEmpty(newName) && newName != currentName)
                {
                    HierarchyBookmarksData.instance.RenameBookmark(bookmark.stableID, newName);
                    RefreshBookmarksList();
                }
            });
            
            // Show popup
            PopupWindow.Show(new Rect(Event.current.mousePosition, Vector2.zero), content);
        }

        // Remove a bookmark
        private void RemoveBookmark(HierarchyBookmarksData.BookmarkData bookmark)
        {
            GameObject go = HierarchyBookmarksData.instance.ResolveBookmark(bookmark);
            
            HierarchyBookmarksData.instance.RemoveBookmark(bookmark.stableID);
            RefreshBookmarksList();
            
            // Clear selection if the removed item was selected
            if (m_currentHighlighted == go)
            {
                m_currentHighlighted = null;
            }
        }

        // Select a bookmark and highlight it in the hierarchy
        private void SelectBookmark(HierarchyBookmarksData.BookmarkData bookmark)
        {
            GameObject gameObject = HierarchyBookmarksData.instance.ResolveBookmark(bookmark);
            if (gameObject == null) return;
            
            // Set as current highlighted
            m_currentHighlighted = gameObject;
            
            // Select in the hierarchy
            Selection.activeGameObject = gameObject;
            
            // Ping the object in the hierarchy window to highlight it
            EditorGUIUtility.PingObject(gameObject);
            
            // Notify listeners
            OnBookmarkSelected?.Invoke(gameObject);
            
            // Refresh to update visual state
            RefreshBookmarksList();
        }

        // Refresh the bookmarks list UI
        private void RefreshBookmarksList()
        {
            // Clear the current list
            m_scrollView.Clear();
            
            // Get bookmarks for the current scene
            var activeSceneBookmarks = HierarchyBookmarksData.instance.GetActiveSceneBookmarks();
            
            // Create visual elements for each bookmark
            foreach (var bookmark in activeSceneBookmarks)
            {
                VisualElement item = CreateBookmarkItem(bookmark);
                if (item != null)
                {
                    m_scrollView.Add(item);
                }
            }
            
            // Show empty state if needed
            if (activeSceneBookmarks.Count == 0)
            {
                // Create an empty VisualElement to serve as a placeholder
                VisualElement emptyPlaceholder = new VisualElement();
                emptyPlaceholder.style.flexGrow = 1;
                m_scrollView.Add(emptyPlaceholder);
            }
        }

        // Clean up when disposed
        public void Dispose()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneClosed -= OnSceneClosed;
        }
    }
    
    // Popup for renaming bookmarks
    public class BookmarkRenamePopup : PopupWindowContent
    {
        private string m_name;
        private Action<string> m_onRename;
        
        public BookmarkRenamePopup(string currentName, Action<string> onRename)
        {
            m_name = currentName;
            m_onRename = onRename;
        }
        
        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 60);
        }
        
        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.LabelField("Rename Bookmark", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            m_name = EditorGUILayout.TextField("Name", m_name);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Cancel", GUILayout.Width(70)))
            {
                editorWindow.Close();
            }
            
            if (GUILayout.Button("Rename", GUILayout.Width(70)))
            {
                if (!string.IsNullOrEmpty(m_name))
                {
                    m_onRename(m_name);
                    editorWindow.Close();
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}