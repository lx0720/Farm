using System;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoSingleton<TimelineManager>
{
    public PlayableDirector startDirector;
    private PlayableDirector currentDirector;

    private bool isDone;
    public bool IsDone { set => isDone = value; }
    private bool isPause;
    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }

    private void OnEnable()
    {
        EventCenter.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventCenter.StartNewGameEvent += OnStartNewGameEvent;
        startDirector.played += OnPlayed;
        startDirector.stopped += OnStopped;
    }


    private void OnDisable()
    {
        EventCenter.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventCenter.StartNewGameEvent -= OnStartNewGameEvent;
    }


    private void Update()
    {
        if (isPause && Input.GetKeyDown(KeyCode.Space) && isDone)
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }

    private void OnStartNewGameEvent(int obj)
    {
        if (startDirector != null)
            startDirector.Play();
    }

    private void OnAfterSceneLoadedEvent()
    {
        // currentDirector = FindObjectOfType<PlayableDirector>();
        // if (currentDirector != null)
        //     currentDirector.Play();
        if (!startDirector.isActiveAndEnabled)
            EventCenter.CallUpdateGameStateEvent(GameState.Gameplay);
    }

    private void OnStopped(PlayableDirector obj)
    {
        EventCenter.CallUpdateGameStateEvent(GameState.Gameplay);
    }

    private void OnPlayed(PlayableDirector obj)
    {
        Debug.Log("111");
        EventCenter.CallUpdateGameStateEvent(GameState.Pause);
    }
    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;

        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause = true;
    }
}
