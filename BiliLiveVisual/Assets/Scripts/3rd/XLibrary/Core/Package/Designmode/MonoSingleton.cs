
using System;
using UnityEngine;
namespace XLibrary.Package
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        private static readonly object locker = new object();
        private static bool bAppQuitting;

        public static T GetInstance()
        {
            if (bAppQuitting)
            {
                instance = null;
                return instance;
            }

            lock (locker)
            {
                if (instance == null)
                {
                    // 保证场景中只有一个 单例
                    T[] managers = FindObjectsOfType(typeof(T)) as T[];
                    if (managers.Length != 0)
                    {
                        if (managers.Length == 1)
                        {
                            instance = managers[0];
                            instance.gameObject.name = typeof(T).Name;
                            return instance;
                        }
                        else
                        {
                            foreach (T manager in managers)
                            {
                                Destroy(manager.gameObject);
                            }
                        }
                    }


                    var singleton = new GameObject();
                    instance = singleton.AddComponent<T>();
                    singleton.name = "(singleton)" + typeof(T);
                    singleton.hideFlags = HideFlags.None;
                    DontDestroyOnLoad(singleton);

                }
                instance.hideFlags = HideFlags.None;
                return instance;
            }
        }

        protected virtual void Awake()
        {
            bAppQuitting = false;
        }

        protected virtual void OnDestroy()
        {
            bAppQuitting = true;
        }

    }
}

