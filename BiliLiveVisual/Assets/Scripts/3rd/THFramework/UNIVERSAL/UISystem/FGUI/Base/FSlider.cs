
using FairyGUI;

namespace THGame.UI
{
    public class FSlider : FComponent
    {
        public new void SetText(string text)
        {
            _obj.asSlider.text = text;
        }

        public new string GetText()
        {
            return _obj.asSlider.text;
        }

        public void SetValue(double value, bool call = false)
        {
            _obj.asSlider.value = value;
        
            if (call)
            {
                _obj.asSlider.onChanged.Call();
            }
        }

        public double GetValue()
        {
            return _obj.asSlider.value;
        }

        public void SetMax(double max)
        {
            _obj.asSlider.max = max;
        }

        public double GetMax()
        {
            return _obj.asSlider.max;
        }

        public void SetValueMax(double value, double max)
        {
            _obj.asSlider.value = value;
            _obj.asSlider.max = max;
        }

        public void SetPercent(double percent)
        {
            var val = percent * _obj.asSlider.value;
            SetValue(val);
        }

        public double GetPercent()
        {
            return _obj.asSlider.value / _obj.asSlider.max;
        }

        public void ChangeOnClick(bool change = true)
        {
            //点击滚动条改变进度，默认是true
            _obj.asSlider.changeOnClick = change;
        }

        public void OnChanged(EventCallback1 func)
        {
            _obj.asSlider.onChanged.Add(func);
        }
    }

}
