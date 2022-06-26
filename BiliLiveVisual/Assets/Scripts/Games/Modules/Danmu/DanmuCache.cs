
using System.Collections.Generic;
using XLibrary.Collection;
using XLibrary.Package.MVC;
namespace BLVisual
{
    public class DanmuCache : Cache
    {
        public int totalDanmuCount = 0;
        public int realDanmuCount = 0;
        public BiliLiveRoomInfo roomInfo;

        Deque<BiliLiveDanmakuData.DanmuMsg> _danmuMsgQueue = new Deque<BiliLiveDanmakuData.DanmuMsg>(100);

        public void PushRecordDanmuMsg(BiliLiveDanmakuData.DanmuMsg data)
        {
            if (_danmuMsgQueue.Count >= _danmuMsgQueue.Capacity)    //防止扩容
                _danmuMsgQueue.RemoveHead();
            _danmuMsgQueue.AddTail(data);
        }

        public Deque<BiliLiveDanmakuData.DanmuMsg> GetRecordDanmuQueue()
        {
            return _danmuMsgQueue;
        }
    }
}
