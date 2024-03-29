using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using XLibrary;

namespace BLVisual
{
    public class DanmakuPlayer
    {
        public const int RECORD_VERSION = 201;
        private Dictionary<int, List<DanmakuFormatMsg>> _playMsgs;
        private DanmakuFormatData _recordMsg;

        List<DanmakuFormatMsg> _recordList;
        Action<DanmakuFormatMsg> _receiveFunc;
        private bool _isPlaying = false;
        private int _curFrame = 0;

        public bool IsPlaying()
        {
            return _isPlaying;
        }

        public bool IsRecording()
        {
            return _recordList != null;
        }


        public void Save(string path)
        {
            if (_recordMsg == null)
                return;

            var jsonStr = JsonConvert.SerializeObject(_recordMsg, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            //将中文unicode转码,免得输出不正常
            Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
            jsonStr = reg.Replace(jsonStr, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });

            XFileTools.WriteAllText(path, jsonStr);
        }

        public void Load(string path)
        {
            var jsonStr = XFileTools.ReadAllText(path);
            _recordMsg = JsonConvert.DeserializeObject<DanmakuFormatData>(jsonStr, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            _playMsgs = RecordData2PlayData(_recordMsg);
        }

        public void StartPlay(float offset = 0)
        {
            _isPlaying = true;
            var starFrame = _recordMsg.startFrame + offset;
            _curFrame = (int)starFrame;

            PollEmit();
        }

        public int GetPlayCurFrame()
        {
            return _curFrame;
        }

        public int GetRecordCurFrame()
        {
            return GetCurFrame();
        }

        public void PlayMessage(Action<DanmakuFormatMsg> func)
        {
            _receiveFunc = func;
        }

        public void StopPlay()
        {
            _isPlaying = false;

        }

        public void Clear()
        {
            _recordMsg = null;
            _playMsgs = null;
        }
        public DanmakuFormatData StartRecord()
        {
            _recordMsg = new DanmakuFormatData();
            _recordList = new List<DanmakuFormatMsg>();
            _recordMsg.createDate = (int)XTimeTools.NowTimeStamp;
            _recordMsg.startFrame = GetCurFrame();

            return _recordMsg;
        }

        public DanmakuFormatMsg RecordMessage(DanmakuFormatMsg msg)
        {
            if (_recordList == null)
                return msg;

            var timeFrame = GetCurFrame();

            msg.frame = timeFrame;

            _recordList.Add(msg);
            return msg;
        }

        public void StopRecord()
        {
            if (_recordMsg == null)
                return;

            _recordMsg.version = RECORD_VERSION;
            _recordMsg.endFrame = GetCurFrame();
            _recordMsg.msgs = (_recordList != null) ? _recordList.ToArray() : null;
            _recordMsg.msgCount = (_recordList != null) ? _recordList.Count : 0;


            _recordList = null;
        }

        public async void PollEmit()
        {
            while (_isPlaying)
            {
                if (_playMsgs == null)
                    return;

                if (_playMsgs.TryGetValue(_curFrame, out var list))
                {
                    foreach(var msg in list)
                    {
                        _receiveFunc?.Invoke(msg);
                    }
                }
                _curFrame++;
                await Task.Delay(10);  //最小时间单位ms
            }
        }

        private int GetTime2Frame(float time)
        {
            return (int)(time * 100);
        }

        private int GetCurFrame()
        {
            return GetTime2Frame(Time.realtimeSinceStartup);
        }

        public Dictionary<int, List<DanmakuFormatMsg>> GetPlayMsgs()
        {
            return _playMsgs;
        }

        public DanmakuFormatData GetRecordMsg()
        {
            return _recordMsg;
        }

        //因为录制的不一定在msg中
        public DanmakuFormatMsg[] GetRecordMsgList()
        {
            var msg = GetRecordMsg();
            if (msg.msgs != null)
            {
                return msg.msgs;
            }
            return _recordList?.ToArray();
        }

        public Dictionary<int, List<DanmakuFormatMsg>> RecordData2PlayData(DanmakuFormatData data)
        {
            Dictionary<int, List<DanmakuFormatMsg>> playData = new Dictionary<int, List<DanmakuFormatMsg>>();
            if (data != null)
            {
                foreach (var msg in data.msgs)
                {
                    if (!playData.TryGetValue(msg.frame, out var list))
                    {
                        list = new List<DanmakuFormatMsg>();
                        playData.Add(msg.frame, list);
                    }
                    list.Add(msg);
                }

            }

            return playData;
        }

        public DanmakuFormatData PlayData2RecordData(Dictionary<int, List<DanmakuFormatMsg>> data)
        {
            DanmakuFormatData recordData = new DanmakuFormatData();
            if (data != null)
            {
                List<DanmakuFormatMsg> msgList = new List<DanmakuFormatMsg>();
                int endFrame = 0;
                foreach (var list in data.Values)
                {
                    foreach (var msg in list)
                    {
                        msgList.Add(msg);
                        if (msg.frame > endFrame)
                            endFrame = msg.frame;
                    }
                }
                recordData.startFrame = 0;
                recordData.endFrame = endFrame;
                recordData.msgs = msgList.ToArray();
                recordData.msgCount = msgList.Count;
            }
           

            return recordData;
        }
    }
}
