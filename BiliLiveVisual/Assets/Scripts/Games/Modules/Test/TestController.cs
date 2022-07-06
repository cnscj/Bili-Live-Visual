using System;
using System.Text.RegularExpressions;
using UnityEngine;
using XLibGame;
using XLibrary;
using XLibrary.Package.MVC;

namespace BLVisual
{
    public class TestController : Controller
    {
        BiliLiveClient client = new BiliLiveClient(22625027);
        protected override void OnInitEvent()
        {
            //UnityEngine.Debug.Log("dd");

            //RecordTest();

            //RecordDanmuku();

            IsAssetBundle("ab:///Users/cnscj/Library/Containers/com.tencent.qq/Data/Documents|as.jpg", out _);
        }

        protected override void OnClear()
        {
            client.Close();
        }

        public void RecordTest()
        {
            DanmakuPlayer player = new DanmakuPlayer();
            var outData = player.StartRecord();

            player.Save(string.Format("/Volumes/Data/{0}.txt", XTimeTools.NowTimeStamp));
        }

        public void RecordDanmuku()
        {
            DanmakuPlayer player = new DanmakuPlayer();
            
            var outData = player.StartRecord();

            client.listener.Clear();
            client.listener.onRoomInfo = (info) =>
            {
                outData.roomId = info.longRoomId;
                outData.desc = info.roomTitle;
            };
            client.listener.onDataDanmuMsg = (data) =>
            {
                var text = string.Format("{0}", data.content);

                player.RecordMessage(new DanmakuFormatMsg()
                {
                    raw = data
                }); ;

                Debug.Log(text);
            };

            client.Start();
            Timer.GetInstance().ScheduleOnce(() =>
            {
                player.StopRecord();
                player.Save(string.Format("/Volumes/Data/{0}.txt", XTimeTools.NowTimeStamp));
                Debug.Log("[Record Over]");
                client.Close();
            }, 5 * 60);
        }

        private bool IsAssetBundle(string str, out string realPath)
        {
            realPath = null;
            var suffix = "ab://";
            var ret = !string.IsNullOrEmpty(str) && str.IndexOf(suffix, StringComparison.OrdinalIgnoreCase) == 0;
            if (ret)
            {
                realPath = Regex.Replace(str, suffix, "", RegexOptions.IgnoreCase);
            }
            return ret;
        }
    }

}
