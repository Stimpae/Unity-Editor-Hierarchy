using UnityEditor;
using UnityEngine;

namespace Hierarchy {
    public static class HierarchyMenu {
        private const string BASE_MENU_PATH = "Tools/Hierarchy/";
        private const string PREFS_PREFIX = "HierarchyPlugin_";

        private static readonly string[] Keys = {
            "CustomHeaderEnabled", "SearchButton", "FavoritesButton", "CollapseButton", "LockButton",
            "ActivationToggle", "Components", "ZebraStripsEnabled", "HierarchyLines", "Layers", "Tags", "ValidationErrors"
        };

        // Menu labels that match the exact menu item paths
        private static readonly string[] MenuLabels = {
            "Custom Header", 
            "Custom Header Features/Search Button", 
            "Custom Header Features/Favorites Button", 
            "Custom Header Features/Collapse Button", 
            "Custom Header Features/Lock Button",
            "Allow Activation Toggle", 
            "Display Components", 
            "Display Zebra Strips", 
            "Use Hierarchy Lines", 
            "Display Layers", 
            "Display Tags", 
            "Display Validation Errors"
        };

        [MenuItem(BASE_MENU_PATH + "Custom Header", false, 100)]
        private static void ToggleCustomHeader() => ToggleFeature(Keys[0]);

        [MenuItem(BASE_MENU_PATH + "Custom Header", true)]
        private static bool ValidateCustomHeader() => ValidateFeature(Keys[0], null, 0);

        [MenuItem(BASE_MENU_PATH + "Custom Header Features/Search Button", false, 200)]
        private static void ToggleSearchButton() => ToggleFeature(Keys[1]);

        [MenuItem(BASE_MENU_PATH + "Custom Header Features/Search Button", true)]
        private static bool ValidateSearchButton() => ValidateFeature(Keys[1], Keys[0], 1);

        [MenuItem(BASE_MENU_PATH + "Custom Header Features/Favorites Button", false, 201)]
        private static void ToggleFavoritesButton() => ToggleFeature(Keys[2]);

        [MenuItem(BASE_MENU_PATH + "Custom Header Features/Favorites Button", true)]
        private static bool ValidateFavoritesButton() => ValidateFeature(Keys[2], Keys[0], 2);

        [MenuItem(BASE_MENU_PATH + "Custom Header Features/Collapse Button", false, 202)]
        private static void ToggleCollapseButton() => ToggleFeature(Keys[3]);

        [MenuItem(BASE_MENU_PATH + "Custom Header Features/Collapse Button", true)]
        private static bool ValidateCollapseButton() => ValidateFeature(Keys[3], Keys[0], 3);

        [MenuItem(BASE_MENU_PATH + "Custom Header Features/Lock Button", false, 203)]
        private static void ToggleLockButton() => ToggleFeature(Keys[4]);

        [MenuItem(BASE_MENU_PATH + "Custom Header Features/Lock Button", true)]
        private static bool ValidateLockButton() => ValidateFeature(Keys[4], Keys[0], 4);

        [MenuItem(BASE_MENU_PATH + "Allow Activation Toggle", false, 300)]
        private static void ToggleActivationToggle() => ToggleFeature(Keys[5]);

        [MenuItem(BASE_MENU_PATH + "Allow Activation Toggle", true)]
        private static bool ValidateActivationToggle() => ValidateFeature(Keys[5], null, 5);

        [MenuItem(BASE_MENU_PATH + "Display Components", false, 301)]
        private static void ToggleComponents() => ToggleFeature(Keys[6]);

        [MenuItem(BASE_MENU_PATH + "Display Components", true)]
        private static bool ValidateComponents() => ValidateFeature(Keys[6], null, 6);

        [MenuItem(BASE_MENU_PATH + "Display Zebra Strips", false, 302)]
        private static void ToggleZebraStrips() => ToggleFeature(Keys[7]);

        [MenuItem(BASE_MENU_PATH + "Display Zebra Strips", true)]
        private static bool ValidateZebraStrips() => ValidateFeature(Keys[7], null, 7);

        [MenuItem(BASE_MENU_PATH + "Use Hierarchy Lines", false, 303)]
        private static void ToggleHierarchyLines() => ToggleFeature(Keys[8]);

        [MenuItem(BASE_MENU_PATH + "Use Hierarchy Lines", true)]
        private static bool ValidateHierarchyLines() => ValidateFeature(Keys[8], null, 8);

        [MenuItem(BASE_MENU_PATH + "Display Layers", false, 304)]
        private static void ToggleLayers() => ToggleFeature(Keys[9]);

        [MenuItem(BASE_MENU_PATH + "Display Layers", true)]
        private static bool ValidateLayers() => ValidateFeature(Keys[9], null, 9);

        [MenuItem(BASE_MENU_PATH + "Display Tags", false, 305)]
        private static void ToggleTags() => ToggleFeature(Keys[10]);

        [MenuItem(BASE_MENU_PATH + "Display Tags", true)]
        private static bool ValidateTags() => ValidateFeature(Keys[10], null, 10);

        [MenuItem(BASE_MENU_PATH + "Display Validation Errors", false, 306)]
        private static void ToggleValidationErrors() => ToggleFeature(Keys[11]);

        [MenuItem(BASE_MENU_PATH + "Display Validation Errors", true)]
        private static bool ValidateValidationErrors() => ValidateFeature(Keys[11], null, 11);

        [MenuItem(BASE_MENU_PATH + "Open Hierarchy Manager", false, 400)]
        private static void OpenHierarchyManager() {
            HierarchyWindow.Instance.ShowWindow("Hierarchy Manager");
        }

        [MenuItem(BASE_MENU_PATH + "Recompile Scripts", false, 500)]
        private static void RecompileScripts() => AssetDatabase.Refresh();

        private static void ToggleFeature(string key) {
            bool enabled = !EditorPrefs.GetBool(PREFS_PREFIX + key, true);
            EditorPrefs.SetBool(PREFS_PREFIX + key, enabled);
            RepaintHierarchyWindow();
        }

        private static bool ValidateFeature(string key, string dependencyKey = null, int menuIndex = -1) {
            bool enabled = EditorPrefs.GetBool(PREFS_PREFIX + key, true);
            
            // Use the menuIndex to get the correct menu path
            if (menuIndex >= 0 && menuIndex < MenuLabels.Length) {
                Menu.SetChecked(BASE_MENU_PATH + MenuLabels[menuIndex], enabled);
            }
            
            // Check dependency if provided
            return dependencyKey == null || EditorPrefs.GetBool(PREFS_PREFIX + dependencyKey, true);
        }

        private static void RepaintHierarchyWindow() => EditorApplication.RepaintHierarchyWindow();
    }
}