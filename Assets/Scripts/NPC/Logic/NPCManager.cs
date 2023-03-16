using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoSingleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteDate;
    public List<NPCPosition> npcInitialPosition;

    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    protected override void Awake()
    {
        base.Awake();

        InitSceneRouteDict();
    }

    private void OnEnable()
    {
        EventCenter.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventCenter.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int obj)
    {
        foreach (var npc in npcInitialPosition)
        {
            npc.npcTransform.position = npc.position;
            npc.npcTransform.GetComponent<NPCMovement>().SetCurrentScene(npc.initialScene);
        }
    }

    //初始化场景路线
    private void InitSceneRouteDict()
    {
        if (sceneRouteDate.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteDate.sceneRouteList)
            {
                var key = KeyHelper.Instance.GetKey(route.fromSceneName,route.gotoSceneName);

                if (sceneRouteDict.ContainsKey(key))
                    continue;

                sceneRouteDict.Add(key, route);
            }
        }
    }

    /// <summary>
    /// 得到场景于场景的连接线路
    /// </summary>
    /// <param name="fromSceneName">出发场景</param>
    /// <param name="gotoSceneName">到达场景</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(GameScene fromSceneName, GameScene gotoSceneName)
    {
        return sceneRouteDict[KeyHelper.Instance.GetKey(fromSceneName, gotoSceneName)];
    }
}
