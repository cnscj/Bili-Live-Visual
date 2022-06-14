using UnityEngine;
using FairyGUI;

namespace THGame.UI
{
    public class FObject : Wrapper<GObject>
    {
        private FComponent __parent;

        public static FObject Create(GObject obj)
        {
            return ObjectManager.GetInstance().GetOrCreate(obj);
        }

        public static T Create<T>(GObject obj) where T : FObject, new()
        {
            return ObjectManager.GetInstance().GetOrCreate<T>(obj);
        }

        public static T1 Create<T1, T2>() where T1 : FObject, new() where T2 : GObject, new()
        {
            T2 gObj = new T2();
            T1 fObj = new T1();

            fObj.InitWithObj(gObj);
            ObjectManager.GetInstance().Add(gObj,fObj);

            return fObj;
        }

        public string GetName()
        {
            return _obj.name;
        }

        public FComponent GetParent()
        {
            var obj = _obj.parent;
            __parent = (obj != null) ? (__parent != null ? __parent.InitWithObj(obj) as FComponent : new FComponent().InitWithObj(obj) as FComponent) : null;
            return __parent;
        }

        public virtual void RemoveFromParent()
        {
            _obj.RemoveFromParent();
        }

        public void SetX(float x)
        {
            _obj.x = x;

        }
        public float GetX()
        {
            return _obj.x;
        }
        public void SetY(float y)
        {
            _obj.y = y;
        }
        public float GetY()
        {
            return _obj.y;
        }
        public void SetXY(float x, float y)
        {
            _obj.xy = new Vector2(x, y);
        }
        public void SetXY(Vector2 xy)
        {
            _obj.xy = xy;
        }
        public Vector2 GetXY()
        {
            return _obj.xy;
        }
        public void SetZ(float z)
        {
            _obj.z = z;
        }
        public float GetZ()
        {
            return _obj.z;
        }

        public void SetXYZ(float x, float y, float z)
        {
            _obj.x = x;
            _obj.y = y;
            _obj.z = z;
        }
        public Vector3 GetXYZ()
        {
            return new Vector3(_obj.x, _obj.y, _obj.z);
        }

        public void SetWidth(float width)
        {
            _obj.width = width;
        }

        public float GetWidth()
        {
            return _obj.width;
        }
        public void SetHeight(float height)
        {
            _obj.height = height;
        }
        public float GetHeight()
        {
            return _obj.height;
        }

        public void SetMaxWidth(int width)
        {
            _obj.maxWidth = width;
        }
        public void SetMaxHeight(int height)
        {
            _obj.maxHeight = height;
        }
        public void SetMinWidth(int width)
        {
            _obj.minWidth = width;
        }
        public void SetMinHeight(int height)
        {
            _obj.minHeight = height;
        }
        public void SetInitWidth()
        {
            _obj.width = _obj.initWidth;
        }
        public int GetInitWidth()
        {
            return _obj.initWidth;
        }
        public void SetInitHeight()
        {
            _obj.height = _obj.initHeight;
        }
        public int GetInitHeight()
        {
            return _obj.initHeight;
        }
        public Vector2 GetSize()
        {
            return new Vector2(_obj.width, _obj.height);
        }
        public void SetSize(float width, float height)
        {
            _obj.width = width;
            _obj.height = height;
        }
        public void SetSize(Vector2 size)
        {
            SetSize(size.x, size.y);
        }

        public Vector2 GetCenter()
        {
            var size = GetSize();
            return new Vector2(_obj.x + size.x / 2, _obj.y + size.y / 2);
        }

        public Vector2 GetSourceSize()
        {
            return new Vector2(_obj.sourceWidth, _obj.sourceHeight);
        }

        public void SetPivot(float x, float y, bool asAnchor)
        {
            _obj.SetPivot(x, y, asAnchor);
        }

        // 数据
        public void SetData(object data)
        {
            _obj.data = data;
        }
        public object GetData()
        {
            return _obj.data;
        }
        public T GetData<T>() where T : Object, new()
        {
            return GetData() as T;
        }

        // 设置拖拽
        public void SetDraggable(bool able)
        {
            _obj.draggable = able;
        }
        public bool GetDraggable()
        {
            return _obj.draggable;
        }

        public bool IsOnStage()
        {
            return _obj.onStage;
        }

        // 显示
        public void SetVisible(bool visible)
        {
            if (visible == IsVisible())
            {
                return;
            }
            _obj.visible = visible;
        }

        public bool IsVisible()
        {
            return _obj.visible;
        }

        // 是否灰显
        public void SsetGrayed(bool grayed)
        {
            _obj.grayed = grayed;
        }
        public bool GetGrayed()
        {
            return _obj.grayed;
        }

        // 是否可点击
        public void SetTouchable(bool able)
        {
            _obj.touchable = able;
        }

        public bool GetTouchable()
        {
            return _obj.touchable;
        }

        // 是否可用，变灰、不可触摸
        public void SetEnabled(bool enable)
        {
            _obj.grayed = !enable;
            _obj.touchable = enable;
        }

        public bool IsEnabled()
        {
            return _obj.touchable;
        }
        //
        public void OnClick(EventCallback0 func)
        {
            _obj.onClick.Set(func);
        }
        public void OnClick(EventCallback1 func)
        {
            _obj.onClick.Set(func);
        }

        public void AddClick(EventCallback0 func)
        {
            _obj.onClick.Add(func);
        }
        public void AddClick(EventCallback1 func)
        {
            _obj.onClick.Add(func);
        }
        public void RemoveClick(EventCallback0 func)
        {
            _obj.onClick.Remove(func);
        }
        public void RemoveClick(EventCallback1 func)
        {
            _obj.onClick.Remove(func);
        }

        public void ClearClick()
        {
            _obj.onClick.Clear();
        }
        public void OnClickLink(EventCallback0 func)
        {
            _obj.onClickLink.Add(func);
        }
        public void OnClickLink(EventCallback1 func)
        {
            _obj.onClickLink.Add(func);
        }
        //
        
        //---------- 触摸 -----------
        public void OnTouchBegin(EventCallback1 func)
        {
            _obj.onTouchBegin.Add(func);
        }

        public void OnTouchMove(EventCallback1 func)
        {
            _obj.onTouchMove.Add(func);
        }

        public void OnTouchEnd(EventCallback1 func)
        {
            _obj.onTouchEnd.Add(func);
        }

        //---------- 拖拽 -----------
        public void OnDragStart(EventCallback1 func)
        {
            _obj.onDragStart.Set(func);
        }

        public void OnDragMove(EventCallback1 func)
        {
            _obj.onDragMove.Add(func);
        }

        public void OnDragEnd(EventCallback1 func)
        {
            _obj.onDragEnd.Add(func);
        }

        //---------- enter - exit -----------
        public void OnAddedToStage(EventCallback1 func)
        {
            _obj.onAddedToStage.Add(func);
        }
        public void OnRemovedFromStage(EventCallback1 func)
        {
            _obj.onRemovedFromStage.Add(func);
        }
        //---------- 改变事件 -----------
        public void OnSizeChanged(EventCallback1 func)
        {
            _obj.onSizeChanged.Add(func);
        }
        public void OnPositionChanged(EventCallback1 func)
        {
            _obj.onPositionChanged.Add(func);
        }


        // 关联
        public void AddRelation(FObject target, RelationType relationType, bool usePercent)
        {
            _obj.AddRelation(target.GetObject(), relationType, usePercent);
        }
        public void AddRelation(FObject target, RelationType relationType)
        {
            _obj.AddRelation(target.GetObject(), relationType);
        }

        //---------- 坐标转换 ----------
        // Transforms a point from the local coordinate system to global (Stage) coordinates.
        public Vector2 LocalToGlobal(Vector2 vector2)
        {
            return _obj.LocalToGlobal(vector2);
        }

        // Transforms a point from global (Stage) coordinates to the local coordinate system.
        public Vector2 GlobalToLocal(Vector2 vector2)
        {
            return _obj.GlobalToLocal(vector2);
        }

        // 如果要转换任意两个UI对象间的坐标，例如需要知道A里面的坐标(10,10)在B里面的位置，可以用：
        public Vector2 TransformPoint(Vector2 vector2,FObject comp)
        {
            return _obj.TransformPoint(vector2, comp.GetObject());
        }


        // 透明度
        public void SetAlpha(float alpha)
        {
            _obj.alpha = alpha;
        }

        public float GetAlpha()
        {
            return _obj.alpha;
        }

        // 缩放
        public void SetScale(float a, float b)
        {
            _obj.SetScale(a, b);
        }
        public void SetScale(float a)
        {
            SetScale(a, a);
        }

        //旋转
        public void SetRotation(float rotation)
        {
            _obj.rotation = rotation;
        }
        public float GetRotation()
        {
            return _obj.rotation;
        }

        public void SetScaleX(float scaleX)
        {
            _obj.scaleX = scaleX;
        }

        public Vector2 GetScale()
        {
            return new Vector2 (_obj.scaleX, _obj.scaleY);
        }

        public float GetScaleX()
        {
            return _obj.scaleX;
        }

        public float GetScaleY()
        {
            return _obj.scaleY;
        }

        public string GetText()
        {
            return _obj.text;
        }
        public void SetText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                _obj.text = text;
            }
            else
            {
                _obj.text = "";
            }
        }

        public void SetSortingOrder(int order)
        {
            _obj.sortingOrder = order;
        }

        public int GetSortingOrder()
        {
            return _obj.sortingOrder;
        }

        //
        public void Center()
        {
            _obj.Center();
        }

        public void MakeFullScreen()
        {
            _obj.MakeFullScreen();
        }

        public virtual void Dispose()
        {
            if (_obj != null)
            {
                _obj.Dispose();
                _obj = null;
            }
        }
        public void StartDrag()
        {
            _obj.StartDrag();
        }
        public void StartDrag(int id)
        {
            _obj.StartDrag(id);
        }

        public void SetHome(FObject obj)
        {
            _obj.SetHome(obj.GetObject());
        }


        // 获取parent
        public bool HasParent()
        {
            return _obj.parent != null;
        }

        public bool IsDisposed()
        {
            if (_obj == null)
            {
                return true;
            }
            return _obj.isDisposed;
        }

        // 展开
        public void Expand()
        {
            SetVisible(true);
            SetInitHeight();
        }

        // 收缩
        public void Shrink()
        {
            SetVisible(false);
            SetHeight(0);
        }

    }
}
