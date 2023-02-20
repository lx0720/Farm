using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField]private RectTransform dayNightImage;
    [SerializeField]private RectTransform clockParent;
    [SerializeField]private Image seasonImage;
    [SerializeField]private TextMeshProUGUI dateText;
    [SerializeField]private TextMeshProUGUI timeText;
    [SerializeField]private Button pauseGameButton;
    [SerializeField]private Sprite[] seasonSprites;

    private List<GameObject> clockBlocks = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }
        pauseGameButton.onClick.AddListener(OnPauseGame);
    }

    private void OnEnable()
    {
        EventCenter.GameMinuteEvent += OnGameMinuteEvent;
        EventCenter.GameDateEvent += OnGameDateEvent;
    }

    private void OnDisable()
    {
        EventCenter.GameMinuteEvent -= OnGameMinuteEvent;
        EventCenter.GameDateEvent -= OnGameDateEvent;
    }
    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        dateText.text = year + "年" + month.ToString("00") + "月" + day.ToString("00") + "日";
        seasonImage.sprite = seasonSprites[(int)season];

        SwitchHourImage(hour);
        DayNightImageRotate(hour);
    }

    private void OnPauseGame()
    {
        
    }

    /// <summary>
    /// 根据小时切换时间块显示
    /// </summary>
    /// <param name="hour"></param>
    private void SwitchHourImage(int hour)
    {
        int index = hour % 6;

        for (int i = 0; i < clockBlocks.Count; i++)
            {
                if (i < index + 1)
                    clockBlocks[i].SetActive(true);
                else
                    clockBlocks[i].SetActive(false);
            }
    }
    /// <summary>
    /// 日月轮替
    /// </summary>
    /// <param name="hour"></param>
    private void DayNightImageRotate(int hour)
    {
        var target = new Vector3(0, 0, hour * 15 - 90);
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);
    }
}
