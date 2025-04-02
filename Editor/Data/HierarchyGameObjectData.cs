using System;
using System.Collections.Generic;
using Hierarchy.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace Hierarchy {
    /// <summary>
    /// Manages hierarchy data for GameObjects in the project.
    /// </summary>
    [FilePath("ProjectSettings/HierarchyData", FilePathAttribute.Location.ProjectFolder)]
    public class HierarchyGameObjectData : ScriptableSingleton<HierarchyGameObjectData> {
        /// <summary>
        /// List of data entries for GameObjects.
        /// </summary>
        [SerializeField] private List<GameObjectDataEntry> objectsData = new List<GameObjectDataEntry>();

        /// <summary>
        /// Represents a data entry for a GameObject.
        /// </summary>
        [Serializable]
        public class GameObjectDataEntry {
            /// <summary>
            /// The stable ID of the GameObject.
            /// </summary>
            public string stableID;
            /// <summary>
            /// The data associated with the GameObject.
            /// </summary>
            public GameObjectData data;
        }

        /// <summary>
        /// Retrieves the data associated with a GameObject.
        /// </summary>
        /// <param name="go">The GameObject to retrieve data for.</param>
        /// <returns>The data associated with the GameObject.</returns>
        public GameObjectData GetGameObjectData(GameObject go) {
            if (go == null) return null;
            string stableID = PathUtils.GetStableID(go);
            foreach (var entry in objectsData) {
                if (entry.stableID == stableID) return entry.data;
            }
            var data = new GameObjectData();
            objectsData.Add(new GameObjectDataEntry { stableID = stableID, data = data });
            return data;
        }

        /// <summary>
        /// Sets the data for a GameObject.
        /// </summary>
        /// <param name="go">The GameObject to set data for.</param>
        /// <param name="data">The data to set.</param>
        public void SetGameObjectData(GameObject go, GameObjectData data) {
            if (go == null || data == null) return;
            string stableID = PathUtils.GetStableID(go);
            for (int i = 0; i < objectsData.Count; i++) {
                if (objectsData[i].stableID == stableID) {
                    objectsData[i].data = data;
                    Save();
                    return;
                }
            }
            objectsData.Add(new GameObjectDataEntry { stableID = stableID, data = data });
            Save();
        }

        /// <summary>
        /// Cleans up data entries for deleted GameObjects.
        /// </summary>
        public void CleanupDeletedObjects() {
            bool hasChanges = false;
            var keysToRemove = new List<string>();
            var loadedSceneGuids = new HashSet<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded) {
                    string guid = AssetDatabase.AssetPathToGUID(scene.path);
                    if (!string.IsNullOrEmpty(guid)) loadedSceneGuids.Add(guid);
                }
            }
            foreach (var entry in objectsData) {
                if (entry.stableID.StartsWith("scene:")) {
                    if (GlobalObjectId.TryParse(entry.stableID.Substring(6), out var globalId)) {
                        string assetGuid = globalId.assetGUID.ToString();
                        if (globalId.identifierType == 0 && globalId.assetGUID.Empty()) continue;
                        bool belongsToLoadedScene = loadedSceneGuids.Contains(assetGuid);
                        if (!belongsToLoadedScene)  continue; 
                        
                        Object obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalId);
                        if (obj == null) {
                            keysToRemove.Add(entry.stableID);
                            hasChanges = true;
                        }
                    }
                } else if (entry.stableID.StartsWith("prefab:")) {
                    if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(entry.stableID.Split(':')[1]))) {
                        keysToRemove.Add(entry.stableID);
                        hasChanges = true;
                    }
                }
            }
            objectsData.RemoveAll(entry => keysToRemove.Contains(entry.stableID));
            if (hasChanges) Save();
        }

        /// <summary>
        /// Saves the hierarchy data.
        /// </summary>
        public new void Save() {
            base.Save(true);
        }
    }

    /// <summary>
    /// Represents data associated with a GameObject.
    /// </summary>
    [Serializable]
    public class GameObjectData {
        /// <summary>
        /// Indicates if the GameObject is locked.
        /// </summary>
        public bool isLocked;
        /// <summary>
        /// Indicates if the GameObject is expanded in the hierarchy.
        /// </summary>
        public bool isExpanded;
        /// <summary>
        /// The custom color assigned to the GameObject.
        /// </summary>
        public Color customColor = Color.white;
        /// <summary>
        /// The custom label assigned to the GameObject.
        /// </summary>
        public string customLabel;
        /// <summary>
        /// The original hide flags of the GameObject.
        /// </summary>
        public HideFlags originalHideFlags;

        /// <summary>
        /// Applies the data to a GameObject.
        /// </summary>
        /// <param name="go">The GameObject to apply the data to.</param>
        public void ApplyToGameObject(GameObject go) {
            if (go == null) return;
            if (isLocked) {
                originalHideFlags = go.hideFlags;
                go.hideFlags |= HideFlags.NotEditable;
            } else if ((go.hideFlags & HideFlags.NotEditable) != 0) {
                go.hideFlags = originalHideFlags;
            }
        }
    }
}