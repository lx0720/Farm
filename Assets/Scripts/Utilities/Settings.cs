using System;
using UnityEngine;

public class Settings
{
    public const float itemFadeDuration = 0.35f;
    public const float targetAlpha = 0.45f;

    //时间相关
    public const float secondThreshold = 0.01f;    //数值越小时间越快
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 30;
    public const int seasonHold = 3;

    //Transition
    public const float fadeDuration = 0.8f;

    //割草数量限制
    public const int reapAmount = 2;

    //NPC网格移动
    public const float gridCellSize = 1;
    public const float gridCellDiagonalSize = 1.41f;
    public const float pixelSize = 0.05f;   
    public const float animationBreakTime = 5f; 
    public const int maxGridSize = 9999;

    //灯光
    public const float lightChangeDuration = 25f;
    public static TimeSpan morningTime = new TimeSpan(5, 0, 0);
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);

    public static Vector3 GameStartPlayerPos = new Vector3(1f, 10f, 0);
    public const int playerStartMoney = 100;
    /// <summary>
    /// ----------------------------------------------
    /// </summary>
    public const float timeInterval = 0.01f;
    public const float minuteInterval = 60;
    public const float hourInterval = 24;
    public const float dayInterval = 30;
    public const float monthInterval = 12;
    public const float hourToAngle = 15;
}
