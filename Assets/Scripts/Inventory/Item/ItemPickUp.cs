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
                if (item.GetItemDetails().canPickedup)
                {
                    InventoryManager.Instance.AddItemInInventory(item, true);
                    EventCenter.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }
    }
}