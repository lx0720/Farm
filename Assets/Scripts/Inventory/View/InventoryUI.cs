using Farm.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]private List<SlotUI> slots;
    [SerializeField]private RectTransform inventory;
    [SerializeField]private Button exitBag;
    [SerializeField]private Button openBag;
    [SerializeField]private Image dragImage;
    [SerializeField]private TextMeshProUGUI text;
    private int currentSelectedItem;

    private bool bagOpenStatus;
    private List<InventoryItem> inventoryslots;

    private int currentHighLightIndex =-1;

    private void Start()
    {
        InitItemUI();
        openBag.onClick.AddListener(OpenBagUI);
        exitBag.onClick.AddListener(CloseBagUI);
    }

    private void OnEnable()
    {
        EventManager.AddEventListener<List<InventoryItem>>(ConstString.RefreshInventoryUIEvent, OnRefreshInventoryUI);
        EventManager.AddEventListener(ConstString.BeforeSceneLoadEvent, OnBeforeSceneLoad);
        EventManager.AddEventListener<int>(ConstString.BeginDragItemEvent, OnBeginDragItem);
        EventManager.AddEventListener(ConstString.DragItemEvent, OnDragItem);
        EventManager.AddEventListener<int,int>(ConstString.EndDragItemEvent, OnEndDragItem);
        EventManager.AddEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
        EventManager.AddEventListener<int>(ConstString.UpdateHightLightEvent, OnShowHightLight);
    }

    private void OnDisable()
    {
        EventManager.RemoveEventListener<List<InventoryItem>>(ConstString.RefreshInventoryUIEvent, OnRefreshInventoryUI);
        EventManager.RemoveEventListener(ConstString.BeforeSceneLoadEvent, OnBeforeSceneLoad);
        EventManager.RemoveEventListener<int>(ConstString.BeginDragItemEvent, OnBeginDragItem);
        EventManager.RemoveEventListener(ConstString.DragItemEvent, OnDragItem);
        EventManager.RemoveEventListener<int,int>(ConstString.EndDragItemEvent, OnEndDragItem);
        EventManager.RemoveEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
        EventManager.RemoveEventListener<int>(ConstString.UpdateHightLightEvent, OnShowHightLight);
    }



    private void OnRefreshInventoryUI(List<InventoryItem> inventoryslots)
    {
        this.inventoryslots = inventoryslots;
        RefreshInventoryUI(inventoryslots);
    }
    private void OnBeforeSceneLoad()
    {
        //OpenBagUI();
    }
    private void OnAfterSceneLoad(GameScene obj)
    {
        RefreshInventoryUI(inventoryslots);
        ClearAllHighLight();
        CloseBagUI();
    }
    private void OnBeginDragItem(int index)
    {
        slots[index].SetCloseStatus();
        dragImage.enabled = true;
        dragImage.sprite = ItemManager.Instance.GetItemDetails(slots[index].GetItemId()).itemIcon;
    }

    private void OnDragItem()
    {
        dragImage.transform.position = Input.mousePosition;
    }

    private void OnEndDragItem(int currentIndex,int targetIndex)
    {
        dragImage.sprite = null;
        dragImage.enabled = false;
    }
    private void InitItemUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].InitSlotUI(i);
        }
    }
    private void OnShowHightLight(int itemIndex)
    {
        currentSelectedItem = slots[itemIndex].GetItemId();
        ClearAllHighLight();
        if (itemIndex != currentHighLightIndex)
        {
            currentHighLightIndex = itemIndex;
            slots[currentHighLightIndex].SetHighLightShow(true);
        }
        else
        {
            currentHighLightIndex = -1;
            currentSelectedItem = -1;
        }

    }
    private void CloseBagUI()
    {
        bagOpenStatus = false;
        inventory.gameObject.SetActive(false);
    }
    private void OpenBagUI()
    {
        if(!bagOpenStatus)
        {
            bagOpenStatus = true;
            inventory.gameObject.SetActive(true);
        }
        else
        {
            bagOpenStatus = false;
            inventory.gameObject.SetActive(false);
        }
    }

    public void RefreshInventoryUI(List<InventoryItem> inventoryslots)
    {
        for(int i=0;i<slots.Count;i++)
        {
            if(inventoryslots[i].itemAmount > 0)
            {
                slots[i].SetItemDetails(ItemManager.Instance.GetItemDetails(inventoryslots[i].itemId));
                slots[i].SetItemAmount(inventoryslots[i].itemAmount);
            }
            else
            {
                slots[i].SetItemDetails(null);
                slots[i].SetItemAmount(0);
            }
            slots[i].RefreshSlotUI();
        }
        text.text = InventoryManager.Instance.GetPlayerMoney().ToString();
    }

    public int GetChooseItemId() => currentSelectedItem;

    private void ClearAllHighLight()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetHighLightShow(false);
        }
    }

}