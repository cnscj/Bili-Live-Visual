
using THGame.UI;
using XLibGame;
using XLibrary.Collection;
using XLibrary.Package.MVC;

namespace BLVisual
{
    public class MainUIDanmuRecordWnd : Window03
    {
        FList danmuList;
        Deque<MainUIRecordListTmpl> tmplInsDeq;
        public MainUIDanmuRecordWnd()
        {
            package = "MainUI";
            component = "MainUIDanmuRecordWnd";

        }

        protected override void OnInitUI()
        {
            danmuList = GetChild<FList>("danmuList");

        }

        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_DANMU_MSG, OnDanmuMsg);
        }


        protected void OnDanmuMsg(EventContext context)
        {
            if (GetTmplQeque().Count >= GetTmplQeque().Capacity)
            {
                var headComp = GetTmplQeque().GetHead();
                headComp.RemoveFromParent();
                GetTmplQeque().RemoveHead();
            }
            var data = context.GetArg<BiliLiveDanmakuData.DanmuMsg>();
            var comp = AddMsgComp2List(data);
            GetTmplQeque().AddTail(comp);
        }

        private Deque<MainUIRecordListTmpl> GetTmplQeque()
        {
            if (tmplInsDeq == null)
            {
                var deque = Cache.Get<DanmuCache>().GetRecordDanmuQueue();
                tmplInsDeq = new Deque<MainUIRecordListTmpl>(deque.Capacity);
            }
            return tmplInsDeq;
        }

        private MainUIRecordListTmpl AddMsgComp2List(BiliLiveDanmakuData.DanmuMsg data)
        {
            var comp = MainUIRecordListTmpl.Create<MainUIRecordListTmpl>();
            comp.SetMsgData(data);
            danmuList.AddChild(comp);

            return comp;
        }
        private void InitDanmuComps()
        {
            var deque = Cache.Get<DanmuCache>().GetRecordDanmuQueue();
            foreach(var msg in deque)
            {
                var comp = AddMsgComp2List((BiliLiveDanmakuData.DanmuMsg)msg);
                GetTmplQeque().AddTail(comp);
            }
        }
        protected override void OnEnter()
        {
            InitDanmuComps();
        }

        protected override void OnExit()
        {
           
        }

    }
}