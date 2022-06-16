
using THGame.UI;

namespace BLVisual
{
    public class DanmuMsgCom : FWidget
    {
        FLabel title;

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