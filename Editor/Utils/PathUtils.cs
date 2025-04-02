using UnityEditor;
using UnityEngine;

namespace Hierarchy {
    /// <summary>
    /// Utility class for handling paths and stable IDs for GameObjects.
    /// </summary>
    public static class PathUtils {
        /// <summary>
        /// Gets a stable ID for a GameObject.
        /// </summary>
        /// <param name="go">The GameObject to get the stable ID for.</param>
        /// <returns>A stable ID string for the GameObject.</returns>
        public static string GetStableID(GameObject go) {
            if (go == null) return null;
            return IsPrefabRelated(go) ? GetPrefabObjectStableID(go) : $"scene:{GlobalObjectId.GetGlobalObjectIdSlow(go)}";
        }

        /// <summary>
        /// Gets a stable ID for a prefab GameObject.
        /// </summary>
        /// <param name="go">The prefab GameObject to get the stable ID for.</param>
        /// <returns>A stable ID string for the prefab GameObject.</returns>
        private static string GetPrefabObjectStableID(GameObject go) {
            string prefabAssetGuid = GetPrefabAssetGuid(go);
            if (string.IsNullOrEmpty(prefabAssetGuid)) {
                return $"scene:{GlobalObjectId.GetGlobalObjectIdSlow(go)}";
            }
            int fileID = GetPrefabFileID(go, prefabAssetGuid);
            return $"prefab:{prefabAssetGuid}:{fileID}";
        }

        /// <summary>
        /// Gets the file ID for a prefab GameObject.
        /// </summary>
        /// <param name="go">The prefab GameObject to get the file ID for.</param>
        /// <param name="prefabAssetGuid">The GUID of the prefab asset.</param>
        /// <returns>The file ID of the prefab GameObject.</returns>
        private static int GetPrefabFileID(GameObject go, string prefabAssetGuid) {
            GameObject prefabAssetObject = IsInPrefabMode(go) ? GetPrefabAssetObject(go) : PrefabUtility.GetCorrespondingObjectFromSource(go);
            if (prefabAssetObject != null) {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefabAssetObject, out _, out long localId);
                return (int)localId;
            }
            Debug.LogWarning($"Could not determine fileID for prefab object {go.name}. Using instanceID as fallback.");
            return go.GetInstanceID();
        }

        /// <summary>
        /// Gets the prefab asset object for a GameObject in prefab mode.
        /// </summary>
        /// <param name="go">The GameObject in prefab mode.</param>
        /// <returns>The prefab asset GameObject.</returns>
        private static GameObject GetPrefabAssetObject(GameObject go) {
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.assetPath);
            return go == prefabStage.prefabContentsRoot ? prefabAsset : FindObjectInPrefabAsset(prefabAsset, GetRelativePath(go.transform, prefabStage.prefabContentsRoot.transform));
        }

        /// <summary>
        /// Finds a child GameObject in a prefab asset by its relative path.
        /// </summary>
        /// <param name="prefabAsset">The root prefab asset GameObject.</param>
        /// <param name="relativePath">The relative path to the child GameObject.</param>
        /// <returns>The child GameObject if found, otherwise null.</returns>
        private static GameObject FindObjectInPrefabAsset(GameObject prefabAsset, string relativePath) {
            if (string.IsNullOrEmpty(relativePath)) return prefabAsset;
            Transform current = prefabAsset.transform;
            foreach (string part in relativePath.Split('/')) {
                current = current.Find(part);
                if (current == null) {
                    Debug.LogWarning($"Could not find child named {part} in prefab path {relativePath}");
                    return null;
                }
            }
            return current.gameObject;
        }

        /// <summary>
        /// Gets the relative path from a target Transform to a root Transform.
        /// </summary>
        /// <param name="target">The target Transform.</param>
        /// <param name="root">The root Transform.</param>
        /// <returns>The relative path as a string.</returns>
        private static string GetRelativePath(Transform target, Transform root) {
            if (target == root) return string.Empty;
            var pathParts = new System.Collections.Generic.List<string>();
            for (Transform current = target; current != null && current != root; current = current.parent) {
                pathParts.Add(current.name);
            }
            pathParts.Reverse();
            return string.Join("/", pathParts);
        }

        /// <summary>
        /// Checks if a GameObject is in prefab mode.
        /// </summary>
        /// <param name="go">The GameObject to check.</param>
        /// <returns>True if the GameObject is in prefab mode, otherwise false.</returns>
        private static bool IsInPrefabMode(GameObject go) {
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            return prefabStage != null && go.scene == prefabStage.scene;
        }

        /// <summary>
        /// Checks if a GameObject is related to a prefab.
        /// </summary>
        /// <param name="go">The GameObject to check.</param>
        /// <returns>True if the GameObject is related to a prefab, otherwise false.</returns>
        public static bool IsPrefabRelated(GameObject go) {
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            return (prefabStage != null && go.scene == prefabStage.scene) || PrefabUtility.GetPrefabInstanceStatus(go) == PrefabInstanceStatus.Connected;
        }

        /// <summary>
        /// Gets the GUID of the prefab asset for a GameObject.
        /// </summary>
        /// <param name="go">The GameObject to get the prefab asset GUID for.</param>
        /// <returns>The GUID of the prefab asset.</returns>
        public static string GetPrefabAssetGuid(GameObject go) {
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null && go.scene == prefabStage.scene) {
                return AssetDatabase.AssetPathToGUID(prefabStage.assetPath);
            }
            if (PrefabUtility.GetPrefabInstanceStatus(go) == PrefabInstanceStatus.Connected) {
                GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
                if (prefabRoot != null) {
                    return AssetDatabase.AssetPathToGUID(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabRoot));
                }
            }
            return null;
        }
    }
}