using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Inventory/InventoryItemDataListSO",fileName ="InventoryItemDataListSO")]
public class InventoryItemDataListSO :ScriptableObject
{
    public List<InventoryItem> list;
}


[System.Serializable]
public struct InventoryItem 
{
    public int itemId;
    public int itemAmount;
}
