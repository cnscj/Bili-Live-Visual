using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame
{
    public class PackageLoader
    {
        public enum LoadMode
        {
            None,
            Path,
            Bundles,
            Custom,
        }
        private Func<string, string> m_loaderWithPath;
        private Func<string, Tuple<AssetBundle, AssetBundle>> m_loaderWithBundle;
        private LoadMode m_mode;

        public PackageLoader()
        {
            m_mode = LoadMode.None;
        }
        public PackageLoader(Func<string, string> loader)
        {
            m_loaderWithPath = loader;
            m_mode = LoadMode.Path;
        }
        public PackageLoader(Func<string, Tuple<AssetBundle, AssetBundle>> loader)
        {
            m_loaderWithBundle = loader;
            m_mode = LoadMode.Bundles;
        }

        public LoadMode GetLoadMode()
        {
            return m_mode;
        }

        public UIPackage Load(string packageName)
        {
            if (GetLoadMode() == PackageLoader.LoadMode.Path)
            {
                object result = m_loaderWithPath?.Invoke(packageName);
                string uiPath = result as string;
                return UIPackage.AddPackage(uiPath);

            }
            else if (GetLoadMode() == PackageLoader.LoadMode.Bundles)
            {
                var resultTuple = m_loaderWithBundle?.Invoke(packageName);
                AssetBundle binaryAb = resultTuple.Item1;
                AssetBundle altasAb = resultTuple.Item2;
                return UIPackage.AddPackage(binaryAb, altasAb);
            }
            return null;
        }

        public void LoadAsync(string packageName, Action<UIPackage> action)
        {
            UIPackage package = Load(packageName);
            action?.Invoke(package);
        }
    }

}
