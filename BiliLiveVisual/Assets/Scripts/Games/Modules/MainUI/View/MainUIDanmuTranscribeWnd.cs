using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEngine;
using XLibGame;
using XLibrary;

namespace BLVisual
{
    public class MainUIDanmuTranscribeWnd : Window03
    {
        public class ListData
        {
            public int type;
            public DanmakuFormatMsg msg;
        }
        FController cIsPlaying;
        FController cIsRecording;
        FButton newBtn;
        FButton recordBtn;
        FButton playBtn;
        FButton openBtn;
        FButton saveBtn;
        FLabel createText;
        FLabel countText;
        FList infoList;

        DanmakuPlayer danmakuPlayer = new DanmakuPlayer();
        List<ListData> formatMsgList = new List<ListData>();
        public MainUIDanmuTranscribeWnd()
        {
            package = "MainUI";
            component = "MainUIDanmuTranscribeWnd";

        }

        protected override void OnInitUI()
        {
            cIsPlaying = GetController("isPlaying");
            cIsRecording = GetController("isRecording");
            newBtn = GetChild<FButton>("newBtn");
            recordBtn = GetChild<FButton>("recordBtn");
            playBtn = GetChild<FButton>("playBtn");
            openBtn = GetChild<FButton>("openBtn");
            saveBtn = GetChild<FButton>("saveBtn");
            createText = GetChild<FLabel>("createText");
            countText = GetChild<FLabel>("countText");
            infoList = GetChild<FList>("infoList");

            infoList.SetVirtual();
            infoList.SetState<ListData>((index,data,comp) =>
            {
                var cType = comp.GetController("type");
                var text = comp.GetChild<FLabel>("text");


                cType.SetSelectedIndex(data.type - 1);
                text.SetText(string.Format("[{0}]{1}({2}):{3}", data.msg.frame, data.msg.username, data.msg.uid, data.msg.content));
            });

            danmakuPlayer.PlayMessage((msg) =>
            {
                var data = new BiliLiveDanmakuData.DanmuMsg()
                {
                    uid = msg.uid,
                    nick = msg.username,
                    content = msg.content,
                    color = msg.color,
                    cmd = BiliLiveDanmakuCmd.DANMU_MSG,
                };
                EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_DANMU_MSG, data);
                formatMsgList.Add(new ListData() { type = 1,msg = msg});
                UpdateList();
            });
            newBtn.OnClick(() =>
            {
                danmakuPlayer.StopPlay(); 
                danmakuPlayer.StopRecord();
                danmakuPlayer.Clear();
                formatMsgList.Clear();
                cIsRecording.SetSelectedName("no");
                cIsPlaying.SetSelectedName("no");
                UpdateList();
            });
            recordBtn.OnClick(() =>
            {
                var recordState = cIsRecording.GetSelectedName();
                if (recordState == "yes")
                {
                    danmakuPlayer.StopRecord();
                    cIsRecording.SetSelectedName("no");
                }
                else if (recordState == "no")
                {
                    var roomInfo = DanmuCache.Get<DanmuCache>().roomInfo;
                    if (string.IsNullOrEmpty(roomInfo.roomTitle))
                        return;

                    var trap = danmakuPlayer.StartRecord();
                    trap.roomId = roomInfo.shortRoomId;
                    trap.desc = roomInfo.roomTitle;

                    cIsRecording.SetSelectedName("yes");
                }
            });

            playBtn.OnClick(() =>
            {
                var playingState = cIsPlaying.GetSelectedName();
                if (playingState == "yes")
                {
                    danmakuPlayer.StopPlay();
                    cIsPlaying.SetSelectedName("no");
                }
                else if (playingState == "no")
                {
                    danmakuPlayer.StartPlay();
                    cIsPlaying.SetSelectedName("yes");
                }

            });

            openBtn.OnClick(() =>
            {
                //TODO:
                newBtn.Call();
                danmakuPlayer.Load(string.Format("{0}.txt",XTimeTools.NowTimeStamp));
                var playMsg = danmakuPlayer.GetPlayMsgs();
                var recordMsg = danmakuPlayer.PlayData2RecordData(playMsg);
                formatMsgList.Clear();

                foreach(var msg in recordMsg.msgs)
                {
                    formatMsgList.Add(new ListData()
                    {
                        type = 1,
                        msg = msg,
                    });
                }
            });

            saveBtn.OnClick(() =>
            {
                //TODO:
                var savePath = string.Format("{0}.txt", XTimeTools.NowTimeStamp);
                danmakuPlayer.Save(savePath);
                Debug.Log(savePath);
            });
        }


        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_DANMU_MSG, OnDanmuMsg);
        }

        protected void OnDanmuMsg(EventContext context)
        {
            var recordState = cIsRecording.GetSelectedName();
            var playingState = cIsPlaying.GetSelectedName();

            if (recordState == "yes")
            {
                var data = context.GetArg<BiliLiveDanmakuData.DanmuMsg>();
                var msg = new DanmakuFormatMsg()
                {
                    uid = data.uid,
                    username = data.nick,
                    content = data.content,
                    color = data.color,
                };
                danmakuPlayer.RecordMessage(msg);
                formatMsgList.Add(new ListData() { type = 2, msg = msg });
                UpdateList();
            }
            else if(playingState == "yes")   //弹幕播放会调用该接口,不与处理
            {
                return;
            }
        }

        private void UpdateList()
        {
            infoList.SetDataProvider(formatMsgList);
            infoList.ScrollToBottom();
            countText.SetText(formatMsgList.Count.ToString());
        }

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {

        }

    }
}