using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hierarchy.Data;
using Hierarchy.GUI;
using Hierarchy.Utils;
using UnityEditor;
using UnityEngine;


namespace Hierarchy {
    [InitializeOnLoad]
    public static class Hierarchy {
        public static HierarchyData HierarchyData { get; private set; }
        public static HierarchyPaletteData PaletteData { get; private set; }  
        private static Dictionary<EditorWindow, HierarchyGUI> HierarchyGuIs { get; } = new Dictionary<EditorWindow, HierarchyGUI>();
        private static Type _sceneHierarchyWindowType;
        private static Type _hostViewType;
        private static Type _editorWindowDelegateType;
        private static EditorWindow _previousFocusedWindow;
        private static bool _initialized;
        
        private static HierarchyEventHandler _eventHandler;
        
        static Hierarchy() {
            EditorApplication.delayCall += Initialize;
        }
        
        private static void Initialize() {
            if (_initialized) return;
            
            try {
                _eventHandler = new HierarchyEventHandler();
                CacheReferences();
                UnregisterEventHandlers();
                RegisterEventHandlers();
                LoadHierarchyData();
                LoadPaletteData();
                UpdateAllHierarchyWindows();
                
                _initialized = true;
            }
            catch (Exception ex) {
                Debug.LogError($"Failed to initialize Hierarchy: {ex.Message}\n{ex.StackTrace}");
                UnregisterEventHandlers();
            }
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
            EditorApplication.quitting += HandleEditorQuitting;
        }
        
        private static void UnregisterEventHandlers() {
            EditorApplication.update -= CheckFocusedWindow;
            EditorApplication.update -= CheckWindowsToRemove;
            EditorApplication.quitting -= HandleEditorQuitting;
        }
        
        private static void HandleEditorQuitting() { ;
            foreach (var hierarchyGui in HierarchyGuIs.Values) {
                hierarchyGui.Dispose();
            }

            _eventHandler = null;
            HierarchyGuIs.Clear();
            UnregisterEventHandlers();
        }
          
        private static void LoadHierarchyData() {
           // hierarchyData = HierarchyData.Load() ?? new HierarchyData();
            //Debug.Log("Hierarchy data loaded.");
        }
        
        private static void LoadPaletteData() {
            //paletteData = HierarchyPaletteData.Load() ?? new HierarchyPaletteData();
            //Debug.Log("Palette data loaded.");
        }
        
        private static void CheckFocusedWindow() {
            var currentFocusedWindow = EditorWindow.focusedWindow;
            if (_previousFocusedWindow != currentFocusedWindow && IsHierarchyWindow(currentFocusedWindow)) {
                UpdateHierarchyWindow(currentFocusedWindow);
            }
            _previousFocusedWindow = currentFocusedWindow;
        }

        private static void CheckWindowsToRemove() {
            var windows = GetAllHierarchyWindows().ToList();
            var windowsToRemove = HierarchyGuIs.Keys.Except(windows).ToList();
            foreach (var window in windowsToRemove) {
                HierarchyGuIs[window].Dispose();
                HierarchyGuIs.Remove(window);
            }
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
            var windows = GetAllHierarchyWindows().ToList();
            foreach (var hierarchyWindow in windows) {
                UpdateHierarchyWindow(hierarchyWindow);
            }
        }
        
        private static void UpdateHierarchyWindow(EditorWindow hierarchyWindow) {
            if (hierarchyWindow == null) return;
            
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
            if (!_initialized) return;
            
            try {
                _eventHandler.ProcessEvent();
                if (!HierarchyGuIs.TryGetValue(hierarchyWindow, out var gui)) {
                    gui = new HierarchyGUI(hierarchyWindow, _eventHandler);
                    HierarchyGuIs[hierarchyWindow] = gui;
                }
                gui.DrawHierarchyGUI();
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