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
        
        private readonly HierarchyBookmarksData.BookmarkData m_bookmarkData;
        private readonly GameObject m_gameObject;
        private readonly Image m_iconElement;
        private readonly Label m_labelElement;
        private readonly VisualElement m_labelContainer;
        
        // Events
        public event Action<HierarchyBookmarksData.BookmarkData> OnSelected;
        public event Action<HierarchyBookmarksData.BookmarkData> OnContextMenu;
        
        public HierarchyBookmarkItem(HierarchyBookmarksData.BookmarkData bookmarkData, GameObject gameObject)
        {
            m_bookmarkData = bookmarkData;
            m_gameObject = gameObject;
            
            // tag this as dynamic to avoid being cached
            // this is important for the hierarchy to work properly
            usageHints = UsageHints.DynamicTransform;
            
            // Setup styles
            AddToClassList(k_bookmarkItemClass);
            
            // Get icon for the GameObject
            Texture2D icon = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject)).image as Texture2D;
            
            // Create icon element
            m_iconElement = new Image {
                image = icon,
                scaleMode = ScaleMode.ScaleToFit,
                style = {
                    width = 16,
                    height = 16
                }
            };
            Add(m_iconElement);
            
            // Create a container for the label that will animate
            m_labelContainer = new VisualElement();
            m_labelContainer.AddToClassList("hierarchy-bookmark-label-container");
            Add(m_labelContainer);
            
            // Create label with bookmark name (hidden by default, shown on hover)
            m_labelElement = new Label(bookmarkData.customName);
            m_labelElement.AddToClassList("hierarchy-bookmark-label");
            m_labelContainer.Add(m_labelElement);
         
            // Register event handlers
            RegisterCallback<ClickEvent>(OnClick);
            RegisterCallback<ContextClickEvent>(OnContextClickEvent);
        }
        

        private void OnClick(ClickEvent evt) {
            OnSelected?.Invoke(m_bookmarkData);
            evt.StopPropagation();
        }
        
        private void OnContextClickEvent(ContextClickEvent evt) {
            OnContextMenu?.Invoke(m_bookmarkData);
            evt.StopPropagation();
        }
    }
}