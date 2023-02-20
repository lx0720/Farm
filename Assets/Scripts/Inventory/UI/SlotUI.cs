using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Farm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Slot的组件")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        public Image slotHightlight;
        [SerializeField] private Button button;

        [Header("Slot的类型")]
        public SlotType slotType;
        public bool isSelected;
        public int slotIndex;

        //信息
        public ItemDetails itemDetails;
        public int itemAmount;

        public InventoryLocation Location
        {
            get
            {
                return slotType switch
                {
                    SlotType.Bag => InventoryLocation.Player,
                    SlotType.Box => InventoryLocation.Box,
                    _ => InventoryLocation.Player
                };
            }
        }

        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Start()
        {
            isSelected = false;
            if (itemDetails == null)
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// 更新Slot的信息
        /// </summary>
        /// <param name="item">ItemDetails</param>
        /// <param name="amount">物体的数量</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            amountText.text = amount.ToString();
            slotImage.enabled = true;
            button.interactable = true;
        }

        /// <summary>
        /// 更新空slot信息
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;

                inventoryUI.UpdateSlotHightlight(-1);
                EventCenter.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null) return;
            isSelected = !isSelected;

            inventoryUI.UpdateSlotHightlight(slotIndex);

            if (slotType == SlotType.Bag)
            {
                ///调用物体被选择的事件
                EventCenter.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0)
            {
                inventoryUI.GetDragImage().gameObject.SetActive(true);
                inventoryUI.GetDragImage().sprite = slotImage.sprite;
                inventoryUI.GetDragImage().SetNativeSize();

                isSelected = true;
                inventoryUI.UpdateSlotHightlight(slotIndex);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.GetDragImage().transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.GetDragImage().gameObject.SetActive(false);

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                    return;

                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;

                //如果拖拽的目标slottype是Bag则交换
                if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }
                else if (slotType == SlotType.Shop && targetSlot.slotType == SlotType.Bag)  //买
                {
                    EventCenter.CallShowTradeUI(itemDetails, false);
                }
                else if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Shop)  //卖
                {
                    EventCenter.CallShowTradeUI(itemDetails, true);
                }
                else if (slotType != SlotType.Shop && targetSlot.slotType != SlotType.Shop && slotType != targetSlot.slotType)
                {
                    //
                    InventoryManager.Instance.SwapItem(Location, slotIndex, targetSlot.Location, targetSlot.slotIndex);
                }
                //清除所有的高亮显示
                inventoryUI.UpdateSlotHightlight(-1);
            }
        }
    }
}