using Farm.CropPlant;
using Farm.Inventory;
using Farm.Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed;
    private Sprite item;

    private Sprite currentSprite;   //获取当前的精灵
    private Image cursorImage;
    private RectTransform cursorCanvas;

    //建造物体的图片
    private Image buildImage;

    //相机
    private Camera mainCamera;
    private Grid currentGrid;

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;
    private bool cursorPositionValid;

    private ItemDetails currentItem;

    private Transform PlayerTransform => FindObjectOfType<Player>().transform;

    private void OnEnable()
    {
        EventCenter.ItemSelectedEvent += OnItemSelectedEvent;
        EventCenter.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventCenter.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventCenter.ItemSelectedEvent -= OnItemSelectedEvent;
        EventCenter.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventCenter.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }



    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        //获取build的图片
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildImage.gameObject.SetActive(false);

        currentSprite = normal;
        SetCursorImage(normal);

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (cursorCanvas == null) return;

        cursorImage.transform.position = Input.mousePosition;
        Cursor.visible = false;

        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
            if(currentItem!=null&& currentItem.itemType == ItemType.Furniture)
                buildImage.gameObject.SetActive(true);
            else
                buildImage.gameObject.SetActive(false);
        }
    }

    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            //鼠标点击
            EventCenter.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }
    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }

    #region 显示物体可用状态
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
        cursorImage.SetNativeSize();
    }

    /// <summary>
    /// 设置鼠标图片可用
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, 0.5f);
        cursorImage.SetNativeSize();
    }
    /// <summary>
    /// 设置鼠标图片不可用
    /// </summary>
    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
        buildImage.color = new Color(1, 0, 0, 0.5f);
        cursorImage.SetNativeSize();
    }
    #endregion

    /// <summary>
    /// 物体选择事件
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {

        if (!isSelected)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
            buildImage.gameObject.SetActive(false);
        }
        else    //选择物体之后鼠标显示对应的图标
        {
            currentItem = itemDetails;
            item = itemDetails.itemOnWorldSprite == null ? itemDetails.itemIcon : itemDetails.itemOnWorldSprite;
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool => tool,
                _ => normal,
            };
            cursorEnable = true;

            //如果物体时蓝图，那么显示家具样子
            if (itemDetails.itemType == ItemType.Furniture)
            {
                buildImage.gameObject.SetActive(true);
                buildImage.sprite = itemDetails.itemOnWorldSprite;
                buildImage.SetNativeSize();
            }
        }
    }

    //检查当前的鼠标是否可用
    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);

        buildImage.rectTransform.position = Input.mousePosition;

        ///检查时否在使用范围内
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }
        ///获取Tile信息
        //Debug.Log("鼠标的CellPos"+ mouseGridPos);
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        Debug.Log(currentTile);
        if (currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);

            //根据选择的物体来判断对应的鼠标显示状态
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currentTile.daysSinceDug > -1 && currentTile.seedItemID == -1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daysSinceDug > -1 && currentTile.daysSinceWatered == -1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.BreakTool:
                case ItemType.ChopTool:
                    if (crop != null)
                    {
                        if (crop.CanHarvest && crop.cropDetails.CheckToolAvailable(currentItem.itemID)) SetCursorValid(); else SetCursorInValid();
                    }
                    else SetCursorInValid();
                    break;
                case ItemType.CollectTool:
                    if (currentCrop != null)
                    {
                        if (currentCrop.CheckToolAvailable(currentItem.itemID))
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays) SetCursorValid(); else SetCursorInValid();
                    }
                    else
                        SetCursorInValid();
                    break;
                case ItemType.ReapTool:
                    if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos, currentItem)) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Furniture:
                    buildImage.gameObject.SetActive(true);
                    buildImage.SetNativeSize();
                    var bluePrintDetails = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(currentItem.itemID);

                    if (currentTile.canPlaceFurniture && InventoryManager.Instance.CheckStock(currentItem.itemID) && !HaveFurnitureInRadius(bluePrintDetails))
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;
            }
        }
        else
        {
            SetCursorInValid();
        }
    }

    private bool HaveFurnitureInRadius(BluePrintDetails bluePrintDetails)
    {
        var buildItem = bluePrintDetails.buildPrefab;
        Vector2 point = mouseWorldPos;
        var size = buildItem.GetComponent<BoxCollider2D>().size;

        var otherColl = Physics2D.OverlapBox(point, size, 0);
        if (otherColl != null)
            return otherColl.GetComponent<Furniture>();
        return false;
    }
    /// <summary>
    /// 是否可以与UI交互
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
