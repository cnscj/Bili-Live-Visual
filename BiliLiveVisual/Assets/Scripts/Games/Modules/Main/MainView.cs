using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEngine;

namespace BLVisual
{
    public class MainView : FView
    {
        FButton okBtn;

        public MainView()
        {
            package = "Main";
            component = "MainView";

        }

        protected override void OnInitUI()
        {

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