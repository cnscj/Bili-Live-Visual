using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLVisual
{
    public static class DanmakuFormat
    {
        public class Head
        {
            public int roomId;
            public int date;
            public int messageCount;
            public Message[] messages;
        }

        public class Message
        {
            public string username;
            public int uid;
            public string message;

            public int frame;
        }

    }
}
