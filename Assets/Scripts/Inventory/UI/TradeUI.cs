using System;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Inventory
{
    /// <summary>
    /// 交易UI
    /// </summary>
    public class TradeUI : MonoBehaviour
    {
        [SerializeField]private Image itemIcon;
        [SerializeField]private Text itemName;
        [SerializeField]private InputField tradeAmount;
        [SerializeField]private Button submitButton;
        [SerializeField]private Button cancelButton;

        private ItemDetails itemDetails;
        private bool isSellTrade;

        private void Awake()
        {
            AddListener();
        }

        /// <summary>
        /// 添加监听器
        /// </summary>
        private void AddListener()
        {
            cancelButton.onClick.AddListener(CancelTrade);
            submitButton.onClick.AddListener(ConfirmTrade);
        }
        /// <summary>
        /// 设置TradeUI显示详情
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isSell"></param>
        public void SetupTradeUI(ItemDetails itemDetails, bool isSell)
        {
            this.itemDetails = itemDetails;
            itemIcon.sprite = itemDetails.itemIcon;
            itemName.text = itemDetails.itemName;
            isSellTrade = isSell;
            tradeAmount.text = string.Empty;
        }
        /// <summary>
        /// 确认交易
        /// </summary>
        private void ConfirmTrade()
        {
            var amount = Convert.ToInt32(tradeAmount.text);

            InventoryManager.Instance.TradeItem(itemDetails, amount, isSellTrade);

            CancelTrade();
        }
        /// <summary>
        /// 取消交易
        /// </summary>
        private void CancelTrade()
        {
            this.gameObject.SetActive(false);
        }
    }
}
