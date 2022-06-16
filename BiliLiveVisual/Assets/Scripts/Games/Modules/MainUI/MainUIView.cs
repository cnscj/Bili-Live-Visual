using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEngine;

namespace BLVisual
{
    public class MainUIView : FView
    {
        FButton okBtn;
        MainUIScShowLayer scShowLayer;
        MainUIDanmuShowLayer danmuShowLayer;

        public MainUIView()
        {
            package = "MainUI";
            component = "MainUIView";

        }

        protected override void OnInitUI()
        {
            scShowLayer = GetChild<MainUIScShowLayer>("scShowLayer");
            danmuShowLayer = GetChild<MainUIDanmuShowLayer>("danmuShowLayer");
            okBtn = GetChild<FButton>("okBtn");


            okBtn.OnClick((context) =>
            {
                Debug.Log("Hello");
            });
        }
        protected override void OnInitEvent()
        {
            
        }

        protected override void OnEnter()
        {
            
        }

        protected override void OnExit()
        {

        }
    }
}