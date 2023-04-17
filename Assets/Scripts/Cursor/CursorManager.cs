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
    private bool interactWithUI = (EventSystem.current !=null && EventSystem.current.IsPointerOverGameObject());
    private ItemDetails chooseItemDetails;


    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        EventManager.AddEventListener(ConstString.BeforeSceneLoadEvent, OnBeforeSceneLoad);
        EventManager.AddEventListener<ItemDetails,bool>(ConstString.ChoseItemEvent, OnChooseItem);
    }

    private void OnDisable()
    {
        EventManager.RemoveEventListener<ItemDetails,bool>(ConstString.ChoseItemEvent, OnChooseItem);
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
        UpdateMousePosition();
        CheckCursorUseable();
    }

    private void OnChooseItem(ItemDetails itemDetails,bool isSelected)
    {
        chooseItemDetails = isSelected == true ?  itemDetails : null;
    }
    private void OnBeforeSceneLoad()
    {
        currentCursorType = CursorType.Normal;
    }


    private void CheckCursorUseable()
    {
        if(chooseItemDetails != null)
        {

        }
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
