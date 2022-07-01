using System;
using THGame.UI;

namespace BLVisual
{
    public static class AlertHelper
    {
        public class ButtonData
        {
            public string title;
            public Action<FView> onClick;
        }

        public class AlertArgs
        {
            public string title;
            public string description;
            public ButtonData[] btnDatas;
            public Action onClose;
        }


        public static void Show(AlertArgs args)
        {
            if (args == null)
                return;

            UIManager.OpenView<AlertView>(args);
        }

        public static void Warn(string description = default, Action onCallback = default ,string title = default)
        {
            Show(new AlertArgs()
            {
                title = title ?? "警告",
                description = description,
                btnDatas = new ButtonData[]
                {
                    new ButtonData()
                    {
                        title = "确认",
                        onClick = (view) =>
                        {
                            view.Close();
                            onCallback?.Invoke();
                        },
                    },
                },
            });
        }

        public static void Confirm(string description = default, Action<int> onCallback = default, string title = default)
        {
            Show(new AlertArgs()
            {
                title = title ?? "确认",
                description = description,
                btnDatas = new ButtonData[]
                {
                    new ButtonData()
                    {
                        title = "取消",
                        onClick = (view) =>
                        {
                            view.Close();
                            onCallback?.Invoke(-1);
                        },
                    },
                    new ButtonData()
                    {
                        title = "确定",
                        onClick = (view) =>
                        {
                            view.Close();
                            onCallback?.Invoke(1);
                        },
                    },
                },
                onClose = () =>
                {
                    onCallback?.Invoke(0);
                },
            });
        }
    }
}
