using XLibrary.Package;
using System.Collections.Generic;
using System;
using FairyGUI;
using UnityEngine;


namespace THGame.UI
{
    public class ViewManager : MonoSingleton<ViewManager>
    {
        private Dictionary<Type, ViewInfo> m_viewsMap = new Dictionary<Type, ViewInfo>();
        private Action<ViewInfo> m_onCreated;

        //模态窗口
        public void Open(Type type, object args = null)
        {
            bool isNeedCreate = true;
            bool isNeedAdd = true;
            ViewInfo viewInfo = null;
            if (m_viewsMap.TryGetValue(type, out viewInfo))
            {
                FView view = viewInfo.view;

                if (IsOpened(type))
                {
                    //XXX:置顶到顶层:判断是否被遮挡,通过移除在添加的方式
                    if (GRoot.inst.numChildren > 0)
                    {
                        if (GRoot.inst.GetChildAt(GRoot.inst.numChildren - 1) != view.GetObject())
                        {
                            Close(__GetViewKey(view), false);
                        }
                        else
                        {
                            isNeedAdd = false;
                            isNeedCreate = false;
                        } 
                    }
                }
                
                if (isNeedAdd)
                {
                    if (!view.IsDisposed())
                    {
                        view.__OpenByManager(args);
                        isNeedCreate = false;
                    }
                }
 
            }

            if (isNeedCreate)
            {
                //加载View
                FView.Create(type, (fWidget) =>
                {
                    FView fView = (FView)fWidget;
                    viewInfo = new ViewInfo();
                    viewInfo.view = fView;

                    m_viewsMap[type] = viewInfo;
                    m_onCreated?.Invoke(viewInfo);

                    fView.__OpenByManager(args);
                } ,args);
            }
        }

        public void Open(FView view, object args = null)
        {
            if (view == null)
                return;

            Type type = __GetViewKey(view);
            Open(type, args);
        }

        public void Open<T>(object args = null) where T : FView, new()
        {
            Open(__GetViewKey<T>(), args);
        }

        public void Open(string typeName, object args = null)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                Open(type, args);
            }
            else
            {
                Debug.LogError(string.Format("找不到View:{0}", typeName));
            }
            
        }

        //这里必须调用View
        public void Close(FView view, bool isDisposed = true)
        {
            if (view != null)
            {
                Type type = __GetViewKey(view);
                ViewInfo viewInfo = null;
                if (m_viewsMap.TryGetValue(type, out viewInfo))
                {
                    if (isDisposed)
                    {
                        m_viewsMap.Remove(type);
                    } 
                }
                view.__CloseByManager(isDisposed);
            }
        }

        public void Close(Type type, bool isDisposed = true)
        {
            ViewInfo viewInfo = null;
            if (m_viewsMap.TryGetValue(type, out viewInfo))
            { 
                if (!viewInfo.isPerpetual)  //不可以被Close
                {
                    FView view = viewInfo.view;
                    if (viewInfo.isResident)//常驻View不应该被移除,直接隐藏
                    {
                        isDisposed = false;
                    }

                    view.__CloseByManager(isDisposed);
                }
            }
        }

        public void Close<T>(bool isDisposed = true) where T : FView
        {
            Close(typeof(T), isDisposed);
        }

        public void Close(string typeName, bool isDisposed = true)
        {
            Type type = Type.GetType(typeName);
            Close(type, isDisposed);
        }

        public bool IsOpened(Type type)
        {
            ViewInfo viewInfo = null;
            if (m_viewsMap.TryGetValue(type, out viewInfo))
            {
                FView view = viewInfo.view;
                if (!view.IsDisposed())
                {
                    return view.IsVisible();
                }
            }
            return false;
        }

        public void CloseAll(bool isDisposed = true)
        {
            List<Type> closeLayers = new List<Type>();
            foreach (var mapPairs in m_viewsMap)
            {
                FView view = mapPairs.Value.view;
                closeLayers.Add(__GetViewKey(view));
            }

            foreach(var layerType in closeLayers)
            {
                Close(layerType, isDisposed);
            }
        }

        public void CloseLayers(int minLayer, int maxLayer, bool isDisposed = true)
        {
            List<Type> closeLayers = new List<Type>();
            foreach (var mapPairs in m_viewsMap)
            {
                FView view = mapPairs.Value.view;
                if (view.layerOrder >= minLayer && view.layerOrder <= maxLayer)
                {
                    closeLayers.Add(__GetViewKey(view));
                }
            }

            foreach (var layerType in closeLayers)
            {
                Close(layerType, isDisposed);
            }
        }

        public void CloseLayers(int []layers, bool isDisposed = true)
        {
            List<Type> closeLayers = new List<Type>();
            HashSet<int> layersSet = new HashSet<int>();
            foreach(var layer in layers)
            {
                if(!layersSet.Contains(layer))
                {
                    layersSet.Add(layer);
                }
            }

            foreach(var mapPairs in m_viewsMap)
            {
                FView view = mapPairs.Value.view;
                if (layersSet.Contains(view.layerOrder))
                {
                    closeLayers.Add(__GetViewKey(view));
                }
            }

            foreach (var layerType in closeLayers)
            {
                Close(layerType, isDisposed);
            }
        }

        public void CloseLayer(int layer, bool isDisposed = true)
        {
            CloseLayers(layer, layer, isDisposed);
        }

        public bool IsOpened<T>() where T : FView
        {
            return IsOpened(__GetViewKey<T>());
        }

        public bool IsOpened(string typeName)
        {
            Type type = Type.GetType(typeName);
            return IsOpened(type);
        }

        public ViewInfo GetViewInfo(Type type)
        {
            ViewInfo viewInfo = null;
            if (m_viewsMap.TryGetValue(type, out viewInfo))
            {
                return viewInfo;
            }
            return null;
        }

        public ViewInfo GetViewInfo<T>() where T : FView
        {
            return GetViewInfo(__GetViewKey<T>());
        }

        public ViewInfo GetViewInfo(string typeName)
        {
            Type type = Type.GetType(typeName);
            return GetViewInfo(type);
        }

        public void OnCreated(Action<ViewInfo> func)
        {
            m_onCreated = func;
        }

        ////
        private Type __GetViewKey<T>()
        {
            return typeof(T);
        }

        private Type __GetViewKey(FView view)
        {
            return view.GetType();
        }
    }

}
