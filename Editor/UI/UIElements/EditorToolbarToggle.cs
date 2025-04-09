using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hierarchy.Elements {
    public class EditorToolbarToggle : ToolbarToggle {
        private string m_prefsKey;
        private bool m_defaultValue;
        private bool m_initialized = false;
        private System.Action<bool> m_onValueChanged;
        private Image m_iconImage;

        public EditorToolbarToggle(string key, bool defaultValue = true, System.Action<bool> onValueChanged = null) {
            AddToClassList("editor-toolbar-toggle");
            var styleSheet = Resources.Load<StyleSheet>("EditorToolbarToggle");
            if (styleSheet != null) styleSheets.Add(styleSheet);
            
            
            this.m_prefsKey = key;
            this.m_defaultValue = defaultValue;
            this.m_onValueChanged = onValueChanged;
            
            // Load the saved value from EditorPrefs
            bool savedValue = EditorPrefs.GetBool(m_prefsKey, defaultValue);
            this.SetValueWithoutNotify(savedValue);
            
            // Register callback to save changes
            this.RegisterValueChangedCallback(OnToggleValueChanged);
            
            // Manually trigger the onValueChanged callback with the initial value
            if (onValueChanged != null) {
                onValueChanged.Invoke(savedValue);
            }
        }

        public sealed override void SetValueWithoutNotify(bool newValue) {
            base.SetValueWithoutNotify(newValue);
        }
        
        private void OnToggleValueChanged(ChangeEvent<bool> evt) {
            if (!string.IsNullOrEmpty(m_prefsKey)) {
                EditorPrefs.SetBool(m_prefsKey, evt.newValue);
            }
            
            // Call the action if provided
            if (m_onValueChanged != null) {
                m_onValueChanged.Invoke(evt.newValue);
            }
        }

        /// <summary>
        /// Get the current saved preference value
        /// </summary>
        /// <returns>The current preference value or default if not initialized</returns>
        public bool GetPreferenceValue() {
            if (string.IsNullOrEmpty(m_prefsKey)) {
                return m_defaultValue;
            }
            return EditorPrefs.GetBool(m_prefsKey, m_defaultValue);
        }

        /// <summary>
        /// Reset the toggle to its default value
        /// </summary>
        public void ResetToDefault() {
            if (!string.IsNullOrEmpty(m_prefsKey)) {
                this.value = m_defaultValue;
                EditorPrefs.SetBool(m_prefsKey, m_defaultValue);
                
                // Call the action if provided
                if (m_onValueChanged != null) {
                    m_onValueChanged.Invoke(m_defaultValue);
                }
            }
        }
        
        /// <summary>
        /// Add an icon to the toggle
        /// </summary>
        /// <param name="iconName">Name of the icon from EditorGUIUtility.IconContent</param>
        public void AddIcon(string iconName) {
            if (m_iconImage == null) {
                m_iconImage = new Image();
                this.hierarchy.Insert(0, m_iconImage);
            }

            var icon = EditorGUIUtility.IconContent(iconName).image;
            m_iconImage.image = icon;
        }

        /// <summary>
        /// Add a custom texture as an icon to the toggle
        /// </summary>
        /// <param name="texture">The texture to use as icon</param>
        public void AddIcon(Texture2D texture) {
            if (m_iconImage == null) {
                m_iconImage = new Image();
                this.hierarchy.Insert(0, m_iconImage);
            }

            m_iconImage.image = texture;
        }

        /// <summary>
        /// Set icon size
        /// </summary>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public void SetIconSize(float width, float height) {
            if (m_iconImage != null) {
                m_iconImage.style.width = width;
                m_iconImage.style.height = height;
            }
        }
    }
}