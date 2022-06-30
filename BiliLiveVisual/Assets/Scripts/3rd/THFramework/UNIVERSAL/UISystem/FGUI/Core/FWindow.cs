using System;
using FairyGUI;

namespace THGame.UI
{
    public class FWindow : FView
    {
        private static readonly string frameName = "frame";
        private static readonly string iconName = "icon";
        private static readonly string titleName = "title";
        private static readonly string closeBtnName = "closeButton";
        private static readonly string dragAreaName = "dragArea";
        private static readonly string contentAreaName = "contentArea";

        protected FComponent _frame;
        protected FLoader _icon;
        protected FRichText _title;
        protected FButton _closeButton;

        protected FGraph _dragArea;
        protected FGraph _contentArea;

        public FWindow(): this(null, null)
        {
            
        }
        public FWindow(string package, string component) : base(package, component)
        { 
            _layerOrder = 200;
        }

        public void SetTitle(string text)
        {
            if (_title != null)
            {
                _title.SetText(text);
            }
        }

        public string GetTitle()
        {
            if (_title != null)
            {
                return _title.GetText();
            }
            return "";
        }

        public new void SetDraggable(bool val)
        {
            _dragArea.SetDraggable(val);
        }


        public new bool GetDraggable()
        {
            return _dragArea.GetDraggable();
        }

        private void __InitWindowUI()
        {
            _frame = GetChild<FComponent>(frameName) ?? this;
            _icon = _frame.GetChild<FLoader>(iconName);
            _title = _frame.GetChild<FRichText>(titleName);
            _closeButton = _frame.GetChild<FButton>(closeBtnName);
            _dragArea = _frame.GetChild<FGraph>(dragAreaName);
            _contentArea = _frame.GetChild<FGraph>(contentAreaName);


            if (_closeButton != null)
            {
                _closeButton.OnClick((context) =>
                {
                    Close();
                });
            }

            if (_dragArea != null)
            {
                _dragArea.SetDraggable(true);
                _dragArea.OnDragStart((context) =>
                {
                    context.PreventDefault();
                    StartDrag();
                });
            }
            
        }

        protected override void OnInitObj(GObject obj)
        {
            __InitWindowUI();
            base.OnInitObj(obj);
        }

        protected override void OnShowAnimation(Action callback)
        {
            var y = GetY();
            SetY(y + 20);
            SetAlpha(0);
            _isPalyingAnimation = true;
            GetObject().TweenFade(1, 0.25f).SetEase(EaseType.QuadOut);
            GetObject().TweenMoveY(y, 0.25f).SetEase(EaseType.BackOut).OnComplete(() =>
            {
                _isPalyingAnimation = false;
                callback?.Invoke();
            });
        }

        protected override void OnHideAnimation(Action callback)
        {
            var y = GetY();
            SetY(y);
            SetAlpha(1);
            _isPalyingAnimation = true;
            GetObject().TweenFade(0, 0.25f).SetEase(EaseType.QuadOut);
            GetObject().TweenMoveY(y+20, 0.25f).SetEase(EaseType.BackOut).OnComplete(() =>
            {
                _isPalyingAnimation = false;
                callback?.Invoke();
            });
        }
    }
}
