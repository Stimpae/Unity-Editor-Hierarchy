using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hierarchy.Data {
    [FilePath("ProjectSettings/HierarchyPalette.asset", FilePathAttribute.Location.ProjectFolder)]
    public class HierarchyPaletteData : ScriptableSingleton<HierarchyPaletteData>
    {
        [Serializable]
        public class ColorRow
        {
            public List<Color> colors = new List<Color>();

            public ColorRow()
            {
                // Initialize with a default color if empty
                colors = new List<Color>();
            }
            
            public ColorRow(params Color[] initialColors)
            {
                colors = new List<Color>(initialColors);
            }
            
            public void AddColor(Color color)
            {
                colors.Add(color);
                HierarchyPaletteData.instance.Save(true);
            }
            
            public void RemoveColor(int index)
            {
                if (index >= 0 && index < colors.Count)
                {
                    colors.RemoveAt(index);
                    HierarchyPaletteData.instance.Save(true);
                }
            }
            
            public void UpdateColor(int index, Color newColor)
            {
                if (index >= 0 && index < colors.Count)
                {
                    colors[index] = newColor;
                    HierarchyPaletteData.instance.Save(true);
                }
            }
        }
        
        [Serializable]
        public class IconRow
        {
            public List<string> iconPaths = new List<string>();
            
            public IconRow()
            {
                // Initialize with empty list
                iconPaths = new List<string>();
            }
            
            public IconRow(params string[] initialPaths)
            {
                iconPaths = new List<string>(initialPaths);
            }
            
            public void AddIcon(string iconPath)
            {
                iconPaths.Add(iconPath);
                HierarchyPaletteData.instance.Save(true);
            }
            
            public void RemoveIcon(int index)
            {
                if (index >= 0 && index < iconPaths.Count)
                {
                    iconPaths.RemoveAt(index);
                    HierarchyPaletteData.instance.Save(true);
                }
            }
            
            public void UpdateIcon(int index, string newPath)
            {
                if (index >= 0 && index < iconPaths.Count)
                {
                    iconPaths[index] = newPath;
                    HierarchyPaletteData.instance.Save(true);
                }
            }
        }
        
        [SerializeField] private List<ColorRow> colorRows = new List<ColorRow>();
        [SerializeField] private List<IconRow> iconRows = new List<IconRow>();

        // Public accessors
        public List<ColorRow> ColorRows => colorRows;
        public List<IconRow> IconRows => iconRows;

        // Row management methods
        public void AddColorRow()
        {
            colorRows.Add(new ColorRow());
            Save(true);
        }
        
        public void RemoveColorRow(int index)
        {
            if (index >= 0 && index < colorRows.Count)
            {
                colorRows.RemoveAt(index);
                Save(true);
            }
        }
        
        
        public void AddIconRow()
        {
            iconRows.Add(new IconRow());
            Save(true);
        }
        
        public void RemoveIconRow(int index)
        {
            if (index >= 0 && index < iconRows.Count)
            {
                iconRows.RemoveAt(index);
                Save(true);
            }
        }
        

        // Reset methods
        public void ResetColors()
        {
            colorRows.Clear();
            
            // Add default color palette
            colorRows.Add(new ColorRow(
                Color.red,
                Color.green,
                Color.blue
            ));
            
            colorRows.Add(new ColorRow(
                Color.yellow,
                Color.cyan,
                Color.magenta
            ));
            
            colorRows.Add(new ColorRow(
                new Color(0.5f, 0.0f, 0.0f), // Dark red
                new Color(0.0f, 0.5f, 0.0f), // Dark green
                new Color(0.0f, 0.0f, 0.5f)  // Dark blue
            ));
            Save(true);
        }
        
        public void ResetIcons()
        {
            iconRows.Clear();
            
            // Add default icons
            iconRows.Add(new IconRow(
                "d_UnityEditor.SceneHierarchyWindow",
                "d_Prefab Icon",
                "d_Folder Icon"
            ));
            
            iconRows.Add(new IconRow(
                "d_GameObject Icon",
                "d_Camera Icon",
                "d_Light Icon"
            ));
            
            Save(true);
        }
    }
}