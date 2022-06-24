
using THGame.UI;
using XLibGame;
using XLibrary;

namespace BLVisual
{
    public class MainUISuperChatCom : FWidget
    {
        FLabel username;
        FLabel content;
        FProgressBar colorBar;
        MainUIUserHeadLoader headLoader;

        int recycleTimerId;
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
            colorBar = GetChild<FProgressBar>("colorBar");
            headLoader = GetChild<MainUIUserHeadLoader>("headLoader");
        }

        public void SetMsgData(BiliLiveDanmakuData.SuperChatMessage superChatMessage)
        {
            msgData = superChatMessage;
            username.SetText(superChatMessage.uname);
            content.SetText(string.Format("[color={0}]{1}[/color]", superChatMessage.message_font_color, superChatMessage.message));
            headLoader.SetHeadData(superChatMessage.face);

            colorBar.SetValueMax(100, 100);
            colorBar.TweenValue(0, superChatMessage.time);
            recycleTimerId = Timer.GetInstance().ScheduleOnce(() =>
            {
                Dispose();
                recycleTimerId = 0;
            }, superChatMessage.time);
            
        }
        protected override void OnExit()
        {
            if (recycleTimerId > 0)
            {
                Timer.GetInstance().Unschedule(recycleTimerId);
                recycleTimerId = 0;
            }
        }
    }
}