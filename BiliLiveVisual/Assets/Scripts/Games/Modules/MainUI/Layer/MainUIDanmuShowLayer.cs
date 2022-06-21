
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
        }

        protected override void OnInitUI()
        {
            stage = GetChild<FComponent>("stage");
        }

        protected void UpdateLayer()
        {
            stage.RemoveAllChildren();

        }

        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_DANMU_MSG, OnDanmuMsg);
        }

        void EmitDanmu(string text)
        {
            var comp = danmuComPool.GetOrCreate();
            stage.SetText(text);
            stage.AddChild(comp);
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
