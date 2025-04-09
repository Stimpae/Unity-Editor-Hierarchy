using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hierarchy {
    /// <summary>
    /// Handles hierarchy-related events and input.
    /// </summary>
    public class HierarchyEventHandler {
        private readonly Dictionary<EventType, Action<Event>> eventHandlers = new();
        private readonly Dictionary<KeyCode, Action<Event>> keyDownHandlers = new();
        private readonly Dictionary<KeyCode, Action<Event>> keyUpHandlers = new();
        private readonly Dictionary<int, Action<Event>> mouseDownHandlers = new();
        private readonly Dictionary<int, Action<Event>> mouseUpHandlers = new();
        private readonly Dictionary<MouseComboKey, Action<Event>> mouseComboHandlers = new();

        public Vector2 MousePosition { get; private set; }
        public bool EventUsed { get; private set; }

        public readonly struct MouseComboKey : IEquatable<MouseComboKey> {
            public readonly EMouseButtonType Button;
            public readonly EventType Type;
            public readonly bool Ctrl, Shift, Alt;

            public MouseComboKey(EMouseButtonType button, EventType type, bool ctrl, bool shift, bool alt) {
                Button = button;
                Type = type;
                Ctrl = ctrl;
                Shift = shift;
                Alt = alt;
            }

            public bool Equals(MouseComboKey other) =>
                Button == other.Button && Type == other.Type && Ctrl == other.Ctrl && Shift == other.Shift && Alt == other.Alt;

            public override bool Equals(object obj) => obj is MouseComboKey other && Equals(other);

            public override int GetHashCode() {
                int hash = (int)Button;
                hash = (hash * 397) ^ (int)Type;
                hash = (hash * 397) ^ (Ctrl ? 1 : 0);
                hash = (hash * 397) ^ (Shift ? 1 : 0);
                hash = (hash * 397) ^ (Alt ? 1 : 0);
                return hash;
            }
        }

        public void ClearAllHandlers() {
            eventHandlers.Clear();
            keyDownHandlers.Clear();
            keyUpHandlers.Clear();
            mouseDownHandlers.Clear();
            mouseUpHandlers.Clear();
            mouseComboHandlers.Clear();
        }

        public bool ProcessEvent() {
            Event e = Event.current;
            if (e == null) return false;

            EventUsed = false;
            MousePosition = e.mousePosition;

            if (eventHandlers.TryGetValue(e.type, out var handler)) {
                handler?.Invoke(e);
                if (e.type != EventType.Repaint && e.type != EventType.Layout) EventUsed = true;
            }

            if (e.type == EventType.KeyDown && keyDownHandlers.TryGetValue(e.keyCode, out var keyDownHandler)) {
                keyDownHandler?.Invoke(e);
                EventUsed = true;
            }

            if (e.type == EventType.KeyUp && keyUpHandlers.TryGetValue(e.keyCode, out var keyUpHandler)) {
                keyUpHandler?.Invoke(e);
                EventUsed = true;
            }

            if (e.type == EventType.MouseDown && mouseDownHandlers.TryGetValue(e.button, out var mouseDownHandler)) {
                mouseDownHandler?.Invoke(e);
                EventUsed = true;
            }

            if (e.type == EventType.MouseUp && mouseUpHandlers.TryGetValue(e.button, out var mouseUpHandler)) {
                mouseUpHandler?.Invoke(e);
                EventUsed = true;
            }

            var comboKey = new MouseComboKey((EMouseButtonType)e.button, e.type, e.control, e.shift, e.alt);
            if ((e.type == EventType.MouseDown || e.type == EventType.MouseUp) && mouseComboHandlers.TryGetValue(comboKey, out var mouseComboHandler)) {
                mouseComboHandler?.Invoke(e);
                EventUsed = true;
            }

            if (EventUsed) e.Use();
            return EventUsed;
        }

        public void RegisterEventHandler(EventType eventType, Action<Event> handler) => AddHandler(eventHandlers, eventType, handler);

        public void UnregisterEventHandler(EventType eventType, Action<Event> handler) => RemoveHandler(eventHandlers, eventType, handler);

        public void RegisterKeyDownHandler(KeyCode keyCode, Action<Event> handler) => AddHandler(keyDownHandlers, keyCode, handler);

        public void UnregisterKeyDownHandler(KeyCode keyCode, Action<Event> handler) => RemoveHandler(keyDownHandlers, keyCode, handler);

        public void RegisterKeyUpHandler(KeyCode keyCode, Action<Event> handler) => AddHandler(keyUpHandlers, keyCode, handler);

        public void UnregisterKeyUpHandler(KeyCode keyCode, Action<Event> handler) => RemoveHandler(keyUpHandlers, keyCode, handler);

        public void RegisterMouseDownHandler(int button, Action<Event> handler) => AddHandler(mouseDownHandlers, button, handler);

        public void UnregisterMouseDownHandler(int button, Action<Event> handler) => RemoveHandler(mouseDownHandlers, button, handler);

        public void RegisterMouseUpHandler(int button, Action<Event> handler) => AddHandler(mouseUpHandlers, button, handler);

        public void UnregisterMouseUpHandler(int button, Action<Event> handler) => RemoveHandler(mouseUpHandlers, button, handler);

        public void RegisterMouseComboHandler(EMouseButtonType button, EventType type, bool ctrl, bool shift, bool alt, Action<Event> handler) {
            var key = new MouseComboKey(button, type, ctrl, shift, alt);
            AddHandler(mouseComboHandlers, key, handler);
        }

        public void UnregisterMouseComboHandler(EMouseButtonType button, EventType type, bool ctrl, bool shift, bool alt, Action<Event> handler) {
            var key = new MouseComboKey(button, type, ctrl, shift, alt);
            RemoveHandler(mouseComboHandlers, key, handler);
        }

        public bool IsMouseInRect(Rect rect) => rect.Contains(MousePosition);

        public bool IsMouseDownInRect(Rect rect, int button = 0) =>
            Event.current.type == EventType.MouseDown && Event.current.button == button && rect.Contains(MousePosition);

        public bool IsMouseUpInRect(Rect rect, int button = 0) =>
            Event.current.type == EventType.MouseUp && Event.current.button == button && rect.Contains(MousePosition);

        public bool IsMouseComboInRect(Rect rect, int button, EventType type, bool ctrl, bool shift, bool alt) =>
            Event.current.type == type && Event.current.button == button && Event.current.control == ctrl &&
            Event.current.shift == shift && Event.current.alt == alt && rect.Contains(MousePosition);

        public bool IsKeyCombo(KeyCode key, bool ctrl = false, bool shift = false, bool alt = false) =>
            (Event.current.type == EventType.KeyDown || Event.current.type == EventType.KeyUp) &&
            Event.current.keyCode == key && Event.current.control == ctrl && Event.current.shift == shift && Event.current.alt == alt;

        private static void AddHandler<T>(Dictionary<T, Action<Event>> handlers, T key, Action<Event> handler) {
            if (!handlers.TryAdd(key, handler)) handlers[key] += handler;
        }

        private static void RemoveHandler<T>(Dictionary<T, Action<Event>> handlers, T key, Action<Event> handler) {
            if (handlers.ContainsKey(key)) {
                handlers[key] -= handler;
                if (handlers[key] == null) handlers.Remove(key);
            }
        }
    }
}