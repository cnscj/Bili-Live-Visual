
using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEngine;

namespace BLVisual
{
    public class MainUIControllerLayer : FWidget
    {
        List<Dictionary<string, object>> _funcArray = new List<Dictionary<string, object>>()
        {
            new Dictionary<string, object>(){
                ["text"] = "Í¶Æ±¼ì²â",
                ["onClick"] = new EventCallback0(()=>
                {
                    UIManager.OpenView<MainUIVoteStatisticsWnd>();
                }),
            },

        };

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
                Debug.Log(roomId);
            });
            funcList.SetState<Dictionary<string, object>,FButton>((index,data,comp) =>
            {

                comp.SetText(data["text"].ToString());
                comp.OnClick((EventCallback0)data["onClick"]);

            });

            funcList.SetDataProvider(_funcArray);
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