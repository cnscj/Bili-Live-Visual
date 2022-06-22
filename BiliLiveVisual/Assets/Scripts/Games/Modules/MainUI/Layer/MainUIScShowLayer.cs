
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

            scList.SetVirtual();
            scList.SetState<int, MainUISuperChatCom>((index, data, comp) =>
            {
     
            });
        }

        protected void OnBiliClientStart(EventContext context)
        {

        }

        protected void UpdateLayer()
        {
            scList.SetDataProvider(new int[] { 1, 2, 3, 4, 5 });
        }


        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_SUPER_CHAT_MESSAGE, OnSuperChatMessage);
            AddEventListener(EventType.BILILIVE_SUPER_CHAT_MESSAGE, OnBiliClientStart);
        }

        protected void OnSuperChatMessage(EventContext context)
        {
            var data = context.GetArg<BiliLiveDanmakuData.SuperChatMessage>();

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