using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEngine;

namespace BLVisual
{
    public class MainUIView : FView
    {
        MainUIControllerLayer ctrlLayer;
        DanmuSCLayer scShowLayer;
        DanmuShowLayer danmuShowLayer;
        FButton ctrlBtn;

        public MainUIView()
        {
            package = "MainUI";
            component = "MainUIView";
        }

        protected override void OnInitUI()
        {
            ctrlLayer = GetChild<MainUIControllerLayer>("ctrlLayer");
            scShowLayer = GetChild<DanmuSCLayer>("scShowLayer");
            danmuShowLayer = GetChild<DanmuShowLayer>("danmuShowLayer");
            ctrlBtn = GetChild<FButton>("ctrlBtn");

            ctrlLayer.SetLayer(danmuShowLayer, scShowLayer);
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

        }

        protected override void OnExit()
        {

        }
    }
}