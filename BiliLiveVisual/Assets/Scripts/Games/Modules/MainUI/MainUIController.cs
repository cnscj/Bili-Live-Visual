using System.Collections;
using System.Collections.Generic;
using XLibGame;
using XLibrary.Package.MVC;

namespace BLVisual
{
    public class MainUIController : Controller
    {
        BiliLiveClient _biliClient = new BiliLiveClient();

        protected override void OnAdd()
        {
            _biliClient.listener.Clear();
            _biliClient.listener.onDataDanmuMsg = (data) =>
            {
                EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_DANMU_MSG, data);
            };
            _biliClient.listener.onDataSuperChatMessage = (data) =>
            {
                EventDispatcher.GetInstance().Dispatch(EventType.BILILIVE_SUPER_CHAT_MESSAGE, data);
            };
        }

        protected override void OnInitEvent()
        {

        }

        public void StartBiliClient(int roomId)
        {
            StopBiliClient();
            _biliClient.Start(roomId);
        }

        public void StopBiliClient()
        {
            _biliClient.Close();
        }
    }

}
