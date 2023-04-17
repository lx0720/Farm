using UnityEngine;

public abstract class MonoSingleton<T> 
    : MonoBehaviour where T 
    : MonoSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            _instance ??= FindObjectOfType<T>();
            if (_instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>();
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if(_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    
}
