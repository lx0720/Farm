using UnityEngine;
using System.Collections.Generic;

namespace Farm.Tool
{
    public class GameObjectPool : MonoSingleton<GameObjectPool>
    {
        private Dictionary<string, Queue<GameObject>> gameObjectPoolDict;
        protected override void Awake()
        {
            base.Awake();
            gameObjectPoolDict = new Dictionary<string, Queue<GameObject>>();
        }
        //���
        public void ReturnObject(string goName, GameObject go)
        {
            go.SetActive(false);
            if (!gameObjectPoolDict.ContainsKey(goName))
            {
                gameObjectPoolDict.Add(goName, new Queue<GameObject>());
            }
            gameObjectPoolDict[goName].Enqueue(go);
        }
        //����
        public GameObject GetObject(string goName)
        {
            if (gameObjectPoolDict.TryGetValue(goName, out Queue<GameObject> queue) && queue.Count != 0)
            {
                return queue.Dequeue();
            }
            else
            {
                return Instantiate(GameObjectFactory.GetGameObject(goName));
            }
        }
        //���
        public void Clear()
        {
            foreach (var queue in gameObjectPoolDict.Values)
            {
                queue.Clear();
            }
        }
    }
}
