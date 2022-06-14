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
            UIManager.SetPackageLoader(new THGame.PackageLoader((packageName) =>
            {
                return string.Format("UI/{0}", packageName);
            }));

            MVCManager.RegisterCtrlAndCache<MainController, MainCache>();
        }
        void Start()
        {
            UIManager.OpenView<MainView>();
        }
    }
}
