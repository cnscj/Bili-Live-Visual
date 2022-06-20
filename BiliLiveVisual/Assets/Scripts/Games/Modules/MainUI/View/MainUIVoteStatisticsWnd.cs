using System.Collections;
using System.Collections.Generic;
using THGame.UI;

namespace BLVisual
{
    public class MainUIVoteStatisticsWnd : FWindow
    {
        FTextInput keyInput;
        FTextInput limitNumInput;
        FTextInput limitTimeInput;
        FList voteList;

        FButton startBtn;
        FButton stopBtn;
        public MainUIVoteStatisticsWnd()
        {
            package = "MainUI";
            component = "MainUIVoteStatisticsWnd";

        }

        protected override void OnInitUI()
        {
            keyInput = GetChild<FTextInput>("keyInput");
            limitNumInput = GetChild<FTextInput>("limitNumInput");
            limitTimeInput = GetChild<FTextInput>("limitTimeInput");
            startBtn = GetChild<FButton>("startBtn");
            stopBtn = GetChild<FButton>("stopBtn");
            voteList = GetChild<FList>("voteList");

            voteList.SetState<string>((index, data, comp) =>
            {
                var voteText = comp.GetChild<FLabel>("voteText");
                var voteNum = comp.GetChild<FLabel>("voteNum");
                var voteBar = comp.GetChild<FProgressBar>("voteBar");

                var limitNum = int.Parse(limitNumInput.GetText());
                var curNum = 0;

                voteText.SetText("data");
                voteNum.SetText(string.Format($"{curNum}"));
                voteBar.SetValueMax(curNum, limitNum);
            });
            startBtn.OnClick(OnStart);
            stopBtn.OnClick(OnStop);
        }

        protected override void OnInitEvent()
        {

        }


        void OnStart()
        {
            var ketText = keyInput.GetText();
            var keyList = ketText.Split(',');
            voteList.SetDataProvider(keyList);
        }

        void OnStop()
        {
            voteList.SetDataProvider(null);
        }
    }
}