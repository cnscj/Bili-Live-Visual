
namespace XLibrary.Package.MVC
{
    public class Cache
    {
        public static T Get<T>() where T : Cache,new()
        {
            return MVCManager.GetInstance().GetCache<T>();
        }

        public void Initilize()
        {
            OnAdd();
        }
        public void Clear()
        {
            OnClear();
        }

        public void Dispose()
        {
            Clear();
            OnRemove();
        }
        protected virtual void OnAdd()
        {

        }

        protected virtual void OnRemove()
        {

        }

        protected virtual void OnClear()
        {

        }

    }
}
