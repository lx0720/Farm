using Farm.Save;
using Farm.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private RectTransform scrollViewContent;
    private RectTransform scrollRect;
    private int scrollViewHight;
    private int minSlotIndex;
    private int maxSlotIndex;
    private int maxSlotCount = 16;
    private int oldMinSlotIndex = 0;
    private int oldMaxSlotIndex = 0;
    [SerializeField]private Transform[] menuTransforms;
    [SerializeField]private Transform[] settingTransform;
    [SerializeField]private Transform pauseTransform;
    [SerializeField]private Transform mainMenu;
    [SerializeField]private Button exitGame;
    [SerializeField]private Button exitMenu;


    private Dictionary<int, GameObject> saveSlotDict = new Dictionary<int, GameObject>();

    private void Awake()
    {
        Init();
    }
    private void OnEnable()
    {
        EventManager.AddEventListener<bool>(ConstString.PauseGameEvent, OnPasueGame);
        EventManager.AddEventListener(ConstString.EndGameEvent, OnEndGame);
        EventManager.AddEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
        
    }
    private void OnDisable()
    {
        EventManager.RemoveEventListener<bool>(ConstString.PauseGameEvent, OnPasueGame);
        EventManager.AddEventListener(ConstString.EndGameEvent, OnEndGame);
        EventManager.RemoveEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
    }


    private void Update()
    {
        CheckShowOrHide();
    }
    private void OnPasueGame(bool isActive)
    {
        pauseTransform.gameObject.SetActive(isActive);
    }

    private void OnEndGame()
    {
        pauseTransform.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    private void OnAfterSceneLoad(GameScene targetScene)
    {
        if(targetScene != GameScene.UIScene)
            mainMenu.gameObject.SetActive(false);
    }
    private void Init()
    {
        scrollViewContent = transform.GetComponentInChildren<ScrollRect>().content;
        scrollRect = transform.GetComponentInChildren<ScrollRect>().gameObject.GetComponent<RectTransform>();
        scrollViewHight = (int)scrollRect.rect.height;
        exitGame.onClick.AddListener(ExitGame);
        exitMenu.onClick.AddListener(ExitToMenu);
    }
    //´æ´¢½ø¶È
    private void CheckShowOrHide()
    {
        minSlotIndex = (int)(scrollViewContent.anchoredPosition.y - 36) / 18;
        maxSlotIndex = ((int)(scrollViewContent.anchoredPosition.y - 36) + scrollViewHight) / 18;
        if (oldMinSlotIndex != minSlotIndex)
        {
            for (int i = oldMinSlotIndex; i < minSlotIndex; i++)
            {
                if (saveSlotDict.ContainsKey(i))
                {
                    GameObjectPool.Instance.ReturnObject("GameProgress", saveSlotDict[i]);
                    saveSlotDict[i].SetActive(false);
                    saveSlotDict.Remove(i);
                }
            }
        }
        if (oldMaxSlotIndex != maxSlotIndex)
        {
            for (int i = maxSlotIndex + 1; i <= oldMaxSlotIndex; i++)
            {
                if (saveSlotDict.ContainsKey(i))
                {
                    GameObjectPool.Instance.ReturnObject("GameProgress", saveSlotDict[i]);
                    saveSlotDict[i].SetActive(false);
                    saveSlotDict.Remove(i);
                }
            }
        }
        oldMinSlotIndex = minSlotIndex;
        oldMaxSlotIndex = maxSlotIndex;
        for (int i = minSlotIndex; i <= maxSlotIndex; i++)
        {
            int index = i;
            if (saveSlotDict.ContainsKey(index))
                continue;
            else
            {
                if (index < maxSlotCount)
                {
                    GameObject go = GameObjectPool.Instance.GetObject("GameProgress");
                    saveSlotDict.Add(i, go);
                    go.transform.SetParent(scrollViewContent);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = new Vector3(0,-9-18 * index, 0);
                    go.GetComponent<GameProgressUI>().SetGameProgress(index);
                    saveSlotDict[index].SetActive(true);
                }
            }
        }


    }
    //ÇÐ»»Panel
    public void SwitchMenuPanel(int slotIndex)
    {
        for (int i = 0; i < menuTransforms.Length; i++)
        {
            if (i == slotIndex)
            {
                menuTransforms[i].SetAsLastSibling();
            }
        }
    }
    public void SwitchSettingPanel(int slotIndex)
    {
        for (int i = 0; i < settingTransform.Length; i++)
        {
            if (i == slotIndex)
            {
                settingTransform[i].SetAsLastSibling();
            }
        }
    }
    private void ExitToMenu()
    {
        EventManager.InvokeEventListener(ConstString.BackToMenuEvent);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
