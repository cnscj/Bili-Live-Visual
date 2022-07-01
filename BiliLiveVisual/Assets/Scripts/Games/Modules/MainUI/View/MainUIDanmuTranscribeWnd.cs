using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        private static DanmakuPlayer s_danmakuPlayer = new DanmakuPlayer();

        FController cIsPlaying;
        FController cIsRecording;
        FButton newBtn;
        FButton recordBtn;
        FButton playBtn;
        FButton openBtn;
        FButton saveBtn;
        FLabel createText;
        FLabel countText;
        FLabel frameText;
        FList infoList;

        
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
            frameText = GetChild<FLabel>("frameText");
            infoList = GetChild<FList>("infoList");

            infoList.SetVirtual();
            infoList.SetState<ListData>((index,data,comp) =>
            {
                var cType = comp.GetController("type");
                var text = comp.GetChild<FLabel>("text");


                cType.SetSelectedIndex(data.type - 1);
                text.SetText(string.Format("[{0}]{1}({2}):{3}", data.msg.frame, data.msg.username, data.msg.uid, data.msg.content));
            });

            s_danmakuPlayer.PlayMessage((msg) =>
            {
                var data = new BiliLiveDanmakuData.DanmuMsg()
                {
                    uid = msg.uid,
                    nick = msg.username,
                    content = msg.content,
                    color = msg.color,
                    cmd = BiliLiveDanmakuCmd.DANMU_MSG,
                };
                frameText.SetText(s_danmakuPlayer.GetPlayCurFrame().ToString());
                EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_DANMU_MSG, data);
            });
            newBtn.OnClick(() =>
            {
                s_danmakuPlayer.StopPlay();
                s_danmakuPlayer.StopRecord();
                s_danmakuPlayer.Clear();
                formatMsgList.Clear();
                cIsRecording.SetSelectedName("no");
                cIsPlaying.SetSelectedName("no");
                frameText.SetText("");
                createText.SetText("");
                UpdateList();
            });
            recordBtn.OnClick(() =>
            {
                var recordState = cIsRecording.GetSelectedName();
                if (recordState == "yes")
                {
                    s_danmakuPlayer.StopRecord();
                    cIsRecording.SetSelectedName("no");
                }
                else if (recordState == "no")
                {
                    var roomInfo = DanmuCache.Get<DanmuCache>().roomInfo;
                    if (string.IsNullOrEmpty(roomInfo.roomTitle))
                        return;

                    var trap = s_danmakuPlayer.StartRecord();
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
                    s_danmakuPlayer.StopPlay();
                    cIsPlaying.SetSelectedName("no");
                }
                else if (playingState == "no")
                {
                    s_danmakuPlayer.StartPlay();
                    cIsPlaying.SetSelectedName("yes");
                }

            });

            openBtn.OnClick(() =>
            {
                UIManager.OpenView<FileView>(new Dictionary<string, object>()
                {
                    ["method"] = FileView.Method.Open,
                    ["onCallback"] = new Action<string>((path) =>
                    {
                        newBtn.Call();
                        s_danmakuPlayer.Load(path);
                        var playMsg = s_danmakuPlayer.GetPlayMsgs();
                        var recordMsg = s_danmakuPlayer.GetRecordMsg();
                        formatMsgList.Clear();

                        foreach (var msg in recordMsg.msgs)
                        {
                            formatMsgList.Add(new ListData()
                            {
                                type = 1,
                                msg = msg,
                            });
                        }
                        createText.SetText(XTimeTools.GetDateTime((long)recordMsg.createDate).ToString());
                        UpdateList();
                        infoList.ScrollToTop();
                    }),
                });
                
            });

            saveBtn.OnClick(() =>
            {
                UIManager.OpenView<FileView>(new Dictionary<string, object>()
                {
                    ["method"] = FileView.Method.Save,
                    ["onCallback"] = new Action<string>((path) =>
                    {
                        if(File.Exists(path))
                        {
                            AlertHelper.Confirm(description: "已存在相同命名的文件,是否覆盖?", onCallback: (state) =>
                            {
                                if (state > 0)
                                {
                                    s_danmakuPlayer.Save(path);
                                    Debug.Log(path);
                                }
                            });
                        }
                        else
                        {
                            s_danmakuPlayer.Save(path);
                            Debug.Log(path);
                        }


                    }),
                });
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
                s_danmakuPlayer.RecordMessage(msg);
                formatMsgList.Add(new ListData() { type = 2, msg = msg });
                frameText.SetText(s_danmakuPlayer.GetRecordCurFrame().ToString());
                UpdateList();
            }
            else if(playingState == "yes")   //锟斤拷幕锟斤拷锟脚伙拷锟斤拷酶媒涌锟�,锟斤拷锟诫处锟斤拷
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
            cIsRecording.SetSelectedNameBoolean(s_danmakuPlayer.IsRecording());
            cIsPlaying.SetSelectedNameBoolean(s_danmakuPlayer.IsPlaying());

            var recordMsg = s_danmakuPlayer.GetRecordMsg();
            if (recordMsg != null)
            {
                foreach (var msg in recordMsg.msgs)
                {
                    formatMsgList.Add(new ListData()
                    {
                        type = s_danmakuPlayer.IsPlaying() ? 1 : 2,
                        msg = msg,
                    });
                }
                createText.SetText(XTimeTools.GetDateTime((long)recordMsg.createDate).ToString());
                UpdateList();
            }
           
        }

        protected override void OnExit()
        {

        }

    }
}