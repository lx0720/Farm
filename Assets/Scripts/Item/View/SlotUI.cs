using Farm.Tool;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [SerializeField]private Image itemImage;
    [SerializeField]private TextMeshProUGUI text;
    [SerializeField]private GameObject highLight;
    private Button slotButton;

    private int itemAmount;
    private int slotIndex;
    private bool clickStatus;
    private ItemDetails itemDetails;

    private void Awake()
    {
        slotButton = GetComponent<Button>();
        slotButton.onClick.AddListener(SelectedItem);
    }

    public void InitSlotUI(int slotIndex)
    {
        this.slotIndex = slotIndex;
        SetSlotInitStatus();
    }
    public void RefreshSlotUI()
    {
        if (itemDetails == null)
        {
            SetSlotInitStatus();
        }
        else
        {
            itemImage.enabled = true;
            text.enabled = true;
            itemImage.sprite = itemDetails.itemIcon != null ? itemDetails.itemIcon : itemDetails.itemSprite;
            text.text = itemAmount.ToString();
            slotButton.interactable = true;
        }
    }

    private void SelectedItem()
    {
        clickStatus = !clickStatus;
        EventManager.InvokeEventListener(ConstString.SelectedItemEvent, itemDetails.itemId, clickStatus);
    }

    #region SetAndGet
    public void SetSlotInitStatus()
    {
        itemDetails = null;
        itemAmount = 0;
        clickStatus = false;
        itemImage.enabled = false;
        text.enabled = false;
        slotButton.interactable = false;
    }
    public void SetItemDetails(ItemDetails itemDetails)
    {
        this.itemDetails = itemDetails;
    }
    public void SetItemAmount(int number)
    {
        itemAmount = number;
    }
    public void SetHighLightStatus(bool isActive)
    {
        highLight.SetActive(isActive);
        clickStatus = isActive;
    }

    public int GetItemId() => itemDetails != null ? itemDetails.itemId : -1;
    public int GetSlotIndex() => slotIndex;

    #endregion

    #region Interface
    public void OnBeginDrag(PointerEventData eventData)
    {
        SetHighLightStatus(false);
        EventManager.InvokeEventListener(ConstString.BeginDragItemEvent, slotIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        EventManager.InvokeEventListener(ConstString.DragItemEvent);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject go = eventData.pointerCurrentRaycast.gameObject;
        if (go == null)
        {
            EventManager.InvokeEventListener(ConstString.EndDragItemEvent, slotIndex, -1);
        }
        else
        {
            SlotUI slotUI = go.GetComponent<SlotUI>();
            if (slotUI != null)
            {
                EventManager.InvokeEventListener(ConstString.EndDragItemEvent, slotIndex, slotUI.slotIndex);
            }
        }
    }
    #endregion 
}
