using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hierarchy.Data {
    [Serializable]
    public class HierarchySearchFilter {
        [SerializeField] private string name;
        [SerializeField] private string filterText;

        public string Name {
            get => name;
            set => name = value;
        }

        public string FilterText {
            get => filterText;
            set => filterText = value;
        }

        public HierarchySearchFilter() {
            name = "Name";
            filterText = "t:GameObject";
        }

        public HierarchySearchFilter(string name, string filterText) {
            this.name = name;
            this.filterText = filterText;
        }
    }

    [FilePath("ProjectSettings/HierarchySettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class HierarchySettings : ScriptableSingleton<HierarchySettings> {
        [SerializeField] private List<HierarchySearchFilter> searchFilters = new() {
            new("Game Object", "t:GameObject"),
        };

        public List<HierarchySearchFilter> SearchFilters => searchFilters;
        
        public void Save() {
            Save(true);
        }
    }
}