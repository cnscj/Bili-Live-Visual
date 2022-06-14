
using UnityEngine;

namespace THGame.UI
{

    public class MonoWrapper<T> : MonoBehaviour
    {
        protected T _obj;
       
        private void Start()
        {
            _obj = GetComponent<T>();
            OnStart();
        }

        public bool IsInited()
        {
            return (_obj != null);
        }
        public T GetObject()
        {
            return _obj;
        }

        protected void SetObject(T obj)
        {
            _obj = obj;
        }

        protected virtual void OnStart()
        {

        }
    }

}
