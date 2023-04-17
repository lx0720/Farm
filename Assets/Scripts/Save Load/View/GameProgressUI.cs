using Farm.Save;
using Farm.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameProgressUI : MonoBehaviour
{
    [SerializeField]private Image gameImage;
    [SerializeField]private Text gameTime;
    [SerializeField]private Text gameScene;
    private Button gameLoadButton;
    private DataSlot dataSlot;
    private int slotIndex;
    private bool isSaved;

    private void Awake()
    {
        slotIndex = -1;
        gameLoadButton = GetComponent<Button>();
        gameLoadButton.onClick.AddListener(OnLoadGame);
    }

    public void SetGameProgress(int index)
    {
        slotIndex = index;
        dataSlot = SaveLoadManager.Instance.GetDataSlot(index);
        gameImage.sprite = null;
        gameTime.text = "游戏时间:" + dataSlot.DataTime;
        gameScene.text = "游戏场景:" + dataSlot.DataScene;

    }

    private void OnLoadGame()
    {
        isSaved = dataSlot.DataScene != null && dataSlot.DataTime != null;
        if (!isSaved)
        {
            EventManager.InvokeEventListener(ConstString.StartNewGameEvent,slotIndex);
        }
        else
        {
            SaveLoadManager.Instance.LoadGame(slotIndex);
        }
    }
}
