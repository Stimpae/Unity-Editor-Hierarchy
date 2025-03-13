using UnityEditor;
using UnityEngine;

public class HierarchyData : ScriptableSingleton<HierarchyData> {
    
    // Data needs to be assigned per scene, when we open a scene we need to load the data for that scene
    // we can use scene name as a key? or guid as names can change?? 
    
    // each scene, hold the data for the hierarchy
    // need a better way to store this data, maybe a dictionary with scene name as key and data as value  
}

public class HierarchyGameObjectData {
    public string name;
    public Color color;
    public bool isBold;
    public bool isItalic;
    public bool isUnderline;
}