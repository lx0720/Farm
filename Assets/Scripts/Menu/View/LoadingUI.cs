using Farm.Tool;
using Farm.Transition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]private Slider gameSlider;
    [SerializeField]private TextMeshProUGUI gameLoadProgress;
    [SerializeField] private RectTransform loadUI;
    [SerializeField] private TextMeshProUGUI gameLoadText;
    private bool isLoading;

    private void OnEnable()
    {
        EventManager.AddEventListener(ConstString.BeforeSceneLoadEvent,OnBeforeSceneLoad);
        EventManager.AddEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
    }
    private void OnDisable()
    {
        EventManager.RemoveEventListener(ConstString.BeforeSceneLoadEvent, OnBeforeSceneLoad);
        EventManager.RemoveEventListener<GameScene>(ConstString.AfterSceneLoadEvent, OnAfterSceneLoad);
    }


    private void Update()
    {
        if(isLoading)
        {
            gameSlider.value = TransitionManager.Instance.SliderValue;
            gameLoadProgress.text = (TransitionManager.Instance.SliderValue * 100).ToString() + "%";
            if((int)TransitionManager.Instance.SliderValue * 100 >=90)
            {
                gameLoadText.text = "按任意键继续";
            }
            else
            {

                gameLoadText.text = ((int)(TransitionManager.Instance.SliderValue*100))%2 == 0 ? "Loading..":"Loading...";
            }
        }
    }
    private void OnBeforeSceneLoad()
    {
        loadUI.gameObject.SetActive(true);
        isLoading = true;
    }


    private void OnAfterSceneLoad(GameScene targetScene)
    {
        loadUI.gameObject.SetActive(false);
        isLoading = false;
    }


}
