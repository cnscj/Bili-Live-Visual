using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEngine;

namespace BLVisual
{
    public class MainUIView : FView
    {
        MainUIControllerLayer ctrlLayer;
        MainUIScShowLayer scShowLayer;
        MainUIDanmuShowLayer danmuShowLayer;
        FButton ctrlBtn;

        public MainUIView()
        {
            package = "MainUI";
            component = "MainUIView";

        }

        protected override void OnInitUI()
        {
            ctrlLayer = GetChild<MainUIControllerLayer>("ctrlLayer");
            scShowLayer = GetChild<MainUIScShowLayer>("scShowLayer");
            danmuShowLayer = GetChild<MainUIDanmuShowLayer>("danmuShowLayer");
            ctrlBtn = GetChild<FButton>("ctrlBtn");

            ctrlBtn.OnClick(() =>
            {
                ctrlLayer.UpdateLayer();
            });
        }
        protected override void OnInitEvent()
        {
            
        }

        protected override void OnEnter()
        {
            UIManager.OpenView<MainUIVoteStatisticsWnd>();
        }

        protected override void OnExit()
        {

        }
    }
}