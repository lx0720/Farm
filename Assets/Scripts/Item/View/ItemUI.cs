using Farm.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUI : MonoBehaviour
{
    private string itemName;
    private int itemAmount = 1;
    [SerializeField] private int itemId;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventManager.AddEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
    }
    private void OnDisable()
    {
        EventManager.RemoveEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
    }

    private void OnAfterSceneLoad(GameScene gameScene)
    {
        RefreshItemUI();
    }

    public void SetItemInfo(int itemId,int number)
    {
        this.itemId = itemId;
        itemAmount = number;
        RefreshItemUI();
    }

    private void RefreshItemUI()
    {
        ItemDetails itemDetails = ItemManager.Instance.GetItemDetails(itemId);
        itemName = itemDetails.itemName;
        if (itemId != 0)
        {
            spriteRenderer.sprite = itemDetails.itemSprite == null ? itemDetails.itemIcon : itemDetails.itemSprite;
        }
    }

    public int GetItemId() => itemId;
    public int GetItemAmount() => itemAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObjectPool.Instance.ReturnObject(itemName, transform.gameObject);
            InventoryManager.Instance.AddItemIntoInventory(itemId, itemAmount);
        }
    }
}
