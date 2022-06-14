using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class FRoot : FComponent
    {
        GRoot _root; 
        private static FRoot s_instance;
        public static FRoot GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new FRoot();
            }
            return s_instance;
        }

        protected FRoot()
        {
            InitWithObj(GRoot.inst);
            _root = GRoot.inst;
        }

        public void ShowPopup(FObject popup)
        {
            _root.ShowPopup(popup.GetObject());
        }
        public void ShowPopup(FObject popup, FObject target)
        {
            _root.ShowPopup(popup.GetObject(), target.GetObject());
        }
        public void ShowPopup(FObject popup, FObject target, PopupDirection dir)
        {
            _root.ShowPopup(popup.GetObject(), target.GetObject(), dir);
        }
        public void ShowPopup(FObject popup, FObject target, PopupDirection dir, bool closeUntilUpEvent)
        {
            _root.ShowPopup(popup.GetObject(), target.GetObject(), dir, closeUntilUpEvent);
        }
        public void HidePopup()
        {
            _root.HidePopup();
        }
        public void HidePopup(FObject popup)
        {
            _root.HidePopup(popup.GetObject());
        }

        public bool HasAnyPopup()
        {
            return _root.hasAnyPopup;
        }


    }

}
