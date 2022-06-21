using System;
using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEngine;
using XLibrary.Package.MVC;

namespace BLVisual
{
    public class MainUIControllerLayer : FWidget
    {
        FButton showScBtn;
        FButton showDanmuBtn;

        FComboBox roomCombo;
        FButton connectBtn;
        FButton disconnectBtn;

        FList funcList;
        protected override void OnInitUI()
        {
            showScBtn = GetChild<FButton>("showScBtn");
            showDanmuBtn = GetChild<FButton>("showDanmuBtn");
            roomCombo = GetChild<FComboBox>("roomCombo");
            connectBtn = GetChild<FButton>("connectBtn");
            disconnectBtn = GetChild<FButton>("disconnectBtn");
            funcList = GetChild<FList>("funcList");

            roomCombo.SetState((index, data) =>
            {
                var newData = (Dictionary<string, object>)data;
                return newData["showStr"].ToString();
            });
            roomCombo.OnChanged(() =>
            {
                var data = roomCombo.GetSelectedData();
                var index = roomCombo.GetSelectedIndex();
                var newData = (Dictionary<string, object>)data;
                roomCombo.SetText(newData["putStr"].ToString());
            });
            connectBtn.OnClick(() =>
            {
                var data = roomCombo.GetSelectedData();
                var newData = (Dictionary<string, object>)data;
                var roomId = int.Parse(newData["putStr"].ToString());

                var ctrl = Controller.Get<MainUIController>();
                ctrl.StartBiliClient(roomId);
            });

            disconnectBtn.OnClick(() =>
            {
                var ctrl = Controller.Get<MainUIController>();
                ctrl.StopBiliClient();
            });

            funcList.SetVirtual();
            funcList.SetState<Dictionary<string, object>,FButton>((index,data,comp) =>
            {

                comp.SetText(data["text"].ToString());
                comp.OnClick((FairyGUI.EventCallback0)data["onClick"]);

            });

            funcList.SetDataProvider(P_CtrlView.funcArray);
            roomCombo.SetDataProvider(ConstVars.DefaultRooms);
            roomCombo.SetSelectedIndex(0);
        }


        public void UpdateLayer()
        {
            showScBtn.SetText(Language.GetString(10201));
            showDanmuBtn.SetText(Language.GetString(10202));


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