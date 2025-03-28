using UnityEditor;
using UnityEngine;

public class HierarchyData : ScriptableSingleton<HierarchyData> {
    // Data needs to be assigned per scene, when we open a scene we need to load the data for that scene
    // we can use scene name as a key? or guid as names can change?? 
    
    // each scene, hold the data for the hierarchy
    // need a better way to store this data, maybe a dictionary with scene name as key and data as value  
}

public class HierarchyGameObjectData {
    // all the data for a game object in the hierarchy
    // hide flags to lock
}