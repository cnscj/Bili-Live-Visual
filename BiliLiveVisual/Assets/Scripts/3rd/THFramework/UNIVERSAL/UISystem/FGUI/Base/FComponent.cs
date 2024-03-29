﻿using UnityEngine;
using FairyGUI;
using System.Collections.Generic;
using System;

namespace THGame.UI
{

    public class FComponent : FObject
    {
        private FScrollPane __scrollPane;

        private Dictionary<object, FComponent> __children;
        private Dictionary<object, FController> __controllers;
        private Dictionary<object, FTransition> __transitions;

        public static new FComponent Create(GObject gObj)
        {
            return Create<FComponent>(gObj);
        }

        public static T Create<T>(string packageName, string copmonentName, bool isAsync = false, Action<FComponent> callback = null) where T : FComponent, new()
        {
            if (string.IsNullOrEmpty(packageName))
                return default;

            //加载包
            if (UIPackageManager.GetInstance().GetPackageInfo(packageName) == null)
            {
                var packageInfo = UIPackageManager.GetInstance().AddPackage(packageName);

                if (packageInfo == null)
                {
                    Debug.LogError(string.Format("{0} | 没有加载到包", packageName));
                    return default;
                }
            }

            var fObj = Create<T>(null);
            if (isAsync)
            {
                UIPackage.CreateObjectAsync(packageName, copmonentName, (gObj) =>
                {
                    if (gObj == null)
                    {
                        Debug.LogError(string.Format("{0} {1} | 没有加载到组件或组件未导出", packageName, copmonentName));
                        return;
                    }
                    fObj.InitWithObj(gObj);
                    callback?.Invoke(fObj);
                });
            }
            else
            {
                GObject gObj = UIPackage.CreateObject(packageName, copmonentName);
                if (gObj == null)
                {
                    Debug.LogError(string.Format("{0} {1} => component not found | 没有加载到组件", packageName, copmonentName));
                    return default;
                }
                fObj.InitWithObj(gObj);
                callback?.Invoke(fObj);
            }

            return fObj;
        }

        public T GetChild<T>(string name) where T : FComponent, new()
        {
            GObject gObj = this._obj.asCom.GetChild(name);
            FComponent fComp = null;
            if (gObj != null)
            {
                __children = __children ?? new Dictionary<object, FComponent>();
                if (!__children.TryGetValue(gObj, out fComp))
                {
                    fComp = FComponent.Create<T>(gObj);
                    __children[gObj] = fComp;
                }
            }
            return fComp as T;
        }

        public FComponent GetChild(string name)
        {
            return GetChild<FComponent>(name);
        }

        public FComponent[] GetChildren()
        {
            List<FComponent> _childList = new List<FComponent>();
            GObject[] children = _obj.asCom.GetChildren();
            foreach (var gObj in children)
            {
                FComponent fComp;
                if (!__children.TryGetValue(gObj, out fComp))
                {
                    fComp = FComponent.Create<FComponent>(gObj);
                    __children[gObj] = fComp;
                }

                _childList.Add(fComp);
            }
            return _childList.ToArray();
        }
        //
        public virtual void AddChild(FComponent comp)
        {
            if (comp.IsDisposed())
                return;

            _obj.asCom.AddChild(comp.GetObject());
            __children = __children ?? new Dictionary<object, FComponent>();
            __children[comp.GetObject()] = comp;
        }
        public virtual void AddChildAt(FComponent comp, int idx)
        {
            if (comp.IsDisposed())
                return;

            _obj.asCom.AddChildAt(comp.GetObject(), idx);
            __children = __children ?? new Dictionary<object, FComponent>();
            __children[comp.GetObject()] = comp;
        }

        public virtual void RemoveChild(FComponent comp, bool isDisposed = false)
        {
            _obj.asCom.RemoveChild(comp.GetObject(), isDisposed);
            __children = __children ?? new Dictionary<object, FComponent>();
            __children.Remove(comp.GetObject());
        }
        public virtual void RemoveChildren()
        {
            _obj.asCom.RemoveChildren();
            __children?.Clear();
        }

        public virtual void RemoveAllChildren(bool isDisposed = false)
        {
            _obj.asCom.RemoveChildren(0, -1, isDisposed);
            __children?.Clear();
        }

        public void SetChildIndexBefore(FComponent child, int index)
        {
            _obj.asCom.SetChildIndexBefore(child.GetObject(), index);
        }

        public void SetChildIndex(FComponent child, int index)
        {
            _obj.asCom.SetChildIndex(child.GetObject(), index);
        }

        public int GetChildIndex(FComponent child)
        {
            return _obj.asCom.GetChildIndex(child.GetObject());
        }

        // controller
        public FController GetController(string name)
        {
            Controller gCtrl = _obj.asCom.GetController(name);
            FController fCtrl = null;
            if (gCtrl != null)
            {
                __controllers = __controllers ?? new Dictionary<object, FController>();
                if (!__controllers.TryGetValue(gCtrl, out fCtrl))
                {
                    fCtrl = FController.Create(gCtrl);
                    __controllers[gCtrl] = fCtrl;
                }
            }
            return fCtrl;
        }

        public FTransition GetTransition(string name)
        {
            Transition gTrans = _obj.asCom.GetTransition(name);
            FTransition fTrans = null;
            if (gTrans != null)
            {
                __transitions = __transitions ?? new Dictionary<object, FTransition>();
                if (!__transitions.TryGetValue(gTrans, out fTrans))
                {
                    fTrans = FTransition.Create(gTrans);
                    __transitions[gTrans] = fTrans;
                }
            }
            return fTrans;
        }
        public FScrollPane GetScrollPane()
        {
            var obj = _obj.asCom.scrollPane;
            __scrollPane = (obj != null) ? (__scrollPane != null ? __scrollPane.InitWithObj(obj) as FScrollPane : FScrollPane.Create(obj)) : null;
            return __scrollPane;
        }

        //
        // 接收拖放之后的【放】
        public void OnDrop(EventCallback1 func)
        {
            _obj.asCom.onDrop.Add(func);
        }

        public void SetZIndex(int index)
        {
            var oldIndex = GetZIndex();
            if (oldIndex >= index)
            {
                GetParent().SetChildIndexBefore(this, index);
            }
            else
            {
                GetParent().SetChildIndex(this, index);
            }
        }

        public int GetZIndex()
        {
            return GetParent().GetChildIndex(this);
        }

        public void SetViewHeight(float height)
        {
            _obj.asCom.viewHeight = height;
        }
        public float GetViewHeight()
        {
            return _obj.asCom.viewHeight;
        }

    }

}
