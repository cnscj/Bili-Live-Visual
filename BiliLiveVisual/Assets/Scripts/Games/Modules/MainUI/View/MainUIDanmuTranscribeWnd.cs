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
        int playCurNum;
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
                var frame = comp.GetChild<FLabel>("frame");
                var raw = data.msg.raw;

                cType.SetSelectedIndex(data.type - 1);
                if (data.msg.raw.cmd == BiliLiveDanmakuCmd.DANMU_MSG)
                {
                    var content = data.msg.raw as BiliLiveDanmakuData.DanmuMsg;
                    text.SetText(string.Format("[DM]{0}({1}):{2}",  content.nick, content.uid, content.content));
                    frame.SetText(string.Format("{0}({1})", data.msg.frame, index+1));
                }
                else if (data.msg.raw.cmd == BiliLiveDanmakuCmd.SUPER_CHAT_MESSAGE)
                {
                    var content = data.msg.raw as BiliLiveDanmakuData.SuperChatMessage;
                    text.SetText(string.Format("[SC]{0}({1}):{2}", data.msg.frame, content.uname, content.uid, content.message));
                    frame.SetText(string.Format("{0}({1})", data.msg.frame,index+1));
                }

            });

            s_danmakuPlayer.PlayMessage((msg) =>
            {
                frameText.SetText( string.Format("{0}({1})", (XTimeTools.GetDateTime(s_danmakuPlayer.GetRecordMsg().createDate + (int)s_danmakuPlayer.GetPlayCurFrame()/100).ToLongTimeString()), s_danmakuPlayer.GetPlayCurFrame().ToString()));
                playCurNum++;
                if (msg.raw.cmd == BiliLiveDanmakuCmd.DANMU_MSG)
                {
                    EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_DANMU_MSG, msg.raw);
                }
                else if(msg.raw.cmd == BiliLiveDanmakuCmd.SUPER_CHAT_MESSAGE)
                {
                    EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_SUPER_CHAT_MESSAGE, msg.raw);
                }
                
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
                    playCurNum = 0;
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
            AddEventListener(EventType.BILILIVE_DANMU_MSG, OnBiliMsg);
            AddEventListener(EventType.BILILIVE_SUPER_CHAT_MESSAGE, OnBiliMsg);
        }

        protected void OnBiliMsg(EventContext context)
        {
            var recordState = cIsRecording.GetSelectedName();
            var playingState = cIsPlaying.GetSelectedName();

            if (recordState == "yes")
            {
                var data = context.GetArg<BiliLiveDanmakuData.Raw>();
                DanmakuFormatMsg msg = new DanmakuFormatMsg()
                {
                    raw = data
                }; 
               
                if (msg != null)
                {
                    s_danmakuPlayer.RecordMessage(msg);
                    formatMsgList.Add(new ListData() { type = 2, msg = msg });
                    frameText.SetText(s_danmakuPlayer.GetRecordCurFrame().ToString());
                    UpdateList();
                }

            }
            else if(playingState == "yes")
            {
                infoList.ScrollToView(playCurNum);
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