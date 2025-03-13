using System;
using Hierarchy.Utils;
using UnityEditor;
using UnityEngine;

namespace Hierarchy.GUI {
    public class HierarchyGUI : IDisposable {
        public HierarchyRowGUI RowGUI { get; }
        public HierarchyHeaderGUI HeaderGUI { get; }

        private readonly EditorWindow m_window;
        private readonly int m_windowId;
        private bool m_disposed;

        public HierarchyGUI(EditorWindow window) {
            m_window = window;
            m_windowId = window.GetHashCode();
            
            HeaderGUI = new HierarchyHeaderGUI(window);
            RowGUI = new HierarchyRowGUI(window);
            
            EditorApplication.hierarchyWindowItemOnGUI -= DrawRowGUI;
            EditorApplication.hierarchyWindowItemOnGUI += DrawRowGUI;
        }
        
        public void DrawHierarchyGUI(Action beforeAction = null, Action afterAction = null) {
            if (m_window == null) {
                Debug.LogWarning($"Cannot draw hierarchy GUI: window is null");
                return;
            }
            
            HeaderGUI.DrawHeaderGUI();
            beforeAction?.Invoke();

            try {
                // Call standard EditorWindow methods
                m_window.InvokeMethod("DoSceneHierarchy");
                m_window.InvokeMethod("ExecuteCommands");
            }
            catch (Exception exception) {
                if (exception.InnerException is ExitGUIException)
                    throw exception.InnerException;
                else
                    throw;
            }
            finally {
                afterAction?.Invoke();
            }
        }
        
        private void DrawRowGUI(int instanceId, Rect selectionRect) {
        }

        public void Clear() {
            if (!m_disposed) {
                EditorApplication.hierarchyWindowItemOnGUI -= DrawRowGUI;
                m_disposed = true;
            }
        }

        public void Dispose() {
            Clear();
            GC.SuppressFinalize(this);
        }
    }
}
