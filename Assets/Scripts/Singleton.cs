using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private bool initialized;
    public static T Instance
    {
        get
        {
            
            if (_instance == null)
            {
                var instance =FindObjectOfType<T>();
                if(instance != null)
                {
                    _instance = instance;
                    (_instance as Singleton<T>).Initialize();
                }
                else
                {
                GameObject Obj = new GameObject($"Singleton({typeof(T)})");
                _instance = Obj.AddComponent<T>();
                (_instance as Singleton<T>).Initialize();
                }
            }       
            
            return _instance;
        }
    }

    private  void Initialize()
    {
        initialized = true;
    }
}

public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private bool initialized;
    public static T Instance
    {
        get
        {

            if (_instance == null)
            {
                var instance = FindObjectOfType<T>();
                if (instance != null)
                {
                    _instance = instance;
                    (_instance as PersistentSingleton<T>).Initialize();
                }
                else
                {
                    GameObject Obj = new GameObject($"Singleton({typeof(T)})");
                    _instance = Obj.AddComponent<T>();
                    (_instance as PersistentSingleton<T>).Initialize();
                }
            }

            return _instance;
        }
    }

    private void Initialize()
    {
        initialized = true;
        DontDestroyOnLoad(gameObject);
    }
}
