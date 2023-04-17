using System;
using System.Collections.Generic;
using Farm.Save;
using Farm.Tool;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
namespace Farm.Map
{
    public class GridMapManager : MonoSingleton<GridMapManager>
    {
        [SerializeField] private List<GameMapData> gameMapDatas;
        [SerializeField] private TileBase digTileBase;
        [SerializeField] private TileBase waterTileBase;
        private Tilemap digTileMap;
        private Tilemap waterTileMap;
        private Dictionary<string, TileDetails> allTileDetailsDict;
        private Dictionary<string, TileDetails> handOnTileDetailsDict;

        private Dictionary<GameScene, bool> sceneFirstLoadDict;

        protected override void Awake()
        {
            base.Awake();
            allTileDetailsDict = new Dictionary<string, TileDetails>();
            handOnTileDetailsDict = new Dictionary<string, TileDetails>();
            sceneFirstLoadDict = new Dictionary<GameScene, bool>();

            for(int i=0;i<gameMapDatas.Count;i++)
            {
                if (!sceneFirstLoadDict.ContainsKey(gameMapDatas[i].gameScene))
                    sceneFirstLoadDict.Add(gameMapDatas[i].gameScene, true);
            }
        }

        private void OnEnable()
        {
            EventManager.AddEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
            EventManager.AddEventListener(ConstString.InvokeEveryDayEvent,OnEveryDayUpdate);
            EventManager.AddEventListener(ConstString.BeforeSceneLoadEvent, OnBeforeSceneLoad);
        }
        private void OnDisable()
        {
            EventManager.RemoveEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
            EventManager.RemoveEventListener(ConstString.InvokeEveryDayEvent,OnEveryDayUpdate);
            EventManager.RemoveEventListener(ConstString.BeforeSceneLoadEvent, OnBeforeSceneLoad);
        }

        private void OnBeforeSceneLoad()
        {

        }

        private void OnAfterSceneLoad(GameScene targetScene)
        {
            digTileMap = GameObject.FindGameObjectWithTag(ConstString.DigTileMapTag).GetComponent<Tilemap>();
            waterTileMap = GameObject.FindGameObjectWithTag(ConstString.WaterTileMapTag).GetComponent<Tilemap>();
            GameMapData mapData = gameMapDatas.Find(i => i.gameScene == targetScene);
            if(mapData!=null)
                InitMapData(mapData);
        }

        private void OnEveryDayUpdate()
        {
            foreach (TileDetails tileDetails in handOnTileDetailsDict.Values)
            {
                if (tileDetails.seedId != -1)
                    tileDetails.growthDays++;
                if (tileDetails.dugDaysCount > -1)
                    tileDetails.dugDaysCount++;
                if (tileDetails.waterDaysCount > -1)
                    tileDetails.waterDaysCount++;
                //到3天不种就移除这块土地
                if(tileDetails.seedId == -1 && tileDetails.dugDaysCount >= 3)
                {
                    handOnTileDetailsDict.Remove(KeyHelper.Instance.GetKey(tileDetails.tileX,tileDetails.tileY,tileDetails.gameScene));
                }
                //到3天都不浇水，种子死去
                if(tileDetails.seedId!=-1 && tileDetails.waterDaysCount ==-1 && tileDetails.growthDays >=3)
                {
                    tileDetails.seedId = -1;
                    tileDetails.growthDays = -1;
                    tileDetails.dugDaysCount = 0;
                }
            }
            LoadMapData(GameTools.StringToEnum(SceneManager.GetSceneAt(SceneManager.sceneCount-1).name));
        }
        private void InitMapData(GameMapData mapData)
        {
            if (sceneFirstLoadDict[mapData.gameScene])
            {
                for (int i = 0; i < mapData.gameTileDatas.Count; i++)
                {
                    int x = mapData.gameTileDatas[i].tileCoordinate.x;
                    int y = mapData.gameTileDatas[i].tileCoordinate.y;
                    string tileKey = KeyHelper.Instance.GetKey(x, y, mapData.gameScene);
                    if (!allTileDetailsDict.ContainsKey(tileKey))
                    {
                        TileDetails tileDetails = new TileDetails()
                        {
                            tileX = x,
                            tileY = y
                        };
                        if (mapData.gameTileDatas[i].tileType == TileType.CanDig)
                            tileDetails.canDig = true;
                        if (mapData.gameTileDatas[i].tileType == TileType.CanDrop)
                            tileDetails.canDrop = true;
                        if (mapData.gameTileDatas[i].tileType == TileType.CanPlace)
                            tileDetails.canPlace = true;
                        if (mapData.gameTileDatas[i].tileType == TileType.CantWalk)
                            tileDetails.cantWalk = true;
                        allTileDetailsDict.Add(tileKey, tileDetails);
                    }
                    sceneFirstLoadDict[mapData.gameScene] = false;
                }
            }
            else
            {
                LoadMapData(mapData.gameScene);
            }
        }

        private void LoadMapData(GameScene targetScene)
        {
            foreach(TileDetails tileDetails in handOnTileDetailsDict.Values)
            {
                if(tileDetails.gameScene == targetScene)
                {
                    if (tileDetails.dugDaysCount > -1)
                        SetDigTilBase(tileDetails);
                    if (tileDetails.waterDaysCount > -1)
                        SetWaterTilBase(tileDetails);
                }
            }
        }

        public TileDetails GetTileDetails(Vector2Int position, GameScene targetScene)
        {
            string tileKey = KeyHelper.Instance.GetKey(position.x, position.y, targetScene);
            if (allTileDetailsDict.ContainsKey(tileKey))
            {
                return allTileDetailsDict[tileKey];
            }
            return null;
        }

        public void SetDigTilBase(TileDetails tileDetails)
        {
            if (digTileMap == null)
                return;
            Vector3Int coordinate = new Vector3Int(tileDetails.tileX, tileDetails.tileY, 0);
            digTileMap.SetTile(coordinate, digTileBase);
            string tileKey = KeyHelper.Instance.GetKey(tileDetails.tileX, tileDetails.tileY, GameTools.StringToEnum(SceneManager.GetActiveScene().ToString()));
            if (!handOnTileDetailsDict.ContainsKey(tileKey))
            {
                handOnTileDetailsDict.Add(tileKey, tileDetails);
            }
        }
        public void SetWaterTilBase(TileDetails tileDetails)
        {
            if (waterTileMap == null)
                return;
            Vector3Int coordinate = new Vector3Int(tileDetails.tileX, tileDetails.tileY, 0);
            waterTileMap.SetTile(coordinate, waterTileBase);
            string tileKey = KeyHelper.Instance.GetKey(tileDetails.tileX, tileDetails.tileY, GameTools.StringToEnum(SceneManager.GetActiveScene().ToString()));
            if (!handOnTileDetailsDict.ContainsKey(tileKey))
            {
                handOnTileDetailsDict.Add(tileKey, tileDetails);
            }
        }

        public Vector2Int GetMapSize(GameScene targetScene)
        {
            GameMapData mapData = gameMapDatas.Find(i => i.gameScene == targetScene);
            return mapData.mapSize;
        }
        public Vector2Int GetMapOriginPosition(GameScene targetScene)
        {
            GameMapData mapData = gameMapDatas.Find(i => i.gameScene == targetScene);
            return mapData.mapOriginPosition;
        }
    }
}