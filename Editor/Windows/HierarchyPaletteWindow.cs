using System;
using Hierarchy.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hierarchy {
    public class HierarchyPaletteWindow : InstancedWindow<HierarchyPaletteWindow> {
        public void CreateGUI() {
            VisualElement root = rootVisualElement;
            VisualElement parentRoot = rootVisualElement.parent;
            parentRoot.SetBorderWidth(new VisualElementUtils.ElementFloat4(1,1,1,1));
        }


        public override void ShowPopup(Vector2 position) {
            base.ShowPopup(position);
            
            // override the size here if you want
            Initialize(position);
        }
        
        private void Initialize(Vector2 targetPosition) {
            
        }

        private void OnLostFocus() {
            Close();
        }
    }
}