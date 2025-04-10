using System;
using Hierarchy.Data;
using Pastime_Hierarchy.Editor.UI.Components.RowView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pastime_Hierarchy.Editor.UI.Components {
    public class RowColorItem : IRowItemFactory<Color>
    {
        public VisualElement CreateItem(int rowIndex, int colorIndex, Color color, ListView listView)
        {
            var container = new VisualElement();
            container.name = $"ColorItem_{rowIndex}_{colorIndex}";
            container.AddToClassList("palette-item");

            var colorField = new ColorField
            {
                showAlpha = false,
                showEyeDropper = false,
                value = color,
                tooltip = $"R: {color.r}, G: {color.g}, B: {color.b}, A: {color.a}"
            };

            colorField.RegisterValueChangedCallback(evt =>
            {
                HierarchyPaletteData.instance.ColorRows[rowIndex].UpdateColor(colorIndex, evt.newValue);
                colorField.tooltip = $"R: {evt.newValue.r}, G: {evt.newValue.g}, B: {evt.newValue.b}, A: {evt.newValue.a}";
                listView.RefreshItem(rowIndex);
                listView.Rebuild();
            });

            colorField.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Reset Color", _ =>
                {
                    colorField.value = Color.white;
                    HierarchyPaletteData.instance.ColorRows[rowIndex].UpdateColor(colorIndex, Color.white);
                });

                evt.menu.AppendAction("Remove Color", _ =>
                {
                    HierarchyPaletteData.instance.ColorRows[rowIndex].RemoveColor(colorIndex);
                    listView.RefreshItem(rowIndex);
                    listView.Rebuild();
                });
            }));

            container.Add(colorField);
            return container;
        }

        public void OnAddItem(int rowIndex)
        {
            HierarchyPaletteData.instance.ColorRows[rowIndex].AddColor(Color.white);
        }

        public void OnRemoveRow(int rowIndex)
        {
            HierarchyPaletteData.instance.RemoveColorRow(rowIndex);
        }

        public bool CanAddMoreItems(int rowIndex) {
            return HierarchyPaletteData.instance.ColorRows[rowIndex].colors.Count < 10;
        }

        public bool CanAddMore(int rowIndex)
        {
            return HierarchyPaletteData.instance.ColorRows[rowIndex].colors.Count < 10;
        }
    }

}