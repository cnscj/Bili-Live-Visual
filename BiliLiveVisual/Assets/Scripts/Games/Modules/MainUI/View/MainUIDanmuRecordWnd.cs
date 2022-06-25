
using THGame.UI;
using XLibGame;
using XLibrary.Package.MVC;

namespace BLVisual
{
    public class MainUIDanmuRecordWnd : Window03
    {
        FList danmuList;

        public MainUIDanmuRecordWnd()
        {
            package = "MainUI";
            component = "MainUIDanmuRecordWnd";

        }

        protected override void OnInitUI()
        {
            danmuList = GetChild<FList>("danmuList");
            danmuList.SetVirtual();
            danmuList.SetState<object, MainUIRecordListTmpl>((index,_,comp) => {
                var queue = Cache.Get<DanmuCache>().GetRecordDanmuQueue();
                var data = queue.GetItem(index);
                comp.SetMsgData(data);
            });
        }

        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_DANMU_MSG, OnDanmuMsg);
        }


        protected void OnDanmuMsg(EventContext context)
        {
            UpdateList();
        }

        void UpdateList()
        {
            var queue = Cache.Get<DanmuCache>().GetRecordDanmuQueue();
            danmuList.SetNumItems(queue.Count);
        }

        protected override void OnEnter()
        {
            UpdateList();
        }

        protected override void OnExit()
        {
           
        }

    }
}