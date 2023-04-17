using Farm.Tool;
using UnityEngine;
using System.Collections.Generic;


public class InventoryManager : MonoSingleton<InventoryManager>
{
    private Dictionary<int, int> inventoryItemsDict;
    private int playerMoney = 100;
    [SerializeField]private InventoryItemDataListSO inventoryItemDataList;
    [SerializeField]private Transform itemParent;
    private int chooseItemId;


    protected override void Awake()
    {
        base.Awake();
        inventoryItemsDict = new Dictionary<int, int>();
    }
    private void Start()
    {
        InitInventory();
    }
    
    private void OnEnable()
    {
        EventManager.AddEventListener<int>(ConstString.UpdateHightLightEvent, OnShowHighLight);
        EventManager.AddEventListener<int, int>(ConstString.EndDragItemEvent, OnEndDragItem);
        EventManager.AddEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
    }

    private void OnDisable()
    {
        EventManager.RemoveEventListener<int>(ConstString.UpdateHightLightEvent, OnShowHighLight);
        EventManager.RemoveEventListener<int, int>(ConstString.EndDragItemEvent, OnEndDragItem);
        EventManager.RemoveEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
    }

    private void OnEndDragItem(int currentIndex,int targetIndex)
    {
        if (targetIndex != -1)
        {
            if (inventoryItemDataList.list[targetIndex].itemAmount == 0)
            {       
                ChangeCurrentItemIndex(currentIndex,targetIndex);
            }
            else if (inventoryItemDataList.list[targetIndex].itemAmount > 0)
            {
                ChangeCurrentItemToTargetIndex(currentIndex, targetIndex);
            }
        }
        else
        {
            DiscardItem(currentIndex);
        }
        EventManager.InvokeEventListener(ConstString.RefreshInventoryUIEvent, inventoryItemDataList.list);
    }

    private void OnShowHighLight(int itemIndex)
    {
        
    }
    private void OnAfterSceneLoad(GameScene gameScene)
    {
        if (gameScene == GameScene.YardScene)
            itemParent = GameObject.FindGameObjectWithTag("ItemParent").transform;
        EventManager.InvokeEventListener(ConstString.RefreshInventoryUIEvent,inventoryItemDataList.list);
    }

    public int GetPlayerMoney() => playerMoney;
    private void InitInventory()
    {
        inventoryItemsDict.Clear();
        foreach(InventoryItem item in inventoryItemDataList.list)
        {
            if(!inventoryItemsDict.ContainsKey(item.itemId))
            {
                inventoryItemsDict.Add(item.itemId, item.itemAmount);
            }
            else
            {
                inventoryItemsDict[item.itemId] = item.itemAmount;
            }
        }
    }
    private void ChangeCurrentItemIndex(int currentIndex, int targetIndex)
    {
        InventoryItem item = new InventoryItem
        {
            itemAmount = inventoryItemDataList.list[currentIndex].itemAmount,
            itemId = inventoryItemDataList.list[currentIndex].itemId
        };
        inventoryItemDataList.list[targetIndex] = item;
        inventoryItemDataList.list[currentIndex] = new InventoryItem { itemId = 0, itemAmount = 0 };
        InitInventory();
    }
    private void ChangeCurrentItemToTargetIndex(int currentIndex,int targetIndex)
    {
        InventoryItem targetItem = inventoryItemDataList.list[targetIndex];
        InventoryItem currentItem = inventoryItemDataList.list[currentIndex];
        inventoryItemDataList.list[currentIndex] = targetItem;
        inventoryItemDataList.list[targetIndex] = currentItem;
        InitInventory();
    }
    private void DiscardItem(int currentIndex)
    {
        int itemAmount = inventoryItemDataList.list[currentIndex].itemAmount - 1;
        int itemId = inventoryItemDataList.list[currentIndex].itemId;
        int _itemId = itemId;
        if (itemAmount==0)
        {
            itemId = 0;
        }
        InventoryItem item = new InventoryItem
        {
            itemId = itemId,
            itemAmount = itemAmount
        };
        inventoryItemDataList.list[currentIndex] = item;
        CreateItemInScene(_itemId,1);
        InitInventory();
    }

    private void CreateItemInScene(int itemId,int number)
    {
        Vector3 mousePosition = CursorManager.Instance.GetMousePosition();
        GameObject createItem = GameObjectPool.Instance.GetObject("Item");
        createItem.GetComponent<ItemUI>().SetItemInfo(itemId,number);
        createItem.transform.position = mousePosition;
        createItem.transform.SetParent(itemParent);
    }
    public void AddItemIntoInventory(int itemId,int number)
    {
        bool itemIsHave;
        int nullIndex = GetIndexInInventory(itemId, out itemIsHave);
        if (nullIndex != -1 && !itemIsHave)
        {
            InventoryItem inventoryItem = new InventoryItem
            {
                itemId = itemId,
                itemAmount = number
            };
            inventoryItemsDict.Add(itemId, number);
            inventoryItemDataList.list[nullIndex] = inventoryItem;
        }
        else
        {
            InventoryItem inventoryItem = new InventoryItem
            {
                itemId = itemId,
                itemAmount = inventoryItemsDict[itemId] + number
            };
            inventoryItemsDict[itemId] += number;
            inventoryItemDataList.list[nullIndex] = inventoryItem;
        }
        EventManager.InvokeEventListener(ConstString.RefreshInventoryUIEvent, inventoryItemDataList.list);
        
    }
    public bool RemoveItemOutInventory(int itemId,int number)
    {
        if (!inventoryItemsDict.ContainsKey(itemId) && inventoryItemsDict[itemId] < number)
        {
            return false;
        }
        else
        {
            inventoryItemsDict[itemId]-= number;
            InventoryItem item = inventoryItemDataList.list.Find(i => i.itemId == itemId);
            item.itemAmount = inventoryItemsDict[itemId];
            EventManager.InvokeEventListener(ConstString.RefreshInventoryUIEvent, inventoryItemDataList.list);
            return true;
        }
    }
    public int GetIndexInInventory(int itemId,out bool isHave)
    {
        int nullIndex = -1;
        for (int i = 0; i < inventoryItemDataList.list.Count; i++)
        {
            if (inventoryItemDataList.list[i].itemId == itemId)
            {
                isHave = true;
                return i;
            }
        }
        for (int i = 0; i < inventoryItemDataList.list.Count; i++)
        {
            if (inventoryItemDataList.list[i].itemId == 0)
            {
                nullIndex = i;
                isHave = false;
                return nullIndex;

            }
        }
        isHave = false;
        return nullIndex;
    }
}
