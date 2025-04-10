using System;
using Hierarchy.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hierarchy {
    public class HierarchyPaletteWindow : InstancedWindow<HierarchyPaletteWindow> {
        
        public override void ShowPopup(Vector2 pos) {
            base.ShowPopup(pos);
            instance.position = instance.position
                .SetPosition(pos)
                .SetSize(200, 150);
            
            Initialize(pos);
        }
        
        private void Initialize(Vector2 targetPosition) {
            
        }

        private void OnLostFocus() {
            Close();
        }
    }
}