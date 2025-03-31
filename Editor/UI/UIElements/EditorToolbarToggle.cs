using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hierarchy.Elements {
    public class EditorToolbarToggle : ToolbarToggle {
        public EditorToolbarToggle() {
            this.AddToClassList("editor-toolbar-toggle");
            var styleSheet = Resources.Load<StyleSheet>("EditorToolbarToggle");
            if (styleSheet != null) styleSheets.Add(styleSheet);
        }
    }
}