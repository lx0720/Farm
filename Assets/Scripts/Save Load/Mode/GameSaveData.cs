using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Save
{
    //游戏数据类
    [System.Serializable]
    public class GameSaveData
    {
        /*        public GameScene dataSceneName;
                public Dictionary<string, SerializableVector3> characterPosDict;
                public Dictionary<string, List<SceneItem>> sceneItemDict;
                public Dictionary<string, List<SceneFurniture>> sceneFurnitureDict;
                //public Dictionary<string, TileDetails> tileDetailsDict;
                public Dictionary<GameScene, bool> firstLoadDict;
                public Dictionary<string, List<InventoryItem>> inventoryDict; 
                public Dictionary<string, int> timeDict;
                public GameSeason currentGameSeason;
                public int playerMoney;
                public GameScene targetScene;
                public bool interactable;
                public int animationInstanceID;
        */
        public string moduleName;
        public GameScene saveGameScene;
        public Dictionary<string, List<InventoryItemInfo>> inventoryItemDict;
        public Dictionary<CharacterName,CharacterInfos> characterPositionDict;
        public Dictionary<GameScene, List<SceneItemInfo>> sceneItemInfoDict;
        public Dictionary<string, TileDetails> tileDetailsDict;
        public GameTime gameTime;

    }
    [System.Serializable]
    public class InventoryItemInfo
    {
        public int itemId;
        public int itemNumber;
    }
    [System.Serializable]
    public class CharacterInfos
    {
        public Vector3Int position;
        public Vector2Int endDirection;
        public CharacterInfos(Vector3 position)
        {
            this.position = new Vector3Int((int)position.x,(int)position.y,(int)position.z);
        }
        
    }

    public class SceneItemInfo
    {
        public int itemId;
        public CharacterInfo itemPosition;
    }


}
