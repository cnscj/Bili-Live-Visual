using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLVisual
{
    public static class UIDUtil
    {
        private static int _eventUid = 1000;


        public static int GetEventUid()
        {
            return ++_eventUid;
        }
    }
}

