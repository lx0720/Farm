using System.Collections;
using System.Collections.Generic;
using Farm.Save;
using UnityEngine;
namespace Farm.Inventory
{
    public class InventoryManager : MonoSingleton<InventoryManager>
    {
        [Header("��Ʒ��Ϣ���ݿ�")]
        public ItemDataList_SO itemDataList_SO;
        [Header("��ͼ��Ϣ���ݿ�")]
        public BluePrintDataList_SO bluePrintData;
        [Header("������Ϣ���ݿ�")]
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
        /// �жϱ����Ƿ���ڿռ�
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
        /// ����Ҿ�
        /// </summary>
        /// <param name="ID">��ͼID</param>
        /// <param name="mousePos">����λ��(�����λ��)</param>
        private void BuildFurniture(int ID,Vector3 mousePos)
        {
            //1.�Ƴ���ͼ  InventoryManager
            //2.�õ���ǰID����ͼ��Ϣ BluePrintDetails
            //3.�Ƴ���ͼ��Ҫ���ĵ���Ʒ InventoryManager BluePrintDetails
            RemoveItem(ID, 1);
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            foreach (var item in bluePrint.resourceItem)
            {
                RemoveItem(item.itemID, item.itemAmount);
            }
        }
        /// <summary>
        /// �ջ����嵽�������
        /// 1.��ȡ��������û�е�ǰ���� �з��ض�Ӧ��λ�ã����򷵻�-1 
        /// 2.��Ӹ�����
        /// 3.ˢ��InventoryUI   
        /// </summary>
        /// <param name="ID">��ƷID</param>
        private void HarvestAtPlayerPosition(int ID)
        {
            int index = GetItemIndexInBag(ID);
            AddItemAtIndex(ID, index, 1);
            EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        /// <summary>
        /// 通过ID返回物品信息
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        /// <summary>
        /// ͨ��Item���������
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">�Ƿ����������</param>
        public void AddItem(Item item, bool toDestory)
        {
            var index = GetItemIndexInBag(item.itemID);

            AddItemAtIndex(item.itemID, index, 1);

            Debug.Log(GetItemDetails(item.itemID).itemID + "Name: " + GetItemDetails(item.itemID).itemName);
            if (toDestory)
            {
                Destroy(item.gameObject);
            }

            //ˢ�±���
            EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }


        /// <summary>
        /// ��ȡ��ǰ��Ʒ�ڱ����е�λ�ñ��
        /// </summary>
        /// <param name="ID">��ƷId</param>
        /// <returns>�����ڱ����е�Index��û���򷵻�-1</returns>
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
        /// ������嵽��ҵı���
        /// 1.�ж�index�Ƿ�Ϊ-1��Ϊ-1��Ϊ�����ڣ�����Ϊ����
        /// 2.indexΪ-1ʱ����Ҫ�жϱ����Ƿ���ڿռ�
        /// 3.��Ϊ-1ʱ����Ϊָ���������������
        /// </summary>
        /// <param name="ID">�����ID</param>
        /// <param name="index">�����ڱ����е�Index</param>
        /// <param name="amount">���������</param>
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
        /// Player背包范围内交换物�?
        /// </summary>
        /// <param name="fromIndex">起始序号</param>
        /// <param name="targetIndex">目标数据序号</param>
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
        /// 跨背包交换数�?
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

                if (targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)  //有不相同的两个物�?
                {
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (currentItem.itemID == targetItem.itemID) //相同的两个物�?
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                else    //目标空格�?
                {
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                EventCenter.CallUpdateInventoryUI(locationFrom, currentList);
                EventCenter.CallUpdateInventoryUI(locationTarget, targetList);
            }
        }

        /// <summary>
        /// 根据位置返回背包数据列表
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
        /// �Ƴ����������������(ˢ�±���)
        /// </summary>
        /// <param name="ID">�����ID</param>
        /// <param name="removeAmount">���������</param>
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
        /// ��������
        /// </summary>
        /// <param name="itemDetails">�����׵�������Ϣ</param>
        /// <param name="amount">��������</param>
        /// <param name="isSellTrade">��?��</param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            //�õ���ǰ������bag�е�λ��
            int index = GetItemIndexInBag(itemDetails.itemID);

            if (isSellTrade)    //��?
            {
                if (playerBag.itemList[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    //���ӽ�Ǯ������
                    cost = (int)(cost * itemDetails.sellPercentage);
                    PlayerMoney += cost;
                }
            }
            else if (PlayerMoney - cost >= 0)   //��?
            {
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemID, index, amount);
                }
                PlayerMoney -= cost;
            }
            //ˢ��InventoryUI
            EventCenter.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 检查建造资源物品库�?
        /// </summary>
        /// <param name="ID">图纸ID</param>
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
        /// 查找箱子数据
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
        /// 加入箱子数据字典
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