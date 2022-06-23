using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using XLibGame;
using XLibrary;

namespace BLVisual
{
    public class MainUIVoteStatisticsWnd : FWindow
    {
        FComboBox keyInput;
        FComboBox limitNumInput;
        FComboBox limitTimeInput;
        FList voteList;
        FLabel cdText;
        FButton startBtn;
        FButton stopBtn;

        int cdTimerId;
        int startTimestamp;
        int countTime;
        int limitCount;

        HashSet<string> ketSet = new HashSet<string>();
        Dictionary<string, int> countMap = new Dictionary<string, int>();

        public MainUIVoteStatisticsWnd()
        {
            package = "MainUI";
            component = "MainUIVoteStatisticsWnd";

        }

        protected override void OnInitUI()
        {
            keyInput = GetChild<FComboBox>("keyInput");
            limitNumInput = GetChild<FComboBox>("limitNumInput");
            limitTimeInput = GetChild<FComboBox>("limitTimeInput");
            startBtn = GetChild<FButton>("startBtn");
            cdText = GetChild<FLabel>("cdText");
            stopBtn = GetChild<FButton>("stopBtn");
            voteList = GetChild<FList>("voteList");

            voteList.SetVirtual();
            voteList.SetState<string>((index, data, comp) =>
            {
                var voteText = comp.GetChild<FLabel>("voteText");
                var voteNum = comp.GetChild<FLabel>("voteNum");
                var voteBar = comp.GetChild<FProgressBar>("voteBar");

                var key = data;
                int curNum = 0;
                countMap.TryGetValue(key, out curNum);

                voteText.SetText(key);
                voteNum.SetText(string.Format($"{curNum}"));
                voteBar.SetValueMax(curNum, limitCount);
            });
            startBtn.OnClick(OnStart);
            stopBtn.OnClick(OnStop);
        }

        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_DANMU_MSG, OnDanmuMsg);
        }

        protected void OnDanmuMsg(EventContext context)
        {
            if (cdTimerId <= 0)
                return;

            var data = (BiliLiveDanmakuData.DanmuMsg)context.args[0];
            if (string.IsNullOrEmpty(data.content))
                return;

            var finalKey = StringUtil.StringEliminateDuplicate(data.content);
            if (ketSet.Contains(finalKey))
            {
                if (!countMap.ContainsKey(finalKey))
                    countMap.Add(finalKey, 0);

                countMap[finalKey]++;
            }
        }

        protected override void OnEnter()
        {
            cdText.SetText("");
        }

        protected override void OnExit()
        {
            StopCountTimer();
        }

        void OnStart()
        {
            var limitNumText = limitNumInput.GetText();
            var cdTimeText = limitTimeInput.GetText();
            var ketText = keyInput.GetText();
            var keyList = ketText.Split(',');

            if (string.IsNullOrEmpty(ketText))
                keyList = null;

            countTime = int.Parse(cdTimeText);
            limitCount = int.Parse(limitNumText);

            ketSet.Clear();
            countMap.Clear();

            foreach(var key in keyList)
            {
                if (!ketSet.Contains(key))
                    ketSet.Add(key);
            }

            voteList.SetDataProvider(keyList);
            StartCountTimer();
        }

        void OnStop()
        {
            StopCountTimer();
        }

        void OnTimerCallback()
        {
            var restTime = (startTimestamp + countTime) - (int)XTimeTools.NowTimeStamp;
            if (restTime < 0)
            {
                StopCountTimer();
                return;
            }
            cdText.SetText(Language.GetString(10401, string.Format("{0}", restTime)));
            voteList.RefreshVirtualList();
        }

        void StartCountTimer()
        {
            StopCountTimer();
            startTimestamp = (int)XTimeTools.NowTimeStamp;
            OnTimerCallback();
            cdTimerId = Timer.GetInstance().Schedule(() =>
            {
                OnTimerCallback();
            }, 1);
        }

        void StopCountTimer()
        {
            if (cdTimerId > 0)
            {
                Timer.GetInstance().Unschedule(cdTimerId);
                cdTimerId = 0;
            }

        }
    }
}