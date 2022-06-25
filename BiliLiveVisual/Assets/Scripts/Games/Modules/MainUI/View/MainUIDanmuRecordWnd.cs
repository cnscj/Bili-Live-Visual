
using THGame.UI;
using XLibGame;
using XLibrary.Collection;
using XLibrary.Package.MVC;

namespace BLVisual
{
    public class MainUIDanmuRecordWnd : Window03
    {
        FList danmuList;
        FTextInput inputText;
        FButton searchBtn;
        FButton lookBtn;
        FLabel countText;

        string searchKey;
        Deque<MainUIRecordListTmpl> tmplInsDeq;
        UIPool tmplPool;
        public MainUIDanmuRecordWnd()
        {
            package = "MainUI";
            component = "MainUIDanmuRecordWnd";

        }

        protected override void OnInitUI()
        {
            inputText = GetChild<FTextInput>("inputText");
            searchBtn = GetChild<FButton>("searchBtn");
            lookBtn = GetChild<FButton>("lookBtn");
            countText = GetChild<FLabel>("countText");
            danmuList = GetChild<FList>("danmuList");
            tmplPool = UIPoolManager.GetInstance().GetOrCreatePool<MainUIRecordListTmpl>();

            searchBtn.OnClick(() =>
            {
                searchKey = inputText.GetText();
                InitDanmuComps(searchKey);
            });
            lookBtn.OnClick(() =>
            {
                searchKey = null;
                InitDanmuComps(searchKey);
            });
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
                tmplPool.Release(headComp);
            }
            var data = context.GetArg<BiliLiveDanmakuData.DanmuMsg>();
            TryToAddMsg(data, searchKey);
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
            var comp = tmplPool.GetOrCreate() as MainUIRecordListTmpl;
            comp.SetMsgData(data);
            danmuList.AddChild(comp);

            countText.SetText(danmuList.GetNumChildren().ToString());
            return comp;
        }

        private void RemoveAllDanmuChildren()
        {
            if (tmplInsDeq == null)
                return;

            while (tmplInsDeq.Count > 0)
            {
                var headComp = GetTmplQeque().GetHead();
                headComp.RemoveFromParent();
                GetTmplQeque().RemoveHead();
                tmplPool.Release(headComp);
            }
        }

        private void TryToAddMsg(BiliLiveDanmakuData.DanmuMsg msg,string key = null)
        {
            bool isCanAdd = true;
            if (key != null)
            {
                isCanAdd = false;
                if (    
                        msg.content.IndexOf(key, System.StringComparison.OrdinalIgnoreCase) >= 0
                        || msg.nick.IndexOf(key, System.StringComparison.OrdinalIgnoreCase) >= 0
                        || msg.uid.ToString().IndexOf(key, System.StringComparison.OrdinalIgnoreCase) >= 0
                    )
                {
                    isCanAdd = true;
                }
            }
            if (isCanAdd)
            {
                var comp = AddMsgComp2List(msg);
                GetTmplQeque().AddTail(comp);
            }
        }
        private void InitDanmuComps(string key = null)
        {
            RemoveAllDanmuChildren();
            var deque = Cache.Get<DanmuCache>().GetRecordDanmuQueue();
            foreach(BiliLiveDanmakuData.DanmuMsg msg in deque)
            {
                TryToAddMsg(msg, key);
            }
        }
        protected override void OnEnter()
        {
            InitDanmuComps();
        }

        protected override void OnExit()
        {
            RemoveAllDanmuChildren();
        }

    }
}