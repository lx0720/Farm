using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farm.Tool
{
    public static class ConstString
    {
        public static string UpdateInventoryUIEvent = "UpdateInventoryUIEvent";
        public static string StartNewGameEvent = "StartNewGameEvent";
        public static string BeforeSceneLoadEvent = "BeforeSceneLoadEvent";
        public static string AfterSceneLoadEvent = "AfterSceneLoadEvent";
        public static string AfterMenuLoadEvent = "AfterMenuLoadEvent";
        public static string TransitionSceneEvent = "TransitionSceneEvent";
        public static string EndGameEvent = "EndGameEvent";
        public static string BackToMenuEvent="BackToMenuEvent";
        public static string PauseGameEvent = "PauseGameEvent";
        public static string InvokeEveryHourEvent = "InvokeEveryHourEvent";
        public static string InvokeEveryMinuteEvent = "InvokeEveryMinuteEvent";
        public static string InvokeEveryDayEvent = "InvokeEveryDayEvent";
        public static string InvokeEverySeasonEvent = "InvokeEverySeasonEvent";
        public static string SceneLoadedEvent = "SceneLoadedEvent";
        public static string ChoseItemEvent = "ChoseItemEvent";
        public static string RefreshInventoryUIEvent = "RefreshInventoryUIEvent";
        public static string UpdateHightLightEvent = "UpdateHightLightEvent";

        public static string BeginDragItemEvent = "BeginDragItemEvent";
        public static string DragItemEvent = "DragItemEvent";
        public static string EndDragItemEvent = "EndDragItemEvent";
        public static string DigTileMapTag = "Dig";
        public static string WaterTileMapTag = "Water";
    }
}
