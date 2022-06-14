
using System;
using System.Collections.Generic;
using XLibGame;

namespace XLibrary.Package.MVC
{
    public class Controller
    {
        private List<Tuple<IComparable, EventCallback1>> __listeners;

        public void Initilize()
        {
            Clear();
            OnInitEvent();

        }

        public void AddEventListener(IComparable eventId, EventCallback1 listener)
        {
            EventDispatcher.GetInstance().AddListener(eventId, listener);
            __listeners = (__listeners != null) ? __listeners : new List<Tuple<IComparable, EventCallback1>>();
            __listeners.Add(new Tuple<IComparable, EventCallback1>(eventId, listener));
        }

        public virtual void Clear()
        {
            if (__listeners != null)
            {
                foreach (var evt in __listeners)
                {
                    EventDispatcher.GetInstance().RemoveListener(evt.Item1, evt.Item2);
                }
                __listeners.Clear();
            }

        }

        //////////////////////////
        protected virtual void OnInitEvent()
        {

        }
    }
}
