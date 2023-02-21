using System.Collections;
using System.Collections.Generic;
using Farm.Save;
using UnityEngine;
namespace Farm.Inventory
{
    public class InventoryManager : MonoSingleton<InventoryManager>
    {
        [Header("物品信息数据库")]
        public ItemDataList_SO itemDataList_SO;
        [Header("蓝图信息数据库")]
        public BluePrintDataList_SO bluePrintData;
        [Header("背包信息数据库")]
        public InventoryBag_SO playerBagTemp;
        public InventoryBag_SO playerBag;

        private InventoryBag_SO currentBoxBag;

        public int PlayerMoney {  
            get;
            private set;
        }

        private Dictionary<string, List<InventoryItem>> boxDataDict = new Dictionary<string, List<InventoryItem>>();

        public int BoxDataAmount => boxDataDict.Count;

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventCenter.DropItemEvent += OnDropItemEvent;
            EventCenter.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
            EventCenter.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventCenter.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventCenter.StartNewGameEvent += OnStartNewGameEvent;
        }

        private void OnDisable()
        {
            EventCenter.DropItemEvent -= OnDropItemEvent;
            EventCenter.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
            EventCenter.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventCenter.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventCenter.StartNewGameEvent -= OnStartNewGameEvent;
        }


        private void Start()
        {
            /*ISaveable saveable = this;
            saveable.RegisterSaveable();*/
            // EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OnStartNewGameEvent(int obj)
        {
            //playerBag = Instantiate(playerBagTemp);
            PlayerMoney = Settings.playerStartMoney;
            boxDataDict.Clear();
            EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            currentBoxBag = bag_SO;
        }


        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            BuildFurniture(ID, mousePos);
        }

        private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(ID, 1);
        }

        private void OnHarvestAtPlayerPosition(int ID)
        {
            HarvestAtPlayerPosition(ID);
        }


        #region Conditions
        /// <summary>
        /// 判断背包是否存在空间
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                    return true;
            }
            return false;
        }

        #endregion


        /// <summary>
        /// 建造家具
        /// </summary>
        /// <param name="ID">蓝图ID</param>
        /// <param name="mousePos">鼠标的位置(建造的位置)</param>
        private void BuildFurniture(int ID,Vector3 mousePos)
        {
            //1.移除蓝图  InventoryManager
            //2.得到当前ID的蓝图信息 BluePrintDetails
            //3.移除蓝图需要消耗的物品 InventoryManager BluePrintDetails
            RemoveItem(ID, 1);
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            foreach (var item in bluePrint.resourceItem)
            {
                RemoveItem(item.itemID, item.itemAmount);
            }
        }
        /// <summary>
        /// 收获物体到玩家手上
        /// 1.获取背包中有没有当前物体 有返回对应的位置，无则返回-1 
        /// 2.添加该物体
        /// 3.刷新InventoryUI   
        /// </summary>
        /// <param name="ID">物品ID</param>
        private void HarvestAtPlayerPosition(int ID)
        {
            int index = GetItemIndexInBag(ID);
            AddItemAtIndex(ID, index, 1);
            EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        /// <summary>
        /// 閫氳繃ID杩斿洖鐗╁搧淇℃伅
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        /// <summary>
        /// 通过Item来添加物体
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">是否是添加物体</param>
        public void AddItem(Item item, bool toDestory)
        {
            var index = GetItemIndexInBag(item.itemID);

            AddItemAtIndex(item.itemID, index, 1);

            Debug.Log(GetItemDetails(item.itemID).itemID + "Name: " + GetItemDetails(item.itemID).itemName);
            if (toDestory)
            {
                Destroy(item.gameObject);
            }

            //刷新背包
            EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }


        /// <summary>
        /// 获取当前物品在背包中的位置编号
        /// </summary>
        /// <param name="ID">物品Id</param>
        /// <returns>物体在背包中的Index，没有则返回-1</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 添加物体到玩家的背包
        /// 1.判断index是否为-1，为-1则为不存在，否则为存在
        /// 2.index为-1时，需要判断背包是否存在空间
        /// 3.不为-1时，则为指定的物体添加数量
        /// </summary>
        /// <param name="ID">物体的ID</param>
        /// <param name="index">物体在背包中的Index</param>
        /// <param name="amount">物体的数量</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if (index == -1 && CheckBagCapacity())    
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
        }

        /// <summary>
        /// Player鑳屽寘鑼冨洿鍐呬氦鎹㈢墿鍝?
        /// </summary>
        /// <param name="fromIndex">璧峰搴忓彿</param>
        /// <param name="targetIndex">鐩爣鏁版嵁搴忓彿</param>
        public void SwapItem(int fromIndex, int targetIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[targetIndex];

            if (targetItem.itemID != 0)
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            else
            {
                playerBag.itemList[targetIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }

            EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 璺ㄨ儗鍖呬氦鎹㈡暟鎹?
        /// </summary>
        /// <param name="locationFrom"></param>
        /// <param name="fromIndex"></param>
        /// <param name="locationTarget"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(InventoryLocation locationFrom, int fromIndex, InventoryLocation locationTarget, int targetIndex)
        {
            var currentList = GetItemList(locationFrom);
            var targetList = GetItemList(locationTarget);

            InventoryItem currentItem = currentList[fromIndex];

            if (targetIndex < targetList.Count)
            {
                InventoryItem targetItem = targetList[targetIndex];

                if (targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)  //鏈変笉鐩稿悓鐨勪袱涓墿鍝?
                {
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (currentItem.itemID == targetItem.itemID) //鐩稿悓鐨勪袱涓墿鍝?
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                else    //鐩爣绌烘牸瀛?
                {
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                EventCenter.CallUpdateInventoryUI(locationFrom, currentList);
                EventCenter.CallUpdateInventoryUI(locationTarget, targetList);
            }
        }

        /// <summary>
        /// 鏍规嵁浣嶇疆杩斿洖鑳屽寘鏁版嵁鍒楄〃
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private List<InventoryItem> GetItemList(InventoryLocation location)
        {
            return location switch
            {
                InventoryLocation.Player => playerBag.itemList,
                InventoryLocation.Box => currentBoxBag.itemList,
                _ => null
            };
        }

        /// <summary>
        /// 移除背包中物体的数量(刷新背包)
        /// </summary>
        /// <param name="ID">物体的ID</param>
        /// <param name="removeAmount">物体的数量</param>
        private void RemoveItem(int ID, int removeAmount)
        {
            int index = GetItemIndexInBag(ID);

            if (playerBag.itemList[index].itemAmount > removeAmount)
            {
                int amount = playerBag.itemList[index].itemAmount - removeAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                playerBag.itemList[index] = item;
            }
            else if (playerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                playerBag.itemList[index] = item;
            }

            EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }


        /// <summary>
        /// 交易物体
        /// </summary>
        /// <param name="itemDetails">被交易的物体信息</param>
        /// <param name="amount">物体数量</param>
        /// <param name="isSellTrade">卖?买？</param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            //得到当前物体在bag中的位置
            int index = GetItemIndexInBag(itemDetails.itemID);

            if (isSellTrade)    //卖?
            {
                if (playerBag.itemList[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    //增加金钱的数量
                    cost = (int)(cost * itemDetails.sellPercentage);
                    PlayerMoney += cost;
                }
            }
            else if (PlayerMoney - cost >= 0)   //买?
            {
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemID, index, amount);
                }
                PlayerMoney -= cost;
            }
            //刷新InventoryUI
            EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 妫�鏌ュ缓閫犺祫婧愮墿鍝佸簱瀛?
        /// </summary>
        /// <param name="ID">鍥剧焊ID</param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetails = bluePrintData.GetBluePrintDetails(ID);

            foreach (var resourceItem in bluePrintDetails.resourceItem)
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;
                }
                else return false;
            }
            return true;
        }

        /// <summary>
        /// 鏌ユ壘绠卞瓙鏁版嵁
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBoxDataList(string key)
        {
            if (boxDataDict.ContainsKey(key))
                return boxDataDict[key];
            return null;
        }

        /// <summary>
        /// 鍔犲叆绠卞瓙鏁版嵁瀛楀吀
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxDataDict(Box box)
        {
            var key = box.name + box.GetBoxIndex();
            if (!boxDataDict.ContainsKey(key))
                boxDataDict.Add(key, box.GetInventoryBagSO().itemList);
            Debug.Log(key);
        }

        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.playerMoney = this.PlayerMoney;

            saveData.inventoryDict = new Dictionary<string, List<InventoryItem>>();
            saveData.inventoryDict.Add(playerBag.name, playerBag.itemList);

            foreach (var item in boxDataDict)
            {
                saveData.inventoryDict.Add(item.Key, item.Value);
            }
            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.PlayerMoney = saveData.playerMoney;
            playerBag = Instantiate(playerBagTemp);
            playerBag.itemList = saveData.inventoryDict[playerBag.name];

            foreach (var item in saveData.inventoryDict)
            {
                if (boxDataDict.ContainsKey(item.Key))
                {
                    boxDataDict[item.Key] = item.Value;
                }
            }

            EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
    }
}