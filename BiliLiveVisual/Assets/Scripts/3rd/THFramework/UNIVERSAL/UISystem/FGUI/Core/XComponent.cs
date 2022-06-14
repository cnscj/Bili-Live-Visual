using System;
using System.Collections.Generic;
using FairyGUI;
using FairyGUI.Utils;
using XLibGame;
using EventCallback1 = XLibGame.EventCallback1;
using EventDispatcher = XLibGame.EventDispatcher;
namespace THGame.UI
{
    //用于带Dispose的扩展组件
    public class XComponent : GComponent
    {
        protected float _interval = -1.0f;

        private int __scheduler = -1;
        private List<Tuple<int, EventCallback1>> __listener;

        public override void Dispose()
        {
            base.Dispose();
            OnDispose();
        }

        public override void ConstructFromXML(XML xml)
        {
            OnInitUI();

            onAddedToStage.Clear();
            onRemovedFromStage.Clear();
            onAddedToStage.Add(_OnAddedToStage);
            onRemovedFromStage.Add(_OnRemovedFromStage);
        }

        public void AddEventListener(int eventId, EventCallback1 listener)
        {
            EventDispatcher.GetInstance().AddListener(eventId, listener);
            __listener = (__listener != null) ? __listener : new List<Tuple<int, EventCallback1>>();
            __listener.Add(new Tuple<int, EventCallback1>(eventId, listener));
        }

        protected virtual void OnInitUI()
        {

        }

        protected virtual void OnInitEvent()
        {

        }

        protected virtual void OnEnter()
        {

        }

        protected virtual void OnExit()
        {

        }

        protected virtual void OnDispose()
        {

        }

        protected virtual void OnTick()
        {

        }
        ///////
        private void _InitScheduler()
        {
            if (_interval >= 0f)
            {
                __scheduler = Timer.GetInstance().Schedule(OnTick, _interval);
            }

        }

        private void _RemoveSchedule()
        {
            if (__scheduler != -1)
            {
                Timer.GetInstance().Unschedule(__scheduler);
                __scheduler = -1;
            }
        }

        private void _OnAddedToStage()
        {
            OnInitEvent();
            _InitScheduler();

            OnEnter();
        }
  
        private void _OnRemovedFromStage()
        {
            _RemoveEvent();
            _RemoveSchedule();
            OnExit();
        }

        private void _RemoveEvent()
        {
            if (__listener != null)
            {
                foreach (var pair in __listener)
                {
                   EventDispatcher.GetInstance().RemoveListener(pair.Item1, pair.Item2);
                }
            }
        }
    }

}
