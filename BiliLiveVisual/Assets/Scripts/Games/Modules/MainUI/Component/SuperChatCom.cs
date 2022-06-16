
using THGame.UI;

namespace BLVisual
{
    public class MainUISuperChatCom : FWidget
    {
        FLabel username;
        FLabel content;
        FButton headBtn;

        protected override void OnInitUI()
        {
            username = GetChild<FLabel>("username");
            content = GetChild<FLabel>("content");
            headBtn = GetChild<FButton>("headBtn");
        }

        protected override void OnEnter()
        {
            username.SetText("Uname");
        }
        public void SetDanmuData(BiliLiveDanmakuData.SuperChatMessage superChatMessage)
        {
            username.SetText(superChatMessage.uname);
            content.SetText(superChatMessage.message);
            headBtn.SetIcon(superChatMessage.face);
        }
    }
}