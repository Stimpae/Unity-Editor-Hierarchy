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

        // Events for UI updates
        public event Action ColorsChanged;
        public event Action IconsChanged;

        [SerializeField] private List<ColorRow> colorRows = new List<ColorRow>();
        [SerializeField] private List<IconRow> iconRows = new List<IconRow>();

        // Public accessors
        public List<ColorRow> ColorRows => colorRows;
        public List<IconRow> IconRows => iconRows;

        // Row management methods
        public void AddColorRow()
        {
            colorRows.Add(new ColorRow());
            ColorsChanged?.Invoke();
            Save(true);
        }
        
        public void RemoveColorRow(int index)
        {
            if (index >= 0 && index < colorRows.Count)
            {
                colorRows.RemoveAt(index);
                ColorsChanged?.Invoke();
                Save(true);
            }
        }
        
        public void MoveColorRow(int sourceIndex, int targetIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= colorRows.Count ||
                targetIndex < 0 || targetIndex >= colorRows.Count ||
                sourceIndex == targetIndex)
                return;
        
            // Get the row to move
            var rowToMove = colorRows[sourceIndex];
    
            // Remove from source position
            colorRows.RemoveAt(sourceIndex);
    
            // Insert at target position
            colorRows.Insert(targetIndex, rowToMove);
    
            // Save changes
            Save(true);
    
            // Notify UI of changes
            ColorsChanged?.Invoke();
        }
        
        public void AddIconRow()
        {
            iconRows.Add(new IconRow());
            IconsChanged?.Invoke();
            Save(true);
        }
        
        public void RemoveIconRow(int index)
        {
            if (index >= 0 && index < iconRows.Count)
            {
                iconRows.RemoveAt(index);
                IconsChanged?.Invoke();
                Save(true);
            }
        }
        
        public void MoveIconRow(int fromIndex, int toIndex)
        {
            if (fromIndex >= 0 && fromIndex < iconRows.Count &&
                toIndex >= 0 && toIndex < iconRows.Count)
            {
                var item = iconRows[fromIndex];
                iconRows.RemoveAt(fromIndex);
                iconRows.Insert(toIndex, item);
                IconsChanged?.Invoke();
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
            
            ColorsChanged?.Invoke();
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
            
            IconsChanged?.Invoke();
            Save(true);
        }
        
        // Initialize defaults if needed
        public void InitializeIfEmpty()
        {
            bool needsSave = false;
            
            if (colorRows.Count == 0)
            {
                ResetColors();
                needsSave = true;
            }
            
            if (iconRows.Count == 0)
            {
                ResetIcons();
                needsSave = true;
            }
            
            if (needsSave)
            {
                Save(true);
            }
        }
    }
}