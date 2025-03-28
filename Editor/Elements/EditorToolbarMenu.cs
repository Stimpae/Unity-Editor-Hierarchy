using System;
using Hierarchy.Libraries;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorToolbarMenu : ToolbarMenu {
    public event Action OnClick;

    private readonly VisualElement m_textContainer;
    private readonly VisualElement m_iconContainer;
    
    public EditorToolbarMenu() {
        // Add the USS class name & load the style sheet for pseudo states
        // we do it this way as we do not have access to the pseudo states yet in ui toolkit.
        AddToClassList("editor-toolbar-menu");
        var styleSheet = Resources.Load<StyleSheet>("EditorToolbarMenu");
        if (styleSheet != null) styleSheets.Add(styleSheet);
        
        style.unityTextAlign = TextAnchor.MiddleCenter;
        style.flexDirection = FlexDirection.RowReverse;
        style.color = EditorColourLibrary.ToolbarText;
        
        this.SetBorderRadius(new VisualElementUtils.ElementLength4(2,2,2,2));
        this.SetBorderWidth(new VisualElementUtils.ElementFloat4(0,0,0,0));
        this.SetPadding(new VisualElementUtils.ElementLength4(0,3,0 ,3));
        this.SetMargin(new VisualElementUtils.ElementLength4(3,0,3,3));
        
        m_textContainer = new VisualElement {
            style = {
                alignItems = Align.Center
            }
        };
        
        m_iconContainer = new VisualElement {
            style = {
                alignItems = Align.Center
            }
        };
        
        Add(m_textContainer);
        Add(m_iconContainer);
        
        m_textContainer.style.display = DisplayStyle.None;
        m_iconContainer.style.display = DisplayStyle.None;

        RegisterCallback<ClickEvent>((evt) => {
            OnClick?.Invoke();
        });
    }
    
    public void AddMenuText(string text) {
        var textElement = new Label {
            text = text,
            style = {
                unityTextAlign = TextAnchor.MiddleCenter
            }
        };
        m_textContainer.Add(textElement);
        m_textContainer.style.display = DisplayStyle.Flex;
    }
    
    public void AddMenuIcon(string iconName, float width, float height) {
        var icon = EditorGUIUtility.IconContent(iconName).image;
        var iconElement = new Image {
            image = icon,
            style = {
                width = width,
                height = height
            }
        };

        m_iconContainer.Add(iconElement);
        m_iconContainer.style.display = DisplayStyle.Flex;
    }
    
    // factory to create a new menu item
}
