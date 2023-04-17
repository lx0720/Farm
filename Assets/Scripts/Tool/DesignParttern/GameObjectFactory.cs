using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectFactory 
{
    public static GameObject GetGameObject(string goName)
    {
        return goName switch
        {
            "GameProgress" => Resources.Load<GameObject>("Prefabs/UI/Menu/GameProgress"),
            "Item"=>Resources.Load<GameObject>("Prefabs/Items/Item"),
            _ => null

        };
    }
}
