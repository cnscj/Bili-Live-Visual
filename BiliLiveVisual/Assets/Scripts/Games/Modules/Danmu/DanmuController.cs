using System.Collections;
using System.Collections.Generic;
using XLibGame;
using XLibrary;
using XLibrary.Package.MVC;

namespace BLVisual
{
    public class DanmuController : Controller
    {
        BiliLiveClient _biliClient = new BiliLiveClient();

        protected override void OnAdd()
        {
            _biliClient.listener.Clear();
            _biliClient.listener.onRoomInfo = OnRoomInfo;
            _biliClient.listener.onDataDanmuMsg = OnDataDanmuMsg;
            _biliClient.listener.onDataSuperChatMessage = OnDataSuperChatMessage;
        }

        public void StartBiliClient(int roomId)
        {
            Cache.Get<DanmuCache>().totalDanmuCount = 0;
            Cache.Get<DanmuCache>().realDanmuCount = 0;
            EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_START, roomId);

            _biliClient.Start(roomId);
        }

        public void StopBiliClient()
        {
            _biliClient.Close();
            EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_STOP);
        }

        public bool IsBiliClientRunning()
        {
            return _biliClient.IsRunning();
        }

        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_DANMU_MSG, OnRecordDanmuMsg);
        }

        protected void OnRoomInfo(BiliLiveRoomInfo info)
        {
            Cache.Get<DanmuCache>().roomInfo = info;
            EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_ROOM_INFO_UPDATE, info);
        }
        protected void OnDataDanmuMsg(BiliLiveDanmakuData.DanmuMsg data)
        {
            
            Cache.Get<DanmuCache>().totalDanmuCount++;
            //需要执行过滤,黑名单等操作
            Cache.Get<DanmuCache>().realDanmuCount++;
            EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_DANMU_MSG, data);
        }

        protected void OnDataSuperChatMessage(BiliLiveDanmakuData.SuperChatMessage data)
        {
            EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_SUPER_CHAT_MESSAGE, data);
        }


        protected void OnRecordDanmuMsg(EventContext context)
        {
            var data = context.GetArg<BiliLiveDanmakuData.DanmuMsg>();
            Cache.Get<DanmuCache>().PushRecordDanmuMsg(data);
        }
     }
}
