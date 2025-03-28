using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Hierarchy.Elements {
    public class EditorToolbarToggle : ToolbarToggle {
        public EditorToolbarToggle() {
            this.AddToClassList("editor-toolbar-toggle");
        }
    }
}