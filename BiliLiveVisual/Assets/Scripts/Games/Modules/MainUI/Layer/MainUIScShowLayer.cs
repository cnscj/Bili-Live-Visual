
using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEngine;
using XLibGame;

namespace BLVisual
{
    public class MainUIScShowLayer : FWidget
    {
        FList scList;
        protected override void OnInitUI()
        {
            scList = GetChild<FList>("scList");
        }

        protected void OnBiliClientStart(EventContext context)
        {
            scList.RemoveAllChildren();
        }

        protected void UpdateLayer()
        {

        }

        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_SUPER_CHAT_MESSAGE, OnSuperChatMessage);
            AddEventListener(EventType.BILILIVE_START, OnBiliClientStart);
        }

        protected void OnSuperChatMessage(EventContext context)
        {
            var data = context.GetArg<BiliLiveDanmakuData.SuperChatMessage>();
            var comp = FWidget.Create<MainUISuperChatCom>();

            comp.SetMsgData(data);
            scList.AddChild(comp);
            scList.ScrollToBottom(true);
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