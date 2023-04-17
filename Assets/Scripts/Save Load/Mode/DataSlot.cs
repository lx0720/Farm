using System.Collections;
using System.Collections.Generic;
using Farm.Transition;
using UnityEngine;

namespace Farm.Save
{
    /// <summary>
    /// 数据存储slot，一个DataSlot代表一个存储进度
    /// </summary>
    public class DataSlot 
    {
        string gameTimeModuleKey;
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();
        public string DataTime
        {
            get
            {
                gameTimeModuleKey = GameTimeManager.Instance.GetGuid();
                if (dataDict.ContainsKey(gameTimeModuleKey))
                {
                    GameSaveData timeModuleData = dataDict[gameTimeModuleKey];
                    return DataSeason + " " + timeModuleData.gameTime.gameYear + "年" +
                        timeModuleData.gameTime.gameMonth + "月" + timeModuleData.gameTime.gameDay + "日";
                }
                return null;
            }
        }
        public string DataScene
        {
            get
            {
                var key = TransitionManager.Instance.GetGuid() ;
                if (dataDict.ContainsKey(key))
                {
                    var transitionData = dataDict[key];
                    return transitionData.saveGameScene switch
                    {
                        GameScene.InitialScene => "初始界面",
                        GameScene.HomeScene => "屋内",
                        GameScene.YardScene =>"庭院",
                        _ => string.Empty
                    };
                }
                return null ;
            }
        }
        
        private string DataSeason
        {
            get
            {
                gameTimeModuleKey = GameTimeManager.Instance.GetGuid();
                if (dataDict.ContainsKey(gameTimeModuleKey))
                {
                    GameSaveData timeModuleData = dataDict[gameTimeModuleKey];
                    return timeModuleData.gameTime.gameSeason switch
                    {
                        GameSeason.Spring => "春季",
                        GameSeason.Summer => "夏季",
                        GameSeason.Autumn => "秋季",
                        GameSeason.Winter => "冬季",
                        _ => "春季"
                    };
                }
                return null;
            }
        }
    }
}