using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hierarchy {
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyEventHandler {
        private readonly Dictionary<EventType, Action<Event>> m_eventHandlers = new Dictionary<EventType, Action<Event>>();
        private readonly Dictionary<KeyCode, Action<Event>> m_keyDownHandlers = new Dictionary<KeyCode, Action<Event>>();
        private readonly Dictionary<KeyCode, Action<Event>> m_keyUpHandlers = new Dictionary<KeyCode, Action<Event>>();
        private readonly Dictionary<int, Action<Event>> m_mouseDownHandlers = new Dictionary<int, Action<Event>>();
        private readonly Dictionary<int, Action<Event>> m_mouseUpHandlers = new Dictionary<int, Action<Event>>();
        private readonly Dictionary<MouseComboKey, Action<Event>> m_mouseComboHandlers = new Dictionary<MouseComboKey, Action<Event>>();

        public Vector2 MousePosition { get; private set; }
        public bool EventUsed { get; private set; }

        public struct MouseComboKey : IEquatable<MouseComboKey> {
            public readonly EMouseButtonType button;
            public readonly EventType type;
            public readonly bool ctrl;
            public readonly bool shift;
            public readonly bool alt;

            public MouseComboKey(EMouseButtonType button, EventType type, bool ctrl, bool shift, bool alt) {
                this.button = button;
                this.type = type;
                this.ctrl = ctrl;
                this.shift = shift;
                this.alt = alt;
            }

            public bool Equals(MouseComboKey other) {
                return button == other.button && type == other.type && ctrl == other.ctrl && shift == other.shift && alt == other.alt;
            }

            public override bool Equals(object obj) {
                return obj is MouseComboKey other && Equals(other);
            }

            public override int GetHashCode() {
                unchecked {
                    int hashCode = (int)button;
                    hashCode = (hashCode * 397) ^ (int)type;
                    hashCode = (hashCode * 397) ^ (ctrl ? 1 : 0);
                    hashCode = (hashCode * 397) ^ (shift ? 1 : 0);
                    hashCode = (hashCode * 397) ^ (alt ? 1 : 0);
                    return hashCode;
                }
            }
        }
        
        public void ClearAllHandlers() { 
            m_eventHandlers.Clear();
            m_keyDownHandlers.Clear();
            m_keyUpHandlers.Clear();
            m_mouseDownHandlers.Clear();
            m_mouseUpHandlers.Clear();
            m_mouseComboHandlers.Clear();
        }

        public bool ProcessEvent() {
            Event e = Event.current;
            if (e == null) return false;

            EventUsed = false;
            MousePosition = e.mousePosition;
            
            if (m_eventHandlers.TryGetValue(e.type, out Action<Event> handler)) {
                handler?.Invoke(e);
                if (e.type != EventType.Repaint && e.type != EventType.Layout) {
                    EventUsed = true;
                }
            }

            if (e.type == EventType.KeyDown && m_keyDownHandlers.TryGetValue(e.keyCode, out Action<Event> keyDownHandler)) {
                keyDownHandler?.Invoke(e);
                EventUsed = true;
            }

            if (e.type == EventType.KeyUp && m_keyUpHandlers.TryGetValue(e.keyCode, out Action<Event> keyUpHandler)) {
                keyUpHandler?.Invoke(e);
                EventUsed = true;
            }

            if (e.type == EventType.MouseDown && m_mouseDownHandlers.TryGetValue(e.button, out Action<Event> mouseDownHandler)) {
                mouseDownHandler?.Invoke(e);
                EventUsed = true;
            }

            if (e.type == EventType.MouseUp && m_mouseUpHandlers.TryGetValue(e.button, out Action<Event> mouseUpHandler)) {
                mouseUpHandler?.Invoke(e);
                EventUsed = true;
            }

            var mouseButton = (EMouseButtonType)e.button;
            MouseComboKey comboKey = new MouseComboKey(mouseButton, e.type, e.control, e.shift, e.alt);

            if ((e.type == EventType.MouseDown || e.type == EventType.MouseUp) && m_mouseComboHandlers.TryGetValue(comboKey, out Action<Event> mouseComboHandler)) {
                mouseComboHandler?.Invoke(e);
                EventUsed = true;
            }

            if (EventUsed) {
                e.Use();
            }

            return EventUsed;
        }

        public void RegisterEventHandler(EventType eventType, Action<Event> handler) {
            if (!m_eventHandlers.TryAdd(eventType, handler)) {
                m_eventHandlers[eventType] += handler;
            }
        }

        public void UnregisterEventHandler(EventType eventType, Action<Event> handler) {
            if (m_eventHandlers.ContainsKey(eventType)) {
                m_eventHandlers[eventType] -= handler;
                if (m_eventHandlers[eventType] == null) {
                    m_eventHandlers.Remove(eventType);
                }
            }
        }

        public void RegisterKeyDownHandler(KeyCode keyCode, Action<Event> handler) {
            if (!m_keyDownHandlers.TryAdd(keyCode, handler)) {
                m_keyDownHandlers[keyCode] += handler;
            }
        }

        public void UnregisterKeyDownHandler(KeyCode keyCode, Action<Event> handler) {
            if (m_keyDownHandlers.ContainsKey(keyCode)) {
                m_keyDownHandlers[keyCode] -= handler;
                if (m_keyDownHandlers[keyCode] == null) {
                    m_keyDownHandlers.Remove(keyCode);
                }
            }
        }

        public void RegisterKeyUpHandler(KeyCode keyCode, Action<Event> handler) {
            if (!m_keyUpHandlers.TryAdd(keyCode, handler)) {
                m_keyUpHandlers[keyCode] += handler;
            }
        }

        public void UnregisterKeyUpHandler(KeyCode keyCode, Action<Event> handler) {
            if (m_keyUpHandlers.ContainsKey(keyCode)) {
                m_keyUpHandlers[keyCode] -= handler;
                if (m_keyUpHandlers[keyCode] == null) {
                    m_keyUpHandlers.Remove(keyCode);
                }
            }
        }

        public void RegisterMouseDownHandler(int button, Action<Event> handler) {
            if (!m_mouseDownHandlers.TryAdd(button, handler)) {
                m_mouseDownHandlers[button] += handler;
            }
        }

        public void UnregisterMouseDownHandler(int button, Action<Event> handler) {
            if (m_mouseDownHandlers.ContainsKey(button)) {
                m_mouseDownHandlers[button] -= handler;
                if (m_mouseDownHandlers[button] == null) {
                    m_mouseDownHandlers.Remove(button);
                }
            }
        }

        public void RegisterMouseUpHandler(int button, Action<Event> handler) {
            if (!m_mouseUpHandlers.TryAdd(button, handler)) {
                m_mouseUpHandlers[button] += handler;
            }
        }

        public void UnregisterMouseUpHandler(int button, Action<Event> handler) {
            if (m_mouseUpHandlers.ContainsKey(button)) {
                m_mouseUpHandlers[button] -= handler;
                if (m_mouseUpHandlers[button] == null) {
                    m_mouseUpHandlers.Remove(button);
                }
            }
        }

        public void RegisterMouseComboHandler(EMouseButtonType button, EventType type, bool ctrl, bool shift, bool alt, Action<Event> handler) {
            MouseComboKey key = new MouseComboKey(button, type, ctrl, shift, alt);

            if (!m_mouseComboHandlers.TryAdd(key, handler)) {
                m_mouseComboHandlers[key] += handler;
            }
        }

        public void UnregisterMouseComboHandler(EMouseButtonType button, EventType type, bool ctrl, bool shift, bool alt, Action<Event> handler) {
            MouseComboKey key = new MouseComboKey(button, type, ctrl, shift, alt);

            if (m_mouseComboHandlers.ContainsKey(key)) {
                m_mouseComboHandlers[key] -= handler;
                if (m_mouseComboHandlers[key] == null) {
                    m_mouseComboHandlers.Remove(key);
                }
            }
        }

        public bool IsMouseInRect(Rect rect) {
            return rect.Contains(MousePosition);
        }

        public bool IsMouseDownInRect(Rect rect, int button = 0) {
            return Event.current.type == EventType.MouseDown && Event.current.button == button && rect.Contains(MousePosition);
        }

        public bool IsMouseUpInRect(Rect rect, int button = 0) {
            return Event.current.type == EventType.MouseUp && Event.current.button == button && rect.Contains(MousePosition);
        }

        public bool IsMouseComboInRect(Rect rect, int button, EventType type, bool ctrl, bool shift, bool alt) {
            return Event.current.type == type && Event.current.button == button && Event.current.control == ctrl && Event.current.shift == shift && Event.current.alt == alt && rect.Contains(MousePosition);
        }

        public bool IsKeyCombo(KeyCode key, bool ctrl = false, bool shift = false, bool alt = false) {
            if (Event.current.type != EventType.KeyDown && Event.current.type != EventType.KeyUp) return false;
            return Event.current.keyCode == key && Event.current.control == ctrl && Event.current.shift == shift && Event.current.alt == alt;
        }
    }
}