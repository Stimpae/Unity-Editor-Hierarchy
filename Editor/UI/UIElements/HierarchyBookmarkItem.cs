using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pastime.Hierarchy.GUI
{
    /// <summary>
    /// A visual element that represents a single bookmark item in the hierarchy
    /// </summary>
    public class HierarchyBookmarkItem : VisualElement
    {
        private const string k_bookmarkItemClass = "hierarchy-bookmark-item";
        private const string k_bookmarkItemSelectedClass = "hierarchy-bookmark-item-selected";
        
        private readonly HierarchyBookmarksData.BookmarkData m_bookmarkData;
        private readonly GameObject m_gameObject;
        private readonly Image m_iconElement;
        
        // Events
        public event Action<HierarchyBookmarksData.BookmarkData> OnSelected;
        public event Action<HierarchyBookmarksData.BookmarkData> OnContextMenu;
        
        public HierarchyBookmarkItem(HierarchyBookmarksData.BookmarkData bookmarkData, GameObject gameObject)
        {
            m_bookmarkData = bookmarkData;
            m_gameObject = gameObject;
            
            // Setup styles
            AddToClassList(k_bookmarkItemClass);
            
            // Get icon for the GameObject
            Texture2D icon = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject)).image as Texture2D;
            
            // Create icon element
            m_iconElement = new Image();
            m_iconElement.image = icon;
            m_iconElement.scaleMode = ScaleMode.ScaleToFit;
            m_iconElement.style.width = 16;
            m_iconElement.style.height = 16;
            Add(m_iconElement);
            
            // Set tooltip to show the full name and path
            tooltip = $"{bookmarkData.customName} ({GetGameObjectPath(gameObject)})";
            
            // Register event handlers
            RegisterCallback<ClickEvent>(OnClick);
            RegisterCallback<ContextClickEvent>(OnContextClickEvent);
        }
        
        public void SetSelected(bool selected)
        {
            if (selected)
            {
                AddToClassList(k_bookmarkItemSelectedClass);
            }
            else
            {
                RemoveFromClassList(k_bookmarkItemSelectedClass);
            }
        }
        
        private void OnClick(ClickEvent evt)
        {
            OnSelected?.Invoke(m_bookmarkData);
            evt.StopPropagation();
        }
        
        private void OnContextClickEvent(ContextClickEvent evt)
        {
            OnContextMenu?.Invoke(m_bookmarkData);
            evt.StopPropagation();
        }
        
        // Helper method to get GameObject path
        private string GetGameObjectPath(GameObject gameObject)
        {
            if (gameObject == null) return string.Empty;
            
            string path = gameObject.name;
            Transform parent = gameObject.transform.parent;
            
            while (parent != null)
            {
                path = $"{parent.name}/{path}";
                parent = parent.parent;
            }
            
            return path;
        }
    }
}