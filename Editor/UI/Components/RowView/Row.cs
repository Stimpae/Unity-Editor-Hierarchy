using System;
using System.Collections;
using System.Collections.Generic;
using Hierarchy.Data;
using Pastime_Hierarchy.Editor.UI.Components.RowView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace Pastime_Hierarchy.Editor.UI.Components {
    // Item data i
    public class Row<T> : VisualElement {
        private IRowItemFactory<T> _factory;
        private VisualElement _rowContainer;
        private VisualElement _itemsContainer;
        private int _rowIndex;

        public Row(IRowItemFactory<T> factory) {
            _factory = factory;
            AddToClassList("palette-row");
            _rowContainer = new VisualElement { style = { flexDirection = FlexDirection.Row, flexGrow = 1 } };
            Add(_rowContainer);
        }

        public void Bind(int rowIndex, List<T> rowData, ListView listView) {
            _rowIndex = rowIndex;
            _rowContainer.Clear();

            // Create delete button as the first element
            var removeColorRowButton = new EditorToolbarButton(() => {
                _factory.OnRemoveRow(rowIndex);
                listView.Rebuild();
            });
            removeColorRowButton.AddIcon("Toolbar Minus");
            removeColorRowButton.SetIconSize(12, 12);
            removeColorRowButton.AddToClassList("palette-remove-button");
            removeColorRowButton.tooltip = "Remove color row";
            _rowContainer.Add(removeColorRowButton);

            _itemsContainer = new VisualElement();
            _itemsContainer.AddToClassList("palette-container");
            
            for (int i = 0; i < rowData.Count; i++)
            {
                var item = _factory.CreateItem(rowIndex, i, rowData[i],listView);
                _itemsContainer.Add(item);
            }
            
            _rowContainer.Add(_itemsContainer);

            if (_factory.CanAddMoreItems(rowIndex))
            {
                var addColorButton = new EditorToolbarButton(() => {
                    _factory.OnAddItem(rowIndex);
                    listView.Rebuild();
                });
                addColorButton.AddIcon("CreateAddNew");
                addColorButton.AddToClassList("palette-add-item-button");
                addColorButton.SetIconSize(12, 12);
                _rowContainer.Add(addColorButton);
            }

            
        }
        
        /*private readonly VisualElement m_contentContainer;
        private readonly EditorToolbarButton m_addItemButton;
        private readonly EditorToolbarButton m_removeRowButton;
        private readonly Func<TItemData, int, TItemVisual> m_makeVisual;
        

        public PaletteListRow(Func<TItemData, int, TItemVisual> makeVisual) {
            AddToClassList("palette-row");
            
            var rowContainer = new VisualElement { style = { flexDirection = FlexDirection.Row, flexGrow = 1 } };

            // Remove Row Button
            m_removeRowButton = new EditorToolbarButton(null);
            m_removeRowButton.AddIcon("Toolbar Minus");
            m_removeRowButton.SetIconSize(12, 12);
            m_removeRowButton.AddToClassList("palette-remove-button");
            m_removeRowButton.tooltip = "Remove row";
            rowContainer.Add(m_removeRowButton);

            // Items container
            m_contentContainer = new VisualElement();
            m_contentContainer.AddToClassList("palette-container");
            m_contentContainer.style.flexGrow = 1;
            m_contentContainer.style.flexDirection = FlexDirection.Row;

            // Add Item Button
            m_addItemButton = new EditorToolbarButton(null);
            m_addItemButton.AddIcon("CreateAddNew");
            m_addItemButton.SetIconSize(12, 12);
            m_addItemButton.AddToClassList("palette-add-item-button");
            m_addItemButton.style.display = DisplayStyle.None;
            m_contentContainer.Add(m_addItemButton);

            rowContainer.Add(m_contentContainer);
            Add(rowContainer);
            
            RegisterCallback<MouseEnterEvent>(_ => m_addItemButton.style.display = DisplayStyle.Flex);
            RegisterCallback<MouseLeaveEvent>(_ => m_addItemButton.style.display = DisplayStyle.None);
            
            m_makeVisual = makeVisual;
        }
        
        public void Bind(
            IList<TItemData> rowData,
            VisualElement element,
            int rowIndex,
            Action<int> onRemoveRow,
            Action<int> onAddItem,
            Action<int, int> onItemRemoved)
        {
            element.Clear();
            // Add visual items
            for (int i = 0; i < rowData.Count; i++) {
                int itemIndex = i;
                var visual = m_makeVisual(rowData[i], itemIndex);

                // Contextual "remove item" via callback
                if (visual is VisualElement ve) {
                    ve.AddManipulator(new ContextualMenuManipulator(evt => {
                        evt.menu.AppendAction("Remove", _ => onItemRemoved?.Invoke(rowIndex, itemIndex));
                    }));;
                }
                m_contentContainer.Insert(itemIndex, visual);
            }

            // Configure add button
            m_addItemButton.clicked -= null;
            m_addItemButton.clicked += () => onAddItem?.Invoke(rowIndex);

            // Example limit — could be passed in as param
            m_addItemButton.style.display = rowData.Count < 10 ? DisplayStyle.None : DisplayStyle.Flex;

            // Configure row removal
            m_removeRowButton.clicked -= null;
            m_removeRowButton.clicked += () => onRemoveRow?.Invoke(rowIndex);
        }*/
    }
}
