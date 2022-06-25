
using System;
using FairyGUI;
using UnityEngine;
using XLibrary.Package;

namespace THGame.UI
{
    
    public class UIManager : MonoSingleton<UIManager>
    {
        public FRoot Root => FRoot.GetInstance();


        private new void Awake()
        {
            base.Awake();
            SetLoaderExtension(typeof(XGLoader));
        }

        // FontManager
        public static void RegisterFont(BaseFont font)
        {
            FontManager.RegisterFont(font);
        }

        public static void UnregisterFont(BaseFont font)
        {
            FontManager.UnregisterFont(font);
        }

        public static void SetDefaultFont(string path)
        {
            UIConfig.defaultFont = path;
        }

        // ViewManager
        public static void OpenView<T>(object args = null) where T : FView, new()
        {
            ViewManager.GetInstance().Open<T>(args);
        }

        public static void CloseView<T>() where T : FView, new()
        {
            ViewManager.GetInstance().Close<T>();
        }

        public static void IsViewOpened<T>() where T : FView, new()
        {
            ViewManager.GetInstance().IsOpened<T>();
        }

        public static void SetPackageLoader(PackageLoader loader)
        {
            UIManager.GetInstance();
            UIPackageManager.GetInstance().SetLoader(loader);
        }

        // Stage
        public static void AddStageOnTouchBegin(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.Add(callback1);
        }

        public static void RemoveStageOnTouchBegin(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.Remove(callback1);
        }

        public static void AddStageOnTouchBeginCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.AddCapture(callback1);
        }

        public static void RemoveStageOnTouchBeginCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.RemoveCapture(callback1);
        }

        public static void AddStageOnTouchEndCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchEnd.AddCapture(callback1);
        }

        public static void RemoveStageOnTouchEndCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchEnd.RemoveCapture(callback1);
        }

        public static void SetStageOnKeyDown(EventCallback1 callback1)
        {
            Stage.inst.onKeyDown.Set(callback1);
        }

        public static void ClearStageOnKeyDown()
        {
            Stage.inst.onKeyDown.Clear();
        }

        public static Vector2 GetStageSize()
        {
            return Stage.inst.size;
        }

        //GRoot
        public static void SetRootContentScaleFactor(int width, int height)
        {
            GRoot.inst.SetContentScaleFactor(width, height);
        }

        public static void AddRootOnSizeChanged(EventCallback0 callback0)
        {
            GRoot.inst.onSizeChanged.Add(callback0);
        }

        public static void RemoveRootOnSizeChanged(EventCallback0 callback0)
        {
            GRoot.inst.onSizeChanged.Remove(callback0);
        }

        public static float GetRootWidth()
        {
            return GRoot.inst.width;
        }

        public static float GetRootHeight()
        {
            return GRoot.inst.height;
        }

        public void ShowPopup(FObject obj)
        {
            Root.ShowPopup(obj);
        }

        public void HidePopup(FObject obj)
        {
            Root.HidePopup(obj);
        }

        public void HidePopup()
        {
            Root.HidePopup();
        }

        //UIObjectFactory
        public void SetLoaderExtension(Type type)
        {
            UIObjectFactory.SetLoaderExtension(type);
        }
        public void SetPackageItemExtension(string url,Type type)
        {
            UIObjectFactory.SetPackageItemExtension(url,type);
        }

        //
        public string GetUIUrl(string package,string component)
        {
            return UIPackage.GetItemURL(package, component);
        }
    }
}