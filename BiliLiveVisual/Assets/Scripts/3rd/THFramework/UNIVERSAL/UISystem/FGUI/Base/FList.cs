using FairyGUI;
using System.Collections.Generic;
using System;

namespace THGame.UI
{

    public class FList : FComponent
    {
        protected List<object> _dataProvider = new List<object>();
        protected Dictionary<GObject,FComponent> _dataTemplate = new Dictionary<GObject, FComponent>();

        public delegate string ItemProvideFunc(object data, int index);
        public delegate void ItemStateFuncT0(int index, object data, FComponent comp);
        public delegate void ItemStateFuncT1<T1>(int index, T1 data, FComponent comp);
        public delegate void ItemStateFuncT2<T1,T2>(int index, T1 data, T2 comp) where T1 : new() where T2: FComponent, new();


        // 设置虚拟列表
        public void SetVirtual()
        {
            _obj.asList.SetVirtual();
        }

        // 循环列表只支持单行或者单列的布局，不支持流动布局和分页布局。
        public void SetLoop()
        {
            _obj.asList.SetVirtualAndLoop();
        }

        // 当点击某个item时，如果这个item处于部分显示状态，那么列表将会自动滚动到整个item显示完整。
        public void ScrollItemToViewOnClick(bool scrollItemToViewOnClick)
        {
            _obj.asList.scrollItemToViewOnClick = scrollItemToViewOnClick;
        }

        // 自动大小
        public void SetAutoResizeItem(bool autoResizeItem)
        {
            _obj.asList.autoResizeItem = autoResizeItem;
        }

        //////////////////////////////////////////////////
        public void OnClickItem(EventCallback0 func)
        {
            _obj.asList.onClickItem.Set(func);
        }
        public void OnClickItem(EventCallback1 func)
        {
            _obj.asList.onClickItem.Set(func);
        }

        public void AddClickItem(EventCallback1 func)
        {
            _obj.asList.onClickItem.Add(func);
        }

        public void RemoveClickItem(EventCallback1 func)
        {
            _obj.asList.onClickItem.Remove(func);
        }

        // 原生itemRenderer
        public void ItemRenderer(ListItemRenderer func)
        {
            _obj.asList.itemRenderer = func;
        }

        // 设置多样式虚拟列表
        public void ItemProvider(ItemProvideFunc func)
        {
            _obj.asList.itemProvider = new ListItemProvider((index) =>
            {
                return func(_dataProvider[index], index);
            });
        }

        public void SetState<T1,T2>(ItemStateFuncT2<T1,T2> func) where T1 : new() where T2 : FComponent, new()
        {
            _obj.asList.itemRenderer = new ListItemRenderer((index,obj) =>
            {
                FComponent fComp = null;
                if (!_dataTemplate.TryGetValue(obj, out fComp))
                {
                    fComp = FComponent.Create<T2>(obj);
                    _dataTemplate[obj] = fComp;
                }
                func?.Invoke(index, (T1)_dataProvider[index], (T2)fComp);
            });
        }
        public void SetState<T1>(ItemStateFuncT1<T1> func) where T1 : new()
        {
            SetState<T1, FComponent>((index,data,comp) =>
            {
                func?.Invoke(index, data, comp);
            });
        }
        public void SetState(ItemStateFuncT0 func)
        {
            SetState<object, FComponent>((index, data, comp) =>
            {
                func?.Invoke(index, data, comp);
            });
        }

        public Dictionary<GObject, FComponent> GetDataTemplate()
        {
            return _dataTemplate;
        }

        public void SetDataProvider<T>(List<T> array) where T : new()
        {
            if (array != null)
            {
                _dataProvider = array.ConvertAll(s => (object)s);
            }
            else
            {
                _dataProvider.Clear();
            }
            SetNumItems(_dataProvider.Count);
        }

        public List<T> GetDataProvider<T>() where T : new()
        {
            return _dataProvider.ConvertAll(s => (T)s);
        }

        //刷新列表
        public void RefreshVirtualList()
        {
            _obj.asList.RefreshVirtualList();
        }

        //重新设置长度大小
        public void ResizeToFit(int count = -1)
        {
            if (count < 0)
            {
                count = _obj.asList.numItems;
            }
            _obj.asList.ResizeToFit(count);
        }


        //////////////////////////////////////////////////

        // 移动scroll
        public void ScrollToView(int index, bool isAction = false)
        {
            if (index < 1)
            {
                index = 1;
            }
            _obj.asList.ScrollToView(index - 1, isAction);
        }

        public void ScrollToTop(bool isAction = false)
        {
            ScrollToView(1, isAction);
        }

        public void ScrollToBottom(bool isAction = false)
        {
            ScrollToView(_dataProvider.Count, isAction);
        }


        // 设置数量
        public void SetNumItems(int num)
        {
            _obj.asList.numItems = num;
        }
        public int GetNumItems()
        {
            return _obj.asList.numItems;
        }


        // 获取当前选中的data，list里面的item是单选按钮才生效
        public object GetSelectedData()
        {
            var index = GetSelectedIndex();
            if (index >= 0)
            {
                return _dataProvider[index];
            }
            return null;
        }

        // 获取当前选中第几个，list里面的item是单选按钮才生效
        public int GetSelectedIndex()
        {
            return _obj.asList.selectedIndex;
        }

        // //转换项目索引为显示对象索引。
        // int childIndex = aList.ItemIndexToChildIndex(1);
        // //转换显示对象索引为项目索引。
        // int itemIndex = aList.ChildIndexToItemIndex(1);
        public void SetSelectedKey(string key, bool click)
        {
            int index = 1;
            for(int i = 0; i < _dataProvider.Count; ++i)
            {
                object v = _dataProvider[i];
                Type Ts = v.GetType();
                object o = Ts.GetProperty("key").GetValue(v, null);
                string value = Convert.ToString(o);
                if (string.IsNullOrEmpty(value))
                {
                    if (value == key)
                    {
                        index = i;
                        break;
                    }
                }
            }
            SetSelectedIndex(index, click);
        }

        // 选中第index个，并且触发点击事件，list里面的item是单选按钮才生效
        public void SetSelectedIndex(int index, bool click)
        {
            if (_obj.asList.numChildren == 0)
            {
                return;
            }

            int realIndex = index;
            if (click)
            {
                // 先判断是否在显示列表
                int childIndex = _obj.asList.ItemIndexToChildIndex(realIndex);
                if (childIndex >= 0 && childIndex < _obj.asList.numChildren)
                {
                    // 如果index在显示列表中，则不需要scrollItToView，最后再点击
                    _obj.asList.AddSelection(realIndex, false);
                    _obj.asList.onClickItem.Call(_obj.asList.GetChildAt(childIndex));
                }
                else
                {
                    // 如果index不在显示列表中，则需要scrollItToView，才能找对显示对象，最后再点击
                    _obj.asList.AddSelection(realIndex, true);
                    var curIndex = _obj.asList.ItemIndexToChildIndex(realIndex);
                    _obj.asList.onClickItem.Call(_obj.asList.GetChildAt(curIndex));
                }

            }
            else
            {
                AddSelection(realIndex);
            }
        }

        public void AddSelection(int index,bool scrollItToView = false)
        {
            _obj.asList.AddSelection(index, scrollItToView);
        }

        // 获取全部选中
        public List<int> GetSelection()
        {
            return _obj.asList.GetSelection();
        }

        // 取消某个选中
        public void RemoveSelection(int index)
        {
            _obj.asList.RemoveSelection(index);
        }

        // 取消全部选择
        public void ClearSelection()
        {
            _obj.asList.ClearSelection();
        }

        //强制刷新当前选中的项 这个选择是有bug的
        public void ForceRefreshSelectedItem(int index)
        {
            int selectedIndex = -1;
            if (index >= 0)
            {
                selectedIndex = index;
            } else
            {
                selectedIndex = _obj.asList.selectedIndex;
            }


            var childIndex = _obj.asList.ItemIndexToChildIndex(selectedIndex);
            if (childIndex >= 0 && childIndex<_obj.asList.numChildren)
            {
                var item = _obj.asList.GetChildAt(childIndex);
                _obj.asList.itemRenderer(selectedIndex, item);
            }
        }

        //////////////////////////////////////////////////
        public void OnPullDownRelease(EventCallback1 func)
        {
            _obj.asList.scrollPane.onPullDownRelease.Add(func);
        }

        public void OnPullUpRelease(EventCallback1 func)
        {
            _obj.asList.scrollPane.onPullUpRelease.Add(func);
        }

        public FComponent GetHeader()
        {
            return GetScrollPane().GetHeader();
        }

        public FComponent GetFooter()
        {
            return GetScrollPane().GetFooter();
        }

        public int ItemIndexToChildIndex(int index)
        {
            return _obj.asList.ItemIndexToChildIndex(index);
        }

        public void SetSelectionMode(ListSelectionMode mode)
        {
            _obj.asList.selectionMode = mode;
        }

        public ListSelectionMode GetSelectionMode()
        {
            return _obj.asList.selectionMode;
        }

        public GObject GetSelectedNode(int index)
        {
            index = index >=0 ? index : GetSelectedIndex();
            int childIndex = _obj.asList.ItemIndexToChildIndex(index);

            if (childIndex >= 0 && childIndex<_obj.asList.numChildren)
            {
                var item = _obj.asList.GetChildAt(childIndex);
                return item;
            }
            return null;
        }

        public FComponent GetSelectionComp(int index)
        {
            var item = GetSelectedNode(index);
            return _dataTemplate[item];
        }

        // 获取某个索引的组件
        public FComponent GetCompByIndex(int index)
        {
            int childIndex = _obj.asList.ItemIndexToChildIndex(index);
            if (childIndex >= 0 && childIndex < _obj.asList.numChildren)
            {
                var obj = _obj.asList.GetChildAt(childIndex);
                return _dataTemplate[obj];
            }
            return null;
        }

        // 返回当前滚动位置是否在最下边
        public bool IsBottomMost()
        {
            return _obj.asList.scrollPane.isBottomMost;
        }

        // 在滚动结束时派发该事件。
        public void OnScrollEnd(EventCallback1 func)
        {
            _obj.asList.scrollPane.onScrollEnd.Add(func);
        }

        public void OnScroll(EventCallback1 func)
        {
            _obj.asList.scrollPane.onScroll.Add(func);
        }

        public void RemoveOnScroll(EventCallback1 func)
        {
            _obj.asList.scrollPane.onScroll.Remove(func);
        }

        public void SetColumnCount(int count)
        {
            _obj.asList.columnCount = count;
        }

        public void SetLineCount(int count)
        {
            _obj.asList.lineCount = count;
        }

        public int GetColumnGap()
        {
            return _obj.asList.columnGap;
        }

        public int GetLineGap()
        {
            return _obj.asList.lineGap;
        }

        public int GetNumChildren()
        {
            return _obj.asList.numChildren;
        }

        //加一层遮罩,List含3d物体时用
        public void SetStencil()
        {
            SetVirtual();
            // 弄一个组件和list一样大
            // 弄一个shape当做遮罩
            // 把这个list放入这个组件
            var container = FComponent.Create<FComponent, GComponent>();
            var graph = FComponent.Create<FGraph, GGraph>();
            var parent = this.GetParent();

            //初始化
            container.SetXY(this.GetXY());

            graph.SetXY(0, 0);
            graph.SetSize(this.GetSize());

            //
            this.RemoveFromParent();
            this.SetXY(0, 0);

            //
            parent.AddChild(container);
            container.AddChild(graph);
            container.AddChild(this);

            graph.AddRelation(this, FairyGUI.RelationType.Size);
            container.GetObject().asCom.mask = graph.GetObject().displayObject;
        }
    }
}
