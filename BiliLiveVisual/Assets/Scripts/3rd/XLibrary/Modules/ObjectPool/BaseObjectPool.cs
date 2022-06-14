using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public abstract class BaseObjectPool
    {
        public abstract T GetOrCreate<T>() where T : class, new();

        public abstract void Release<T>(T obj) where T : class, new();

        public abstract void Update();

        public abstract void Clear();

    }
}
