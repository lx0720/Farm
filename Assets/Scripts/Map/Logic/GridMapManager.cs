using System.Collections.Generic;
using Farm.CropPlant;
using Farm.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
namespace Farm.Map
{
    public class GridMapManager : MonoSingleton<GridMapManager>,ISaveable
    {
        [SerializeField,Header("digͼƬ")]private RuleTile digTile;
        [SerializeField,Header("waterͼƬ")]private RuleTile waterTile;
        private Tilemap digTilemap;
        private Tilemap waterTilemap;

        [Header("���е�Map����")]
        [SerializeField]private List<MapData_SO> mapDataList;

        private Season currentSeason;


        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

        private Dictionary<string, bool> firstLoadDict = new Dictionary<string, bool>();

        private List<ReapItem> itemsInRadius;

        private Grid currentGrid;

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
           EventCenter.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
           EventCenter.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
           EventCenter.GameDayEvent += OnGameDayEvent;
           EventCenter.RefreshCurrentMap += RefreshMap;
        }

        private void OnDisable()
        {
           EventCenter.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
           EventCenter.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
           EventCenter.GameDayEvent -= OnGameDayEvent;
           EventCenter.RefreshCurrentMap -= RefreshMap;
        }


        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();


            foreach (var mapData in mapDataList)
            {
                firstLoadDict.Add(mapData.sceneName, true);
                InitTileDetailsDict(mapData);
            }
        }

        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            digTilemap = GameObject.FindGameObjectWithTag("Dig").GetComponent<Tilemap>();
            waterTilemap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();

           /* if (firstLoadDict[SceneManager.GetActiveScene().name])
            {
               EventCenter.CallGenerateCropEvent();
                firstLoadDict[SceneManager.GetActiveScene().name] = false;
            }*/
            RefreshMap();
        }


        /// <summary>
        /// ��Ϸһ�����
        /// </summary>
        /// <param name="day"></param>
        /// <param name="season"></param>
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;

            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daysSinceWatered > -1)
                {
                    tile.Value.daysSinceWatered = -1;
                }
                if (tile.Value.daysSinceDug > -1)
                {
                    tile.Value.daysSinceDug++;
                }
                //ʱ�䳤��5��־���ʧ
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemID == -1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }
                if (tile.Value.seedItemID != -1)
                {
                    tile.Value.growthDays++;
                }
            }

            RefreshMap();
        }


        /// <summary>
        /// ��ʼ��TileDetails
        /// </summary>
        /// <param name="mapData">地图信息</param>
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    girdX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y
                };

                //Key
                string key = tileDetails.girdX + "x" + tileDetails.gridY + "y" + mapData.sceneName;

                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                }

                if (GetTileDetails(key) != null)
                    tileDetailsDict[key] = tileDetails;
                else
                    tileDetailsDict.Add(key, tileDetails);
            }
        }


        /// <summary>
        /// ͨ��key�õ�TileDetails
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key];
            }
            return null;
        }

        /// <summary>
        /// �õ�TileDetails�����λ��
        /// </summary>
        /// <param name="mouseGridPos">����Gridλ��</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }


        /// <summary>
        /// ��Ϊ����ִ�ж���
        /// </summary>
        /// <param name="mouseWorldPos">���λ��</param>
        /// <param name="itemDetails">��Ʒ��Ϣ</param>
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);

            if (currentTile != null)
            {
                Crop currentCrop = GetCropObject(mouseWorldPos);

                //ͨ����Ʒ���������ж���Ϊ
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                       EventCenter.CallPlantSeedEvent(itemDetails.itemID, currentTile);
                       EventCenter.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType);
                       EventCenter.CallPlaySoundEvent(SoundName.Plant);
                        break;
                    case ItemType.Commodity:
                       EventCenter.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType);
                        break;
                    case ItemType.HoeTool:
                        SetDigGround(currentTile);
                        currentTile.daysSinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        //��������
                       EventCenter.CallPlaySoundEvent(SoundName.Hoe);
                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;
                        //��ˮ����
                       EventCenter.CallPlaySoundEvent(SoundName.Water);
                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        
                        currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                        break;
                    case ItemType.CollectTool:
                        currentCrop.ProcessToolAction(itemDetails, currentTile);
                       EventCenter.CallPlaySoundEvent(SoundName.Basket);
                        break;
                    case ItemType.ReapTool:
                        var reapCount = 0;
                        for (int i = 0; i < itemsInRadius.Count; i++)
                        {
                           EventCenter.CallParticleEffectEvent(ParticleEffectType.ReapableScenery, itemsInRadius[i].transform.position + Vector3.up);
                            itemsInRadius[i].SpawnHarvestItems();
                            Destroy(itemsInRadius[i].gameObject);
                            reapCount++;
                            if (reapCount >= Settings.reapAmount)
                                break;
                        }
                       EventCenter.CallPlaySoundEvent(SoundName.Reap);
                        break;

                    case ItemType.Furniture:
                       EventCenter.CallBuildFurnitureEvent(itemDetails.itemID, mouseWorldPos);
                        break;
                }

                UpdateTileDetails(currentTile);
            }
        }

        /// <summary>
        /// �õ�Grid����
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <returns></returns>
        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);

            Crop currentCrop = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                    currentCrop = colliders[i].GetComponent<Crop>();
            }
            return currentCrop;
        }


        /// <summary>
        /// �ڷ�Χ�ڴ��ڿ����ո������
        /// </summary>
        /// <param name="tool">����</param>
        /// <returns></returns>
        public bool HaveReapableItemsInRadius(Vector3 mouseWorldPos, ItemDetails tool)
        {
            itemsInRadius = new List<ReapItem>();

            Collider2D[] colliders = new Collider2D[20];

            Physics2D.OverlapCircleNonAlloc(mouseWorldPos, tool.itemUseRadius, colliders);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<ReapItem>())
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            itemsInRadius.Add(item);
                        }
                    }
                }
            }
            return itemsInRadius.Count > 0;
        }

        /// <summary>
        /// �����ڿ�
        /// </summary>
        /// <param name="tile"></param>
        private void SetDigGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.girdX, tile.gridY, 0);
            if (digTilemap != null)
                digTilemap.SetTile(pos, digTile);
        }

        /// <summary>
        /// ���ý�ˮ
        /// </summary>
        /// <param name="tile"></param>
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.girdX, tile.gridY, 0);
            if (waterTilemap != null)
                waterTilemap.SetTile(pos, waterTile);
        }

        /// <summary>
        /// ����Tile����Ϣ
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.girdX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
        }


        /// <summary>
        /// ˢ�µ�ͼ
        /// </summary>
        private void RefreshMap()
        {
            if (digTilemap != null)
                digTilemap.ClearAllTiles();
            if (waterTilemap != null)
                waterTilemap.ClearAllTiles();

            foreach (var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }

            DisplayMap(SceneManager.GetSceneAt(SceneManager.sceneCount-1).name);
        }


        /// <summary>
        /// չʾ��ǰ������ͼ����Ϣ
        /// </summary>
        /// <param name="sceneName">��������</param>
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;

                if (key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDug > -1)
                        SetDigGround(tileDetails);
                    if (tileDetails.daysSinceWatered > -1)
                        SetWaterGround(tileDetails);
                    if (tileDetails.seedItemID > -1)
                       EventCenter.CallPlantSeedEvent(tileDetails.seedItemID, tileDetails);
                }
            }
        }


        /// <summary>
        /// ͨ��������������ȡ��ǰ������ͼ����Ϣ
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <param name="gridDimensions">�����ĳߴ�</param>
        /// <param name="gridOrigin">�����ĳ�ʼλ��</param>
        /// <returns>�Ƿ������Щ��Ϣ</returns>
        public bool GetGridDimensions(string sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            foreach (var mapData in mapDataList)
            {
                if (mapData.sceneName == sceneName)
                {
                    gridDimensions.x = mapData.gridWidth;
                    gridDimensions.y = mapData.gridHeight;

                    gridOrigin.x = mapData.originX;
                    gridOrigin.y = mapData.originY;

                    return true;
                }
            }
            return false;
        } 

        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.tileDetailsDict = this.tileDetailsDict;
            saveData.firstLoadDict = this.firstLoadDict;
            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.tileDetailsDict = saveData.tileDetailsDict;
            this.firstLoadDict = saveData.firstLoadDict;
        }
    }
}