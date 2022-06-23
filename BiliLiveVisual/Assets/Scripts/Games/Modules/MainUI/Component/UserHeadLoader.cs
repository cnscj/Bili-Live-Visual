
using THGame.UI;
using XLibGame;
using XLibrary;

namespace BLVisual
{
    public class MainUIUserHeadLoader : FWidget
    {
        FLoader frame;
        FLoader icon;

        public MainUIUserHeadLoader()
        {
            package = "MainUI";
            component = "UserHeadLoader";
        }
        protected override void OnInitUI()
        {
            frame = GetChild<FLoader>("frame");
            icon = GetChild<FComponent>("head").GetChild<FLoader>("icon");
        }

        public void SetHeadData(string url)
        {
            icon.SetUrl(url);
        }
    }

}