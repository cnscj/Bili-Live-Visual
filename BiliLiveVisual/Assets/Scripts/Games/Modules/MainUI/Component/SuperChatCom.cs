
using THGame.UI;
using XLibGame;
using XLibrary;

namespace BLVisual
{
    public class MainUISuperChatCom : FWidget
    {
        FLabel username;
        FLabel content;
        FButton headBtn;

        BiliLiveDanmakuData.SuperChatMessage msgData;
        public MainUISuperChatCom()
        {
            package = "MainUI";
            component = "SuperChatCom";
        }
        protected override void OnInitUI()
        {
            username = GetChild<FLabel>("username");
            content = GetChild<FLabel>("content");
            headBtn = GetChild<FButton>("headBtn");
        }

        public void SetMsgData(BiliLiveDanmakuData.SuperChatMessage superChatMessage)
        {
            msgData = superChatMessage;
            username.SetText(superChatMessage.uname);
            content.SetText(string.Format("[color={0}]{1}[/color]", superChatMessage.message_font_color, superChatMessage.message));
            //headBtn.SetIcon(superChatMessage.face);

            
            var restTime = superChatMessage.end_time - XTimeTools.NowTimeStamp;
            if (restTime <= 0)
                Dispose();
            else
                Timer.GetInstance().ScheduleOnce(() =>
                {
                    //Dispose();
                    
                }, restTime);
            
        }
    }
}