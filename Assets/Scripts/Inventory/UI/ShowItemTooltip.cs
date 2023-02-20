using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Farm.Inventory
{
    /// <summary>
    /// 展示物品的Tooltip
    /// </summary>
    [RequireComponent(typeof(SlotUI))]
    public class ShowItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private SlotUI slotUI;
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails != null)
            {
                ItemTooltip itemTooltip = inventoryUI.GetItemTooltip();
                inventoryUI.SetItemTooltipStatus(true);
                itemTooltip.SetupTooltip(slotUI.itemDetails, slotUI.slotType);

                itemTooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                itemTooltip.transform.position = transform.position + Vector3.up * 60;

                if (slotUI.itemDetails.itemType == ItemType.Furniture)
                {
                    itemTooltip.resourcePanel.SetActive(true);
                    itemTooltip.SetupResourcePanel(slotUI.itemDetails.itemID);
                }
                else
                {
                    itemTooltip.resourcePanel.SetActive(false);
                }
            }
            else
            {
                inventoryUI.SetItemTooltipStatus(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.SetItemTooltipStatus(false);
        }

    }
}