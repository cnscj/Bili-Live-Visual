using FairyGUI;

namespace THGame.UI
{

    public class FTransition : Wrapper<Transition>
    {
        public static FTransition Create(Transition obj)
        {
            if (obj != null)
            {
                return new FTransition().InitWithObj(obj) as FTransition;
            }
            return null;
        }

        public void Play()
        {
            _obj.Play();
        }

        public void Play(PlayCompleteCallback onComplete)
        {
            _obj.Play(onComplete);
        }

        public void Stop()
        {
            _obj.Stop();
        }

        public bool IsPlaying()
        {
            return _obj.playing;
        }
    }

}
