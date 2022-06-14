using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class FViewTabBar : FView
    {
        public class ViewParams
        {
            public Type cls;
            public string key;
            public string title;
            public object args;
        }

        public class ViewInfo
        {
            public FView view;
            public ViewParams viewParams;

        }
        public static readonly string barListName = "barList";

        protected FList _barList;
        protected List<ViewParams> _layers;
        protected Dictionary<int, ViewInfo> _children;

        private int __preIndex = -1;


        public FViewTabBar(string package,string component):base(package, component)
        {
        }

        public override void Close(bool isDisposed = true)
        {
            if (_children != null)
            {
                foreach(var pair in _children)
                {
                    var viewInfo = pair.Value;
                    if (viewInfo.view != null)
                    {
                        viewInfo.view.Close(isDisposed);
                    }
                }
                _children.Clear();
                _children = null;
            }
            base.Close(isDisposed);
        }

        private void __InitBarList()
        {
            _barList = GetChild<FList>(barListName);
            if (_barList != null)
            {
                _barList.SetVirtual();

                _barList.SetState((index, data, comp) =>
                {
                    var viewParams = data as ViewParams;
                    var title = comp.GetChild<FRichText>("title");
                    title.SetText(viewParams.title);
                });

                _barList.AddClickItem((context) =>
                {
                    _children = (_children != null) ? _children : new Dictionary<int, ViewInfo>();

                    var data = _barList.GetSelectedData() as ViewParams;
                    var index = _barList.GetSelectedIndex();

                    if (index == __preIndex)
                    {
                        return;
                    }

                    ViewInfo preViewInfo = null;
                    if (_children.TryGetValue(__preIndex, out preViewInfo))
                    {
                        var preView = preViewInfo.view;
                        if (preView != null)
                        {
                            preView.RemoveFromParent();
                        }
                    }

                    var curIndex = index;
                    __preIndex = curIndex;

                    bool isNeedCreate = true;
                    ViewInfo curViewInfo = null;
                    if (_children.TryGetValue(curIndex, out curViewInfo))
                    {
                        var curView = curViewInfo.view;
                        if (curView != null)
                        {
                            if (!curView.IsDisposed())
                            {
                                AddChild(curView);
                                isNeedCreate = false;
                            }
                        }
                    }

                    if (isNeedCreate)
                    {
                        var newData = _barList.GetSelectedData() as ViewParams;
                        var newIndex = _barList.GetSelectedIndex();
                        if (curIndex != newIndex)
                        {
                            return;
                        }
                        
                        FView.Create(newData.cls, (fWidget) =>
                        {
                            var fView = (FView)fWidget;
                            ViewInfo newViewInfo = new ViewInfo();
                            newViewInfo.view = fView;
                            newViewInfo.viewParams = newData;

                            _children[curIndex] = newViewInfo;

                            AddChild(fView);

                        },newData.args);

                    }

                });
            }
        }
        private void __InitLayerStack()
        {
            OnInitTabBar();

            if (_layers != null && _barList != null)
            {
                if (_children != null)
                {
                    _children.Clear();
                }
                _barList.SetDataProvider(_layers);

                _barList.SetSelectedIndex(0, true);
                _barList.ScrollToView(_barList.GetSelectedIndex());
            }
        }


        //
        protected virtual void OnInitTabBar()
        {
            
        }


        public override Wrapper<GObject> InitWithObj(GObject obj)
        {
            SetObject(obj);
            __InitBarList();
            __InitLayerStack();
            base.InitWithObj(obj);

            return this;
        }
    }

}
