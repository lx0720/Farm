using Farm.Save;
using UnityEngine;
using UnityEngine.UI;
public class SaveSlotUI : MonoBehaviour
{
    [SerializeField]private Text dataTime, dataScene;
    private Button currentButton;
    private DataSlot currentData;

    private int Index => transform.GetSiblingIndex();

    private void Awake()
    {
        currentButton = GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);
    }

    private void OnEnable()
    {
        SetupSlotUI();
    }

    private void SetupSlotUI()
    {
        currentData = SaveLoadManager.Instance.dataSlots[Index];

        if (currentData != null)
        {
            dataTime.text = currentData.DataTime;
            dataScene.text = currentData.DataScene;
        }
        else
        {
            dataTime.text = "ʱ�仹û��ʼ";
            dataScene.text = "���绹δ����";
        }
    }

    private void LoadGameData()
    {
        if (currentData != null)
        {
            SaveLoadManager.Instance.Load(Index);
        }
        else
        {
            EventCenter.CallStartNewGameEvent(Index);
        }
    }
}
