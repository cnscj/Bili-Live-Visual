
using System;
using FairyGUI;

namespace THGame.UI
{
	public class FView : FWidget
    {
        public enum FullscreenMode
        {
            Default,
            FitScreen,
            FitParent,
            SafeArea,
        }
        protected int _layerOrder = 0;                                               //层
        protected FullscreenMode _fullScreenMode = FullscreenMode.Default;           //全屏模式

        protected FTransition _tShow;
        protected FTransition _tHide;
        protected bool _isPalyingAnimation;

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
            base.OnInitObj(obj);
            AdjustScreenAdaptation();
            SetSortingOrder(_layerOrder);   //设置渲染层级,保证层级低的不覆盖
        }

        protected void AdjustScreenAdaptation()
        {
            if (_fullScreenMode == FullscreenMode.FitParent)
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
            else if (_fullScreenMode == FullscreenMode.SafeArea)
            {

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
