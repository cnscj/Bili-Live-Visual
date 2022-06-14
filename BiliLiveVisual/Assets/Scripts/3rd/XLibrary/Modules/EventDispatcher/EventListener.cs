using System.Collections.Generic;

namespace XLibGame
{
    public class EventListener
    {
        private bool _isLock; //防止递归派发
        private EventCallback0 _callbacks0;
        private EventCallback1 _callbacks1;

        public void Add(EventCallback0 callback0, int priority = 1)
        {
            _callbacks0 -= callback0;
            _callbacks0 += callback0;
        }
        public void Add(EventCallback1 callback1, int priority = 1)
        {
            _callbacks1 -= callback1;
            _callbacks1 += callback1;
        }

        public void Remove(EventCallback0 callback0)
        {
            _callbacks0 -= callback0;
        }

        public void Remove(EventCallback1 callback1)
        {
            _callbacks1 -= callback1;
        }

        public void Clear()
        {
            _callbacks0 = null;
            _callbacks1 = null;
        }

        public void Call(EventContext context)
        {
            if (_isLock) return;

            _isLock = true;

            _callbacks0?.Invoke();
            _callbacks1?.Invoke(context);

            _isLock = false;
        }
    }
}