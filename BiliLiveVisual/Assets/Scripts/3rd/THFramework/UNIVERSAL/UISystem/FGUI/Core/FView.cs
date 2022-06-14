
using System;
using FairyGUI;

namespace THGame.UI
{
	public class FView : FWidget
    {
        protected int _layerOrder = 0;          //层
        protected bool _isFullScreen;           //是否全屏

        public int layerOrder { get { return _layerOrder; } }

        public FView(string package = null, string component = null) : base(package, component)
        {

        }
        public virtual void Open(object args = null)
        {
            ViewManager.GetInstance().Open(this, args);
        }

        public virtual void Close(bool isDisposed = true)
        {
            ViewManager.GetInstance().Close(this, isDisposed);
        }

        //给ViewManager调用
        public void __OpenByManager(object args = null)
        {
            SetArgs(args);
            GRoot.inst.AddChild(GetObject());

            OnShowAnimation();
        }

        public void __CloseByManager(bool isDisposed = true)
        {
            OnHideAnimation();

            if (isDisposed)
                Dispose();
            else
                RemoveFromParent();
        }

        public override Wrapper<GObject> InitWithObj(GObject obj)
        {
            base.InitWithObj(obj);

            if (_isFullScreen)
            {
                SetSize(GetParent().GetWidth(), GetParent().GetHeight());
                AddRelation(GetParent(), RelationType.Size);
            }

            SetSortingOrder(_layerOrder);   //设置渲染层级,保证层级低的不覆盖

            return this;
        }

        //TODO:
        protected virtual void OnShowAnimation()
        {
            
        }

        protected virtual void OnHideAnimation()
        {

        }
    }

}
