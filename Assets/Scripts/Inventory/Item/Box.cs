using Farm.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Inventory
{
    public class Box : MonoBehaviour
    {
        [SerializeField,Header("初始的箱子数据")] private InventoryBag_SO boxBagTemplate;
        [SerializeField,Header("实际的箱子数据")] private InventoryBag_SO boxBagData;

        [SerializeField,Header("交互提示显示")] private GameObject mouseIcon;
        [SerializeField,Header("箱子号数")] private int index;

        //箱子的状态
        private bool canOpen = false;
        private bool isOpen;

        private void OnEnable()
        {
            if (boxBagData == null)
            {
                boxBagData = Instantiate(boxBagTemplate);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = true;
                mouseIcon.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = false;
                mouseIcon.SetActive(false);
            }
        }

        #region SetAndGet
        public void SetBoxIndex(int boxIndex) { index = boxIndex; }
        public int GetBoxIndex() => index;
        public InventoryBag_SO GetInventoryBagSO() => boxBagData;
        #endregion


        private void Update()
        {
            if (!isOpen && canOpen && InputManager.Instance.GetMouseRight())
            {
                //打开箱子
                EventCenter.CallBaseBagOpenEvent(SlotType.Box, boxBagData);
                isOpen = true;
            }

            if (!canOpen && isOpen)
            {
                //关闭箱子
                EventCenter.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }

            if (isOpen && InputManager.Instance.GetEscBack())
            {
                //关闭箱子
                EventCenter.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
        }

        /// <summary>
        /// 初始化Box和数据
        /// </summary>
        /// <param name="boxIndex"></param>
        public void InitBox(int boxIndex)
        {
            index = boxIndex;
            var key = this.name + index;
            if (InventoryManager.Instance.GetBoxDataList(key) != null)  //刷新地图读取数据
            {
                boxBagData.itemList = InventoryManager.Instance.GetBoxDataList(key);
            }
            else     //新建箱子
            {
                InventoryManager.Instance.AddBoxDataDict(this);
            }
        }
    }
}
