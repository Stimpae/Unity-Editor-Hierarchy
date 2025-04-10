/*using UnityEngine.UIElements;

namespace Hierarchy {
    public class Notes {
        
        private VisualElement CreatePaletteTabContent() {
            var container = new ScrollView { style = { flexGrow = 1 } };
            // Colors Palette
            var colorsFoldout = CreateFoldout("Colours Palette");
            _colorRowsListView = new PaletteListView<PaletteColorItem>(
                "ColorRowsListView",
                HierarchyPaletteData.instance.ColorRows,
                (index) => new PaletteColorItem(index, -1, Color.white, RefreshColorRows),
                (element, index) => {
                    var rowData = HierarchyPaletteData.instance.ColorRows[index];
                    ((PaletteColorItem)element).Bind(rowData, index, RefreshColorRows);
                }
            );
            colorsFoldout.Add(_colorRowsListView);
            colorsFoldout.Add(new AddRemoveToolbar(() => {
                HierarchyPaletteData.instance.AddColorRow();
                RefreshColorRows();
            }, null) { ShowRemoveButton = false });
            container.Add(colorsFoldout);

            // Icons Palette
            var iconsFoldout = CreateFoldout("Icons Palette");
            _iconRowsListView = new PaletteListView<PaletteIconItem>(
                "IconRowsListView",
                HierarchyPaletteData.instance.IconRows,
                (index) => new PaletteIconItem(index, -1, string.Empty, RefreshIconRows),
                (element, index) => {
                    var rowData = HierarchyPaletteData.instance.IconRows[index];
                    ((PaletteIconItem)element).Bind(rowData, index, RefreshIconRows);
                }
            );
            iconsFoldout.Add(_iconRowsListView);
            iconsFoldout.Add(new AddRemoveToolbar(() => {
                HierarchyPaletteData.instance.AddIconRow();
                RefreshIconRows();
            }, null) { ShowRemoveButton = false });
            container.Add(iconsFoldout);

            return container;
        }
    }
    
    ```csharp
private VisualElement CreatePaletteTabContent() {
    var container = new ScrollView { style = { flexGrow = 1 } };

    // Colors Palette
    var colorsFoldout = CreateFoldout("Colours Palette");
    _colorRowsListView = CreateListView(
        "ColorRowsListView",
        HierarchyPaletteData.instance.ColorRows,
        (row, index) => BindColorRow(row, index)
    );
    colorsFoldout.Add(_colorRowsListView);
    colorsFoldout.Add(CreateAddToolbar(() => {
        HierarchyPaletteData.instance.AddColorRow();
        RefreshColorRows();
    }));
    container.Add(colorsFoldout);

    // Icons Palette
    var iconsFoldout = CreateFoldout("Icons Palette");
    _iconRowsListView = CreateListView(
        "IconRowsListView",
        HierarchyPaletteData.instance.IconRows,
        (row, index) => BindIconRow(row, index)
    );
    iconsFoldout.Add(_iconRowsListView);
    iconsFoldout.Add(CreateAddToolbar(() => {
        HierarchyPaletteData.instance.AddIconRow();
        RefreshIconRows();
    }));
    container.Add(iconsFoldout);

    return container;
}

private ListView CreateListView(string name, IList itemsSource, Action<VisualElement, int> bindItem) {
    return new ListView {
        name = name,
        itemsSource = itemsSource,
        showAlternatingRowBackgrounds = AlternatingRowBackground.None,
        reorderable = true,
        reorderMode = ListViewReorderMode.Animated,
        fixedItemHeight = 22,
        makeItem = () => new VisualElement { classList = { "palette-row" } },
        bindItem = bindItem
    };
}

private void BindColorRow(VisualElement row, int index) {
    row.Clear();
    if (index < 0 || index >= HierarchyPaletteData.instance.ColorRows.Count) return;

    var rowData = HierarchyPaletteData.instance.ColorRows[index];
    var rowContainer = CreateRowContainer();
    rowContainer.Add(CreateRemoveButton(() => {
        HierarchyPaletteData.instance.RemoveColorRow(index);
        RefreshColorRows();
    }));

    var colorsContainer = CreatePaletteContainer(rowData.colors, (colorIndex, color) => 
        CreateColorItem(index, colorIndex, color), () => {
        rowData.AddColor(Color.white);
        RefreshColorRows();
    });
    rowContainer.Add(colorsContainer);
    row.Add(rowContainer);
}

private void BindIconRow(VisualElement row, int index) {
    row.Clear();
    if (index < 0 || index >= HierarchyPaletteData.instance.IconRows.Count) return;

    var rowData = HierarchyPaletteData.instance.IconRows[index];
    var rowContainer = CreateRowContainer();
    rowContainer.Add(CreateRemoveButton(() => {
        HierarchyPaletteData.instance.RemoveIconRow(index);
        RefreshIconRows();
    }));

    var iconsContainer = CreatePaletteContainer(rowData.iconPaths, (iconIndex, iconPath) => 
        CreateIconItem(index, iconIndex, iconPath), () => {
        OpenIconWindow(rowData);
    });
    rowContainer.Add(iconsContainer);
    row.Add(rowContainer);
}

private VisualElement CreateRowContainer() {
    return new VisualElement { style = { flexDirection = FlexDirection.Row, flexGrow = 1 } };
}

private VisualElement CreatePaletteContainer<T>(IList<T> items, Func<int, T, VisualElement> createItem, Action addItem) {
    var container = new VisualElement { classList = { "palette-container" } };
    for (int i = 0; i < items.Count; i++) {
        container.Add(createItem(i, items[i]));
    }
    if (items.Count < 10) {
        var addButton = CreateAddButton(addItem);
        addButton.style.display = DisplayStyle.None;
        container.Add(addButton);
    }
    return container;
}

private EditorToolbarButton CreateRemoveButton(Action onClick) {
    var button = new EditorToolbarButton(onClick) { tooltip = "Remove row" };
    button.AddIcon("Toolbar Minus");
    button.SetIconSize(12, 12);
    button.AddToClassList("palette-remove-button");
    return button;
}

private EditorToolbarButton CreateAddButton(Action onClick) {
    var button = new EditorToolbarButton(onClick);
    button.AddIcon("CreateAddNew");
    button.SetIconSize(12, 12);
    button.AddToClassList("palette-add-item-button");
    return button;
}

private AddRemoveToolbar CreateAddToolbar(Action onAdd) {
    var toolbar = new AddRemoveToolbar(onAdd, null);
    toolbar.ShowRemoveButton = false;
    return toolbar;
}
```
}*/