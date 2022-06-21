
using THGame.UI;
using XLibGame;

namespace BLVisual
{
    public class MainUIDanmuShowLayer : FWidget
    {
        FComponent stage;
        FLabel danmuPerSecond;

        ObjectPool<DanmuMsgCom> danmuComPool;
        public MainUIDanmuShowLayer()
        {
            danmuComPool = ObjectPoolManager.GetInstance().GetOrCreatePool<DanmuMsgCom>();
            danmuComPool.onDispose = (comp) =>
            {
                comp.Dispose();
            };
        }

        protected override void OnInitUI()
        {
            stage = GetChild<FComponent>("stage");
            danmuPerSecond = GetChild<FLabel>("danmuPerSecond");
        }

        protected void UpdateLayer()
        {
            stage.RemoveAllChildren();
            danmuPerSecond.SetText(Language.GetString(10501, 0));
        }

        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_DANMU_MSG, OnDanmuMsg);
        }

        void EmitDanmu(string text)
        {
            var comp = danmuComPool.GetOrCreate();

            if (!comp.IsCreated()) comp.TryCreate();
            comp.SetText(text);

            stage.AddChild(comp);

            Timer.GetInstance().ScheduleOnce(() =>
            {
                comp.RemoveFromParent();
                ObjectPoolManager.GetInstance().GetOrCreatePool<DanmuMsgCom>().Release(comp);
            }, 5);
        }

        protected void OnDanmuMsg(EventContext context)
        {
            var data = (BiliLiveDanmakuData.DanmuMsg)context.args[0];
            EmitDanmu(data.content);
        }

        protected override void OnEnter()
        {
            UpdateLayer();
        }

        protected override void OnExit()
        {

        }
    }
}
