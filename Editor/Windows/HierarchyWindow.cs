using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hierarchy {
    public class HierarchyWindow : InstancedWindow<HierarchyWindow> {
        public void CreateGUI() {
            // Create a new VisualElement to be the root of our UI
            VisualElement root = rootVisualElement;

            // Create a label and add it to the root
            Label label = new Label("Hierarchy Window");
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(label);

            // Add more UI elements here
        }
    }
}