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
         
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region Menu的slot显示
        //保存游戏时间
        public string DataTime
        {
            get
            {
                var key = TimeManager.Instance.GUID;

                if (dataDict.ContainsKey(key))
                {
                    var timeData = dataDict[key];
                    return timeData.timeDict["gameYear"] + "年/" + (Season)timeData.timeDict["gameSeason"] + "/" + timeData.timeDict["gameMonth"] + "月/" + timeData.timeDict["gameDay"] + "日/";
                }
                else return string.Empty;
            }
        }
        //保存游戏场景
        public string DataScene
        {
            get
            {
                var key = TransitionManager.Instance.GUID;
                if (dataDict.ContainsKey(key))
                {
                    var transitionData = dataDict[key];
                    return transitionData.dataSceneName switch
                    {
                        GameScene.InitialScene => "庭院",
                        GameScene.HomeScene => "屋内",
                        _ => string.Empty
                    };
                }
                else return string.Empty;
            }
        }
        #endregion
    }
}
