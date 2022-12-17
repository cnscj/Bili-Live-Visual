
using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEngine;
using XLibGame;

namespace BLVisual
{
    public class DanmuSCLayer : FWidget
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

            //根据SC金额排序
            var allChildren = scList.GetChildren();
            int index = allChildren.Length;

            for (var i = 0; i < allChildren.Length; i++)
            {
                var child = allChildren[i];
                if (child is MainUISuperChatCom)
                {
                    var scCom = child as MainUISuperChatCom;
                    var msgData = scCom.GetMsgData();
                    if (msgData != null)
                    {
                        if (data.price >= msgData.price)
                        {
                            index = i;
                            break;
                        }
                    }
                }
            }
            scList.AddChildAt(comp, index);
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