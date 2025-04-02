using Hierarchy.Utils;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Reflection;
using Hierarchy.Libraries;
using Pastime.Hierarchy.GUI;
using UnityEditor.Toolbars;
using EditorToolbarToggle = Hierarchy.Elements.EditorToolbarToggle;


namespace Hierarchy.GUI {
    public class HierarchyToolbarGUI : IDisposable {
        private readonly EditorWindow m_window;
        private readonly Type m_hierarchyType;
        private readonly object m_sceneHierarchy;
        private readonly MethodInfo m_addCreateGameObjectItemsToMenuMethod;

        private VisualElement m_favoritesBar;
        private VisualElement m_toolbar;
        
        private ToolbarPopupSearchField m_searchField;
        private EditorToolbarMenu m_gameObjectDropdown;
        
        // get the editor prefs for this instead
        public bool IsFavoritesVisible => EditorPrefs.GetBool("Editor_FavoritesVisible_" + m_window.GetHashCode());

        public void Dispose() {
            if (m_searchField != null) {
                m_searchField.UnregisterValueChangedCallback(OnSearchValueChanged);
                m_searchField = null;
            }
            
            m_favoritesElement = null;
            m_toolbar = null;
            m_favoritesBar = null;
            m_gameObjectDropdown = null;
        }

        public HierarchyToolbarGUI(EditorWindow window) {
            m_window = window;

            m_hierarchyType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            if (m_hierarchyType != null && m_hierarchyType.IsInstanceOfType(m_window)) {
                m_sceneHierarchy = m_window.GetFieldValue("m_SceneHierarchy");
                m_addCreateGameObjectItemsToMenuMethod = m_sceneHierarchy?.GetType()
                    .GetMethod("AddCreateGameObjectItemsToMenu", ReflectionUtils.MAX_BINDING_FLAGS);
            }
        }

        private HierarchyBookmarksElement m_favoritesElement;
        
        public void CreateGUI(float toolbarHeight, float favoritesHeight) {
            VisualElement root = m_window.rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;

            m_toolbar = new Toolbar() {
                style = {
                    height = toolbarHeight,
                    flexDirection = FlexDirection.Row,
                    borderBottomWidth = 0
                }
            };
            root.Add(m_toolbar);

            // Left container - fixed width, no grow/shrink
            VisualElement leftContainer = new VisualElement() {
                style = {
                    flexGrow = 0,
                    flexShrink = 0,
                    flexBasis = StyleKeyword.Auto, // Use auto as the base size
                    width = StyleKeyword.Auto, // Let content determine width
                    justifyContent = Justify.FlexStart,
                    alignSelf = Align.Center
                }
            };
            m_toolbar.Add(leftContainer);

            // Center container - should take available space and hug the right side
            VisualElement centerContainer = new VisualElement() {
                style = {
                    flexGrow = 1, // Allow it to grow to fill available space
                    flexShrink = 0, // Don't shrink below its minimum content size
                    justifyContent = Justify.FlexStart, // Align content to left within container
                }
            };
            m_toolbar.Add(centerContainer);

            // Right container - fixed position on right
            VisualElement rightContainer = new VisualElement() {
                style = {
                    flexGrow = 0,
                    flexShrink = 0,
                    width = StyleKeyword.Auto, // Let content determine width
                    justifyContent = Justify.FlexEnd,
                    flexDirection = FlexDirection.Row,
                    marginRight = 2,
                    marginLeft = 2,
                }
            };
            m_toolbar.Add(rightContainer);

            m_favoritesBar = new Toolbar {
                style = {
                    height = favoritesHeight,
                    borderBottomWidth = 0,
                }
            };
            root.Add(m_favoritesBar);
            
            // Create the favorites element
            m_favoritesElement = new HierarchyBookmarksElement {
                style = {
                    flexGrow = 1
                }
            };

    
            // Add to favorites bar
            m_favoritesBar.Add(m_favoritesElement);

            m_gameObjectDropdown = new EditorToolbarMenu(ShowGameObjectDropdown);
            m_gameObjectDropdown.AddIcon("CreateAddNew");
            m_gameObjectDropdown.SetIconSize(16,16);
            m_gameObjectDropdown.tooltip = "Create GameObject";
            leftContainer.Add(m_gameObjectDropdown);

            m_searchField = new ToolbarPopupSearchField();
            m_searchField.RegisterValueChangedCallback(OnSearchValueChanged);
            m_searchField.style.flexShrink = 1;
            m_searchField.style.flexGrow = 1; // Allow search field to grow
            m_searchField.style.width = new StyleLength(StyleKeyword.Auto); // Use auto width
            centerContainer.Add(m_searchField);
            
            // Create the toggle
            var searchToggle = new EditorToolbarToggle();
            searchToggle.Initialize("Editor_SearchVisible_" + m_window.GetHashCode(),false, ToggleSearchToolbarVisibility);
            searchToggle.AddIcon("d_SearchOverlay@2x");
            rightContainer.Add(searchToggle);
            
            var favoritesButton = new EditorToolbarToggle();
            favoritesButton.Initialize("Editor_FavoritesVisible_" + m_window.GetHashCode(),false, ToggleFavoritesToolbarVisibility);
            favoritesButton.AddIcon("d_PreMatCube@2x");
            favoritesButton.SetIconSize(16,16);
            rightContainer.Add(favoritesButton);
            
            // create collapse button
            var collapseButton = new EditorToolbarButton(CollapseHierarchy);
            collapseButton.AddIcon("d_Animation.NextKey@2x");
            collapseButton.IconImage.style.rotate = new StyleRotate(new Rotate(new Angle(90)));
            collapseButton.SetIconSize(16,16);
            rightContainer.Add(collapseButton);
        }
        
        private void CollapseHierarchy() {
        }

        private void ToggleSearchToolbarVisibility(bool isVisible) {
            
            m_searchField.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (!isVisible && !string.IsNullOrEmpty(m_searchField.value)) {
                m_searchField.value = "";
                ClearHierarchySearch();
            }
        }

        private void ToggleFavoritesToolbarVisibility(bool isVisible) {
            m_favoritesBar.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void ShowGameObjectDropdown() {
            try {
                if (m_hierarchyType == null) return;
                if (!m_hierarchyType.IsInstanceOfType(m_window)) return;

                var customParentForNewGameObjects =
                    m_sceneHierarchy.GetFieldValue("m_CustomParentForNewGameObjects") as Transform;
                var targetSceneHandle = customParentForNewGameObjects != null
                    ? customParentForNewGameObjects.gameObject.scene.handle
                    : 0;
                var menu = new GenericMenu();

                try {
                    m_addCreateGameObjectItemsToMenuMethod?.Invoke(m_sceneHierarchy, new object[] {
                        menu, null, true, true, false, targetSceneHandle, 3
                    });
                }
                catch (Exception ex) {
                    Debug.LogWarning($"AddCreateGameObjectItemsToMenu failed: {ex.Message}");
                }

                Vector2 position = m_gameObjectDropdown.worldBound.position;
                position.y += m_gameObjectDropdown.worldBound.height;
                menu.DropDown(new Rect(position, Vector2.zero));
            }
            catch (Exception ex) {
                Debug.LogError($"Error showing GameObject dropdown: {ex.Message}\n{ex.StackTrace}");
            }
        }


        private void OnSearchValueChanged(ChangeEvent<string> evt) {
            string searchString = evt.newValue;

            // Apply the search filter to the hierarchy window
            if (string.IsNullOrEmpty(searchString)) {
                // Clear the search filter
                ClearHierarchySearch();
            }
            else {
                // Apply the search filter
                FilterHierarchyByName(searchString);
            }

            // Force focus back to this window and specifically this search field
            if (evt.target is VisualElement element) {
                // Delay the focus to ensure it happens after Unity's internal focus handling
                EditorApplication.delayCall += () => {
                    m_window.Focus();
                    element.Focus();
                };
            }
        }

        private void FilterHierarchyByName(string searchString) {
            // Get all scene objects
            var allObjects = GetAllSceneObjects();
            List<int> filteredInstanceIDs = new List<int>();

            // Filter objects based on name
            foreach (var obj in allObjects) {
                if (obj != null && obj.name.IndexOf(searchString, System.StringComparison.OrdinalIgnoreCase) >= 0) {
                    filteredInstanceIDs.Add(obj.GetInstanceID());

                    // Also include all parent objects to maintain hierarchy structure
                    Transform parent = obj.transform.parent;
                    while (parent != null) {
                        filteredInstanceIDs.Add(parent.gameObject.GetInstanceID());
                        parent = parent.parent;
                    }
                }
            }

            // Apply the filter using SearchableEditorWindow's SetSearchFilter
            // Important: Use THIS specific window instance
            var hierarchyType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            if (hierarchyType != null && hierarchyType.IsInstanceOfType(m_window)) {
                try {
                    // Check parameter count by getting the method first
                    var parameters = hierarchyType.GetMethod("SetSearchFilter", ReflectionUtils.MAX_BINDING_FLAGS)
                        ?.GetParameters();

                    if (parameters != null) {
                        if (parameters.Length == 4) {
                            // Your Unity version has 4 parameters
                            m_window.InvokeMethod("SetSearchFilter", searchString,
                                SearchableEditorWindow.SearchMode.All, true, true);
                        }
                        else if (parameters.Length == 3) {
                            // Newer Unity versions might have: SetSearchFilter(string searchFilter, SearchMode searchMode, bool setAll)
                            m_window.InvokeMethod("SetSearchFilter", searchString,
                                SearchableEditorWindow.SearchMode.All, true);
                        }
                        else if (parameters.Length == 2) {
                            // Older Unity versions might have: SetSearchFilter(string searchFilter, SearchMode searchMode)
                            m_window.InvokeMethod("SetSearchFilter", searchString,
                                SearchableEditorWindow.SearchMode.All);
                        }
                        else {
                            Debug.LogError(
                                $"SetSearchFilter method has unexpected parameter count: {parameters.Length}");
                        }
                    }
                }
                catch (Exception ex) {
                    Debug.LogError($"Error setting search filter: {ex.Message}");
                }
            }
        }

        private void ClearHierarchySearch() {
            // Clear the search filter
            var hierarchyType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            if (hierarchyType != null && hierarchyType.IsInstanceOfType(m_window)) {
                try {
                    // Check parameter count by getting the method first
                    var parameters = hierarchyType.GetMethod("SetSearchFilter", ReflectionUtils.MAX_BINDING_FLAGS)
                        ?.GetParameters();

                    if (parameters != null) {
                        if (parameters.Length == 4) {
                            // Your Unity version has 4 parameters
                            m_window.InvokeMethod("SetSearchFilter", "", SearchableEditorWindow.SearchMode.All, true,
                                true);
                        }
                        else if (parameters.Length == 3) {
                            m_window.InvokeMethod("SetSearchFilter", "", SearchableEditorWindow.SearchMode.All, true);
                        }
                        else if (parameters.Length == 2) {
                            m_window.InvokeMethod("SetSearchFilter", "", SearchableEditorWindow.SearchMode.All);
                        }
                        else {
                            Debug.LogError(
                                $"SetSearchFilter method has unexpected parameter count: {parameters.Length}");
                        }
                    }
                }
                catch (Exception ex) {
                    Debug.LogError($"Error clearing search filter: {ex.Message}");
                }
            }
        }

        private GameObject[] GetAllSceneObjects() {
            return Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => go.scene.isLoaded && !EditorUtility.IsPersistent(go))
                .ToArray();
        }
    }
}