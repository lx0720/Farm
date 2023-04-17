using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemId;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public Sprite itemSprite;
    public string itemDescription;
    public int itemUseRadius;
    public bool canPickedup;
    public bool canDropped;
    public bool canCarried;
    public int itemPrice;
    [Range(0, 1)]
    public float sellPercentage;
}