
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    [RequireComponent(typeof(UIPanel))]
    public class FUIPanel : MonoWrapper<UIPanel>
    {
        public string packageName;
        public string componentName;

        public void CreateUI()
        {
            if (string.IsNullOrEmpty(packageName))
                return;

            UIPackageManager.GetInstance().AddPackage(packageName);
            _obj.packageName = packageName;
            _obj.componentName = componentName;

            _obj.CreateUI();
        }

        protected override void OnStart()
        {
            CreateUI();
        }

        public void CacheNativeChildrenRenderers()
        {
            _obj.CacheNativeChildrenRenderers();
        }

        public void MoveUI(Vector3 delta)
        {
            _obj.MoveUI(delta);
        }

        public Vector3 GetUIWorldPosition()
        {
            return _obj.GetUIWorldPosition();
        }

        public void SetSortingOrder(int order)
        {
            _obj.sortingOrder = order;
        }

        public int GetSortingOrder()
        {
            return _obj.sortingOrder;
        }
    }

}
