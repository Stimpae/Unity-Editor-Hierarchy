using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using Hierarchy.Data;

namespace Hierarchy {
    public class HierarchyManagerWindow : InstancedWindow<HierarchyManagerWindow> {
        public void CreateGUI() {
            // Get the root element
            VisualElement root = rootVisualElement;

            // Load and apply stylesheet from package
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/Pastime Hierarchy/Editor/UI/USS/HierarchyWindowStyles.uss");
            if (styleSheet != null) {
                root.styleSheets.Add(styleSheet);
            }
            else {
                Debug.LogError("Could not load HierarchyManagerWindow stylesheet");
            }

            // Create main container
            var mainContainer = new VisualElement();
            mainContainer.name = "MainContainer";
            mainContainer.style.flexGrow = 1;
            root.Add(mainContainer);

            // Add toolbar
            var toolbar = CreateToolbar();
            mainContainer.Add(toolbar);

            // Create TabView for main content
            var tabView = new TabView();
            tabView.name = "MainTabView";
            tabView.style.flexGrow = 1;
            mainContainer.Add(tabView);

            // Add all tabs to the TabView
            AddTabsToTabView(tabView);
        }

        private Toolbar CreateToolbar() {
            var toolbar = new Toolbar();
            toolbar.name = "HierarchyToolbar";

            // Add spacer to push remaining items to the right
            var spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            toolbar.Add(spacer);

            // Add help button with icon
            var helpButton = new EditorToolbarButton(() => OpenDocumentation()) {
                tooltip = "Open Documentation"
            };
            helpButton.AddIcon(EditorGUIUtility.IconContent("_Help").image as Texture2D);
            helpButton.SetIconSize(16, 16);
            toolbar.Add(helpButton);

            // Add reset dropdown menu with icon
            var resetDropdown = new EditorToolbarMenu(null);
            resetDropdown.AddIcon(EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D);
            resetDropdown.SetIconSize(13, 13);
            resetDropdown.tooltip = "Reset Options";
            resetDropdown.menu.AppendAction("Reset All Settings", action => ResetAllSettings());
            resetDropdown.menu.AppendAction("Reset Colors", action => ResetColors());
            resetDropdown.menu.AppendAction("Reset Icons", action => ResetIcons());
            resetDropdown.menu.AppendAction("Reset Rules", action => ResetRules());

            toolbar.Add(resetDropdown);

            return toolbar;
        }

        private void AddTabsToTabView(TabView tabView) {
            // Create and add all tabs with their content

            // 1. Palette Tab
            var paletteTab = new Tab();
            paletteTab.name = "PaletteTab";
            paletteTab.label = "Palette";
            paletteTab.contentContainer.Add(CreatePaletteTabContent());
            tabView.Add(paletteTab);
            
            // 5. Rules Tab
            var iconTab = new Tab();
            iconTab.name = "IconTab";
            iconTab.label = "Icons";
            iconTab.contentContainer.Add(CreateIconTabContent());
            tabView.Add(iconTab);

            // 5. Rules Tab
            var rulesTab = new Tab();
            rulesTab.name = "RulesTab";
            rulesTab.label = "Rules";
            rulesTab.contentContainer.Add(CreateRulesTabContent());
            tabView.Add(rulesTab);
            
            
            // 4. Settings Tab
            var settingsTab = new Tab();
            settingsTab.name = "SettingsTab";
            settingsTab.label = "Settings";
            settingsTab.contentContainer.Add(CreateSettingsTabContent());
            tabView.Add(settingsTab);
        }

        private VisualElement CreateIconTabContent() {
            var container = new ScrollView();
            container.style.flexGrow = 1;
            
            return container;
        }

        private ListView _colorRowsListView;
        private ListView _iconRowsListView;

        private VisualElement CreatePaletteTabContent() {
            var container = new ScrollView();
            container.style.flexGrow = 1;

            // Colors section
            var colorsFoldout = CreateFoldout("Colours Pallete");
            container.Add(colorsFoldout);

            // Create color rows ListView
            _colorRowsListView = new ListView();
            _colorRowsListView.name = "ColorRowsListView";
            _colorRowsListView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;
            _colorRowsListView.reorderable = true; // Allow reordering
            _colorRowsListView.reorderMode = ListViewReorderMode.Animated;
            _colorRowsListView.fixedItemHeight = 32;
            
            // Create a horizontal scrolling container for each row
            _colorRowsListView.makeItem = () => {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.flexGrow = 1;
                row.AddToClassList("palette-row");
                return row;
            };

            // Bind each row to the data
            _colorRowsListView.bindItem = (element, index) => {
                var row = element;
                row.Clear();

                if (index >= 0 && index < HierarchyPaletteData.instance.ColorRows.Count) {
                    var rowData = HierarchyPaletteData.instance.ColorRows[index];

                    // Add each color in this row
                    var colorsContainer = new VisualElement();
                    colorsContainer.style.flexDirection = FlexDirection.Row;
                    colorsContainer.style.flexGrow = 1;
                    colorsContainer.AddToClassList("palette-container");

                    for (int i = 0; i < rowData.colors.Count; i++) {
                        int colorIndex = i; // Capture for lambda
                        var colorItem = CreateColorItem(index, colorIndex, rowData.colors[i]);
                        colorsContainer.Add(colorItem);
                    }
                    
                    if (rowData.colors.Count < 8) {
                        // Add button to add new color
                        var addColorButton = new EditorToolbarButton(() => {
                            rowData.AddColor(Color.white);
                            RefreshColorRows();
                        });
                        addColorButton.AddIcon("CreateAddNew");
                        addColorButton.AddToClassList("palette-add-item-button");
                        addColorButton.SetIconSize(12, 12);
                        colorsContainer.Add(addColorButton); 
                    }
                    
                    // Add button to remove color row
                    var removeColorRowButton = new EditorToolbarButton(() => {
                        HierarchyPaletteData.instance.RemoveColorRow(index);
                        RefreshColorRows();
                    });
                    
                    removeColorRowButton.AddIcon("Toolbar Minus");
                    removeColorRowButton.SetIconSize(12, 12);
                    removeColorRowButton.AddToClassList("palette-remove-button");
                    removeColorRowButton.tooltip = "Remove color row";
                    colorsContainer.Add(removeColorRowButton);
                    
                    row.Add(colorsContainer);
                }
            };
            
            _colorRowsListView.itemIndexChanged += (targetIndex, sourceIndex) => {
                Debug.Log($"Item moved from {sourceIndex} to {targetIndex}");
                HierarchyPaletteData.instance.MoveColorRow(sourceIndex, targetIndex);
                
                // Force the ListView to rebuild
                _colorRowsListView.Rebuild();
            };
            
            _colorRowsListView.itemsSource = HierarchyPaletteData.instance.ColorRows;
            colorsFoldout.Add(_colorRowsListView);
            
            var addColorRowButtonContainer = new VisualElement();
            addColorRowButtonContainer.AddToClassList("palette-add-button-container");

            var addColorRowButton = new EditorToolbarButton(() => {
                HierarchyPaletteData.instance.AddColorRow();
                RefreshColorRows();
            });

            addColorRowButton.AddIcon("CreateAddNew");
            addColorRowButton.SetIconSize(14, 14);
            addColorRowButton.AddToClassList("palette-add-button");
            addColorRowButtonContainer.Add(addColorRowButton);

            colorsFoldout.Add(addColorRowButtonContainer);


            /*// Icons section
            var iconsFoldout = CreateFoldout("Icons Palette");
            container.Add(iconsFoldout);
            
            _iconRowsListView = new ListView();
            _iconRowsListView.name = "IconRowsListView";
            _iconRowsListView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;
            _iconRowsListView.reorderable = true; // Allow reordering
            _iconRowsListView.reorderMode = ListViewReorderMode.Animated;
            _iconRowsListView.fixedItemHeight = 28;
            
            _iconRowsListView.makeItem = () => {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.flexGrow = 1;
                row.AddToClassList("palette-row");
                return row;
            };
            
            _iconRowsListView.bindItem = (element, index) => {
                var row = element;
                row.Clear();

                if (index >= 0 && index < HierarchyPaletteData.instance.IconRows.Count) {
                    var rowData = HierarchyPaletteData.instance.IconRows[index];

                    // Add each icon in this row
                    var iconsContainer = new VisualElement();
                    iconsContainer.style.flexDirection = FlexDirection.Row;
                    iconsContainer.style.flexGrow = 1;
                    iconsContainer.AddToClassList("palette-container");

                    for (int i = 0; i < rowData.iconPaths.Count; i++) {
                        int iconIndex = i; // Capture for lambda
                        var iconItem = CreateIconItem(index, iconIndex, rowData.iconPaths[i]);
                        iconsContainer.Add(iconItem);
                    }

                    // Add button to add new icon
                    var addIconButton = new EditorToolbarButton(() => {
                        rowData.AddIcon("d_UnityEditor.SceneHierarchyWindow");
                        RefreshIconRows();
                    }) { text = "+" };
                    addIconButton.AddToClassList("palette-add-item-button");
                    iconsContainer.Add(addIconButton);
                    row.Add(iconsContainer);
                    
                    //addIconButton.style.display = DisplayStyle.None;
                }

                // Always register callbacks, even if the row is empty
                element.RegisterCallback<MouseOverEvent>(evt => {
                    var addIconButton = element.Q<EditorToolbarButton>(className: "palette-add-item-button");
                    if (addIconButton != null) {
                        addIconButton.style.display = DisplayStyle.Flex;
                    }
                });

                element.RegisterCallback<MouseOutEvent>(evt => {
                    var addIconButton = element.Q<EditorToolbarButton>(className: "palette-add-item-button");
                    if (addIconButton != null) {
                        addIconButton.style.display = DisplayStyle.None;
                    }
                });
            };
            
            _iconRowsListView.itemIndexChanged += (targetIndex, sourceIndex) => {
                HierarchyPaletteData.instance.MoveIconRow(sourceIndex, targetIndex);
            };

            _iconRowsListView.itemsSource = HierarchyPaletteData.instance.IconRows;

            iconsFoldout.Add(_iconRowsListView);

            var iconButtonContainer = new Toolbar();
            iconButtonContainer.AddToClassList("palette-button-container");
            iconsFoldout.Add(iconButtonContainer);
            
            var addIconRowButton = new EditorToolbarButton(() => {
                HierarchyPaletteData.instance.AddIconRow();
                RefreshIconRows();
            });
            addIconRowButton.AddIcon("CreateAddNew");
            addIconRowButton.SetIconSize(14, 14);

            addIconRowButton.AddToClassList("palette-add-button");
            iconButtonContainer.Add(addIconRowButton);

            var removeIconRowButton = new EditorToolbarButton(() => {
                if (_iconRowsListView.selectedIndex >= 0) {
                    HierarchyPaletteData.instance.RemoveIconRow(_iconRowsListView.selectedIndex);
                    RefreshIconRows();
                }
            });

            removeIconRowButton.AddIcon("Toolbar Minus");
            removeIconRowButton.SetIconSize(14, 14);
            removeIconRowButton.tooltip = "Remove selected icon row";
            removeIconRowButton.AddToClassList("palette-remove-button");
            iconButtonContainer.Add(removeIconRowButton);*/

            return container;
        }

        private VisualElement CreateColorItem(int rowIndex, int colorIndex, Color color) {
            var container = new VisualElement();
            container.name = $"ColorItem_{rowIndex}_{colorIndex}";
            container.AddToClassList("palette-item");

            // Create the color field
            var colorField = new ColorField();
            colorField.showAlpha = false;
            colorField.showEyeDropper = false;
            colorField.value = color;
            colorField.tooltip = $"R: {color.r}, G: {color.g}, B: {color.b}, A: {color.a}";

            // Register for value changes
            colorField.RegisterValueChangedCallback(evt => {
                HierarchyPaletteData.instance.ColorRows[rowIndex].UpdateColor(colorIndex, evt.newValue);
                colorField.tooltip =
                    $"R: {evt.newValue.r}, G: {evt.newValue.g}, B: {evt.newValue.b}, A: {evt.newValue.a}";
            });

            // Add context menu
            colorField.AddManipulator(new ContextualMenuManipulator(evt => {
                evt.menu.AppendAction("Reset Color", action => {
                    colorField.value = Color.white;
                    HierarchyPaletteData.instance.ColorRows[rowIndex].UpdateColor(colorIndex, Color.white);
                });

                evt.menu.AppendAction("Remove Color", action => {
                    HierarchyPaletteData.instance.ColorRows[rowIndex].RemoveColor(colorIndex);
                    RefreshColorRows();
                });
            }));

            container.Add(colorField);

            return container;
        }

        private VisualElement CreateIconItem(int rowIndex, int iconIndex, string iconPath) {
            var container = new VisualElement();
            container.name = $"IconItem_{rowIndex}_{iconIndex}";
            container.AddToClassList("palette-item");

            // Create the icon button
            var iconButton = new Button(() => {
                // Open icon selector (simplified version)
                OpenIconSelector(rowIndex, iconIndex);
            });
            iconButton.AddToClassList("icon-button");

            // Try to load the icon
            var iconContent = EditorGUIUtility.IconContent(iconPath);
            if (iconContent != null && iconContent.image != null) {
                var iconImage = new Image();
                iconImage.image = iconContent.image;
                iconImage.AddToClassList("palette-icon-image");
                iconButton.Add(iconImage);
            }
            else {
                iconButton.text = "?";
            }

            // Add context menu
            iconButton.AddManipulator(new ContextualMenuManipulator(evt => {
                evt.menu.AppendAction("Select Icon", action => { OpenIconSelector(rowIndex, iconIndex); });

                evt.menu.AppendAction("Remove Icon", action => {
                    HierarchyPaletteData.instance.IconRows[rowIndex].RemoveIcon(iconIndex);
                    RefreshIconRows();
                });
            }));

            container.Add(iconButton);

            return container;
        }

        private void OpenIconSelector(int rowIndex, int iconIndex) {
            // For demo purposes, just rotate through some common icons
            string[] commonIcons = new string[] {
                "d_UnityEditor.SceneHierarchyWindow",
                "d_Prefab Icon",
                "d_Folder Icon",
                "d_GameObject Icon",
                "d_Camera Icon",
                "d_Light Icon"
            };

            // For a real implementation, you would create a proper icon picker window

            int randomIndex = UnityEngine.Random.Range(0, commonIcons.Length);
            HierarchyPaletteData.instance.IconRows[rowIndex].UpdateIcon(iconIndex, commonIcons[randomIndex]);
            RefreshIconRows();
        }

        private void RefreshColorRows() {
            if (_colorRowsListView != null) {
                // This will refresh the ListView with the updated data
                _colorRowsListView.Rebuild();
            }
        }

        private void RefreshIconRows() {
            if (_iconRowsListView != null) {
                // This will refresh the ListView with the updated data
                _iconRowsListView.Rebuild();
            }
        }

        private void ResetColors() {
            if (EditorUtility.DisplayDialog(
                    "Reset Colors",
                    "Are you sure you want to reset all colors to default values?",
                    "Reset Colors", "Cancel")) {
                HierarchyPaletteData.instance.ResetColors();
                RefreshColorRows();
            }
        }

        private void ResetIcons() {
            if (EditorUtility.DisplayDialog(
                    "Reset Icons",
                    "Are you sure you want to reset all icons to default values?",
                    "Reset Icons", "Cancel")) {
                HierarchyPaletteData.instance.ResetIcons();
                RefreshIconRows();
            }
        }

        private VisualElement CreateSettingsTabContent() {
            var container = new ScrollView();
            container.style.flexGrow = 1;
            return container;
        }

        private VisualElement CreateRulesTabContent() {
            var container = new ScrollView();
            container.style.flexGrow = 1;
            return container;
        }

        private Foldout CreateFoldout(string title) {
            var foldout = new Foldout();
            foldout.text = title;
            foldout.value = true;
            return foldout;
        }

        private void OpenDocumentation() {
            Application.OpenURL("https://docs.unity3d.com/Manual/index.html");
            // Replace with your actual documentation URL
        }

        private void ResetAllSettings() {
            Debug.Log("Resetting all settings to defaults");
            // Implement reset all logic
        }

        private void ResetRules() {
            Debug.Log("Resetting rules to defaults");
            // Implement rules reset logic
        }
    }
}