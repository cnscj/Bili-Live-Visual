
using THGame.UI;
using XLibGame;
using XLibrary;

namespace BLVisual
{
    public class MainUISuperChatCom : FWidget
    {
        FLabel username;
        FLabel content;
        MainUIUserHeadLoader headLoader;

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
            headLoader = GetChild<MainUIUserHeadLoader>("headLoader");
        }

        public void SetMsgData(BiliLiveDanmakuData.SuperChatMessage superChatMessage)
        {
            msgData = superChatMessage;
            username.SetText(superChatMessage.uname);
            content.SetText(string.Format("[color={0}]{1}[/color]", superChatMessage.message_font_color, superChatMessage.message));
            headLoader.SetHeadData(superChatMessage.face);

            
            var restTime = superChatMessage.end_time - XTimeTools.NowTimeStamp;
            if (restTime <= 0)
                Dispose();
            else
                Timer.GetInstance().ScheduleOnce(() =>
                {
                    Dispose();
                    
                }, restTime);
            
        }
    }
}