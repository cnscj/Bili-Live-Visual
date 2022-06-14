
using System.Collections.Generic;

namespace XLibrary.Package.MVC
{
    public class MVCManager : Singleton<MVCManager>
    {
        Dictionary<string, Cache> _cacheDict = new Dictionary<string, Cache>();
        Dictionary<string, Controller> _controllerDict = new Dictionary<string, Controller>(); 

        public static void RegisterCtrlAndCache<T1,T2>() where T1 : Controller, new() where T2 : Cache, new()
        {
            var mgr = GetInstance();
            mgr.AddController<T1>();
            mgr.AddCache<T2>();
        }

        public void AddCache<T>() where T : Cache, new()
        {
            var key = GetTypeKey<T>();
            if (!_cacheDict.TryGetValue(key, out var cache))
            {
                cache = new T();
                _cacheDict.Add(key, cache);
                cache.Initilize();
            }
        }

        public void RemoveCache<T>() where T : Cache, new()
        {
            var key = GetTypeKey<T>();
            if (_cacheDict.TryGetValue(key, out var cache))
            {
                _cacheDict.Clear();
                _cacheDict.Remove(key);
            }
        }

        public void AddController<T>() where T : Controller, new()
        {
            var key = GetTypeKey<T>();
            if (!_controllerDict.TryGetValue(key,out var controller))
            {
                controller = new T();
                _controllerDict.Add(key, controller);
                controller.Initilize();
            }
        }

        public void RemoveController<T>() where T : Controller, new()
        {
            var key = GetTypeKey<T>();
            if (_controllerDict.TryGetValue(key, out var controller))
            {
                controller.Clear();
                _controllerDict.Remove(key);
            }
        }

        public T GetCache<T>() where T : Cache, new()
        {
            var key = GetTypeKey<T>();
            if (_cacheDict.TryGetValue(key, out var cache))
            {
                return (T)cache;

            }
            return default;
        }

        public T GetController<T>() where T : Controller, new()
        {
            var key = GetTypeKey<T>();
            if (_controllerDict.TryGetValue(key, out var controller))
            {
                return (T)controller;

            }
            return default;
        }

        private void ClearCache()
        {
            foreach (var pair in _cacheDict)
            {
                pair.Value.Clear();
            }
            _cacheDict.Clear();
        }

        private void ClearController()
        {
            foreach(var pair in _controllerDict)
            {
                pair.Value.Clear();
            }
            _controllerDict.Clear();
        }

        private string GetTypeKey<T>()
        {
            var typeCls = typeof(T);
            return typeCls.FullName;
        }

        /////////

        public void Clera()
        {
            ClearCache();
            ClearController();
        }


    }
}
