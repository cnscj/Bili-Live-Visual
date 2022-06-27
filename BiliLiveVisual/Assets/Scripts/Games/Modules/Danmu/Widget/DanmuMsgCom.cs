
using THGame.UI;
using UnityEngine;

namespace BLVisual
{
    public class MainUIDanmuMsgCom : FWidget
    {
        FLabel title;

        BiliLiveDanmakuData.DanmuMsg msgData;
        public MainUIDanmuMsgCom()
        {
            package = "Danmu";
            component = "DanmuMsgCom";
        }
        protected override void OnInitUI()
        {
            title = GetChild<FLabel>("title");

            OnClick(() => {
                Debug.LogFormat("{0}({1}):{2}",msgData.nick, msgData.uid, msgData.content);
            });
        }

        public void SetMsgData(BiliLiveDanmakuData.DanmuMsg danmuMsg)
        {
            msgData = danmuMsg;
            var colorStr = string.IsNullOrEmpty(danmuMsg.color) ? "#FFFFFF" : danmuMsg.color;
            title.SetText(string.Format("[color={0}]{1}[/color]", colorStr, danmuMsg.content));
        }

    }
}