using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
namespace THGame.UI
{
    public abstract class GoBaseUpdater
    {
        public GoUpdateContext context;

        public virtual void OnAdded() { }
        public virtual void OnRemove() { }
        public virtual void OnReplace(GameObject oldGameObject, GameObject newGameObject) { }
        public virtual void OnUpdate() { }
        public virtual void OnReset() { }
        public virtual void OnRefresh() { }

        public T AddUpdater<T>() where T : GoBaseUpdater, new()
        {
            return context?.goWrapper?.AddUpdater<T>();
        }
        public T GetUpdater<T>() where T : GoBaseUpdater
        {
            return context?.goWrapper?.GetUpdater<T>();
        }
        public void RemoveUpdater<T>() where T : GoBaseUpdater
        {
            context?.goWrapper?.RemoveUpdater<T>();
        }
    }

}