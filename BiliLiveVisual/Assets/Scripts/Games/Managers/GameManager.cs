using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using XLibrary.Package;
using XLibrary.Package.MVC;

namespace BLVisual
{
    public class GameManager : MonoSingleton<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();
            //采用Resource加载方式
            UIManager.SetDefaultFont("Font/MaoKen.ttf");
            UIManager.SetPackageLoader(new THGame.PackageLoader((packageName) =>
            {
                return string.Format("UI/{0}", packageName);
            }));
            InitPackage();
            InitMVC();
        }

        void InitPackage()
        {

        }
        void InitMVC()
        {
            MVCManager.RegisterCtrlAndCache<DanmuController, DanmuCache>();
            MVCManager.RegisterCtrlAndCache<MainUIController, MainUICache>();
            MVCManager.RegisterCtrlAndCache<TestController, TestCache>();
        }


        void Start()
        {
            UIManager.OpenView<MainUIView>();
        }

        new void OnDestroy()
        {
            MVCManager.ClearCrtlAndCache();
        }
    }
}
