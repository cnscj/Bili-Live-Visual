using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame
{
    public class MonoManager : MonoSingleton<MonoManager>
    {
        private MonoCallback _awakeCallback;
        private MonoCallback _startCallback;
        private MonoCallback _updateCallback;
        private MonoCallback _fixedUpdateCallback;
        private MonoCallback _lateUpdateCallback;

        private Dictionary<string, GameObject> _debugDict;

        private new void Awake()
        {
            _awakeCallback?.Invoke();
        }

        private void Start()
        {
            _startCallback?.Invoke();
        }

        private void Update()
        {
            _updateCallback?.Invoke();
        }

        private void FixedUpdate()
        {
            _fixedUpdateCallback?.Invoke();
        }

        private void LateUpdate()
        {
            _lateUpdateCallback?.Invoke();
        }

        ///////
        public void AddAwakeListener(MonoCallback callback)
        {
            _awakeCallback += callback;
        }
        public void RemoveAwakeListener(MonoCallback callback)
        {
            _awakeCallback -= callback;
        }

        public void AddStartListener(MonoCallback callback)
        {
            _startCallback += callback;
        }
        public void RemoveStartListener(MonoCallback callback)
        {
            _startCallback -= callback;
        }

        public void AddUpdateListener(MonoCallback callback)
        {
            _updateCallback += callback;
            AddDebugNode("Update",callback);
        }
        public void RemoveUpdateListener(MonoCallback callback)
        {
            _updateCallback -= callback;
            RemoveDebugNode("Update", callback);
        }

        public void AddFixedUpdateListener(MonoCallback callback)
        {
            _fixedUpdateCallback += callback;
            AddDebugNode("FixedUpdate", callback);
        }
        public void RemoveFixedUpdateListener(MonoCallback callback)
        {
            _fixedUpdateCallback -= callback;
            RemoveDebugNode("FixedUpdate", callback);
        }

        public void AddLateUpdateListener(MonoCallback callback)
        {
            _lateUpdateCallback += callback;
            AddDebugNode("LateUpdate", callback);
        }
        public void RemoveLateUpdateListener(MonoCallback callback)
        {
            _lateUpdateCallback -= callback;
            RemoveDebugNode("LateUpdate", callback);
        }

        public void RemoveAllListener()
        {
            _awakeCallback = null;
            _startCallback = null;
            _updateCallback = null;
            _fixedUpdateCallback = null;
            _lateUpdateCallback = null;
            if (_debugDict != null)
            {
                foreach (var pair in _debugDict)
                {
                    Object.Destroy(pair.Value);
                }
                _debugDict.Clear();
            }

        }
        /////////////////////////
        private Dictionary<string, GameObject> GetDebugDict()
        {
            _debugDict = _debugDict ?? new Dictionary<string, GameObject>();
            return _debugDict;
        }

        private GameObject CreateDebugNode(string name)
        {
            GameObject debugObj = new GameObject(name);
            debugObj.transform.SetParent(transform);
            return debugObj;
        }

        private string GetCallbackKey(string type,MonoCallback callback)
        {
            return string.Format("{0}+{1}", type, callback.Method);
        }

        private void AddDebugNode(string type, MonoCallback callback)
        {
            #if !UNITY_EDITOR
                return;
            #endif
            var key = GetCallbackKey(type, callback);
            var dict = GetDebugDict();
            if (!dict.ContainsKey(key))
                dict.Add(key, CreateDebugNode(key));
            
        }

        private void RemoveDebugNode(string type, MonoCallback callback)
        {
            #if !UNITY_EDITOR
                return;
            #endif
            var key = GetCallbackKey(type, callback);
            if (_debugDict != null && _debugDict.Count > 0)
            {
                if (_debugDict.TryGetValue(key,out var debugNode))
                {
                    Object.Destroy(debugNode);
                    _debugDict.Remove(key);
                }

            }
        }
    }

}
