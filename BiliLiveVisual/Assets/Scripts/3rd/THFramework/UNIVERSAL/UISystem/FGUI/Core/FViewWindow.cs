using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class FViewWindow : FView
    {
        public static readonly string frameName = "frame";

        protected FWindow _frame;

        public FViewWindow(string package, string component) : base(package, component)
        {
            _layerOrder = 100;
        }

        private void __InitWindowUI()
        {
            _frame = GetChild<FWindow>(frameName);

            if (_frame != null)
            {
                if (_frame.closeButton != null)
                {
                    _frame.closeButton.OnClick((context) =>
                    {
                        Close();
                    });
                }

                if (_frame.dragArea != null)
                {
                    _frame.dragArea.SetDraggable(true);
                    _frame.dragArea.OnDragStart((context) =>
                    {
                        context.PreventDefault();
                        StartDrag();
                    });
                }
            }
        }

        public override Wrapper<GObject> InitWithObj(GObject obj)
        {
            SetObject(obj);
            __InitWindowUI();
            base.InitWithObj(obj);

            return this;
        }
    }
}
