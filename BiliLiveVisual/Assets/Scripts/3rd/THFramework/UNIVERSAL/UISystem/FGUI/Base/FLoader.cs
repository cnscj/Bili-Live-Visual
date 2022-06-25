using FairyGUI;
using UnityEngine;

namespace THGame.UI
{

    public class FLoader : FComponent
    {
        public void SetUIUrl(string package, string component)
        {
            string url = UIManager.GetInstance().GetUIUrl(package, component);
            _obj.asLoader.url = url;
        }

        public void SetUrl(string url)
        {
            _obj.asLoader.url = url;
        }

        public string GetUrl()
        {
            return _obj.asLoader.url;
        }

        public void SetTexture(NTexture texture)
        {
            _obj.asLoader.texture = texture;
        }

        public NTexture GetTexture()
        {
            return _obj.asLoader.texture;
        }

        public void SetColor(Color color)
        {
            _obj.asLoader.color = color;
        }

        public Color GetColor()
        {
            return _obj.asLoader.color;
        }

        public new void Center()
        {
            _obj.asLoader.Center();
        }

        public void SetPrecent(float value)
        {
            _obj.asLoader.fillAmount = value;
        }
    }

}
