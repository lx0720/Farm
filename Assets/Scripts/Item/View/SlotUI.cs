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
    private int itemIndex;
    private ItemDetails itemDetails;

    private void Awake()
    {
        slotButton = GetComponent<Button>();
        slotButton.onClick.AddListener(UpdateHighLightShow);
    }

    public void InitSlotUI(int index)
    {
        itemIndex = index;
        itemAmount = 0;
        itemImage.enabled = false;
        text.enabled = false;
    }

    public void RefreshSlotUI()
    {
        if (itemDetails == null)
        {
            itemImage.enabled = false;
            text.enabled = false;
            slotButton.interactable = false;
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

    public void SetItemDetails(ItemDetails itemDetails)
    {
        this.itemDetails = itemDetails;
    }
    public void SetItemAmount(int number)
    {
        itemAmount = number;
    }

    public int GetItemId() => itemDetails.itemId;
    public int GetItemIndex() => itemIndex;

    public void SetCloseStatus()
    {
        itemImage.enabled = false;
        text.enabled = false;
        slotButton.interactable = false;
    }

    private void UpdateHighLightShow()
    {
        EventManager.InvokeEventListener(ConstString.UpdateHightLightEvent, itemIndex);
    }

    public void SetHighLightShow(bool isActive)
    {
        highLight.SetActive(isActive);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetHighLightShow(false);
        EventManager.InvokeEventListener(ConstString.BeginDragItemEvent, itemIndex);
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
            EventManager.InvokeEventListener(ConstString.EndDragItemEvent, itemIndex, -1);
        }
        else
        {
            SlotUI slotUI = go.GetComponent<SlotUI>();
            if (slotUI != null)
            {
                EventManager.InvokeEventListener(ConstString.EndDragItemEvent, itemIndex, slotUI.itemIndex);
            }
        }
    }
}
