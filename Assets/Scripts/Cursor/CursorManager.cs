using Farm.Input;
using Farm.Tool;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoSingleton<CursorManager>
{
    private CursorType currentCursorType = CursorType.Normal;
    private bool cursorUseable;
    private Vector3 mousePosition;
    private Camera mainCamera;
    private bool interactWithUI => (EventSystem.current !=null && EventSystem.current.IsPointerOverGameObject());
    private ItemDetails selectedItemDetails;
    private int selectedItemId;
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<Player>();
        mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        EventManager.AddEventListener(ConstString.BeforeSceneLoadEvent, OnBeforeSceneLoad);
        EventManager.AddEventListener<int,bool>(ConstString.SelectedItemEvent, OnSelectedItem);
    }

    private void OnDisable()
    {
        EventManager.RemoveEventListener<int,bool>(ConstString.SelectedItemEvent, OnSelectedItem);
        EventManager.RemoveEventListener(ConstString.BeforeSceneLoadEvent, OnBeforeSceneLoad);
    }

    private void Update()
    {
        if(InputManager.Instance.GetEscDown() && currentCursorType != CursorType.None)
        {
            currentCursorType = CursorType.None;
        }
        if((InputManager.Instance.GetMouseLeftDown()&& currentCursorType == CursorType.None)|| interactWithUI)
        {
            currentCursorType = CursorType.Normal;
        }
        UpdateCursorStatus();
        UpdateMousePosition();
        CheckCursorUseable();
    }

    #region Events
    private void OnSelectedItem(int itemId,bool isSelected)
    {
        selectedItemDetails = isSelected == true ? ItemManager.Instance.GetItemDetails(itemId) : null;
    }
    private void OnBeforeSceneLoad()
    {
        currentCursorType = CursorType.Normal;
    }

    #endregion
    private void UpdateCursorStatus()
    {
        currentCursorType = selectedItemDetails == null ? CursorType.Normal:
                selectedItemDetails.itemType == ItemType.Seed ? CursorType.Choose : CursorType.Build;
        if (interactWithUI)
            currentCursorType = CursorType.Normal;
    }

    private void CheckCursorUseable()
    {
        if(currentCursorType == CursorType.Choose || currentCursorType == CursorType.Build)
        {
            int radius = selectedItemDetails.itemUseRadius;
            Vector3Int position = new Vector3Int((int)mousePosition.x,(int) mousePosition.y, 0);
            Vector3Int playerPosition = new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, 0);
            cursorUseable = Mathf.Abs(position.x - playerPosition.x) <= radius &&
                Mathf.Abs(position.y - playerPosition.y) <= radius;
        }
        else
        {
            cursorUseable = true;
        }
        if (interactWithUI)
            cursorUseable = true;
    }
    public void UpdateMousePosition()
    {
        mousePosition = mainCamera.ScreenToWorldPoint(
            new Vector3((int)Input.mousePosition.x, (int)Input.mousePosition.y, -mainCamera.transform.position.z));
    }
    public CursorType GetCursorType() => currentCursorType;
    public Vector3 GetMousePosition() => mousePosition;
    public bool CursorUseable() => cursorUseable;
}
