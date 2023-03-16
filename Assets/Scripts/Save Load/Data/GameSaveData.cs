using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Save
{
    /// <summary>
    /// 游戏的数据存储
    /// </summary>
    [System.Serializable]
    public class GameSaveData
    {
        public GameScene dataSceneName;
        //存储人物三维坐标，string人物名字
        public Dictionary<string, SerializableVector3> characterPosDict;
        //存储场景中的物体
        public Dictionary<string, List<SceneItem>> sceneItemDict;
        //存储场景中的家具
        public Dictionary<string, List<SceneFurniture>> sceneFurnitureDict;
        //存储地图数据
        public Dictionary<string, TileDetails> tileDetailsDict;
        //是否是第一次加载
        public Dictionary<GameScene, bool> firstLoadDict;
        //存储游戏背包数据
        public Dictionary<string, List<InventoryItem>> inventoryDict;
        //存储游戏时间
        public Dictionary<string, int> timeDict;
        //玩家的金钱数
        public int playerMoney;
        //NPC
        public GameScene targetScene;
        public bool interactable;
        public int animationInstanceID;

    }
}
