using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Pastime_Hierarchy.Editor.UI.Components {
    public class PaletteIconItem : VisualElement {
        public PaletteIconItem(int rowIndex, int iconIndex, string iconPath, Action onRemoveIcon) {
            name = $"IconItem_{rowIndex}_{iconIndex}";
            AddToClassList("palette-item");

            var iconContent = EditorGUIUtility.IconContent(iconPath);
            if (iconContent?.image != null) {
                var iconImage = new Image { image = iconContent.image };
                iconImage.AddToClassList("palette-icon-image");
                Add(iconImage);
            }
            
            this.AddManipulator(new ContextualMenuManipulator(evt => {
                evt.menu.AppendAction("Remove Icon", action => {
                    onRemoveIcon?.Invoke();
                });
            }));
        }
    }
}