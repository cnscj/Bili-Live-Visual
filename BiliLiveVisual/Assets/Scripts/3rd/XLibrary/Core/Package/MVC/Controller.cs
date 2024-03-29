
using System;
using System.Collections.Generic;
using XLibGame;

namespace XLibrary.Package.MVC
{
    public class Controller
    {
        private List<Tuple<IComparable, EventCallback1>> __listeners;

        public static T Get<T>() where T : Controller, new()
        {
            return MVCManager.GetInstance().GetController<T>();
        }

        public void Initilize()
        {
            Clear();
            OnAdd();
            OnInitEvent();

        }

        public void AddEventListener(IComparable eventId, EventCallback1 listener)
        {
            EventDispatcher.GetInstance().AddListener(eventId, listener);
            __listeners = (__listeners != null) ? __listeners : new List<Tuple<IComparable, EventCallback1>>();
            __listeners.Add(new Tuple<IComparable, EventCallback1>(eventId, listener));
        }

        public void Clear()
        {
            if (__listeners != null)
            {
                foreach (var evt in __listeners)
                {
                    EventDispatcher.GetInstance().RemoveListener(evt.Item1, evt.Item2);
                }
                __listeners.Clear();
            }
            OnClear();
        }

        public void Dispose()
        {
            Clear();
            OnRemove();
        }

        //////////////////////////
        protected virtual void OnAdd()
        {

        }

        protected virtual void OnRemove()
        {

        }
        protected virtual void OnInitEvent()
        {

        }

        protected virtual void OnClear()
        {

        }
    }
}
