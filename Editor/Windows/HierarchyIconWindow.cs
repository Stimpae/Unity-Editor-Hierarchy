using System;
using Hierarchy;
using Hierarchy.Utils;
using UnityEngine;
using UnityEngine.UIElements;

public class HierarchyIconWindow : InstancedWindow<HierarchyIconWindow> {
    public override void ShowPopup(Vector2 pos) {
        base.ShowPopup(pos);
        instance.position = instance.position.SetPosition(pos);
    }
    
    private void OnLostFocus() {
        Close();
    }
}
 