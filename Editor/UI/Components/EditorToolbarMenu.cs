using System;
using Hierarchy.Libraries;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorToolbarMenu : ToolbarMenu {
    private Image m_iconImage;

    /// <summary>
    /// 
    /// </summary>
    public EditorToolbarMenu(Action onClick) {
        AddToClassList("editor-toolbar-menu");
        var styleSheet = Resources.Load<StyleSheet>("EditorToolbarMenu");
        if (styleSheet != null) styleSheets.Add(styleSheet);
        
        RegisterCallback<ClickEvent>((evt) => {
            onClick?.Invoke();
        });
    }

    /// <summary>
    /// Add an icon to the toggle
    /// </summary>
    /// <param name="iconName">Name of the icon from EditorGUIUtility.IconContent</param>
    public void AddIcon(string iconName) {
        if (m_iconImage == null) {
            m_iconImage = new Image();
            this.hierarchy.Insert(0, m_iconImage);
        }

        var icon = EditorGUIUtility.IconContent(iconName).image;
        m_iconImage.image = icon;
    }

    /// <summary>
    /// Add a custom texture as an icon to the toggle
    /// </summary>
    /// <param name="texture">The texture to use as icon</param>
    public void AddIcon(Texture2D texture) {
        if (m_iconImage == null) {
            m_iconImage = new Image();
            this.hierarchy.Insert(0, m_iconImage);
        }

        m_iconImage.image = texture;
    }

    /// <summary>
    /// Set icon size
    /// </summary>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    public void SetIconSize(float width, float height) {
        if (m_iconImage != null) {
            m_iconImage.style.width = width;
            m_iconImage.style.height = height;
        }
    }
}