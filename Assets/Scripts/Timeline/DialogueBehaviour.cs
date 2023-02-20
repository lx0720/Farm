using System.Collections;
using System.Collections.Generic;
using Farm.Dialogue;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    private PlayableDirector director;
    public DialoguePiece dialoguePiece;

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //呼叫启动UI
        EventCenter.CallShowDialogueEvent(dialoguePiece);
        if (Application.isPlaying)
        {
            if (dialoguePiece.hasToPause)
            {
                //暂停timeline
                TimelineManager.Instance.PauseTimeline(director);
            }
            else
            {
                EventCenter.CallShowDialogueEvent(null);
            }
        }
    }

    //在Timeline播放期间每帧执行
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (Application.isPlaying)
        {
            TimelineManager.Instance.IsDone = dialoguePiece.isDone;
            EventCenter.CallUpdateGameStateEvent(GameState.Pause);
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        EventCenter.CallShowDialogueEvent(null);
    }

    public override void OnGraphStart(Playable playable)
    {
        EventCenter.CallUpdateGameStateEvent(GameState.Pause);
    }

    public override void OnGraphStop(Playable playable)
    {
        EventCenter.CallUpdateGameStateEvent(GameState.Gameplay);
    }
}
