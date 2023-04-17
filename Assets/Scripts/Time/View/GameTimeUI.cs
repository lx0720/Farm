using DG.Tweening;
using Farm.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeUI : MonoBehaviour
{
    [SerializeField] private RectTransform sunRiseAndSetImage;
    [SerializeField] private GameObject[] timeBlock;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image seasonImage;
    [SerializeField] private Button pauseGame;
    [SerializeField] private Sprite[] images;

    private int showHourCount;
    private bool isPauseGame;


    private void Awake()
    {
        for(int i = 0;i<timeBlock.Length;i++)
        {
            timeBlock[i].SetActive(false);
        }
        pauseGame.onClick.AddListener(PauseGame);
    }
    private void OnEnable()
    {

        EventManager.AddEventListener(ConstString.InvokeEveryMinuteEvent, OnInvokeEveryMinute);
        EventManager.AddEventListener(ConstString.InvokeEveryDayEvent, OnInvokeEveryDay);
        EventManager.AddEventListener(ConstString.InvokeEverySeasonEvent, OnInvokeSeason);
        EventManager.AddEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGame);
        EventManager.AddEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
    }
    private void OnDisable()
    {

        EventManager.RemoveEventListener(ConstString.InvokeEveryMinuteEvent, OnInvokeEveryMinute);
        EventManager.RemoveEventListener(ConstString.InvokeEveryDayEvent, OnInvokeEveryDay);
        EventManager.RemoveEventListener(ConstString.InvokeEverySeasonEvent, OnInvokeSeason);
        EventManager.RemoveEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGame);
        EventManager.RemoveEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
    }

    private void PauseGame()
    {
        isPauseGame = !isPauseGame;
        EventManager.InvokeEventListener(ConstString.PauseGameEvent,isPauseGame);
    }
    private void OnStartNewGame(int obj)
    {
        OnInvokeEveryMinute();
        OnInvokeEveryDay();
    }
    private void OnInvokeSeason()
    {
        if (GameTimeManager.Instance.GameTime().gameSeason == GameSeason.Spring)
            seasonImage.sprite = images[0];
        if (GameTimeManager.Instance.GameTime().gameSeason == GameSeason.Summer)
            seasonImage.sprite = images[1];
        if (GameTimeManager.Instance.GameTime().gameSeason == GameSeason.Autumn)
            seasonImage.sprite = images[2];
        if (GameTimeManager.Instance.GameTime().gameSeason == GameSeason.Winter)
            seasonImage.sprite = images[3];
    }
    private void OnInvokeEveryMinute()
    {
        timeText.text = IntToString(GameTimeManager.Instance.GameTime().gameHour, false) + "时"+
            IntToString(GameTimeManager.Instance.GameTime().gameMinute, false)+"分";
    }
    private void OnInvokeEveryDay()
    {
        Vector3 rotateTarget = new Vector3(0, 0, GameTimeManager.Instance.GameTime().gameHour * Settings.hourToAngle - 90);
        sunRiseAndSetImage.DORotate(rotateTarget, 1f, RotateMode.Fast);
        showHourCount = GameTimeManager.Instance.GameTime().gameHour % 6;
        for (int i = 0; i < 6; i++)
        {
            if (i < showHourCount)
            {
                timeBlock[i].SetActive(true);
            }
            else
            {
                timeBlock[i].SetActive(false);
            }
        }
        dateText.text = IntToString(GameTimeManager.Instance.GameTime().gameYear, true) + "年" +
            IntToString(GameTimeManager.Instance.GameTime().gameMonth, false) + "月" +
            IntToString(GameTimeManager.Instance.GameTime().gameDay, false) + "日";
    }
    private void OnAfterSceneLoad(GameScene gameScene)
    {
        OnInvokeEveryDay();
    }
    private string IntToString(int number,bool isFour)
    {
        if (!isFour) 
        {
            if (number < 10)
                return "0" + number.ToString();
            return number.ToString();
        }
        else
        {
            if (number < 10)
                return "000" + number.ToString();
            if (number < 100)
                return "00" + number.ToString();
            if (number < 1000)
                return "0" + number.ToString();
            return number.ToString();
        }
    }
}
