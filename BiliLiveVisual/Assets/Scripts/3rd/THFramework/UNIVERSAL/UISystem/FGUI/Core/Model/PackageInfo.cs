
using FairyGUI;
using UnityEngine;

namespace THGame
{
    public class PackageInfo
    {
        public UIPackage package;
        public int refCount;

        public float residentTimeS = -1;
        public float accessTimestamp;

        //刷新进入缓冲区时间
        public void UpdateTick()
        {
            accessTimestamp = Time.realtimeSinceStartup;
        }
    }
}
