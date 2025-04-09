using System;
using System.Collections.Generic;
using System.Linq;
using Hierarchy.Data;
using UnityEngine;

namespace Hierarchy {
    public enum EMouseButtonType {
        LEFT = 0,
        RIGHT = 1,
        MIDDLE = 2
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyShortcutHandler : IDisposable {
        private readonly HierarchyEventHandler m_eventHandler;
        
        private readonly Dictionary<string, Action<Event>> m_shortcuts = new Dictionary<string, Action<Event>>();
        
        // Store registered handlers for potential unregistration
        private readonly Dictionary<string, (KeyCode None, EventType type, EMouseButtonType button, bool ctrl, bool shift, bool alt, Action<Event> handler)> m_registeredHandlers = new Dictionary<string, (KeyCode key, EventType type, EMouseButtonType button,
            bool ctrl, bool shift, bool alt, Action<Event> handler)>();
        
        public void Dispose() {
            m_shortcuts.Clear();
            m_registeredHandlers.Clear();
            m_eventHandler.ClearAllHandlers();
        }
        
        public HierarchyShortcutHandler(HierarchyEventHandler eventHandler) {
            m_eventHandler = eventHandler;
        }
        
        public void RegisterKeyShortcut(string id,KeyCode key, Action<Event> handler, bool ctrl = false, bool shift = false, bool alt = false) {
            m_shortcuts[id] = handler;
            Action<Event> wrappedHandler = (e) => {
                if (e.control == ctrl && e.shift == shift && e.alt == alt) {
                    handler?.Invoke(e);
                }
            };
            m_eventHandler.RegisterKeyDownHandler(key, wrappedHandler);
            m_registeredHandlers[id] = (key, EventType.KeyDown, 0, ctrl, shift, alt, wrappedHandler);
        }
        
        public void RegisterMouseShortcut(string id, EMouseButtonType button, Action<Event> handler, EventType type, bool ctrl = false, bool shift = false, bool alt = false) {
            m_shortcuts[id] = handler;
            Action<Event> wrappedHandler = (e) => {
                handler?.Invoke(e);
            };
            m_eventHandler.RegisterMouseComboHandler(button, type, ctrl, shift, alt, wrappedHandler);
            m_registeredHandlers[id] = (KeyCode.None, type, button, ctrl, shift, alt, handler: wrappedHandler);
        }
        
        /// <summary>
        /// Remove a registered shortcut
        /// </summary>
        public void UnregisterShortcut(string id) {
            if (m_shortcuts.ContainsKey(id) && m_registeredHandlers.ContainsKey(id)) {
                var handlerInfo = m_registeredHandlers[id];
                if (handlerInfo.type == EventType.KeyDown) {
                    m_eventHandler.UnregisterKeyDownHandler(handlerInfo.None, handlerInfo.handler);
                } else {
                    m_eventHandler.UnregisterMouseComboHandler(
                        handlerInfo.button, 
                        handlerInfo.type, 
                        handlerInfo.ctrl, 
                        handlerInfo.shift, 
                        handlerInfo.alt, 
                        handlerInfo.handler
                    );
                }
                
                m_shortcuts.Remove(id);
                m_registeredHandlers.Remove(id);
            }
        }
    }
}