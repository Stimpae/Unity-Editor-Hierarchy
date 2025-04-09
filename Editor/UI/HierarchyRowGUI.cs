using System;
using Hierarchy.Data;
using Hierarchy.Libraries;
using Hierarchy.Utils;
using UnityEditor;
using UnityEngine;

namespace Hierarchy.GUI {
    public class HierarchyRowGUI : IDisposable {
        private readonly EditorWindow m_window;
        
        private bool m_isHovered;
        private bool m_isSelected;
        
        
        public HierarchyRowGUI(EditorWindow window) {
            m_window = window;
            
            // Register for hierarchy changed events
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
        
        private void OnHierarchyChanged() {
            HierarchyGameObjectData.instance.CleanupDeletedObjects();
            HierarchyGameObjectData.instance.Save();
        }
        
        public void OnGUI(int instanceId, Rect selectionRect) {
            if (Event.current.type != EventType.Repaint) return;
            if (m_window == null) return;
            if (Event.current.type == EventType.Layout) return;
            
            // Get the GameObject from the instance ID
            GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (go == null) return;
            
            GameObjectData data = HierarchyGameObjectData.instance.GetGameObjectData(go);
            
            // Apply data to the GameObject (for things like lock state)
            data.ApplyToGameObject(go);
            
            DrawRowGUI(go, selectionRect, data);
        }
        
        private void DrawRowGUI(GameObject go, Rect rowRect, GameObjectData data) {
            Rect fullRect = new Rect(32, rowRect.y, m_window.position.width + 4, rowRect.height);
            // Handle states
            HandleStates(go, fullRect, out Color colour);
            
            HideDefaultLabelGUI(rowRect, go, colour);
            HideDefaultIconGUI(rowRect, data, colour);
            
            // Draw Labels
            // Draw Custom Icons
            // Draw zebra background effect
            DrawZebraBackground(go, fullRect);
        }

        private void HandleStates(GameObject go, Rect rowRect, out Color backgroundColor) {
            m_isHovered = rowRect.Contains(Event.current.mousePosition);
            m_isSelected = Selection.activeGameObject == go;
            
            backgroundColor = Color.clear;
        }
        
        private void HideDefaultLabelGUI(Rect rowRect, GameObject go, Color backgroundColour) {
            var newColour = backgroundColour;
            var normal = ColorUtils.AlternatedRowsBackground;
            var hovered = ColorUtils.ToolbarButtonHoverBackground;
            var selected = ColorUtils.ListItemSelectedBackground;
            
            if (m_isHovered && !m_isSelected) newColour = hovered;
            else if (m_isSelected) newColour = selected;
            else newColour = normal;
            
            var labelWidth = UnityEngine.GUI.skin.label.CalcSize(new GUIContent(go.name)).x;
            var rect = rowRect.MoveX(16).SetWidth(labelWidth);
            EditorGUI.DrawRect(rect, newColour);
        }
        
        private void HideDefaultIconGUI(Rect rowRect, GameObjectData data, Color backgroundColour) {
            // if we don't have a custom icon, draw the default one
            var rect = rowRect.SetWidth(16);
            EditorGUI.DrawRect(rect, backgroundColour);
        }
        
        private void DrawZebraBackground(GameObject go, Rect rowRect) {
            bool isEvenRow = Mathf.FloorToInt(rowRect.y / 16f) % 2 == 0;
            bool isDarkMode = EditorGUIUtility.isProSkin;
            
            var evenColour = ColorUtils.WithAlpha(isDarkMode ? Color.black : Color.white, 0.066f);
            var oddColour = ColorUtils.WithAlpha(isDarkMode ? Color.black : Color.white, 0);
            Color backgroundColor = isEvenRow ? evenColour : oddColour;
            EditorGUI.DrawRect(rowRect, backgroundColor);
        }
        
        public void Dispose() {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        }
    }
}