
using UnityEngine;

namespace XLibGame
{
    public class PoolObjectBundle<T> where T : class, new()
    {
        public float stayTime = -1;
        private T _obj;
        private float _tick;
        public PoolObjectBundle() { }
        public PoolObjectBundle(T obj)
        {
            _obj = obj;
        }
        public T GetOrCreate()
        {
            _obj = Get() ?? new T();
            return _obj;
        }
        public T Get()
        {
            return _obj;
        }
        public bool IsOverTime()
        {
            if (stayTime > 0f)
            {
                return Time.realtimeSinceStartup >= _tick + stayTime;
            }
            return false;
        }
        public void UpdateTick()
        {
            _tick = Time.realtimeSinceStartup;
        }
    }
}