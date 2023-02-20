using UnityEngine;

namespace Farm.CropPlant
{
    public class CropManager : MonoSingleton<CropManager>
    {
        public CropDataList_SO cropData;
        private Transform cropParent;
        private Grid currentGrid;
        private Season currentSeason;

        private void OnEnable()
        {
            EventCenter.PlantSeedEvent += OnPlantSeedEvent;
            EventCenter.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventCenter.GameDayEvent += OnGameDayEvent;
        }

        private void OnDisable()
        {
            EventCenter.PlantSeedEvent -= OnPlantSeedEvent;
            EventCenter.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventCenter.GameDayEvent -= OnGameDayEvent;
        }

        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
        }

        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            cropParent = GameObject.FindWithTag("CropParent").transform;
        }

        private void OnPlantSeedEvent(int ID, TileDetails tileDetails)
        {
            CropDetails currentCrop = GetCropDetails(ID);
            if (currentCrop != null && SeasonAvailable(currentCrop) && tileDetails.seedItemID == -1)  
            {
                tileDetails.seedItemID = ID;
                tileDetails.growthDays = 0;

                DisplayCropPlant(tileDetails, currentCrop);
            }
            else if (tileDetails.seedItemID != -1)  //��ǰtile��������
            {
                DisplayCropPlant(tileDetails, currentCrop);
            }
        }


        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="tileDetails">tile����Ϣ</param>
        /// <param name="cropDetails">�������Ϣ</param>
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;

            //�жϽ׶ε�ʱ��
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }

            //����Go
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];

            Vector3 pos = new Vector3(tileDetails.girdX + 0.5f, tileDetails.gridY + 0.5f, 0);

            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;

            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;
            cropInstance.GetComponent<Crop>().tileDetails = tileDetails;
        }


        /// <summary>
        /// �õ�������Ϣ
        /// </summary>
        /// <param name="ID">����ID</param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int ID)
        {
            return cropData.cropDetailsList.Find(c => c.seedItemID == ID);
        }

        /// <summary>
        /// �жϼ����Ƿ����
        /// </summary>
        /// <param name="crop">��ǰ�������Ϣ</param>
        /// <returns></returns>
        private bool SeasonAvailable(CropDetails crop)
        {
            for (int i = 0; i < crop.seasons.Length; i++)
            {
                if (crop.seasons[i] == currentSeason)
                    return true;
            }
            return false;
        }
    }
}