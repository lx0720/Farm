using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Farm.Tool
{
    public class GameTools
    {
        public static GameScene StringToEnum(string sceneName)
        {
            return sceneName switch
            {
                "InitialScene" => GameScene.InitialScene,
                "UIScene" => GameScene.UIScene,
                "YardScene" => GameScene.YardScene,
                "HomeScene" => GameScene.HomeScene,
                _ => GameScene.ErrorScene
            };
        }

        public static GameScene GetCurrentSceneEnum()
        {
            return StringToEnum(SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name);
        }
    }
}
