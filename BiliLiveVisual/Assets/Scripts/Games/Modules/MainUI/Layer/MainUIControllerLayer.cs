using System;
using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using XLibGame;
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
        FSlider speedSlide;
        FSlider densitySlider;
        FTextInput speedInput;
        FTextInput densityInput;

        FList funcList;

        FComponent scShowLayer;
        FComponent danmuShowLayer;

        protected override void OnInitUI()
        {
            showScBtn = GetChild<FButton>("showScBtn");
            showDanmuBtn = GetChild<FButton>("showDanmuBtn");
            roomCombo = GetChild<FComboBox>("roomCombo");
            connectBtn = GetChild<FButton>("connectBtn");
            disconnectBtn = GetChild<FButton>("disconnectBtn");
            speedSlide = GetChild<FSlider>("speedSlide");
            densitySlider = GetChild<FSlider>("densitySlider");
            speedInput = GetChild<FTextInput>("speedInput");
            densityInput = GetChild<FTextInput>("densityInput");

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
                var roomIdStr = roomCombo.GetText();
                var roomId = int.Parse(roomIdStr);

                var ctrl = Controller.Get<DanmuController>();
                ctrl.StartBiliClient(roomId);
            });

            disconnectBtn.OnClick(() =>
            {
                var ctrl = Controller.Get<DanmuController>();
                ctrl.StopBiliClient();
            });

            funcList.SetVirtual();
            funcList.SetState<Dictionary<string, object>,FButton>((index,data,comp) =>
            {

                comp.SetText(data["text"].ToString());
                comp.OnClick((FairyGUI.EventCallback0)data["onClick"]);

            });

            showScBtn.OnClick(() =>
            {
                scShowLayer.SetVisible(!scShowLayer.IsVisible());
                UpdateButton();
            });

            showDanmuBtn.OnClick(() =>
            {
                danmuShowLayer.SetVisible(!danmuShowLayer.IsVisible());
                UpdateButton();
            });

            speedSlide.OnChanged((context) =>
            {
                var val = (int)speedSlide.GetValue();
                speedInput.SetText(val.ToString());
                EventDispatcher.GetInstance().Dispatch(EventType.DANMUSHOWLAYER_CHANGE_DANMU_ARGS, 1, val);
            });

            densitySlider.OnChanged((context) =>
            {
                var val = (int)densitySlider.GetValue();
                densityInput.SetText(val.ToString());
                EventDispatcher.GetInstance().Dispatch(EventType.DANMUSHOWLAYER_CHANGE_DANMU_ARGS, 2, val);
            });
            speedInput.SetEnabled(false);
            densityInput.SetEnabled(false);

            funcList.SetDataProvider(P_CtrlView.FuncArray);
            roomCombo.SetDataProvider(P_RoomIds.DefaultRooms);
            roomCombo.SetSelectedIndex(0);
        }


        public void UpdateLayer()
        {
            UpdateButton();
        }

        public void UpdateButton()
        {
            showScBtn.SetText(scShowLayer.IsVisible() ? Language.GetString(10203) : Language.GetString(10201));
            showDanmuBtn.SetText(danmuShowLayer.IsVisible() ? Language.GetString(10204) : Language.GetString(10202));
        }

        public void SetLayer(FComponent danmuLayer,FComponent scLayer)
        {
            danmuShowLayer = danmuLayer;
            scShowLayer = scLayer;
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