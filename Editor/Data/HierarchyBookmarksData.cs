using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;
using Hierarchy;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Pastime.Hierarchy {
    
    [FilePath("ProjectSettings/HierarchyBookmarks.asset", FilePathAttribute.Location.ProjectFolder)]
    public class HierarchyBookmarksData : ScriptableSingleton<HierarchyBookmarksData> {
        [Serializable]
        public class BookmarkData {
            public string stableID;      // Stable ID from PathUtils
            public string customName;    // Optional custom name for the bookmark
            public string scenePath;     // Scene path this bookmark belongs to (or prefab path)
            
            // Constructor for creating a new bookmark
            public BookmarkData(GameObject gameObject, string scene, string name = null) {
                stableID = PathUtils.GetStableID(gameObject);
                scenePath = scene;
                customName = string.IsNullOrEmpty(name) ? gameObject.name : name;
            }
        }
        
        [SerializeField] private List<BookmarkData> bookmarks = new List<BookmarkData>();
        public List<BookmarkData> Bookmarks => bookmarks;
        
        // Get bookmarks for the specified scene
        public List<BookmarkData> GetBookmarksForScene(string scenePath) {
            return bookmarks.Where(b => b.scenePath == scenePath).ToList();
            
        }
        
        // Get bookmarks for all currently loaded scenes
        public List<BookmarkData> GetActiveScenesBookmarks() {
            HashSet<string> loadedScenePaths = new HashSet<string>();
            List<BookmarkData> bookmarks = new List<BookmarkData>();
            
            // Get paths for all loaded scenes
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded) {
                    loadedScenePaths.Add(scene.path);
                }
            }
            
            // Also add prefab stage path if applicable
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null) {
                loadedScenePaths.Add(prefabStage.assetPath);
            }
            
            // Get bookmarks for all loaded scenes
            foreach (var scenePath in loadedScenePaths) {
                bookmarks.AddRange(GetBookmarksForScene(scenePath));
            }
            
            return bookmarks;
        }
        
        // Add a bookmark
        public void AddBookmark(GameObject gameObject, string customName = null) {
            if (gameObject == null) return;
            
            string stableID = PathUtils.GetStableID(gameObject);
            string scenePath = DetermineScenePath(gameObject);
            
            // Check if bookmark already exists by stable ID
            if (bookmarks.Any(b => b.stableID == stableID)) {
                // Update the scene path if it's different (handles scene renames)
                var existingBookmark = bookmarks.First(b => b.stableID == stableID);
                if (existingBookmark.scenePath != scenePath) {
                    existingBookmark.scenePath = scenePath;
                    Save(true);
                }
                return; // Already exists
            }
            
            // Create and add the bookmark
            var bookmark = new BookmarkData(gameObject, scenePath, customName);
            bookmarks.Add(bookmark);
            Save(true);
        }
        
        // Determine the appropriate scene path for a GameObject
        private string DetermineScenePath(GameObject gameObject) {
            // For scene objects, use the scene path
            string scenePath = gameObject.scene.path;
            if (!string.IsNullOrEmpty(scenePath)) {
                return scenePath;
            }
            
            // For prefab assets in the project window
            if (EditorUtility.IsPersistent(gameObject)) {
                return AssetDatabase.GetAssetPath(gameObject);
            }
            
            // For prefab stage objects
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null && prefabStage.scene == gameObject.scene) {
                return prefabStage.assetPath;
            }
            
            // For prefab instances, find their scene
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded) {
                    foreach (var root in scene.GetRootGameObjects()) {
                        if (gameObject == root || gameObject.transform.IsChildOf(root.transform)) {
                            return scene.path;
                        }
                    }
                }
            }
            
            // Fallback - use prefab asset path
            var prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
            if (!string.IsNullOrEmpty(prefabAssetPath)) {
                return prefabAssetPath;
            }
            
            return string.Empty;
        }
        
        // Remove a bookmark
        public void RemoveBookmark(string stableID) {
            bookmarks.RemoveAll(b => b.stableID == stableID);
            Save(true);
        }
        
        // Rename a bookmark
        public void RenameBookmark(string stableID, string newName) {
            var bookmark = bookmarks.FirstOrDefault(b => b.stableID == stableID);
            if (bookmark != null) {
                bookmark.customName = newName;
                Save(true);
            }
        }
        
        // Handle scene rename
        public void HandleSceneRename(string oldScenePath, string newScenePath) {
            bool needsSave = false;
            
            // Update all bookmarks with the old scene path
            foreach (var bookmark in bookmarks) {
                if (bookmark.scenePath == oldScenePath) {
                    bookmark.scenePath = newScenePath;
                    needsSave = true;
                }
            }
            
            if (needsSave) {
                Save(true);
            }
        }
        
        // Resolve bookmark to GameObject
        public GameObject ResolveBookmark(BookmarkData bookmark) {
            if (bookmark == null) return null;
            
            try {
                // Use PathUtils and find the referenced object by stableID
                if (bookmark.stableID.StartsWith("scene:")) {
                    // Scene objects
                    string globalIdStr = bookmark.stableID.Substring(6);
                    if (GlobalObjectId.TryParse(globalIdStr, out GlobalObjectId id)) {
                        return GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id) as GameObject;
                    }
                } 
                else if (bookmark.stableID.StartsWith("prefab:")) {
                    // Prefab objects
                    string[] parts = bookmark.stableID.Split(':');
                    if (parts.Length >= 3) {
                        string guid = parts[1];
                        int fileID = int.Parse(parts[2]);
                        
                        // Get asset path from GUID
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        if (string.IsNullOrEmpty(assetPath)) return null;
                        
                        // Check if we're in prefab mode
                        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                        if (prefabStage != null && prefabStage.assetPath == assetPath) {
                            // Find the object in the prefab stage
                            var allTransforms = prefabStage.prefabContentsRoot.GetComponentsInChildren<Transform>(true);
                            foreach (var transform in allTransforms) {
                                if (GetLocalFileIDForPrefab(transform.gameObject, guid) == fileID) {
                                    return transform.gameObject;
                                }
                            }
                        }
                        
                        // Look for instances in all loaded scenes
                        for (int sceneIdx = 0; sceneIdx < SceneManager.sceneCount; sceneIdx++) {
                            Scene scene = SceneManager.GetSceneAt(sceneIdx);
                            if (!scene.isLoaded) continue;
                            
                            foreach (var rootObj in scene.GetRootGameObjects()) {
                                // Check if this is a prefab instance with the right asset path
                                string rootPrefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(rootObj);
                                if (rootPrefabPath != assetPath) continue;
                                
                                // Check the root
                                if (GetLocalFileIDForPrefab(rootObj, guid) == fileID) {
                                    return rootObj;
                                }
                                
                                // Check children
                                var childrenTransforms = rootObj.GetComponentsInChildren<Transform>(true);
                                foreach (var childTransform in childrenTransforms) {
                                    if (GetLocalFileIDForPrefab(childTransform.gameObject, guid) == fileID) {
                                        return childTransform.gameObject;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                // Handle exceptions silently
                Debug.LogWarning($"Error resolving bookmark: {ex.Message}");
            }
            
            return null;
        }
        
        // Helper to get local file ID for prefab
        private int GetLocalFileIDForPrefab(GameObject go, string guid) {
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(go, out string objGuid, out long localId)) {
                if (objGuid == guid) {
                    return (int)localId;
                }
            }
            
            // For prefab instances, we need to get the corresponding source object
            GameObject sourceObj = PrefabUtility.GetCorrespondingObjectFromSource(go);
            if (sourceObj != null && 
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sourceObj, out string sourceGuid, out long sourceLocalId)) {
                if (sourceGuid == guid) {
                    return (int)sourceLocalId;
                }
            }
            
            return -1;
        }
        
        // Clean up missing bookmarks
        public void CleanupMissingBookmarks() {
            bool hasChanges = false;
            var bookmarksToRemove = new List<BookmarkData>();
            
            foreach (var bookmark in bookmarks) {
                // Try to resolve the bookmark
                GameObject go = ResolveBookmark(bookmark);
                if (go == null) {
                    // Don't remove prefab bookmarks if they're just not loaded currently
                    if (bookmark.stableID.StartsWith("prefab:")) {
                        string[] parts = bookmark.stableID.Split(':');
                        if (parts.Length >= 2) {
                            string guid = parts[1];
                            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                            
                            // Only remove if the asset no longer exists
                            if (string.IsNullOrEmpty(assetPath)) {
                                bookmarksToRemove.Add(bookmark);
                                hasChanges = true;
                            }
                        }
                    } else {
                        bookmarksToRemove.Add(bookmark);
                        hasChanges = true;
                    }
                }
            }
            
            // Remove invalid bookmarks
            foreach (var bookmark in bookmarksToRemove) {
                bookmarks.Remove(bookmark);
            }
            
            if (hasChanges) {
                Save(true);
            }
        }
    }
}