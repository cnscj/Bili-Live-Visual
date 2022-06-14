
using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame.UI
{
    public class UIPoolManager : MonoSingleton<UIPoolManager>
    {
        public class UIPool : MonoBehaviour
        {
            public class PoolObject 
            {
                public FComponent obj;
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
                            if (m_disposeTime > 0 )
                            {
                                if (Time.realtimeSinceStartup >= m_disposeTime)
                                {
                                    return true;
                                }  
                            }
                            else
                            {
                                var timeMs = UnityEngine.Random.Range(0, delayTimeMs);
                                m_disposeTime = Time.realtimeSinceStartup + timeMs/1000;
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

            public static readonly Action<FComponent> DEFAULT_CREATE_CALL = (obj) =>
            {

            };
            public static readonly Action<FComponent> DEFAULT_GET_CALL = (obj) =>
            {

            };
            public static readonly Action<FComponent> DEFAULT_RELEASE_CALL = (obj) =>
            {
                if (obj != null && obj.HasParent())
                {
                    obj.RemoveFromParent();
                }
            };
            public static readonly Action<FComponent> DEFAULT_DISPOSE_CALL = (obj) =>
            {
                obj?.Dispose();
            };

            public string poolName;
            public Type fComponent;
            public int maxCount = 50;
            public int minCount = 0;
            public float checkFrequence = 1f;
            public Action<FComponent> onCreate = DEFAULT_CREATE_CALL;
            public Action<FComponent> onGet = DEFAULT_GET_CALL;
            public Action<FComponent> onRelease = DEFAULT_RELEASE_CALL;
            public Action<FComponent> onDispose = DEFAULT_DISPOSE_CALL;

            private Dictionary<FComponent,PoolObject> m_recordObjs;
            private LinkedList<PoolObject> m_availableObjs;
            private float m_lastCheckTic;

            public static UIPool Create(string name, Type component, Transform parent = null)
            {
                GameObject poolGObj = new GameObject(name);
                var pool = poolGObj.AddComponent<UIPool>();
                pool.poolName = name;
                pool.fComponent = component;

                if (parent != null)
                {
                    poolGObj.transform.SetParent(parent, false);
                }
                return pool;
            }

            public int MaxCount { get { return maxCount; } }
            public int AvailableCount { get { return m_availableObjs != null ? m_availableObjs.Count : 0; } }

            public FComponent GetOrCreate()
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

            public void Release(FComponent uiObj)
            {
                if (uiObj == null)
                    return;

                if (AvailableCount >= MaxCount)
                {
                    onDispose?.Invoke(uiObj);
                    m_recordObjs?.Remove(uiObj);
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

                if (m_recordObjs == null)
                    return;

                foreach (var poolObj in m_availableObjs)
                {
                    var uiObj = poolObj.obj;
                    onDispose?.Invoke(uiObj);
                }
                m_availableObjs.Clear();
                m_recordObjs.Clear();
            }

            public void RemoveFromParent()
            {
                UIPoolManager.GetInstance().RemovePool(name);
            }

            private PoolObject CreatePoolObj(FComponent uiObj = null)
            {
                var newFobj = uiObj;
                if (newFobj == null)
                {
                    newFobj = (FComponent)System.Activator.CreateInstance(fComponent);
                    onCreate?.Invoke(newFobj);
                }

                var recordDict = GetObjectDict();
                PoolObject newPoolObj = null;

                if (!recordDict.ContainsKey(newFobj)) recordDict[newFobj] = new PoolObject();
                newPoolObj = recordDict[newFobj];
                newPoolObj.obj = newFobj;
                newPoolObj.UpdateTick();

                return newPoolObj;
            }

            private LinkedList<PoolObject> GetObjectList()
            {
                m_availableObjs = m_availableObjs ?? new LinkedList<PoolObject>();
                return m_availableObjs;
            }
            private Dictionary<FComponent,PoolObject> GetObjectDict()
            {
                m_recordObjs = m_recordObjs ?? new Dictionary<FComponent, PoolObject>();
                return m_recordObjs;
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

                for (LinkedListNode<PoolObject> iterNode = m_availableObjs.Last; iterNode != null; iterNode = iterNode.Previous)
                {
                    var poolObj = iterNode.Value;
                    if (poolObj.CheckDispose())
                    {
                        onDispose?.Invoke(poolObj.obj);
                        m_availableObjs.Remove(iterNode);
                    }
                }

                m_lastCheckTic = Time.realtimeSinceStartup;
            }
        }

        private Dictionary<string, UIPool> m_poolDict;

        public UIPool GetOrCreatePool<T>() where T : FComponent ,new()
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

        public UIPool CreatePool(string poolName, Type fComponent)
        {
            if (string.IsNullOrEmpty(poolName))
                return null;

            if (fComponent == null)
                return null;

            var poolDict = GetPoolDict();
            if (poolDict.ContainsKey(poolName))
                return poolDict[poolName];

            var pool = UIPool.Create(poolName, fComponent, transform);
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