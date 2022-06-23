using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;

namespace BLVisual
{
    public class GameObjectEmiter : MonoBehaviour
    {
        public new string name;
        public float emitInterval = 0;
        public float emitMaxCount = 20;

        public GameObject prefab;
        public Action<GameObject, object> onEmit;

        GameObjectPool _objectPool;
        Queue<object> _msgQueue = new Queue<object>();
        float _emitTick;
        bool _isRunning;

        public void Clear()
        {
            _msgQueue.Clear();
        }

        public void Start()
        {
            _isRunning = true;

        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Emit(object args)
        {
            var gObj = GetGameObject();
            onEmit?.Invoke(gObj,args);
        }

        public void Push(object msg)
        {
            if (msg == null)
                return;

            _msgQueue.Enqueue(msg);
        }

        public GameObjectPool GetPool()
        {
            if (_objectPool == null)
            {
                _objectPool = GameObjectPoolManager.GetInstance().GetPool(prefab.name) ?? GameObjectPoolManager.GetInstance().NewPool(prefab.name, prefab);
            }

            return _objectPool;
        }

        public GameObject GetGameObject()
        {
            return GetPool().GetOrCreate();
        }

        public void ReleaseGameObject(GameObject gObj)
        {
            GetPool().Release(gObj);
        }

        public void Destroy()
        {
            GameObjectEmiterManager.GetInstance().DestroyEmiter(this);
        }

        void UpdateQueue()
        {
            if (!_isRunning)
                return;

            if (_msgQueue.Count <= 0)
                return;

            if (_emitTick + emitInterval >= Time.realtimeSinceStartup)
                return;

            //各项发射参数
            var maxNum = Mathf.Min(_msgQueue.Count, emitMaxCount);
            if (emitMaxCount < 0)
                maxNum = _msgQueue.Count;

            for (int i = 0; i < maxNum; i++)
            {
                var args = _msgQueue.Dequeue();
                Emit(args);
            }

            _emitTick = Time.realtimeSinceStartup;
        }

        void Update()
        {
            UpdateQueue();
        }

    }

}
