using UnityEngine;

namespace ww.Utilities.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour, ISingleton where T : MonoBehaviour
    {
        private static T instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock) // Prevent Multithreading issues
                    {
                        instance = FindFirstObjectByType<T>();
                        // or
                        instance = (T)FindFirstObjectByType(typeof(T));

                        if (instance == null)
                        {
                            GameObject singletonObject = new GameObject(typeof(T).Name);
                            instance = singletonObject.AddComponent<T>();
                        }
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this as T;
                //Uncomment following line if you want to keep the singleton object alive between scenes
                //DontDestroyOnLoad(gameObject);
            }
        }
    }
}

