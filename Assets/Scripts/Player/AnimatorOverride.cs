using Farm.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators; 

    [SerializeField]private SpriteRenderer holdItem;

    [SerializeField]private List<AnimatorType> animatorTypes;

    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (var anim in animators)
        {
            animatorNameDict.Add(anim.name, anim);
        }
    }

    private void OnEnable()
    {
        EventManager.AddEventListener(ConstString.BeforeSceneLoadEvent, OnBeforSceneLoad);
        EventManager.AddEventListener<int, bool>(ConstString.SelectedItemEvent, OnSelectedItem);
    }

    private void OnDisable()
    {
        EventManager.AddEventListener(ConstString.BeforeSceneLoadEvent, OnBeforSceneLoad);
        EventManager.RemoveEventListener<int, bool>(ConstString.SelectedItemEvent, OnSelectedItem);
    }

    #region Events

    private void OnBeforSceneLoad()
    {
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);
    }

    private void OnSelectedItem(int itemId,bool isSelected)
    {
        ItemDetails itemDetails = ItemManager.Instance.GetItemDetails(itemId);

        PartType currentType = itemDetails.itemType switch
        {
            ItemType.Seed => PartType.Carry,
            ItemType.Commodity => PartType.Carry,
            ItemType.HoeTool => PartType.Hoe,
            ItemType.WaterTool => PartType.Water,
            ItemType.CollectTool => PartType.Collect,
            ItemType.ChopTool => PartType.Chop,
            ItemType.BreakTool => PartType.Break,
            ItemType.ReapTool => PartType.Reap,
            ItemType.Furniture => PartType.Carry,
            _ => PartType.None
        };

        if (isSelected == false)
        {
            currentType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            if (currentType == PartType.Carry)
            {
                holdItem.sprite = itemDetails.itemSprite;
                holdItem.enabled = true;
            }
            else
            {
                holdItem.enabled = false;
            }
        }

        SwitchAnimator(currentType);
    }

    private void OnHarvestAtPlayerPosition(int ID)
    {
        //Sprite itemSprite = InventoryManager.Instance.GetItemDetails(ID).itemOnWorldSprite;
        /*if (!holdItem.gameObject.activeInHierarchy)
        {
            StartCoroutine(ShowItem(itemSprite));
        }*/

    }

    #endregion

    private IEnumerator ShowItem(Sprite itemSprite)
    {
        holdItem.sprite = itemSprite;
        holdItem.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        holdItem.gameObject.SetActive(false);
    }

    private void SwitchAnimator(PartType partType)
    {
        foreach (var item in animatorTypes)
        {
            if (item.partType == partType)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
            else if (item.partType == PartType.None)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}
