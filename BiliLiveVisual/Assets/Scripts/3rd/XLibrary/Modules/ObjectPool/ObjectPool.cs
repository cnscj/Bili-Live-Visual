using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class ObjectPool<T> : BaseObjectPool where T : class, new()
    {
        public float stayTime = -1;
        public int maxCount = -1;

        private Dictionary<T, ObjectPoolObject> m_recordDict = new Dictionary<T, ObjectPoolObject>();
        private LinkedList<T> m_available = new LinkedList<T>();
        private Queue<T> m_release = new Queue<T>();

        public int AvailableCount { get => m_available.Count; }
        public int TotalCount { get => m_recordDict.Count; }

        public ObjectPool(int defaultNum = 0)
        {
            Fill(defaultNum);
        }

        public T GetOrCreate()
        {
            if (m_available.Count <= 0)
            {
                var newObj = new T();
                m_available.AddLast(newObj);

                var poolObj = new ObjectPoolObject();
                poolObj.stayTime = stayTime;

                m_recordDict.Add(newObj, poolObj);
            }

            var availableObj = m_available.Last.Value;
            m_available.RemoveLast();
            var availableObjInfo = m_recordDict[availableObj];
            availableObjInfo.UpdateTick();

            return availableObj;
        }

        public void Release(T obj, bool isForce = false)
        {
            if (maxCount > 0 && m_available.Count >= maxCount)
                return;

            if (!m_recordDict.ContainsKey(obj))
            {
                if (isForce)
                {
                    return;
                }
                else
                {
                    var poolObj = new ObjectPoolObject();
                    poolObj.UpdateTick();

                    m_recordDict.Add(obj, poolObj);
                }
            }

            m_available.AddLast(obj);
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
        }

        public override void Clear()
        {
            m_available.Clear();
            m_recordDict.Clear();
        }

        public override void Update()
        {
            UpdateCheck();
            UpdateRelease();
        }

        private void UpdateCheck()
        {
            if (m_available != null && m_available.Count > 0)
            {
                foreach (var availableObj in m_available)
                {
                    if (m_recordDict.TryGetValue(availableObj, out var poolObj))
                    {
                        if (poolObj.CheckRemove())
                        {
                            m_release.Enqueue(availableObj);
                        }
                    }
                }
            }
        }

        private void UpdateRelease()
        {
            while(m_release.Count > 0)
            {
                var releaseObj = m_release.Dequeue();
                m_available.Remove(releaseObj);
                m_recordDict.Remove(releaseObj);
            }
        }

        public override T1 GetOrCreate<T1>() 
        {
            return GetOrCreate() as T1;
        }

        public override void Release<T1>(T1 obj)
        {
            Release(obj);
        }
    }
}
