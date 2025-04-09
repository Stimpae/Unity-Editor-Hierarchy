using System;
using Hierarchy.Utils;
using UnityEngine;
using UnityEditor;

namespace Hierarchy {
    public abstract class InstancedWindow<T> : EditorWindow where T : EditorWindow {
        // Static reference to the instance
        protected static T instance;
        
        protected Action OnClose;
        
        // Public accessor with lazy initialization
        public static T Instance {
            get {
                if (instance == null) {
                    CreateInstance();
                }
                return instance;
            }
        }

        /// <summary>
        /// Creates and shows a new instance of the window, positioning it as specified
        /// </summary>
        /// <returns>The instance of the window</returns>
        private static T CreateInstance() {
            // Find any existing instance
            instance = ScriptableObject.CreateInstance<T>();
            return instance;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public virtual void ShowPopup(Vector2 position) {
            instance.ShowPopup();
            instance.position = instance.position.SetPosition(position).SetSize(200, 150);
            instance.Focus();
            
        }
        
        public virtual void ShowWindow(string title) {
            instance.Show();
            instance.titleContent = new GUIContent(title);
        }
        
        public virtual void CloseWindow() {
            instance.Close();
        }
        
        /// <summary>
        /// Called when the window is enabled
        /// </summary>
        protected virtual void OnEnable() {
            // Cast this to T and assign to instance
            instance = this as T;
        }
        
        /// <summary>
        /// Called when the window is destroyed
        /// </summary>
        protected virtual void OnDestroy() {
            // Clear the instance reference when the window is closed
            if (instance == this) {
                instance = null;
            }
        }
    }
}