using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class ObjectPool<T> : IObjectPool where T : class, new()
    {
        public float stayTime = 60f;
        public int maxCount = -1;
        public int minCount = 10;
        public float idleTime = 3*60f;
        public float checkTime = 30f;

        public Action<T> onCreate;
        public Action<T> onGet;
        public Action<T> onRelease;
        public Action<T> onDispose;

        private float m_lastIdleTick;
        private float m_lastCheckTick;
        private LinkedList<PoolObjectBundle<T>> m_available = new LinkedList<PoolObjectBundle<T>>();

        public int AvailableCount { get => m_available.Count; }

        public ObjectPool(int defaultNum = 0)
        {
            Fill(defaultNum);
            UpdateIdleTick();
            UpdateCheckTick();
        }

        public T GetOrCreate()
        {
            T obj = null;
            if (m_available.Count <= 0)
            {
                var objBundle = new PoolObjectBundle<T>();
                obj = objBundle.GetOrCreate();
                m_available.AddLast(objBundle);
                onCreate?.Invoke(obj);
            }

            var availableBundle = m_available.Last.Value;
            m_available.RemoveLast();

            obj = availableBundle.GetOrCreate();
            UpdateIdleTick();

            onGet?.Invoke(obj);
            return obj;
        }

        public void Release(T obj)
        {
            if (maxCount > 0 && m_available.Count >= maxCount)
                return;

            var objBundle = new PoolObjectBundle<T>(obj);
            objBundle.stayTime = stayTime;
            objBundle.UpdateTick();

            onRelease?.Invoke(obj);
            m_available.AddLast(objBundle);

            UpdateIdleTick();
        }

        public void Fill(int count)
        {
            if (count > 0)
            {
                Queue<T> tempList = new Queue<T>();
                if (maxCount > 0)
                {
                    while (m_available.Count < maxCount && m_available.Count < count)
                    {
                        tempList.Enqueue(GetOrCreate());
                    }
                }
                else
                {
                    while (m_available.Count < count)
                    {
                        tempList.Enqueue(GetOrCreate());
                    }
                }

                while (tempList.Count > 0)
                {
                    var newObject = tempList.Dequeue();
                    Release(newObject);
                }
            }
            UpdateIdleTick();
        }

        public void Clear()
        {
            m_available.Clear();
        }
        private void UpdateIdleTick()
        {
            m_lastIdleTick = Time.realtimeSinceStartup;
        }

        private void UpdateCheckTick()
        {
            m_lastCheckTick = Time.realtimeSinceStartup;
        }

        public void Update()
        {
            UpdatePool();
            UpdatePoolObjects();
        }

        private void UpdatePool()
        {
            if (idleTime <= 0)
                return;

            if (minCount > 0 && m_available.Count > minCount)
            {
                if (m_lastIdleTick + idleTime <= Time.realtimeSinceStartup)
                {
                    for (LinkedListNode<PoolObjectBundle<T>> iterNode = m_available.Last; iterNode != null;)
                    {
                        var nextIter = iterNode.Previous;

                        var poolObj = iterNode.Value;
                        if (poolObj != null)
                        {
                            iterNode = iterNode.Previous;
                            m_available.Remove(iterNode);
                        }

                        iterNode = nextIter;

                        if (m_available.Count <= minCount)
                            break;
                    }
                }
            }
        }
        private void UpdatePoolObjects()
        {
            if (checkTime <= 0)
                return;

            if (m_lastCheckTick + checkTime <= Time.realtimeSinceStartup)
            {
                if (m_available.Count > 0)
                {
                    for (LinkedListNode<PoolObjectBundle<T>> iterNode = m_available.First; iterNode != null;)
                    {
                        var nextIter = iterNode.Next;

                        var poolObj = iterNode.Value;
                        if (poolObj != null)
                        {
                            if (poolObj.IsOverTime())
                            {
                                m_available.Remove(iterNode);
                                onDispose?.Invoke(poolObj.GetOrCreate());
                            }
                            else
                            {
                                break;
                            }
                        }

                        iterNode = nextIter;
                    }
                }
                UpdateCheckTick();
            }
        }
    }
}

   
