using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLVisual
{
    public class DanmakuFormatMsg
    {
        public int frame;
        public BiliLiveDanmakuData.Raw raw;
    }

    public class DanmakuFormatData
    {
        public int version;
        public int roomId;
        public string desc;
        public int createDate;
        public int startFrame;
        public int endFrame;
        public int msgCount;
        public DanmakuFormatMsg[] msgs;
    }
}
