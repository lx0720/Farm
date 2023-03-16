using System.Collections;
using System.Collections.Generic;
using Farm.CropPlant;
using UnityEngine;
namespace Farm.Inventory
{
    public class Item : MonoBehaviour
    {
        private int itemId;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider2D;
        private ItemDetails itemDetails;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (itemId != 0)
            {
                ItemInit(itemId);
            }
        }

        #region SetsAndGets

        public void SetItemId(int id) { itemId = id; }
        public int GetItemId() => itemId;
        public ItemDetails GetItemDetails() => itemDetails;

        #endregion

        public void ItemInit(int id)
        {
            itemId = id;

            itemDetails = InventoryManager.Instance.GetItemDetails(itemId);

            if (itemDetails != null)
            {
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite != null ? itemDetails.itemOnWorldSprite : itemDetails.itemIcon;

                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                boxCollider2D.size = newSize;
                boxCollider2D.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }

            if (itemDetails.itemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ReapItem>();
                gameObject.GetComponent<ReapItem>().InitCropData(itemDetails.itemID);
                gameObject.AddComponent<ItemInteractive>();
            }
        }
    }
}