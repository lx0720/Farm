using System.Collections;
using System.Collections.Generic;
using Farm.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Transition
{
    public class TransitionManager : MonoSingleton<TransitionManager>,ISaveable
    {
        [SceneName]
        public string startSceneName = string.Empty;
        private CanvasGroup fadeCanvasGroup;
        private bool isFade;

        public string GUID => GetComponent<DataGUID>().guid;

        protected override void Awake()
        {
            base.Awake();
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed, 0);
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        private void OnEnable()
        {
            EventCenter.TransitionEvent += OnTransitionEvent;
            EventCenter.StartNewGameEvent += OnStartNewGameEvent;
            EventCenter.EndGameEvent += OnEndGameEvent;
        }

        private void OnDisable()
        {
            EventCenter.TransitionEvent -= OnTransitionEvent;
            EventCenter.StartNewGameEvent -= OnStartNewGameEvent;
            EventCenter.EndGameEvent -= OnEndGameEvent;
        }

        private void OnEndGameEvent()
        {
            StartCoroutine(UnloadScene());
        }

        private void OnStartNewGameEvent(int obj)
        {
            StartCoroutine(LoadSceneSetActive(startSceneName));
        }


        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();

            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
        }


        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            if (!isFade)
                StartCoroutine(Transition(sceneToGo, positionToGo));
        }

        /// <summary>
        /// 场景切换
        /// </summary>
        /// <param name="sceneName">目标场景</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventCenter.CallBeforeSceneUnloadEvent();
            yield return Fade(1);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);
            //移动人物坐标
            EventCenter.CallMoveToPosition(targetPosition);
            EventCenter.CallAfterSceneLoadedEvent();
            yield return Fade(0);
        }

        /// <summary>
        /// 加载场景并设置为激活
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            EventCenter.CallAfterSceneLoadedEvent();

            SceneManager.SetActiveScene(newScene);
        }

        /// <summary>
        /// 淡入淡出场景
        /// </summary>
        /// <param name="targetAlpha">1是黑，0是透明</param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;

            fadeCanvasGroup.blocksRaycasts = true;

            float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / Settings.fadeDuration;

            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }

            fadeCanvasGroup.blocksRaycasts = false;

            isFade = false;
        }

        /// <summary>
        /// 加载场景(如果当前场景是最初的场景，那么就需要卸载当前的场景，异步加载目标场景)
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private IEnumerator LoadSaveDataScene(string sceneName)
        {
            yield return Fade(1f);
            ///判断当前场景是不是最初的场景
            if (SceneManager.GetActiveScene().name != "PersistentScene")    
            {
                EventCenter.CallBeforeSceneUnloadEvent();
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            }

            yield return LoadSceneSetActive(sceneName);
            EventCenter.CallAfterSceneLoadedEvent();
            yield return Fade(0);
        }


        private IEnumerator UnloadScene()
        {
            EventCenter.CallBeforeSceneUnloadEvent();
            yield return Fade(1f);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            yield return Fade(0);
        }



        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.dataSceneName = SceneManager.GetActiveScene().name;

            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            //加载游戏进度场景
            StartCoroutine(LoadSaveDataScene(saveData.dataSceneName));
        }
    }
}