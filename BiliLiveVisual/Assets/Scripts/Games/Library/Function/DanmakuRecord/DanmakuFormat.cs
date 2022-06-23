using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLVisual
{
    public class DanmakuFormatMsg
    {

        public string username;
        public int uid;
        public string content;

        public int frame;
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
