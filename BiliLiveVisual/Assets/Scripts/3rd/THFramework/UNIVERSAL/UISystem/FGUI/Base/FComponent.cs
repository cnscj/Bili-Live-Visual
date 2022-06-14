using UnityEngine;
using FairyGUI;
using System.Collections.Generic;
using System;

namespace THGame.UI
{

    public class FComponent : FObject
    {
#if DEBUG
        private FGraph __graph;
#endif
        private FScrollPane __scrollPane;

        private Dictionary<string, FComponent> __children;
        private Dictionary<string, FController> __controllers;
        private Dictionary<string, FTransition> __transitions;

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
                    Debug.LogError(string.Format("{0} => package not found | 没有加载到包", packageName));
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
                        Debug.LogError(string.Format("{0} {1} => component not found | 没有加载到组件", packageName, copmonentName));
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
            __children = __children ?? new Dictionary<string, FComponent>();
            if (!__children.TryGetValue(name, out FComponent fComp))
            {
                GObject obj = this._obj.asCom.GetChild(name);
                fComp = FComponent.Create<T>(obj);
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
            foreach (var child in children)
            {
                FComponent fObj = FComponent.Create<FComponent>(child);
                _childList.Add(fObj);
            }
            return _childList.ToArray();
        }
        //

        public void DebugUI()
        {
#if DEBUG
            var size = GetSize();
            if (__graph != null)
            {
                __graph.Dispose();
                __graph = null;
            }

            if (__graph == null)
            {
                __graph = new FGraph().InitWithObj(new GGraph()) as FGraph;
                __graph.DrawRect(size.x, size.y, 5, new Color(0xff,0x00,0x00,0xff),new Color(0x00,0x00,0x00,0x00));
                __graph.SetTouchable(false);

                AddChild(__graph);
            }
#endif
        }
        //
        public virtual void AddChild(FComponent comp)
        {
            _obj.asCom.AddChild(comp.GetObject());
            __children = __children ?? new Dictionary<string, FComponent>();
            __children[comp.GetName()] = comp;
        }
        public virtual void AddChildAt(FComponent comp, int idx)
        {
            _obj.asCom.AddChildAt(comp.GetObject(), idx);
            __children = __children ?? new Dictionary<string, FComponent>();
            __children[comp.GetName()] = comp;
        }

        public virtual void RemoveChild(FComponent comp, bool isDisposed = false)
        {
            _obj.asCom.RemoveChild(comp.GetObject(), isDisposed);
            __children = __children ?? new Dictionary<string, FComponent>();
            __children.Remove(comp.GetName());
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
            __controllers = __controllers ?? new Dictionary<string, FController>();
            if (!__controllers.TryGetValue(name, out FController fCtrl))
            {
                Controller ctrl = _obj.asCom.GetController(name);
                fCtrl = FController.Create(ctrl);
            }
            return fCtrl;
        }

        public FTransition GetTransition(string name)
        {
            __transitions = __transitions ?? new Dictionary<string, FTransition>();
            if (!__transitions.TryGetValue(name, out FTransition fTrans))
            {
                Transition trans = _obj.asCom.GetTransition(name);
                fTrans = FTransition.Create(trans);
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
