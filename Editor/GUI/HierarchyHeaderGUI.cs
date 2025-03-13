using Hierarchy.Utils;
using UnityEditor;
using UnityEngine;

namespace Hierarchy.GUI {
    public class HierarchyHeaderGUI {
        private readonly EditorWindow m_window;
        public HierarchyHeaderGUI(EditorWindow window) {
            m_window = window;
        }
        
        private Rect GetHeaderRect() {
            var windowPos = m_window.position;
            var headerRect = windowPos
                .SetPosition(0, 0)
                .SetHeight(20);
            
            return headerRect;
        }
        
        public void DrawHeaderGUI() {
            // draw the background
            var backgroundColor = new Color32(60, 60, 60, 255);
            var headerRect = GetHeaderRect();
            EditorGUI.DrawRect(headerRect, backgroundColor);
            
            
            //m_window.InvokeMethod("SearchFieldGUI");
        }
    }
}