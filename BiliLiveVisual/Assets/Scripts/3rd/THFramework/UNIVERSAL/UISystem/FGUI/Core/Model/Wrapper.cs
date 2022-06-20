
namespace THGame.UI
{

    public class Wrapper<T>
    {
        protected T _obj;
        public Wrapper<T> InitWithObj(T obj)
        {
            SetObject(obj);
            OnInitObj(obj);

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
        //
        protected virtual void OnInitObj(T obj)
        {

        }
    }

}
