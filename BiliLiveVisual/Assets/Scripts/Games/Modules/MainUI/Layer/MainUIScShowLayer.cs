
using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEngine;

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
                Debug.Log(data);
            });
        }


        protected void UpdateLayer()
        {
            scList.SetDataProvider(new int[] { 1, 2, 3, 4, 5 });
        }


        protected override void OnInitEvent()
        {
  
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