using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHelper
{
    private static KeyHelper instance;

    public static KeyHelper Instance 
    {
        get 
        {
            if (instance == null)
                instance = new KeyHelper();
            return instance;
        }
    }


    public string GetKey(int x,int y,GameScene sceneName)
    {
        return x + "x" + y + "y" + sceneName.ToString();
    }
    public string GetKey(int x,int y,GameSceneName currentSceneName)
    {
        return x + "x" + y + "y" + currentSceneName.ToString();
    }

/*    private GameSceneName GetScenenName(string name)
    {
        switch(name)
        {
            case "PersistentScene": return GameSceneName.PersistentScene;
            case "UI": return GameSceneName.UI;
            case "01.Field": return GameSceneName.Field;
            case "02.Home": return GameSceneName.Home;
            default: return GameSceneName.None;
        }
    }*/

    public string GetKey(GameScene fromScene,GameScene toScene)
    {
        return fromScene.ToString() + toScene.ToString();
    }
}
