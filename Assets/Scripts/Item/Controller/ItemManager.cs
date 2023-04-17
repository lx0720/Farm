using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoSingleton<ItemManager>
{
    [SerializeField]private ItemDetailsListSO itemDetailsListSO;

    private Dictionary<int, ItemDetails> itemDetailsListDict;

    protected override void Awake()
    {
        base.Awake();
        itemDetailsListDict = new Dictionary<int, ItemDetails>();
        InitDict();
    }

    private void InitDict()
    {
        foreach(ItemDetails itemDetails in itemDetailsListSO.itemDetailsList)
        {
            if(!itemDetailsListDict.ContainsKey(itemDetails.itemId))
            {
                itemDetailsListDict.Add(itemDetails.itemId, itemDetails);
            }
            else
            {
                itemDetailsListDict[itemDetails.itemId] = itemDetails;
            }
        }
    }

    public ItemDetails GetItemDetails(int itemId)
    {
        return itemDetailsListDict[itemId] != null ? itemDetailsListDict[itemId] : null;
    }
}
