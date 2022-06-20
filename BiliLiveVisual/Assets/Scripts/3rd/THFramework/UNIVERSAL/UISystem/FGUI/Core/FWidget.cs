﻿using FairyGUI;
using System;
using System.Collections.Generic;
using XLibGame;
using EventCallback1 = XLibGame.EventCallback1;
using EventDispatcher = XLibGame.EventDispatcher;

namespace THGame.UI
{
    public class FWidget : FComponent
    {
        public string package { get; protected set; }
        public string component { get; protected set; }

        protected object _args;                 //参数
        protected bool _isAsync;                //是否异步加载
        protected float _interval = -1.0f;

        private int __scheduler = -1;
        private bool __isDisposed;
        private bool __isCreating;
        private List<Tuple<IComparable, EventCallback1>> __listeners;

        //不受ViewManager管理
        public static FWidget Create(Type cls, Action<FWidget> callback ,object args = null)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            FWidget widget = asm.CreateInstance(cls.FullName) as FWidget;
            if (widget != null)
            {
                string packageName = widget.package;
                string componentName = widget.component;

                widget.__isCreating = true;
                FComponent.Create<FComponent>(packageName, componentName, widget._isAsync, (fComponent) =>
                {
                    widget.__isCreating = false;
                    if (widget.__isDisposed)
                    {
                        widget.__isDisposed = false;
                        fComponent.Dispose();
                        return;
                    }
                    OnCreateSuccess(fComponent.GetObject(), widget, args);
                    callback?.Invoke(widget);
                });
            }

            return widget;
        }

        public static T Create<T>(FWidget parent = null, object args = null) where T : FWidget, new()
        {
            T view = Create(typeof(T), null, args) as T;
            if (parent != null)
            {
                parent.AddChild(view);
            }
            return view;
        }

        private static void OnCreateSuccess(GObject obj, FWidget widget, object args = null)
        {
            if (obj == null)
            {
                return;
            }

            widget.SetArgs(args);
            widget.InitWithObj(obj);

            RetainPackage(widget);
        }

        private static void RetainPackage(FWidget widget)
        {
            if (widget != null )
            {
                UIPackageManager.GetInstance().RetainPackage(widget.package);
            }
        }

        private static void ReleasePackage(FWidget widget)
        {
            if (widget != null)
            {
                UIPackageManager.GetInstance().ReleasePackage(widget.package);
            }
        }

        public void AddEventListener(int eventId, EventCallback1 listener)
        {
            EventDispatcher.GetInstance().AddListener(eventId, listener);
            __listeners = (__listeners != null) ? __listeners : new List<Tuple<IComparable, EventCallback1>>();
            __listeners.Add(new Tuple<IComparable, EventCallback1>(eventId, listener));
        }

        public FWidget(string packageName = null, string componentName = null)
        {
            package = packageName;
            component = componentName;

            if (!string.IsNullOrEmpty(packageName))
            {
                var emptyObj = new GComponent();
                if (!string.IsNullOrEmpty(componentName))
                {
                    emptyObj.name = componentName;
                    emptyObj.gameObjectName = componentName;
                }
                
                InitWithObj(emptyObj);
            }
        }

        //
        public override void Dispose()
        {
            if (_obj == null)
            {
                __isDisposed = true;
            }
            else
            {
                base.Dispose();
                ReleasePackage(this);
            }
        }

        public bool IsCreated()
        {
            return _obj != null;
        }
        public bool IsCreating()
        {
            return __isCreating;
        }
        //
        public void SetArgs(object args)
        {
            _args = args;
        }

        public object GetArgs()
        {
            return _args;
        }

        //
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

        protected virtual void OnTick()
        {

        }

        //
        protected virtual void _OnAddedToStage()
        {
            OnInitEvent();
            _InitScheduler();

            OnEnter();
        }

        protected virtual void _OnRemovedFromStage()
        {
            _RemoveEvent();
            _RemoveSchedule();

            OnExit();
        }

        private void _InitScheduler()
        {
            if (_interval >= 0f)
            {
                __scheduler = Timer.GetInstance().Schedule(OnTick, _interval);
            }

        }

        private void _RemoveEvent()
        {
            if (__listeners != null)
            {
                foreach (var pair in __listeners)
                {
                    EventDispatcher.GetInstance().RemoveListener(pair.Item1, pair.Item2);
                }
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

        ///
        protected override void OnInitObj(GObject obj)
        {
            base.OnInitObj(obj);
            if (obj != null)
            {
                OnInitUI();

                obj.onAddedToStage.Clear();
                obj.onRemovedFromStage.Clear();
                obj.onAddedToStage.Add(_OnAddedToStage);
                obj.onRemovedFromStage.Add(_OnRemovedFromStage);
                
            }
        }
    }

}
