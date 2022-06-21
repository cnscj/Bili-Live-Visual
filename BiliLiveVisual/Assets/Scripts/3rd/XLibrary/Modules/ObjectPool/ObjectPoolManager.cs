using System;
using System.Collections;
using System.Collections.Generic;
using XLibrary.Package;

namespace XLibGame
{
    public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
    {
        private Dictionary<string, IObjectPool> m_objPool = new Dictionary<string, IObjectPool>();
        public void AddPool(string key, IObjectPool pool) 
        {
            if (pool != null)
            {
                if (!m_objPool.ContainsKey(key))
                {
                    m_objPool.Add(key, pool);
                }
            }
        }

        public IObjectPool GetPool(string key)
        {
            if (!m_objPool.TryGetValue(key, out var poolObj))
            {
                return poolObj;
            }
            return null;
        }

        public void RemovePool(string key)
        {
            if (!m_objPool.TryGetValue(key, out var poolObj))
            {
                poolObj.Clear();
                m_objPool.Remove(key);
            }
            
        }

        public ObjectPool<T> GetOrCreatePool<T>(int defaultNum = 0) where T : class, new()
        {
            Type type = typeof(T);
            string poolKey = type.FullName;
            IObjectPool poolObj;
            if (!m_objPool.TryGetValue(poolKey, out poolObj))
            {
                poolObj = new ObjectPool<T>(defaultNum);
                m_objPool.Add(poolKey, poolObj);
            }
            return poolObj as ObjectPool<T>;
        }

        public void Clear()
        {
            foreach(var poolObj in m_objPool.Values)
            {
                poolObj.Clear();
            }
            m_objPool.Clear();
        }

        private void Update()
        {
            foreach(var pool in m_objPool.Values)
            {
                pool.Update();
            }
        }
    }
}
