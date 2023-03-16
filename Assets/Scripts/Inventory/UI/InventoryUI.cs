using Farm.Input;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField]private ItemTooltip itemTooltip;
        [SerializeField,Header("拖拽图片")] private Image dragItem;
        [SerializeField,Header("玩家背包")] private GameObject bagUI;

        private bool bagOpened;

        [SerializeField,Header("通用背包")] private GameObject baseBag;
        [SerializeField,Header("商店slotPrefab")] private GameObject shopSlotPrefab;
        [SerializeField,Header("箱子slotPrefab")] private GameObject boxSlotPrefab;

        [SerializeField,Header("交易UI")] TradeUI tradeUI;
        [SerializeField]private TextMeshProUGUI playerMoneyText;

        [SerializeField] private SlotUI[] playerSlots;
        [SerializeField] private List<SlotUI> baseBagSlots;

        private void OnEnable()
        {
            EventCenter.UpdateInventoryUI += OnUpdateInventoryUI;
            EventCenter.BeforeSceneUnloadEvent += OnBeforeSceneUnloadedEvent;
            EventCenter.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventCenter.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventCenter.ShowTradeUI += OnShowTradeUI;
        }

        private void OnDisable()
        {
            EventCenter.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventCenter.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadedEvent;
            EventCenter.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventCenter.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventCenter.ShowTradeUI -= OnShowTradeUI;
        }


        private void Start()
        {
            //初始化背包
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            bagOpened = bagUI.activeInHierarchy;
            playerMoneyText.text = InventoryManager.Instance.PlayerMoney.ToString();
        }

        private void Update()
        {
            if (InputManager.Instance.GetbPack())
            {
                OpenBagUI();
            }
        }

        #region SetAndGet

        public ItemTooltip GetItemTooltip() => itemTooltip;
        public Image GetDragImage() => dragItem;

        /// <summary>
        /// 设置ItemTooltip的状态
        /// </summary>
        /// <param name="isOpen">是否激活</param>
        public void SetItemTooltipStatus(bool isOpen)
        {
            itemTooltip.gameObject.SetActive(isOpen);
        }

        #endregion


        #region Events

        private void OnShowTradeUI(ItemDetails item, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetupTradeUI(item, isSell);
        }

        /// <summary>
        /// 打开背包事件
        /// </summary>
        /// <param name="slotType">slot的类型</param>
        /// <param name="bagData">bag数据库</param>
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,
                SlotType.Box => boxSlotPrefab,
                _ => null,
            };

            //激活背包UI
            baseBag.SetActive(true);

            baseBagSlots = new List<SlotUI>();

            for (int i = 0; i < bagData.itemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());

            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);
                bagUI.SetActive(true);
                bagOpened = true;
            }
            //刷新背包UI显示
            OnUpdateInventoryUI(InventoryLocation.Box, bagData.itemList);
        }

        /// <summary>
        /// 关闭BaseBag
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            baseBag.SetActive(false);
            itemTooltip.gameObject.SetActive(false);
            UpdateSlotHightlight(-1);

            foreach (var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                bagUI.SetActive(false);
                bagOpened = false;
            }
        }


        private void OnBeforeSceneUnloadedEvent()
        {
            UpdateSlotHightlight(-1);
        }


        /// <summary>
        /// 更新InventoryUI
        /// </summary>
        /// <param name="location">搴撳瓨浣嶇疆</param>
        /// <param name="list">鏁版嵁鍒楄〃</param>
        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }

            playerMoneyText.text = InventoryManager.Instance.PlayerMoney.ToString();
        }

        /// <summary>
        /// 打开背包UI
        /// </summary>
        public void OpenBagUI()
        {
            bagOpened = !bagOpened;

            bagUI.SetActive(bagOpened);
        }


        #endregion

        /// <summary>
        /// 更新slot的高亮
        /// </summary>
        /// <param name="index">slot的编号</param>
        public void UpdateSlotHightlight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if (slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHightlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHightlight.gameObject.SetActive(false);
                }
            }
        }
    }
}