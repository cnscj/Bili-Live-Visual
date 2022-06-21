
namespace XLibGame
{
    public class PoolObjectBundle<T> where T : class, new()
    {
        private T _obj;

        public PoolObjectBundle() { }
        public PoolObjectBundle(T obj)
        {
            _obj = obj;
        }
        public T GetOrCreate()
        {
            _obj = _obj ?? new T();
            return _obj;
        }
    }
}