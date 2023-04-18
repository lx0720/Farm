using System.Collections;
using System.Collections.Generic;
using Farm.Input;
using Farm.Tool;
using Farm.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Transition
{
    public class TransitionManager : MonoSingleton<TransitionManager>,ISaveLoad
    {
        private GameScene startSceneName = GameScene.YardScene;
        private CanvasGroup fadeCanvasGroup;
        private float sliderValue;

        public float SliderValue => sliderValue;

        protected override void Awake()
        {
            base.Awake();
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed, 0);
            SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
            RegisterInterface();
        }

        private void OnEnable()
        {
            EventManager.AddEventListener<GameScene, Vector3>(ConstString.TransitionSceneEvent, OnTransitionEvent);
            EventManager.AddEventListener(ConstString.BackToMenuEvent, OnBackToMenu);
            EventManager.AddEventListener(ConstString.EndGameEvent, OnEndGameEvent);
            EventManager.AddEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGame);
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<GameScene, Vector3>(ConstString.TransitionSceneEvent, OnTransitionEvent);
            EventManager.RemoveEventListener(ConstString.BackToMenuEvent, OnBackToMenu);
            EventManager.RemoveEventListener(ConstString.EndGameEvent, OnEndGameEvent);
            EventManager.RemoveEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGame); 
        }

        private void OnEndGameEvent()
        {
            
        }

        private void OnStartNewGame(int obj)
        {
            StartCoroutine(LoadNewGame());
        }


        private void Start()
        {
            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
        }


        private void OnTransitionEvent(GameScene transitionScene, Vector3 positionToGo)
        {
            StartCoroutine(LoadAnotherGame(transitionScene));
        }

        private void OnBackToMenu()
        {
            StartCoroutine(UnLoadSceneAsync());
        }

        public IEnumerator LoadNewGame()
        {
            yield return LoadSceneAsync();
        }
        public IEnumerator LoadSaveGame(GameScene targetScene)
        {
            yield return LoadSceneAsync(targetScene);
        }
        public IEnumerator LoadAnotherGame(GameScene targetScene)
        {
            EventManager.InvokeEventListener(ConstString.BeforeSceneLoadEvent);
            yield return SceneManager.UnloadSceneAsync(GameTools.GetCurrentSceneEnum().ToString());
            yield return LoadSceneAsync(targetScene);
            EventManager.InvokeEventListener(ConstString.AfterSceneLoadEvent, targetScene);
        }
        public IEnumerator LoadSceneAsync()
        {
            EventManager.InvokeEventListener(ConstString.BeforeSceneLoadEvent);
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(startSceneName.ToString(), LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                sliderValue = asyncOperation.progress;
                if (asyncOperation.progress >= 0.9f)
                {
                    sliderValue = 1;

                    if (UnityEngine.Input.anyKey)
                    {
                        asyncOperation.allowSceneActivation = true;
                    }
                }
                yield return null;
            }
            EventManager.InvokeEventListener(ConstString.AfterSceneLoadEvent, startSceneName);
        }
        public IEnumerator LoadSceneAsync(GameScene targetScene)
        {
            EventManager.InvokeEventListener(ConstString.BeforeSceneLoadEvent);
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetScene.ToString(), LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                sliderValue = asyncOperation.progress;
                if (asyncOperation.progress >= 0.9f)
                {
                    sliderValue = 1;

                    if (UnityEngine.Input.anyKey)
                    {
                        asyncOperation.allowSceneActivation = true;
                    }
                }
                yield return null;
            }
            EventManager.InvokeEventListener(ConstString.AfterSceneLoadEvent, targetScene);
        }

        public IEnumerator UnLoadSceneAsync()
        {
            EventManager.InvokeEventListener(ConstString.EndGameEvent);
            yield return SceneManager.UnloadSceneAsync(GameTools.GetCurrentSceneEnum().ToString());
        }
       

        private void LoadGameSaveScene(GameScene targetScene)
        {
            if(GameTools.GetCurrentSceneEnum() == GameScene.UIScene)
            {
                StartCoroutine(LoadSaveGame(targetScene));
            }
        }

        public string GetGuid() => GetComponent<Guid>().ModuleGuid;


        public GameSaveData SaveGameData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.saveGameScene = GameTools.GetCurrentSceneEnum();
            saveData.moduleName = "Transition";
            return saveData;
        }

        public void LoadGameData(GameSaveData gameSaveData)
        {
            LoadGameSaveScene(gameSaveData.saveGameScene);
        }

        public void RegisterInterface()
        {
            SaveLoadManager.Instance.RegisterInterface(this);
        }
    }
}