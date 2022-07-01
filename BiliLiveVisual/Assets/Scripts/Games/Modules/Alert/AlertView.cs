using System;
using THGame.UI;

namespace BLVisual
{

    public class AlertView : Window03
    {
        FLabel descText;
        FList btnList;
        Action onClose;
        public AlertView()
        {
            package = "Alert";
            component = "AlertView";
            _layerOrder = LayerType.Alert;
        }


        protected override void OnInitUI()
        {
            descText = GetChild<FLabel>("descText");
            btnList = GetChild<FList>("btnList");
            btnList.SetState<AlertHelper.ButtonData, FButton>((index,data,comp) =>
            {
                comp.SetText(data.title);
                comp.OnClick(()=>
                {
                    data.onClick?.Invoke(this);
                });

            });

        }

        protected override void OnEnter()
        {
            var args = GetArgs() as AlertHelper.AlertArgs;

            onClose = args.onClose;
            btnList.SetDataProvider(args.btnDatas);
            descText.SetText(args.description ?? "");
            SetTitle(args.title ?? "");
        }

        protected override void OnExit()
        {
            onClose?.Invoke();
        }
    }
}


