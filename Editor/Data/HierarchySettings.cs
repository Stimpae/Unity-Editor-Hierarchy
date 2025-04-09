using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hierarchy.Data {
    [FilePath("ProjectSettings/HierarchySettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class HierarchySettings : ScriptableSingleton<HierarchySettings> {
        
        public void Save() {
            Save(true);
        }
    }
}