using System;
using UnityEngine;

[Serializable]
public class ScheduleDetails : IComparable<ScheduleDetails>
{
    private GameScene targetScene;
    private int hour, minute,day;
    private int priority;    //优先级越小优先执行
    private Season season;
    private Vector2Int targetGridPosition;
    private AnimationClip clipAtStop;
    private bool npcCanInteractable;

    public ScheduleDetails(int hour, int minute, int day, int priority, Season season, GameScene targetScene, Vector2Int targetGridPosition, AnimationClip clipAtStop, bool interactable)
    {
        this.hour = hour;
        this.minute = minute;
        this.day = day;
        this.priority = priority;
        this.season = season;
        this.targetScene = targetScene;
        this.targetGridPosition = targetGridPosition;
        this.clipAtStop = clipAtStop;
        this.npcCanInteractable = interactable;
    }

    #region Gets
    public GameScene GetTargetScene() => targetScene;
    public Vector2Int GetTargetPosition() => targetGridPosition;
    public AnimationClip GetAnimationClip() => clipAtStop;
    public Season GetSeason() => season;
    public bool GetNPCInteractableState() => npcCanInteractable;
    public int GetScheduleDay() => day;
    public int Time => (hour * 100) + minute;
    #endregion
    public int CompareTo(ScheduleDetails other)
    {
        if (Time == other.Time)
        {
            if (priority > other.priority)
                return 1;
            else
                return -1;
        }
        else if (Time > other.Time)
        {
            return 1;
        }
        else if (Time < other.Time)
        {
            return -1;
        }

        return 0;
    }
}
