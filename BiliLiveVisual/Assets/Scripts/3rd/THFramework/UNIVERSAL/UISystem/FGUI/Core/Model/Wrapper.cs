
namespace THGame.UI
{

    public class Wrapper<T>
    {
        protected T _obj;
        public virtual Wrapper<T> InitWithObj(T obj)
        {
            _obj = obj;
            return this;
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
    }

}
