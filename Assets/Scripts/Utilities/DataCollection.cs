using System.Collections.Generic;
using UnityEngine;

/*[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public Sprite itemOnWorldSprite;
    public string itemDescription;
    public int itemUseRadius;
    public bool canPickedup;
    public bool canDropped;
    public bool canCarried;
    public int itemPrice;
    [Range(0, 1)]
    public float sellPercentage;
}*/
/*
[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}
*/
[System.Serializable]
public struct SaveItemInfo
{
    public int itemId;
    public int itemCount;
}

[System.Serializable]
public class AnimatorType
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}

[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}

[System.Serializable]
public class SceneFurniture
{
    public int itemID;
    public SerializableVector3 position;
    public int boxIndex;
}



[System.Serializable]
public class NPCPosition
{
    public Transform npcTransform;
    public GameScene initialScene;
    public Vector3 position;
}

//场景路径
[System.Serializable]
public class SceneRoute
{
    public GameScene fromSceneName;
    public GameScene gotoSceneName;
    public List<ScenePath> scenePathList;
}

[System.Serializable]
public class ScenePath
{
    public GameScene sceneName;
    public Vector2Int fromGridCell;
    public Vector2Int gotoGridCell;
}