using System.Collections;
using System.Collections.Generic;

namespace XLibGame
{
    public class ObjectPool<T> : IObjectPool where T : class, new()
    {
        public float stayTime = -1;
        public int maxCount = -1;

        private LinkedList<PoolObjectBundle<T>> m_available = new LinkedList<PoolObjectBundle<T>>();

        public int AvailableCount { get => m_available.Count; }

        public ObjectPool(int defaultNum = 0)
        {
            Fill(defaultNum);
        }

        public T GetOrCreate()
        {
            if (m_available.Count <= 0)
            {
                var objBundle = new PoolObjectBundle<T>();
                m_available.AddLast(objBundle);
            }

            var availableObj = m_available.Last.Value;
            m_available.RemoveLast();

            return availableObj.GetOrCreate();
        }

        public void Release(T obj)
        {
            if (maxCount > 0 && m_available.Count >= maxCount)
                return;

            var objBundle = new PoolObjectBundle<T>(obj);
            m_available.AddLast(objBundle);
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

        public void Clear()
        {
            m_available.Clear();
        }

        public void Update()
        {
           
        }
    }
}
