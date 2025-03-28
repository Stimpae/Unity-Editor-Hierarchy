using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hierarchy.Data {
    [FilePath("Assets/Editor/Hierarchy/HierarchySettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class HierarchySettings : ScriptableSingleton<HierarchySettings> {
        public List<ShortcutPreference> shortcuts = new List<ShortcutPreference>();

        public void Save() {
            Save(true);
        }
    }
    
    [Serializable]
    public class ShortcutPreference {
        public string id;
        public string name;
        public string description;
        public KeyCode key = KeyCode.None;
        public EMouseButtonType mouseButton;
        public EventType eventType = EventType.Ignore;
        public bool useCtrl;
        public bool useShift;
        public bool useAlt;

        public ShortcutPreference() {
        }

        public ShortcutPreference(string id, string name, string description) {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        public string GetDisplayString() {
            List<string> parts = new List<string>();

            if (useCtrl) parts.Add("Ctrl");
            if (useAlt) parts.Add("Alt");
            if (useShift) parts.Add("Shift");

            if (key != KeyCode.None) {
                parts.Add(key.ToString());
            }
            else if (mouseButton >= 0) {
                string mouseAction = eventType == EventType.MouseDown ? "Click" : "Release";
                string mouseButtonAction = this.mouseButton == 0
                    ? "Left"
                    : (this.mouseButton == EMouseButtonType.RIGHT ? "Right" : "Middle");
                parts.Add($"{mouseButtonAction} {mouseAction}");
            }

            return string.Join(" + ", parts);
        }
    }
}