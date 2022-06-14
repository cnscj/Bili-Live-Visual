
using System;
using System.Collections.Generic;
using FairyGUI;
using XLibrary.Package;

namespace THGame.UI
{
    //TODO:
    public class PopupManager : MonoSingleton<PopupManager>
    {
        private Dictionary<string, ToolTipBase> _tipDitc;
        public PopupManager()
        {

        }

        public GComponent GetCurPopup()
        {
            return null;
        }

        public void Show<T>(PopupParams args) where T : ToolTipBase, new()
        {
            //TODO:如果上一次的和本次一样的话,就不需要再弹了
            var tipCom = GetOrCreateToolTip<T>();

            if (args != null)
            {
                tipCom.SetToolTipData(args.tipData);

            }
            UIManager.GetInstance().ShowPopup(tipCom);
        }

        public void Hide<T>() where T : ToolTipBase, new()
        {
            if (TryGetToolTip<T>(out var tipCom))
            {
                UIManager.GetInstance().HidePopup(tipCom);
            }
        }

        public bool IsShow<T>() where T : ToolTipBase, new()
        {
            return TryGetToolTip<T>(out _);
        }

        public void HideAll()
        {
            UIManager.GetInstance().HidePopup();
        }

        public void Clear()
        {
            HideAll();
        }

        private bool TryGetToolTip(string key, out ToolTipBase tipCom)
        {
            tipCom = null;
            if (string.IsNullOrEmpty(key))
                return false;

            if (_tipDitc == null)
                return false;

            return _tipDitc.TryGetValue(key, out tipCom);
        }
        private bool TryGetToolTip<T>(out ToolTipBase tipCom) where T : ToolTipBase, new()
        {
            var key = GetToolTipKey<T>();
            return TryGetToolTip(key, out tipCom);
        }

        private T GetOrCreateToolTip<T>() where T : ToolTipBase, new()
        {
            Type toolTipType = typeof(T);
            string poolName = toolTipType.FullName;

            var pool = UIPoolManager.GetInstance().GetPool(poolName);
            if (pool == null)
            {
                pool = UIPoolManager.GetInstance().CreatePool(poolName, toolTipType);
                pool.onCreate = (tipCom) =>
                {
                    tipCom.OnAddedToStage((EventContext _) =>
                    {
                        var dict = GetToolTipDict();
                        var key = GetToolTipKey<T>();
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add(key, tipCom as ToolTipBase);
                        }
                    });

                    tipCom.OnRemovedFromStage((EventContext _) =>
                    {
                        var dict = GetToolTipDict();
                        var key = GetToolTipKey<T>();
                        dict.Remove(key);

                        pool.Release(tipCom);
                    });

                };
                pool.onRelease = (fObj) => { };
            }
            return pool.GetOrCreate() as T;
        }

        private string GetToolTipKey<T>() where T : ToolTipBase, new()
        {
            Type toolTipType = typeof(T);
            string toolTipKey = toolTipType.FullName;

            return toolTipKey;
        }

        private Dictionary<string,ToolTipBase> GetToolTipDict()
        {
            _tipDitc = _tipDitc ?? new Dictionary<string, ToolTipBase>();
            return _tipDitc;
        }

    }
}

