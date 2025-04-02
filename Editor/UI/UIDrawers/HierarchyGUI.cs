using System;
using System.Collections.Generic;
using Hierarchy.Libraries;
using Hierarchy.Utils;
using UnityEditor;
using UnityEngine;


namespace Hierarchy.GUI {
    public class HierarchyGUI : IDisposable {
        private readonly EditorWindow m_window;
        private readonly int m_windowId;
        private bool m_disposed;

        private Rect m_rowRect;
        private int m_hoveredInstanceId;

        private HierarchyEventHandler m_eventHandler;
        private HierarchyShortcutHandler m_shortcutHandler;
        private HierarchyRowGUI m_rowGUI;
        private HierarchyToolbarGUI m_toolbarGUI;

        private readonly Dictionary<int, Rect> m_instanceRects = new Dictionary<int, Rect>();

        private const float K_TOP_BAR_HEIGHT = 26f;
        private const float K_FAVORITES_BAR_HEIGHT = 26f;
        
        public void Dispose() {
            if (m_shortcutHandler != null) {
                m_shortcutHandler.UnregisterShortcut("Show Something");
                m_shortcutHandler.Dispose();
                m_shortcutHandler = null;
            }

            m_toolbarGUI.Dispose();
            m_rowGUI.Dispose();

            EditorApplication.hierarchyWindowItemOnGUI -= OnRowGUI;
            m_instanceRects.Clear();

            m_toolbarGUI = null;
            m_rowGUI = null;
            m_eventHandler = null;
            m_shortcutHandler = null;
        }

        public HierarchyGUI(EditorWindow window, HierarchyEventHandler eventHandler) {
            m_window = window;
            m_windowId = window.GetHashCode();

            m_toolbarGUI = new HierarchyToolbarGUI(window);
            m_rowGUI = new HierarchyRowGUI(window);

            EditorApplication.hierarchyWindowItemOnGUI -= OnRowGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnRowGUI;

            m_eventHandler = eventHandler;
            m_shortcutHandler = new HierarchyShortcutHandler(m_eventHandler);

            m_shortcutHandler.RegisterMouseShortcut("Show Something", EMouseButtonType.LEFT, (e) => {
                if (TryGetHoveredObject(out var hoveredObject, out var hoveredRect)) {
                    var position =
                        GUIUtility.GUIToScreenPoint(new Vector2(Event.current.mousePosition.x + 20,
                            hoveredRect.y - 13));
                    HierarchyPaletteWindow.Instance.ShowPopup(position);
                }
            }, EventType.MouseDown, alt: true);

            // Create our custom header gui with toolkit
            m_toolbarGUI.CreateGUI(K_TOP_BAR_HEIGHT, K_FAVORITES_BAR_HEIGHT);

            m_shortcutHandler.RegisterPreferenceShortcut("Show Something", @event => { Debug.Log("Show Something"); });
        }

        public void OnHierarchyGUI() {
            if (m_window == null) {
                Debug.LogWarning($"Cannot draw hierarchy GUI: window is null");
                return;
            }

            var favoritesVisible = m_toolbarGUI.IsFavoritesVisible;
            float topGap = K_TOP_BAR_HEIGHT + (favoritesVisible ? K_FAVORITES_BAR_HEIGHT : 0);

            float defaultTopBarHeight = 21f;
            float topOffset = topGap - defaultTopBarHeight;

            // Get the original position rect from the window
            Rect posOriginal = (Rect)m_window.GetFieldValue("m_Pos");

            try {
                UnityEngine.GUI.BeginGroup(posOriginal.SetPosition(0, 0)
                    .Inset(0, 0, topOffset, 0));

                m_window.InvokeMethod("DoSceneHierarchy");
                m_window.InvokeMethod("ExecuteCommands");

                UnityEngine.GUI.EndGroup();
            }
            catch (Exception exception) {
                if (exception.InnerException is ExitGUIException)
                    throw exception.InnerException;
                else
                    throw;
            }
        }

        private void OnRowGUI(int instanceId, Rect selectionRect) {
            if (m_window == null) return;
            m_instanceRects[instanceId] = selectionRect;
            m_rowGUI.OnGUI(instanceId, selectionRect);
        }

        private bool TryGetHoveredObject(out GameObject hoveredObject, out Rect hoveredRect) {
            hoveredObject = null;
            hoveredRect = new Rect();

            if (EditorWindow.focusedWindow.GetHashCode() != m_windowId) {
                return false;
            }

            Vector2 mousePos = Event.current.mousePosition;
            Vector2 offsetMousePos = new Vector2(mousePos.x, mousePos.y - 20);

            foreach (var kvp in m_instanceRects) {
                Rect rect = kvp.Value;
                if (rect.Contains(offsetMousePos)) {
                    int hoveredInstanceId = kvp.Key;
                    hoveredObject = EditorUtility.InstanceIDToObject(hoveredInstanceId) as GameObject;
                    hoveredRect = rect;
                    return hoveredObject != null;
                }
            }

            return false;
        }
    }
}