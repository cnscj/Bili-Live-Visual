
using FairyGUI;

namespace THGame.UI
{

    public class FTextInput : FLabel
    {

        // 设置默认显示名字
        public void SetPlaceHolder(string text)
        {
            _obj.asTextInput.promptText = text;
        }

        // 设置焦点
        public void RequestFocus()
        {
            _obj.asTextInput.RequestFocus();
        }



        public void OnFocusIn(EventCallback1 func)
        {
            _obj.asTextInput.onFocusIn.Add(func);
        }

        public void OnFocusOut(EventCallback1 func)
        {
            _obj.asTextInput.onFocusOut.Add(func);
        }

        public void OnChanged(EventCallback1 func)
        {
            _obj.asTextInput.onChanged.Add(func);
        }


        public void SetFocusIn(EventCallback1 func)
        {
            _obj.asTextInput.onFocusIn.Set(func);
        }

        public void SetFocusOut(EventCallback1 func)
        {
            _obj.asTextInput.onFocusOut.Set(func);
        }

        public void SetChanged(EventCallback1 func)
        {
            _obj.asTextInput.onChanged.Set(func);
        }


        public void OnChangedCall()
        {
            _obj.asTextInput.onChanged.Call();
        }
    }

}
