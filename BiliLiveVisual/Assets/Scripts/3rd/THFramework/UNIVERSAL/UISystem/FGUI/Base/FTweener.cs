using FairyGUI;

namespace THGame.UI
{

    public class FTweener : Wrapper<GTweener>
    {
        public void Kill()
        {
            _obj.Kill();
        }

        public void OnComplete(GTweenCallback callbcak)
        {
            _obj.OnComplete(callbcak);
        }
        public void OnComplete(GTweenCallback1 callbcak)
        {
            _obj.OnComplete(callbcak);
        }

        public void SetEase(EaseType value)
        {
            _obj.SetEase(value);
        }
    }

}
