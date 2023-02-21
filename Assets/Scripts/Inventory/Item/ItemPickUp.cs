using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                if (item.itemDetails.canPickedup)
                {
                    //拾取物品添加到背包
                    Debug.Log("拾取");
                    InventoryManager.Instance.AddItem(item, true);

                    //播放音效
                    EventCenter.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }
    }
}