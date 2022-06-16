using System.Collections;
using System.Collections.Generic;
using XLibrary.Package.MVC;

namespace BLVisual
{
    public class MainUIController : Controller
    {
        BiliLiveClient _biliClient = new BiliLiveClient();

        protected override void OnInitEvent()
        {
            //UnityEngine.Debug.Log("dd");
        }
    }

}
