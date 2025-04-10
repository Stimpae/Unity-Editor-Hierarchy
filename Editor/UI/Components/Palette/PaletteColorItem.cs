using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pastime_Hierarchy.Editor.UI.Components {
    public class PaletteColorItem : VisualElement {
        public PaletteColorItem( Color color, Action onColorChanged, Action onRemoveColor) {
            AddToClassList("palette-item");

            var colorField = new ColorField {
                showAlpha = false,
                showEyeDropper = false,
                value = color,
            };

            colorField.RegisterValueChangedCallback(evt => {
                onColorChanged?.Invoke();
            });

            colorField.AddManipulator(new ContextualMenuManipulator(evt => {
                evt.menu.AppendAction("Remove Color", action => { onRemoveColor?.Invoke(); });
            }));

            Add(colorField);
        }
    }
}