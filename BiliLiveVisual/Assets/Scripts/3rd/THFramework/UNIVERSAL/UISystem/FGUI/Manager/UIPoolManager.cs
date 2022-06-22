
using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame.UI
{
    public class UIPoolObject
    {
        public FWidget obj;
        public float stayTime = 60;
        public float delayTimeMs = 6000;
        private float m_disposeTime;
        private float m_visitTick;

        public bool CheckDispose()
        {
            if (stayTime > 0)
            {
                if (Time.realtimeSinceStartup - m_visitTick >= stayTime)
                {
                    if (m_disposeTime > 0)
                    {
                        if (Time.realtimeSinceStartup >= m_disposeTime)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        var timeMs = UnityEngine.Random.Range(0, delayTimeMs);
                        m_disposeTime = Time.realtimeSinceStartup + timeMs / 1000;
                    }

                }
            }
            return false;
        }

        public void UpdateTick()
        {
            m_visitTick = Time.realtimeSinceStartup;
            m_disposeTime = -1;
        }

    }

    public class UIPool : MonoBehaviour
    {
        public static readonly Action<FWidget> DEFAULT_CREATE_CALL = (uiObj) =>
        {
            if (!uiObj.IsCreated() && !uiObj.IsCreating())
                uiObj.TryCreate();
        };
        public static readonly Action<FWidget> DEFAULT_GET_CALL = (uiObj) =>
        {

        };
        public static readonly Action<FWidget> DEFAULT_RELEASE_CALL = (uiObj) =>
        {
            if (uiObj != null && uiObj.HasParent())
            {
                uiObj.RemoveFromParent();
            }
        };
        public static readonly Action<FWidget> DEFAULT_DISPOSE_CALL = (uiObj) =>
        {
            uiObj.Dispose();
        };

        public string poolName;
        public Type compType;
        public int maxCount = 50;
        public int minCount = 0;
        public float checkFrequence = 1f;
        public Action<FWidget> onCreate = DEFAULT_CREATE_CALL;
        public Action<FWidget> onGet = DEFAULT_GET_CALL;
        public Action<FWidget> onRelease = DEFAULT_RELEASE_CALL;
        public Action<FWidget> onDispose = DEFAULT_DISPOSE_CALL;

        private LinkedList<UIPoolObject> m_availableObjs;
        private float m_lastCheckTic;

        public static UIPool Create(string name, Type type, Transform parent = null)
        {
            GameObject poolGObj = new GameObject(name);
            var pool = poolGObj.AddComponent<UIPool>();
            pool.poolName = name;
            pool.compType = type;

            if (parent != null)
            {
                poolGObj.transform.SetParent(parent, false);
            }
            return pool;
        }

        public int MaxCount { get { return maxCount; } }
        public int AvailableCount { get { return m_availableObjs != null ? m_availableObjs.Count : 0; } }

        public FWidget GetOrCreate()
        {
            var poolList = GetObjectList();
            if (poolList.Count <= 0)
            {
                var newPoolObj = CreatePoolObj();
                poolList.AddLast(newPoolObj);
            }

            var poolObj = poolList.Last.Value;
            poolList.RemoveLast();

            var uiObj = poolObj.obj;
            onGet?.Invoke(uiObj);
            poolObj.UpdateTick();
            return uiObj;
        }

        public void Release(FWidget uiObj)
        {
            if (uiObj == null)
                return;

            if (AvailableCount >= MaxCount)
            {
                onDispose?.Invoke(uiObj);
            }
            else
            {
                var poolList = GetObjectList();
                var newPoolObj = CreatePoolObj(uiObj);
                onRelease?.Invoke(uiObj);
                poolList.AddLast(newPoolObj);
            }
        }

        public void Full()
        {
            var restNum = MaxCount - AvailableCount;
            if (restNum > 0)
            {
                var poolList = GetObjectList();
                for (int i = 0; i < restNum; i++)
                {
                    var newPoolObj = CreatePoolObj();
                    poolList.AddLast(newPoolObj);
                }
            }
        }

        public void Clear()
        {
            if (m_availableObjs == null)
                return;


            foreach (var poolObj in m_availableObjs)
            {
                var uiObj = poolObj.obj;
                onDispose?.Invoke(uiObj);
            }
            m_availableObjs.Clear();
        }

        public void RemoveFromParent()
        {
            UIPoolManager.GetInstance().RemovePool(name);
        }

        private UIPoolObject CreatePoolObj(FWidget uiObj = null)
        {
            var newFobj = uiObj;
            if (newFobj == null)
            {
                newFobj = (FWidget)System.Activator.CreateInstance(compType);
                onCreate?.Invoke(newFobj);
            }
          
            UIPoolObject newPoolObj = new UIPoolObject();
            newPoolObj.obj = newFobj;
            newPoolObj.UpdateTick();

            return newPoolObj;
        }

        private LinkedList<UIPoolObject> GetObjectList()
        {
            m_availableObjs = m_availableObjs ?? new LinkedList<UIPoolObject>();
            return m_availableObjs;
        }

        private void Update()
        {
            UpdateCheck();
        }

        private void UpdateCheck()
        {
            if (checkFrequence < 0)
                return;

            if (m_availableObjs == null)
                return;

            if (Time.realtimeSinceStartup - m_lastCheckTic < checkFrequence)
                return;

            if (AvailableCount <= minCount)
                return;

            for (LinkedListNode<UIPoolObject> iterNode = m_availableObjs.Last; iterNode != null;)
            {
                var nextIter = iterNode.Previous;

                var poolObj = iterNode.Value;
                if (poolObj.CheckDispose())
                {
                    onDispose?.Invoke(poolObj.obj);
                    m_availableObjs.Remove(iterNode);
                }

                iterNode = nextIter;
            }

            m_lastCheckTic = Time.realtimeSinceStartup;
        }
    }

    public class UIPoolManager : MonoSingleton<UIPoolManager>
    {
        private Dictionary<string, UIPool> m_poolDict;

        public UIPool GetOrCreatePool<T>() where T : FWidget ,new()
        {
            var type = typeof(T);
            var poolName = type.FullName;
            var pool = GetPool(poolName);
            if (pool == null)
            {
                pool = CreatePool(poolName, type);
            }
            return pool;
        }

        public UIPool CreatePool(string poolName, Type widgetType)
        {
            if (string.IsNullOrEmpty(poolName))
                return null;

            if (widgetType == null)
                return null;

            var poolDict = GetPoolDict();
            if (poolDict.ContainsKey(poolName))
                return poolDict[poolName];

            var pool = UIPool.Create(poolName, widgetType, transform);
            poolDict[poolName] = pool;

            return pool;

        }

        public UIPool GetPool(string poolName)
        {
            if (string.IsNullOrEmpty(poolName))
                return null;

            if (m_poolDict == null)
                return null;

            var poolDict = GetPoolDict();
            if (!poolDict.ContainsKey(poolName))
                return null;

            return poolDict[poolName];
        }

        public void RemovePool(string poolName)
        {
            if (m_poolDict == null)
                return;

            var pool = GetPool(poolName);
            if (pool != null)
            {
                pool.Clear();
                Destroy(pool.gameObject);
                m_poolDict.Remove(poolName);
            }
        }

        private Dictionary<string, UIPool> GetPoolDict()
        {
            m_poolDict = m_poolDict ?? new Dictionary<string, UIPool>();
            return m_poolDict;
        }
    }
}