using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hierarchy.Utils;
using UnityEditor;
using UnityEngine;


namespace Hierarchy {
    public static class Hierarchy {
        private static Dictionary<EditorWindow, HierarchyGUI> HierarchyGuIs { get; set; }
        private static Type _sceneHierarchyWindowType;
        private static Type _hostViewType;
        private static Type _editorWindowDelegateType;
        private static EditorWindow _previousFocusedWindow;
        
        private static HierarchyEventHandler _eventHandler;
        
        [InitializeOnLoadMethod]
        private static void Initialize() {
            HierarchyGuIs ??= new Dictionary<EditorWindow, HierarchyGUI>();
            
            try {
                _eventHandler = new HierarchyEventHandler();
                CacheReferences();
                UnregisterEventHandlers(); // Good that you're already doing this
                RegisterEventHandlers();
            }
            catch (Exception ex) {
                Debug.LogError($"Failed to initialize Hierarchy: {ex.Message}\n{ex.StackTrace}");
                UnregisterEventHandlers();
            }
            
            EditorApplication.delayCall += UpdateAllHierarchyWindows;
        }
        
        
        private static void CacheReferences() {
            _sceneHierarchyWindowType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            if (_sceneHierarchyWindowType == null) throw new NullReferenceException("Failed to find SceneHierarchyWindow type.");
            
            _hostViewType = typeof(Editor).Assembly.GetType("UnityEditor.HostView");
            if (_hostViewType == null) throw new NullReferenceException("Failed to find HostView type.");
                
            _editorWindowDelegateType = _hostViewType.GetNestedType("EditorWindowDelegate", ReflectionUtils.MAX_BINDING_FLAGS);
            if (_editorWindowDelegateType == null) throw new NullReferenceException("Failed to find EditorWindowDelegate type.");
        }
        
        private static void RegisterEventHandlers() {
            EditorApplication.update += CheckFocusedWindow;
            EditorApplication.update += CheckWindowsToRemove;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            EditorApplication.quitting += HandleEditorQuitting;
        }
        
        private static void UnregisterEventHandlers() {
            EditorApplication.update -= CheckFocusedWindow;
            EditorApplication.update -= CheckWindowsToRemove;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            EditorApplication.quitting -= HandleEditorQuitting;
        }
        
        private static void HandleEditorQuitting() => CleanupHierarchy();
        private static void OnBeforeAssemblyReload() => CleanupHierarchy();
        
        private static void CleanupHierarchy() {
            foreach (var hierarchyGui in HierarchyGuIs.Values) {
                hierarchyGui.Dispose();
            }

            _eventHandler = null;
            HierarchyGuIs.Clear();
            UnregisterEventHandlers();
        }

        
        private static void CheckFocusedWindow() {
            var currentFocusedWindow = EditorWindow.focusedWindow;
            if (_previousFocusedWindow != currentFocusedWindow && IsHierarchyWindow(currentFocusedWindow)) {
                UpdateHierarchyWindow(currentFocusedWindow);
            }
            _previousFocusedWindow = currentFocusedWindow;
        }

        private static void CheckWindowsToRemove() {
            _eventHandler.ProcessEvent();
            
            var windows = GetAllHierarchyWindows().ToList();
            var windowsToRemove = HierarchyGuIs.Keys.Except(windows).ToList();
            foreach (var window in windowsToRemove) {
                HierarchyGuIs[window].Dispose();
                HierarchyGuIs.Remove(window);
            }
        }
        
        private static void OnAfterAssemblyReload() {
            EditorApplication.delayCall += UpdateAllHierarchyWindows;
        }
        
        private static bool IsHierarchyWindow(EditorWindow window) {
            return window != null && window.GetType() == _sceneHierarchyWindowType;
        }
        
        private static IEnumerable<EditorWindow> GetAllHierarchyWindows() {
            try {
                var sceneHierarchyWindows = (IList)_sceneHierarchyWindowType.GetFieldValue("s_SceneHierarchyWindows");
                if (sceneHierarchyWindows == null) {
                    return Enumerable.Empty<EditorWindow>();
                }
                return sceneHierarchyWindows.Cast<EditorWindow>();
            }
            catch (Exception ex) {
                Debug.LogError($"Error getting hierarchy windows: {ex.Message}");
                return Enumerable.Empty<EditorWindow>();
            }
        }
        
        private static void UpdateAllHierarchyWindows() {
            // Schedule this after a frame to ensure hierarchy windows are ready
            EditorApplication.delayCall += () => {
                var windows = GetAllHierarchyWindows().ToList();
                foreach (var hierarchyWindow in windows) {
                    UpdateHierarchyWindow(hierarchyWindow);
                }
            };
        }
        
        private static void UpdateHierarchyWindow(EditorWindow hierarchyWindow) {
            if (hierarchyWindow == null) return;
            if (!hierarchyWindow.hasFocus) return;
            
            try {
                var hostView = hierarchyWindow.GetMemberValue("m_Parent");
                var onGUIMethod = typeof(Hierarchy).GetMethod(nameof(HandleGUI), ReflectionUtils.MAX_BINDING_FLAGS);
                var onGUIDelegate = onGUIMethod?.CreateDelegate(_editorWindowDelegateType, hierarchyWindow);
                hostView.SetMemberValue("m_OnGUI", onGUIDelegate);
                hierarchyWindow.Repaint();
            }
            catch (Exception ex) {
                Debug.LogError($"Failed to update hierarchy window {hierarchyWindow.GetHashCode()}: {ex.Message}");
            }
        }
        
        private static void HandleGUI(EditorWindow hierarchyWindow) {
            try {
                if (!HierarchyGuIs.TryGetValue(hierarchyWindow, out var gui) || gui == null) {
                    // Only create if null or missing
                    gui = new HierarchyGUI(hierarchyWindow, _eventHandler);
                    HierarchyGuIs[hierarchyWindow] = gui;
                    Debug.Log($"Created new HierarchyGUI for window {hierarchyWindow.GetInstanceID()}");
                }
                gui.OnHierarchyGUI();
            }
            catch (Exception exception) {
                if (exception.InnerException is ExitGUIException)
                    throw exception.InnerException;
                else
                    throw;
            }
        }
    }
}