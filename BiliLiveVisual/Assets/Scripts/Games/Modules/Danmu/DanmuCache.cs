
using System.Collections.Generic;
using XLibrary.Package.MVC;
namespace BLVisual
{
    public class DanmuCache : Cache
    {
        private Queue<BiliLiveDanmakuData.DanmuMsg> _danmuQueue = new Queue<BiliLiveDanmakuData.DanmuMsg>();

        public void UpdateDanmuMsg(BiliLiveDanmakuData.DanmuMsg danmuMsg)
        {

        }
    }
}
