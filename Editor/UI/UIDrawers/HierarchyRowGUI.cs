using System;
using Hierarchy.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hierarchy.GUI {
    public class HierarchyRowGUI : IDisposable {
        private readonly EditorWindow m_window;
        
        // Special icon content
        private GUIContent m_lockIcon;
        
        // Style cache
        private GUIStyle m_labelStyle;
        private GUIStyle m_boldLabelStyle;
        
        public HierarchyRowGUI(EditorWindow window) {
            m_window = window;
            
            // Initialize styles and content
            m_lockIcon = EditorGUIUtility.IconContent("LockIcon-On");
            
            // Set up styles
            m_labelStyle = new GUIStyle(EditorStyles.label);
            m_boldLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            
            // Register for hierarchy changed events
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
        
        private void OnHierarchyChanged() {
            HierarchyGameObjectData.instance.CleanupDeletedObjects();
            HierarchyGameObjectData.instance.Save();
        }
        
        public void OnGUI(int instanceId, Rect selectionRect) {
            // Get the GameObject from the instance ID
            GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (go == null) return;
            
            // Get object data directly from HierarchyData using GlobalObjectId
            GameObjectData data = HierarchyGameObjectData.instance.GetGameObjectData(go);
            if (data == null) return;
            
            // Apply data to the GameObject (for things like lock state)
            data.ApplyToGameObject(go);
            
            // Draw custom GUI elements
            DrawRowGUI(go, selectionRect, data);
            
        }
        
        private void DrawRowGUI(GameObject go, Rect rowRect, GameObjectData data) {
            // Draw custom background if needed
            if (data.customColor != Color.white) {
                // Create a slightly inset rect to avoid covering Unity's selection highlight
                Rect bgRect = new Rect(rowRect);
                bgRect.x += 16; // Offset to not cover the foldout arrow
                
                EditorGUI.DrawRect(bgRect, data.customColor);
            }
            
            // Draw lock icon if locked
            if (data.isLocked) {
                Rect lockRect = new Rect(rowRect);
                lockRect.x = rowRect.xMax - 20;
                lockRect.width = 16;
                
                UnityEngine.GUI.Label(lockRect, m_lockIcon);
            }
            
            // Draw custom label if needed
            if (!string.IsNullOrEmpty(data.customLabel)) {
                Rect labelRect = new Rect(rowRect);
                labelRect.x = rowRect.xMax - 80;
                labelRect.width = 60;
                
                UnityEngine.GUI.Label(labelRect, data.customLabel, m_labelStyle);
            }
        }
        
        public void Dispose() {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            m_lockIcon = null;
            m_labelStyle = null;
            m_boldLabelStyle = null;
        }

    }
}