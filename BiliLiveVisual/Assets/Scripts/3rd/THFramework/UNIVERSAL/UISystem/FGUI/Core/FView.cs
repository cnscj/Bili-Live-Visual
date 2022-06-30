
using System;
using FairyGUI;

namespace THGame.UI
{
	public class FView : FWidget
    {
        public enum FullscreenMode
        {
            Default,    //自由模式（做多大就是多大）
            FitScreen,  //屏幕大小（全屏）
            FitParent,  //其父节点的大小
        }
        public enum MaskType
        {
            None,           //无底
            Translucent,    // 半透黑底
            Black,          // 全遮盖纯黑底，可隐藏场景
        }

        protected int _layerOrder = 0;                                               //层
        protected FullscreenMode _fullScreenMode = FullscreenMode.Default;           //全屏模式
        protected MaskType _maskType = MaskType.None;                                //遮罩类型

        protected FTransition _tShow;
        protected FTransition _tHide;
        protected bool _isPalyingAnimation;
        protected FComponent _maskCom;

        public FView()
        {

        }

        public FView(string package, string component) : base(package, component)
        {

        }

//
        public virtual void Open(object args = null)
        {
            ViewManager.GetInstance().Open(this, args);
        }

        public virtual void Close(bool isImmediate = false, bool isDisposed = true)
        {
            ViewManager.GetInstance().Close(this, isImmediate, isDisposed);
        }
//
        public bool IsCanOpen(object args = null)
        {
            return OnCanOpen(args);
        }

        public int GetLayerOrder()
        {
            return _layerOrder;
        }

        //给ViewManager调用,勿嵌套调用
        public void __OpenByManager(object args = null)
        {
            SetArgs(args);
            GRoot.inst.AddChild(GetObject());

            OnShowAnimation(OnShow);
        }

        public void __CloseByManager(bool isImmediate = false, bool isDisposed = true)
        {
            if (IsDisposed())
                return;

            if (isImmediate)
            {
                OnHide();
                if (isDisposed)
                    Dispose();
                else
                    RemoveFromParent();
            }
            else
            {
                OnHide();
                if (isDisposed)
                    OnHideAnimation(()=>
                    {
                        Dispose();
                    });
                else
                    OnHideAnimation(() =>
                    {
                        RemoveFromParent();
                    });
            }
        }

        protected override void OnInitObj(GObject obj)
        {
            CreateBackgroundMask();

            base.OnInitObj(obj);
            AdjustScreenAdaptation();
            SetSortingOrder(_layerOrder);   //设置渲染层级,保证层级低的不覆盖
        }

        protected void AdjustScreenAdaptation()
        {
            if (_fullScreenMode == FullscreenMode.FitScreen)
            {
                var pObj = GetParent();
                var rootWidth = UIManager.GetRootWidth();
                var rootHeight = UIManager.GetRootHeight();
                SetSize(rootWidth, rootHeight);
                AddRelation(pObj, FairyGUI.RelationType.Size);
            }
            else if (_fullScreenMode == FullscreenMode.FitParent)
            {
                SetSize(GetParent().GetWidth(), GetParent().GetHeight());
                AddRelation(GetParent(), RelationType.Size);
            }
        }

        protected void CreateBackgroundMask()
        {
            if (_maskType == MaskType.Translucent || _maskType == MaskType.Black)
            {
                var maskCom = FComponent.Create<FGraph>(new GGraph());
                _maskCom = maskCom;
                UnityEngine.ColorUtility.TryParseHtmlString("#000000",out var drawColor);
                maskCom.SetXY(0,0);
                maskCom.SetSize(GetSize());
                maskCom.SetPivot(0.5f,0.5f,false);
                maskCom.AddRelation(this, RelationType.Size);
                maskCom.DrawRect(GetWidth(), GetHeight(), 1, drawColor, drawColor);
                AddChildAt(maskCom,0);

                if (_maskType == MaskType.Translucent)
                {
                    maskCom.SetAlpha(0.7f);
                }
                else if (_maskType == MaskType.Black)
                {
                    maskCom.SetAlpha(1f);
                }
            }
        }

        protected virtual bool OnCanOpen(object args)
        {
            return true;
        }

        protected virtual void OnShow()
        {

        }

        protected virtual void OnHide()
        {

        }

        protected virtual void OnShowAnimation(Action callback)
        {
            _tShow = _tShow ?? GetTransition("show");
            if (_tShow != null)
            {
                _isPalyingAnimation = true;
                _tShow.Play(() =>
                {
                    _isPalyingAnimation = false;
                    callback?.Invoke();
                });
            }
            else
            {
                callback?.Invoke();
            }
        }

        protected virtual void OnHideAnimation(Action callback)
        {
            _tHide = _tHide ?? GetTransition("hide");
            if (_tHide != null)
            {
                _isPalyingAnimation = true;
                _tHide.Play(() =>
                {
                    _isPalyingAnimation = false;
                    callback?.Invoke();
                });
            }
            else
            {
                callback?.Invoke();
            }
        }
    }

}
