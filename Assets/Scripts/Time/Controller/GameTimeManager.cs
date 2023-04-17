using Farm.Save;
using Farm.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : MonoSingleton<GameTimeManager>,ISaveLoad 
{
    private GameTime gameTime;
    private bool pauseGame;
    private bool startGame;
    private float timer;


    public TimeSpan GameSpanTime() => new TimeSpan(gameTime.gameHour, gameTime.gameMinute, gameTime.gameSecond);
    public GameTime GameTime() => gameTime;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        EventManager.AddEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGame);
        EventManager.AddEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
        EventManager.AddEventListener(ConstString.BeforeSceneLoadEvent, OnBeforeSceneLoad);
        EventManager.AddEventListener<bool>(ConstString.PauseGameEvent, OnPauseGame);
    }

    private void OnDisable()
    {
        EventManager.RemoveEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGame);
        EventManager.RemoveEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
        EventManager.RemoveEventListener(ConstString.BeforeSceneLoadEvent, OnBeforeSceneLoad);
        EventManager.RemoveEventListener<bool>(ConstString.PauseGameEvent, OnPauseGame);
    }

    private void Start()
    {
        RegisterInterface();
    }

    private void FixedUpdate()
    {
        if(startGame)
            UpdateGameTime();
    }


    private void OnPauseGame(bool isActive)
    {
        pauseGame = isActive;
    }

    private void OnStartNewGame(int index)
    {
        gameTime = new GameTime();
        gameTime.gameSecond = 0;
        gameTime.gameMonth = 0;
        gameTime.gameHour = 6;
        gameTime.gameDay = 1;
        gameTime.gameMonth = 1;
        gameTime.gameYear = 1;
        gameTime.gameSeason = GameSeason.Spring;
        startGame = true;
    }
    private void OnAfterSceneLoad(GameScene gameScene)
    {
        startGame = true;
    }
    private void OnBeforeSceneLoad()
    {
        startGame = false;
    }
    private void UpdateGameTime()
    {
        if (!pauseGame)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= Settings.timeInterval)
            {
                gameTime.gameSecond++;
                timer = 0;
            }
            if (gameTime.gameSecond >= Settings.minuteInterval)
            {
                gameTime.gameSecond = 0;
                gameTime.gameMinute++;
                //每分钟调用
                EventManager.InvokeEventListener(ConstString.InvokeEveryMinuteEvent);
            }
            if (gameTime.gameMinute >= Settings.minuteInterval)
            {
                gameTime.gameMinute = 0;
                gameTime.gameHour++;
                //每小时调用
                EventManager.InvokeEventListener(ConstString.InvokeEveryMinuteEvent);
            }
            if (gameTime.gameHour >=Settings.hourInterval)
            {
                gameTime.gameDay++;
                gameTime.gameHour = 0;
                //每天调用
                EventManager.InvokeEventListener(ConstString.InvokeEveryDayEvent,gameTime.gameSeason);
            }
            if(gameTime.gameDay >=Settings.dayInterval)
            {
                gameTime.gameMonth++;
                gameTime.gameDay = 0;
                UpdateGameSeason();
            }
            if(gameTime.gameMonth >=Settings.monthInterval)
            {
                gameTime.gameYear++;
                gameTime.gameMonth = 0;
            }

        }
    }

    private void UpdateGameSeason()
    {
        if (1 <= gameTime.gameMonth && gameTime.gameMonth <= 3)
            gameTime.gameSeason = GameSeason.Spring;
        if (4 <= gameTime.gameMonth && gameTime.gameMonth <= 6)
            gameTime.gameSeason = GameSeason.Summer;
        if (7 <= gameTime.gameMonth && gameTime.gameMonth <= 9)
            gameTime.gameSeason = GameSeason.Autumn;
        if (10 <= gameTime.gameMonth && gameTime.gameMonth <= 12)
            gameTime.gameSeason = GameSeason.Winter;
    }

    public GameSaveData SaveGameData()
    {
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.gameTime = gameTime;
        gameSaveData.moduleName = "Time";
        return gameSaveData;
    }

    public void LoadGameData(GameSaveData gameSaveData)
    {
        gameTime = gameSaveData.gameTime;
    }

    public void RegisterInterface()
    {
        SaveLoadManager.Instance.RegisterInterface(this);
    }

    public string GetGuid()=> GetComponent<Guid>().ModuleGuid;
   
}
