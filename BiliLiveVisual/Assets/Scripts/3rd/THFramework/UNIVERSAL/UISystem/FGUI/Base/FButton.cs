using UnityEngine;
using FairyGUI;

namespace THGame.UI
{

    public class FButton : FComponent
    {
        public float GetSoundVolumeScale()
        {
            return _obj.asButton.soundVolumeScale;
        }

        public void SetSoundVolumeScale(float volume)
        {
            _obj.asButton.soundVolumeScale = volume;
        }

        public void SetIcon(string url)
        {
            _obj.asButton.icon = url;
        }

        public new void SetText(string text)
        {
            _obj.asButton.title = text;
        }

        public new string GetText()
        {
            return _obj.asButton.title;
        }

        public void SetTextSize(int size)
        {
            _obj.asButton.titleFontSize = size;
        }

        public void SetColor(Color color)
        {
            _obj.asButton.color = color;
        }


        public void OnChanged(EventCallback1 func)
        {
            _obj.asButton.onChanged.Add(func);
        }

        public void Call()
        {
            _obj.asButton.onClick.Call();
        }

//
        public void SetSelected(bool selected,bool call)
        {
            _obj.asButton.selected = selected;
	        if (call)
	        {
    	        Call();
	        }
        }
        public bool IsSelected()
        {
            return _obj.asButton.selected;
        }

        public void SetMode(ButtonMode mode)
        {
            _obj.asButton.mode = mode;
        }
        public ButtonMode GetMode()
        {
            return _obj.asButton.mode;
        }
    }

}
