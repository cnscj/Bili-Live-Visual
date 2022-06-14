
namespace THGame.UI
{

    public class FProgressBar : FComponent
    {

        public void SetValue(double value)
        {
            _obj.asProgress.value = value;
        }

        public void SetMax(double max)
        {
            _obj.asProgress.max = max;
        }

        public double GetValue()
        {
            return _obj.asProgress.value;
        }

        public double GetMax()
        {
            return _obj.asProgress.max;
        }

        public void SetValueMax(double value, double max)
        {
            _obj.asProgress.value = value;
            _obj.asProgress.max = max;
        }

        public FTweener TweenValue(double value, float duration)
        {
            var obj = _obj.asProgress.TweenValue(value, duration);
            return new FTweener().InitWithObj(obj) as FTweener;
        }
    }

}
