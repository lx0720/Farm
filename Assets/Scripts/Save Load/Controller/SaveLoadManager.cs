using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine;
using Farm.Input;
using Farm.Tool;

namespace Farm.Save
{
    /// <summary>
    /// 保存加载管理器
    /// </summary>
    public class SaveLoadManager : MonoSingleton<SaveLoadManager>
    {
        private List<ISaveLoad> saveLoads;
        private List<DataSlot> gameSaveDataSlots;
        private Dictionary<int, bool> indexSaveLoad;
        private string gameSaveDataDirectoryPath;
        private string gameSaveDataFilePath;
        public int currentDataIndex;


        protected override void Awake()
        {
            base.Awake();
            saveLoads = new List<ISaveLoad>();
            gameSaveDataSlots = new List<DataSlot>(new DataSlot[16]);
            indexSaveLoad = new Dictionary<int, bool>();
            gameSaveDataDirectoryPath = Application.persistentDataPath + "/GameSaveData/";
            
        }
        private void Start()
        {
            ReadGameSaveData();
        }
        private void OnEnable()
        {
            EventManager.AddEventListener<int>(ConstString.StartNewGameEvent,OnStartNewGameEvent);
            EventManager.AddEventListener(ConstString.EndGameEvent, OnEndGame);
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGameEvent);
            EventManager.RemoveEventListener(ConstString.EndGameEvent, OnEndGame);
        }

        private void OnEndGame()
        {
            SaveGame(currentDataIndex);
        }

        private void Update()
        {
            if (InputManager.Instance.GetI())
                SaveGame(currentDataIndex);
            if (InputManager.Instance.GetG())
                LoadGame(currentDataIndex);
        }

        private void OnStartNewGameEvent(int index)
        {
            currentDataIndex = index;
        }

        public void RegisterInterface(ISaveLoad saveLoad)
        {
            if (!saveLoads.Contains(saveLoad))
                saveLoads.Add(saveLoad);
        }

        private void ReadGameSaveData()
        {
            if (!Directory.Exists(gameSaveDataDirectoryPath))
            {
                Debug.Log("文件夹不存在！");
                Directory.CreateDirectory(gameSaveDataDirectoryPath);
            }
            else
            {
                for(int i=0;i<16;i++)
                {
                    string filePath = gameSaveDataDirectoryPath + "GAMEPROGRESS_SAVEDATA_" + i + ".json";
                    if(File.Exists(filePath))
                    {
                        string josnContent = File.ReadAllText(filePath);
                        gameSaveDataSlots[i] = JsonConvert.DeserializeObject<DataSlot>(josnContent);
                    }
                    else
                    {
                        gameSaveDataSlots[i] = new DataSlot();
                    }
                }
            }

        }

        private void SaveGame(int index)
        {
            DataSlot dataSlot = new DataSlot();

            foreach (ISaveLoad saveLoad in saveLoads)
            {
                dataSlot.dataDict.Add(saveLoad.GetGuid(), saveLoad.SaveGameData());
            }
            gameSaveDataSlots[index] = dataSlot;
            gameSaveDataFilePath = gameSaveDataDirectoryPath + "GAMEPROGRESS_SAVEDATA_" + index + ".json";
            string jsonData = JsonConvert.SerializeObject(gameSaveDataSlots[index], Formatting.Indented);
            if (!Directory.Exists(gameSaveDataDirectoryPath))
                Directory.CreateDirectory(gameSaveDataFilePath);
            //提示存储成功
            File.WriteAllText(gameSaveDataFilePath, jsonData);

        }

        public void LoadGame(int index)
        {
            currentDataIndex = index;
            string filePath = gameSaveDataDirectoryPath + "GAMEPROGRESS_SAVEDATA_" + index + ".json";
            string jsonContent = File.ReadAllText(filePath);
            DataSlot dataSlot = JsonConvert.DeserializeObject<DataSlot>(jsonContent);
            foreach (ISaveLoad saveLoad in saveLoads)
            {
                saveLoad.LoadGameData(dataSlot.dataDict[saveLoad.GetGuid()]);
            }
        }

        public DataSlot GetDataSlot(int index) => gameSaveDataSlots[index];

    }
}
