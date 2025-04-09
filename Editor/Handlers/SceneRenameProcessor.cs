using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Pastime.Hierarchy {
    /// <summary>
    /// Asset processor that detects scene renames and updates bookmarks accordingly
    /// </summary>
    public class SceneRenameProcessor : AssetPostprocessor {
        // Static dictionary to keep track of scene paths before/after import
        private static Dictionary<string, string> _sPreviousScenePaths = new Dictionary<string, string>();
        
        // Called before any asset is imported
        private static void OnPostprocessAllAssets(
            string[] importedAssets, 
            string[] deletedAssets,
            string[] movedAssets, 
            string[] movedFromAssetPaths) {
            
            // Look for moved/renamed scene files
            for (int i = 0; i < movedAssets.Length; i++) {
                string newPath = movedAssets[i];
                string oldPath = movedFromAssetPaths[i];
                
                // Check if this is a scene file
                if (newPath.EndsWith(".unity")) {
                    // Found a renamed scene, update bookmarks
                    HierarchyBookmarksData.instance.HandleSceneRename(oldPath, newPath);
                }
            }
        }
    }
}