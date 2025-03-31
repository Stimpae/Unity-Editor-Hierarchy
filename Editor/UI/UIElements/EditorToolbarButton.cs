using System;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorToolbarButton : ToolbarButton {
    public EditorToolbarButton(Action onClick) {
        AddToClassList("editor-toolbar-button");
        var styleSheet = Resources.Load<StyleSheet>("EditorToolbarButton");
        if (styleSheet != null) styleSheets.Add(styleSheet);
        
        RegisterCallback<ClickEvent>((evt) => {
            onClick?.Invoke();
        });
    }
    
}
