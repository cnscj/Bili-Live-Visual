
using FairyGUI;
namespace THGame.UI
{
    public class XGObject : GObject
    {
        public override void Dispose()
        {
            base.Dispose();
            OnDispose();
        }

        protected virtual void OnDispose()
        {

        }
    }
}
