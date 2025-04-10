using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using Hierarchy.Data;
using Pastime_Hierarchy.Editor.UI.UIElements;

namespace Hierarchy {
    public class HierarchyManagerWindow : EditorWindow {
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
            var container = new ScrollView {
                style = {
                    flexGrow = 1
                }
            };

            var colorsFoldout = CreateFoldout("Colours Palette");
            container.Add(colorsFoldout);


            _colorRowsListView = new ListView {
                name = "ColorRowsListView",
                showAlternatingRowBackgrounds = AlternatingRowBackground.None,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                fixedItemHeight = 22,
                makeItem = () => {
                    var row = new VisualElement();
                    row.AddToClassList("palette-row");

                    // Register hover events for the entire row
                    row.RegisterCallback<MouseEnterEvent>(evt => {
                        var addBtn = row.Q(className: "palette-add-item-button");
                        if (addBtn != null) {
                            addBtn.style.display = DisplayStyle.Flex;
                        }
                    });

                    row.RegisterCallback<MouseLeaveEvent>(evt => {
                        var addBtn = row.Q(className: "palette-add-item-button");
                        if (addBtn != null) {
                            addBtn.style.display = DisplayStyle.None;
                        }
                    });

                    return row;
                },
                bindItem = (element, index) => {
                    var row = element;
                    row.Clear();

                    if (index >= 0 && index < HierarchyPaletteData.instance.ColorRows.Count) {
                        var rowData = HierarchyPaletteData.instance.ColorRows[index];

                        // Create the container for all row elements
                        var rowContainer = new VisualElement();
                        rowContainer.style.flexDirection = FlexDirection.Row;
                        rowContainer.style.flexGrow = 1;

                        // Create delete button as the first element
                        var removeColorRowButton = new EditorToolbarButton(() => {
                            HierarchyPaletteData.instance.RemoveColorRow(index);
                            RefreshColorRows();
                        });
                        removeColorRowButton.AddIcon("Toolbar Minus");
                        removeColorRowButton.SetIconSize(12, 12);
                        removeColorRowButton.AddToClassList("palette-remove-button");
                        removeColorRowButton.tooltip = "Remove color row";


                        // Add the delete button first
                        rowContainer.Add(removeColorRowButton);

                        // Create container for colors
                        var colorsContainer = new VisualElement();
                        colorsContainer.AddToClassList("palette-container");

                        // Add existing colors
                        for (int i = 0; i < rowData.colors.Count; i++) {
                            int colorIndex = i;
                            var colorItem = CreateColorItem(index, colorIndex, rowData.colors[i]);
                            colorsContainer.Add(colorItem);
                        }

                        // Create add color button (visible on hover)
                        if (rowData.colors.Count < 10) {
                            var addColorButton = new EditorToolbarButton(() => {
                                rowData.AddColor(Color.white);
                                RefreshColorRows();
                            });
                            addColorButton.AddIcon("CreateAddNew");
                            addColorButton.AddToClassList("palette-add-item-button");
                            addColorButton.SetIconSize(12, 12);
                            colorsContainer.Add(addColorButton);

                            addColorButton.style.display = DisplayStyle.None;
                        }

                        // Add colors container to the row
                        rowContainer.Add(colorsContainer);
                        row.Add(rowContainer);

                        // Store row index as user data for hover handling
                        row.userData = index;
                    }
                },
                itemsSource = HierarchyPaletteData.instance.ColorRows
            };

            colorsFoldout.Add(_colorRowsListView);

            var addColorToolbar = new AddRemoveToolbar(() => {
                HierarchyPaletteData.instance.AddColorRow();
                RefreshColorRows();
            }, null);
            addColorToolbar.ShowRemoveButton = false;
            
            colorsFoldout.Add(addColorToolbar);


            var iconsFoldout = CreateFoldout("Icons Palette");
            container.Add(iconsFoldout);

            _iconRowsListView = new ListView {
                name = "IconRowsListView",
                showAlternatingRowBackgrounds = AlternatingRowBackground.None,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                fixedItemHeight = 22,
                makeItem = () => {
                    var row = new VisualElement();
                    row.AddToClassList("palette-row");

                    // Register hover events for the entire row
                    row.RegisterCallback<MouseEnterEvent>(evt => {
                        var addBtn = row.Q(className: "palette-add-item-button");
                        if (addBtn != null) {
                            addBtn.style.display = DisplayStyle.Flex;
                        }
                    });

                    row.RegisterCallback<MouseLeaveEvent>(evt => {
                        var addBtn = row.Q(className: "palette-add-item-button");
                        if (addBtn != null) {
                            addBtn.style.display = DisplayStyle.None;
                        }
                    });

                    return row;
                },
                bindItem = (element, index) => {
                    var row = element;
                    row.Clear();

                    if (index >= 0 && index < HierarchyPaletteData.instance.IconRows.Count) {
                        var rowData = HierarchyPaletteData.instance.IconRows[index];

                        // Create the container for all row elements
                        var rowContainer = new VisualElement();
                        rowContainer.style.flexDirection = FlexDirection.Row;
                        rowContainer.style.flexGrow = 1;

                        // Create delete button as the first element
                        var removeIconRowButton = new EditorToolbarButton(() => {
                            HierarchyPaletteData.instance.RemoveIconRow(index);
                            RefreshIconRows();
                        });
                        removeIconRowButton.AddIcon("Toolbar Minus");
                        removeIconRowButton.SetIconSize(12, 12);
                        removeIconRowButton.AddToClassList("palette-remove-button");
                        removeIconRowButton.tooltip = "Remove icon row";


                        // Add the delete button first
                        rowContainer.Add(removeIconRowButton);

                        // Create container for icons
                        var iconsContainer = new VisualElement();
                        iconsContainer.AddToClassList("palette-container");

                        // Add existing icons
                        for (int i = 0; i < rowData.iconPaths.Count; i++) {
                            int iconIndex = i;
                            var iconItem = CreateIconItem(index, iconIndex, rowData.iconPaths[i]);
                            iconsContainer.Add(iconItem);
                        }

                        // Create add icon button (visible on hover)
                        if (rowData.iconPaths.Count < 10) {
                            var addIconButton = new EditorToolbarButton(() => {
                                OpenIconWindow(rowData);
                            });
                            addIconButton.AddIcon("CreateAddNew");
                            addIconButton.AddToClassList("palette-add-item-button");
                            addIconButton.SetIconSize(12, 12);
                            iconsContainer.Add(addIconButton);
                        }

                        // Add icons container to the row
                        rowContainer.Add(iconsContainer);
                        row.Add(rowContainer);

                        // Store row index as user data for hover handling
                        row.userData = index;
                    }
                },
                itemsSource = HierarchyPaletteData.instance.IconRows
            };

            iconsFoldout.Add(_iconRowsListView);

            var addIconToolbar = new AddRemoveToolbar(() => {
                HierarchyPaletteData.instance.AddIconRow();
                RefreshIconRows();
            }, null);
            addIconToolbar.ShowRemoveButton = false;

            iconsFoldout.Add(addIconToolbar);

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
            
            // Try to load the icon
            var iconContent = EditorGUIUtility.IconContent(iconPath);
            if (iconContent != null && iconContent.image != null) {
                var iconImage = new Image();
                iconImage.image = iconContent.image;
                iconImage.AddToClassList("palette-icon-image");
                container.Add(iconImage);
            }

            // Add context menu
            container.AddManipulator(new ContextualMenuManipulator(evt => {
                evt.menu.AppendAction("Remove Icon", action => {
                    HierarchyPaletteData.instance.IconRows[rowIndex].RemoveIcon(iconIndex);
                    RefreshIconRows();
                });
            }));
            
            return container;
        }

        private void OpenIconWindow(HierarchyPaletteData.IconRow rowData) {
            var iconWindowPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            HierarchyIconWindow.Instance.ShowPopup(iconWindowPosition);
             
            // we set the row data for the what ever window we select
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
            var container = new VisualElement { style = { flexGrow = 1 } };

            var settingsFoldout = CreateFoldout("Search Filters");
            container.Add(settingsFoldout);

            var multiColumnListView = new MultiColumnListView {
                name = "SearchFiltersListView",
                itemsSource = HierarchySettings.instance.SearchFilters,
                showBorder = true,
                reorderable = false
            };

            multiColumnListView.columns.Add(new Column {
                title = "Filter Name",
                name = "name",
                makeCell = () => new TextField { name = "nameField", style = { flexGrow = 1 } },
                bindCell = (element, index) => BindTextField(element, index, "nameField",
                    filter => filter.Name, (filter, value) => filter.Name = value),
                stretchable = true
            });

            multiColumnListView.columns.Add(new Column {
                title = "Filter Text",
                name = "filterText",
                makeCell = () => new TextField { name = "filterTextField", style = { flexGrow = 1 } },
                bindCell = (element, index) => BindTextField(element, index, "filterTextField",
                    filter => filter.FilterText, (filter, value) => filter.FilterText = value),
                stretchable = true
            });

            settingsFoldout.Add(multiColumnListView);

            var toolbar = new AddRemoveToolbar(() => {
                HierarchySettings.instance.SearchFilters.Add(new HierarchySearchFilter("New Filter", "t:GameObject"));
                HierarchySettings.instance.Save();
                multiColumnListView.Rebuild();
            }, () => {
                var selected = multiColumnListView.selectedIndex;
                if (selected >= 0 && selected < HierarchySettings.instance.SearchFilters.Count) {
                    HierarchySettings.instance.SearchFilters.RemoveAt(selected);
                    HierarchySettings.instance.Save();
                    multiColumnListView.Rebuild();
                }
            });

            settingsFoldout.Add(toolbar);
            return container;
        }

        private void BindTextField(VisualElement element, int index, string fieldName,
            Func<HierarchySearchFilter, string> getValue, Action<HierarchySearchFilter, string> setValue) {
            var textField = element.Q<TextField>(fieldName);
            if (index >= 0 && index < HierarchySettings.instance.SearchFilters.Count) {
                var filter = HierarchySettings.instance.SearchFilters[index];
                textField.SetValueWithoutNotify(getValue(filter));
                textField.RegisterValueChangedCallback(evt => {
                    setValue(filter, evt.newValue);
                    HierarchySettings.instance.Save();
                });
            }
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