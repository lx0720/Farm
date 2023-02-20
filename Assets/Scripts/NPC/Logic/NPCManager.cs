using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoSingleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteDate;
    public List<NPCPosition> npcPositionList;

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
        foreach (var character in npcPositionList)
        {
            character.npc.position = character.position;
            character.npc.GetComponent<NPCMovement>().SetCurrentScene(character.startScene);
        }
    }

    /// <summary>
    /// ��ʼ������·��
    /// <summary>
    private void InitSceneRouteDict()
    {
        if (sceneRouteDate.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteDate.sceneRouteList)
            {
                var key = route.fromSceneName + route.gotoSceneName;

                if (sceneRouteDict.ContainsKey(key))
                    continue;

                sceneRouteDict.Add(key, route);
            }
        }
    }

    /// <summary>
    /// �õ������ڳ�����������·
    /// </summary>
    /// <param name="fromSceneName">��������</param>
    /// <param name="gotoSceneName">���ﳡ��</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName)
    {
        return sceneRouteDict[fromSceneName + gotoSceneName];
    }
}
