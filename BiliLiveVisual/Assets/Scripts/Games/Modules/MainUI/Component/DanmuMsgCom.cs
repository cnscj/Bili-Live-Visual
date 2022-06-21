
using THGame.UI;

namespace BLVisual
{
    public class DanmuMsgCom : FWidget
    {
        FLabel title;
        public DanmuMsgCom()
        {
            package = "MainUI";
            component = "DanmuMsgCom";
        }
        protected override void OnInitUI()
        {
            title = GetChild<FLabel>("title");
        }

        public void SetDanmuData(BiliLiveDanmakuData.DanmuMsg danmuMsg)
        {
            title.SetText(danmuMsg.content);
        }
    }
}