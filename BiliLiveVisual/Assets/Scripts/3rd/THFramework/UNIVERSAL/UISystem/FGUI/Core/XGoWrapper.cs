using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class XGoWrapper : GoWrapper
    {
        private Dictionary<string, GoBaseUpdater> _updaters;
        private GoUpdateContext _updateContext;

        public T AddUpdater<T>() where T : GoBaseUpdater, new()
        {
            var updaterKey = GetUpdaterKey<T>();
            if (_updaters != null)
            {
                if (_updaters.ContainsKey(updaterKey))
                {
                    return _updaters[updaterKey] as T;
                }
            }

            T iUpdater = new T();
            if (iUpdater == null)
                return default;

            var updateContext = GetOrCreateContext();
            updateContext.goWrapper = this;

            iUpdater.context = updateContext;
            GetUpdaters().Add(updaterKey, iUpdater);
            iUpdater.OnAdded();

            return iUpdater;
        }

        public T GetUpdater<T>() where T : GoBaseUpdater
        {
            if (_updaters == null)
                return default;

            var updaterKey = GetUpdaterKey<T>();
            if (_updaters.TryGetValue(updaterKey, out var iUpdater))
            {
                return iUpdater as T;
            }
            return default;
        }

        public void RemoveUpdater<T>() where T : GoBaseUpdater
        {
            if (_updaters == null)
                return;

            var updaterKey = GetUpdaterKey<T>();
            if (_updaters.TryGetValue(updaterKey, out var iUpdater))
            {
                iUpdater.OnRemove();
                iUpdater.context = null;
            }
            GetUpdaters().Remove(updaterKey);

        }

        public void RemoveAllUpdater()
        {
            if (_updaters == null)
                return;

            foreach (var iUpdater in _updaters.Values)
            {
                iUpdater.OnRemove();
                iUpdater.context = null;
            }
            _updaters.Clear();
        }

        public new void CacheRenderers()
        {
            base.CacheRenderers();

            if (_updaters == null)
                return;

            foreach (var iUpdater in _updaters.Values)
            {
                iUpdater.OnRefresh();
            }
        }

        override public void Update(UpdateContext context)
        {
            UpdateContext(context);
            base.Update(context);
        }

        public override void Dispose()
        {
            RemoveAllUpdater();
            base.Dispose();
        }

        virtual public void Reset()
        {
            if (_updaters == null || _updaters.Count < 0)
                return;

            foreach (var updater in _updaters.Values)
            {
                updater.OnReset();
            }
        }

        private void UpdateContext(UpdateContext context)
        {
            if (_updaters == null || _updaters.Count < 0)
                return;

            var updateContext = GetOrCreateContext();
            var oldGameObejct = updateContext.wrapperTarget;
            var newGameObject = wrapTarget;

            updateContext.wrapperTarget = wrapTarget;
            updateContext.wrapperContext = context;

            foreach (var updater in _updaters.Values)
            {
                if (oldGameObejct != newGameObject)
                {
                    updater.OnReplace(oldGameObejct, newGameObject);
                }
                updater.OnUpdate();
            }
        }

        private GoUpdateContext GetOrCreateContext()
        {
            _updateContext = _updateContext ?? new GoUpdateContext();
            return _updateContext;
        }

        private string GetUpdaterKey<T>()
        {
            var type = typeof(T);
            return type.FullName;
        }

        private Dictionary<string, GoBaseUpdater> GetUpdaters()
        {
            _updaters = _updaters ?? new Dictionary<string, GoBaseUpdater>();
            return _updaters;
        }

    }
}
