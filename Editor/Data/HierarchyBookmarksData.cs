using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;
using Hierarchy;

namespace Pastime.Hierarchy {
    
    [FilePath("ProjectSettings/PastimeHierarchyBookmarks.asset", FilePathAttribute.Location.ProjectFolder)]
    public class HierarchyBookmarksData : ScriptableSingleton<HierarchyBookmarksData> {
        [Serializable]
        public class BookmarkData {
            // Use the same stable ID system as HierarchyGameObjectData
            public string stableID;      
            public string customName;    // Optional custom name for the bookmark
            public string scenePath;     // Path to the scene this bookmark belongs to
            public int objectInstanceID; // For runtime use, not serialized
            
            // Constructor for creating a new bookmark
            public BookmarkData(GameObject gameObject, string scene, string name = null) {
                stableID = PathUtils.GetStableID(gameObject);
                scenePath = scene;
                customName = string.IsNullOrEmpty(name) ? gameObject.name : name;
                objectInstanceID = gameObject.GetInstanceID();
            }
        }
        
        [SerializeField]
        private List<BookmarkData> m_bookmarks = new List<BookmarkData>();
        
        public List<BookmarkData> Bookmarks => m_bookmarks;
        
        // Get bookmarks for the specified scene
        public List<BookmarkData> GetBookmarksForScene(string scenePath) {
            return m_bookmarks.Where(b => b.scenePath == scenePath).ToList();
        }
        
        // Get active scene bookmarks
        public List<BookmarkData> GetActiveSceneBookmarks() {
            string activeScene = EditorSceneManager.GetActiveScene().path;
            return GetBookmarksForScene(activeScene);
        }
        
        // Add a bookmark
        public void AddBookmark(GameObject gameObject, string customName = null) {
            if (gameObject == null) return;
            
            // Get the scene path
            string scenePath = gameObject.scene.path;
            
            // Check if bookmark already exists
            string stableID = PathUtils.GetStableID(gameObject);
            if (m_bookmarks.Any(b => b.stableID == stableID)) {
                return; // Already exists
            }
            
            // Create and add the bookmark
            var bookmark = new BookmarkData(gameObject, scenePath, customName);
            m_bookmarks.Add(bookmark);
            
            // Save changes
            Save(true);
        }
        
        // Remove a bookmark
        public void RemoveBookmark(string stableID) {
            m_bookmarks.RemoveAll(b => b.stableID == stableID);
            Save(true);
        }
        
        // Remove bookmarks for a specific scene
        public void RemoveBookmarksForScene(string scenePath) {
            m_bookmarks.RemoveAll(b => b.scenePath == scenePath);
            Save(true);
        }
        
        // Rename a bookmark
        public void RenameBookmark(string stableID, string newName) {
            var bookmark = m_bookmarks.FirstOrDefault(b => b.stableID == stableID);
            if (bookmark != null) {
                bookmark.customName = newName;
                Save(true);
            }
        }
        
        // Resolve bookmark to GameObject - handles both scene and prefab objects
        public GameObject ResolveBookmark(BookmarkData bookmark) {
            if (bookmark == null) return null;
            
            try {
                if (bookmark.stableID.StartsWith("scene:")) {
                    // Parse the GlobalObjectId part from the stableID
                    string globalIdStr = bookmark.stableID.Substring(6);
                    if (GlobalObjectId.TryParse(globalIdStr, out GlobalObjectId id)) {
                        // Check if we're in a valid scene context before resolving
                        if (EditorSceneManager.GetActiveScene().isLoaded) {
                            // Convert GlobalObjectId back to Object
                            GameObject gameObject = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id) as GameObject;
                            
                            // Update the instance ID for quick access later
                            if (gameObject != null) {
                                bookmark.objectInstanceID = gameObject.GetInstanceID();
                            }
                            
                            return gameObject;
                        }
                    }
                }
                else if (bookmark.stableID.StartsWith("prefab:")) {
                    // Handle prefab references
                    string[] parts = bookmark.stableID.Split(':');
                    if (parts.Length >= 3) {
                        string guid = parts[1];
                        int fileID = int.Parse(parts[2]);
                        
                        // Get the asset path from GUID
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        if (!string.IsNullOrEmpty(assetPath)) {
                            // Attempt to find the prefab instance in the current scene
                            var rootGameObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
                            foreach (var root in rootGameObjects) {
                                GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(root);
                                if (prefabRoot != null) {
                                    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabRoot);
                                    if (prefabPath == assetPath) {
                                        // We found a prefab instance with matching path, now find child with matching fileID
                                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefabRoot, out _, out long localId);
                                        if ((int)localId == fileID) return prefabRoot;
                                        
                                        // Search children
                                        var allChildren = prefabRoot.GetComponentsInChildren<Transform>(true);
                                        foreach (var child in allChildren) {
                                            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(child.gameObject, out _, out localId);
                                            if ((int)localId == fileID) return child.gameObject;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                // Silently catch errors during scene changes
                // Debug.LogWarning($"Error resolving bookmark: {ex.Message}");
            }
            
            return null;
        }
        
        // Clean up missing bookmarks
        public void CleanupMissingBookmarks() {
            bool hasChanges = false;
            var bookmarksToRemove = new List<BookmarkData>();
            var loadedSceneGuids = new HashSet<string>();
            
            // Collect loaded scene GUIDs
            for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (scene.isLoaded) {
                    string guid = AssetDatabase.AssetPathToGUID(scene.path);
                    if (!string.IsNullOrEmpty(guid)) {
                        loadedSceneGuids.Add(guid);
                    }
                }
            }
            
            foreach (var bookmark in m_bookmarks) {
                bool shouldCheck = true;
                
                // For scene objects, only check if they belong to loaded scenes
                if (bookmark.stableID.StartsWith("scene:")) {
                    string globalIdStr = bookmark.stableID.Substring(6);
                    if (GlobalObjectId.TryParse(globalIdStr, out GlobalObjectId globalId)) {
                        string assetGuid = globalId.assetGUID.ToString();
                        shouldCheck = loadedSceneGuids.Contains(assetGuid);
                    }
                }
                
                if (shouldCheck) {
                    GameObject obj = ResolveBookmark(bookmark);
                    if (obj == null) {
                        bookmarksToRemove.Add(bookmark);
                        hasChanges = true;
                    }
                }
            }
            
            // Remove all invalid bookmarks
            foreach (var bookmark in bookmarksToRemove) {
                m_bookmarks.Remove(bookmark);
            }
            
            if (hasChanges) {
                Save(true);
            }
        }
    }
}