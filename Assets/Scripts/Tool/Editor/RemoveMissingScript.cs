using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class RemoveMissingScript : EditorWindow
{
     [MenuItem("Tool/RemoveMissingScripts")]
     public static void Clear()
     {
         GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        int count = 0;
         foreach(GameObject go in gameObjects)
         {
            count+=GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
         }
        Debug.Log(count+"¸ö½Å±¾É¾³ýÍê±Ï");
     }
}
