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
using UnityEditor.Toolbars;


namespace Hierarchy.GUI {
    public class HierarchyToolbarGUI : IDisposable {
        private readonly EditorWindow m_window;
        private EditorToolbarMenu m_gameObjectDropdown;
        
        private readonly Type m_hierarchyType;
        private readonly object m_sceneHierarchy;
        private readonly MethodInfo m_addCreateGameObjectItemsToMenuMethod;
        
        private Toolbar m_searchToolbar;
        private bool m_isSearchVisible = true;
        private ToolbarPopupSearchField m_searchField;
        
        private Toolbar m_favoritesToolbar;
        private bool m_isFavoritesVisible = true;
        
        public bool IsSearchVisible => m_isSearchVisible;
        public bool IsFavoritesVisible => m_isFavoritesVisible;

        public void Dispose() {
            
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

        public void CreateGUI(float toolbarHeight, float searchHeight, float favoritesHeight) {
            VisualElement root = m_window.rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;

            Toolbar topToolbar = new Toolbar {
                style = {
                    height = toolbarHeight,
                    borderBottomWidth = 0,
                }
            };
            root.Add(topToolbar);
            
            m_searchToolbar = new Toolbar {
                style = {
                    height = searchHeight,
                    borderBottomWidth = 0,
                }
            };
            root.Add(m_searchToolbar);
            
            m_favoritesToolbar = new Toolbar {
                style = {
                    height = favoritesHeight,
                    borderBottomWidth = 0,
                }
            };
            root.Add(m_favoritesToolbar);
            
            m_gameObjectDropdown = new EditorToolbarMenu();
            m_gameObjectDropdown.AddMenuIcon("CreateAddNew", 16, 16);
            m_gameObjectDropdown.OnClick += ShowGameObjectDropdown;
            m_gameObjectDropdown.style.flexShrink = 0;
            m_gameObjectDropdown.tooltip = "Create GameObject";

            /*m_favoriteDropdown = new EditorToolbarMenu();
            m_favoriteDropdown.AddMenuIcon("d_FolderFavorite On Icon", 16, 16);
            m_favoriteDropdown.style.flexShrink = 0;
            m_favoriteDropdown.tooltip = "View Favorites";
            
            // collapse button
            // toggle search field visibility
            */
            
            m_searchField = new ToolbarPopupSearchField();
            m_searchField.RegisterValueChangedCallback(OnSearchValueChanged);
            m_searchField.style.flexShrink = 1;
            m_searchField.style.flexGrow = 1;
            
            // add these to the toolbar
            topToolbar.Add(m_gameObjectDropdown);
            topToolbar.Add(new EditorToolbarToggle() {
                tooltip = "Toggle Search",
                icon = (Texture2D)EditorGUIUtility.IconContent("d_FolderFavorite On Icon").image,
            });
            
            topToolbar.Add(new EditorToolbarToggle() {
                tooltip = "Toggle Favorites",
                icon = (Texture2D)EditorGUIUtility.IconContent("d_FolderFavorite On Icon").image,
            });
            
            m_searchToolbar.Add(m_searchField);
            
            Debug.Log("HierarchyToolbarGUI created");
        }
        
        private void ToggleSearchToolbarVisibility() {
            m_isSearchVisible = !m_isSearchVisible;
            m_searchToolbar.style.display = m_isSearchVisible ? DisplayStyle.Flex : DisplayStyle.None;

            if (!m_isSearchVisible && !string.IsNullOrEmpty(m_searchField.value)) {
                m_searchField.value = "";
                ClearHierarchySearch();
            }
        }
        
        private void ToggleFavoritesToolbarVisibility() {
            m_isFavoritesVisible = !m_isFavoritesVisible;
            m_favoritesToolbar.style.display = m_isFavoritesVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void ShowGameObjectDropdown() {
            try {
                if (m_hierarchyType == null) return;
                if (!m_hierarchyType.IsInstanceOfType(m_window)) return;

                var customParentForNewGameObjects = m_sceneHierarchy.GetFieldValue("m_CustomParentForNewGameObjects") as Transform;
                var targetSceneHandle = customParentForNewGameObjects != null ? customParentForNewGameObjects.gameObject.scene.handle : 0;
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