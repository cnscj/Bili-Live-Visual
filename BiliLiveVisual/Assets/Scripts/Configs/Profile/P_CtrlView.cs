

using System.Collections.Generic;
using THGame.UI;
using XLibGame;
using XLibrary;

namespace BLVisual
{
    public static class P_CtrlView
    {
        public static readonly List<Dictionary<string, object>> FuncArray = new List<Dictionary<string, object>>()
        {
            new Dictionary<string, object>(){
                ["text"] = "投票检测",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {
                    UIManager.OpenView<MainUIVoteStatisticsWnd>();
                }),
            },
            new Dictionary<string, object>(){
                ["text"] = "弹幕记录",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {
                    UIManager.OpenView<MainUIDanmuRecordWnd>();
                }),
            },
            new Dictionary<string, object>(){
                ["text"] = "弹幕录制",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {
                    UIManager.OpenView<MainUIDanmuTranscribeWnd>();
                }),
            },
            new Dictionary<string, object>(){
                ["text"] = "弹幕设置",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {

                }),
            },
            new Dictionary<string, object>(){
                ["text"] = "黑名单",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {

                }),
            },
            new Dictionary<string, object>(){
                ["text"] = "100*弹幕",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {
                    for(int i=0;i<100;i++)
                    {
                        EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_DANMU_MSG, new BiliLiveDanmakuData.DanmuMsg(){
                            uid = i,
                            nick = "Test",
                            color = "#FFFFFF",
                            content= string.Format("{0}",i),
                        });
                    }
                }),
            },
            new Dictionary<string, object>(){
                ["text"] = "100*SC",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {
                    for(int i=0;i<100;i++)
                    {
                        EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_SUPER_CHAT_MESSAGE, new BiliLiveDanmakuData.SuperChatMessage(){
                            uid = i,
                            uname = "Test",
                            face = "http://i1.hdslb.com/bfs/face/e0b5ae65a4fe580d26587fff6bc147d1d020ea46.jpg",
                            face_frame = "",
                            message= string.Format("{0}",i),
                            start_time = (int)XTimeTools.NowTimeStamp,
                            end_time = (int)XTimeTools.NowTimeStamp + 60,
                            time = 60,
                            price = 30,
                            message_font_color = "#000000",
                            background_price_color= "#FFFFFF",
                        }) ;
                    }
                }),
            },
        };
    }
}