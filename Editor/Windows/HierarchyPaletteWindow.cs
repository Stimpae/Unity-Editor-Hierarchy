using System;
using Hierarchy.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hierarchy {
    public class HierarchyPaletteWindow : InstancedWindow<HierarchyPaletteWindow> {
        public void CreateGUI() {
            // Create a new VisualElement to be the root of our UI
            VisualElement root = rootVisualElement;

            // Create a label and add it to the root
            Label label = new Label("Hierarchy Palette Window");
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(label);

            // Add more UI elements here
        }

        public override void ShowPopup(Vector2 position) {
            base.ShowPopup(position);
            Initialize(position);
        }
        
        private void Initialize(Vector2 targetPosition) {
            
        }

        private void OnLostFocus() {
            Close();
        }
    }
}