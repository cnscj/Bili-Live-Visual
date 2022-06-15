using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
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

            RecordDanmuku();
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
                    uid = data.uid,
                    username = data.nick,
                    content = data.content,
                });

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
    }

}
