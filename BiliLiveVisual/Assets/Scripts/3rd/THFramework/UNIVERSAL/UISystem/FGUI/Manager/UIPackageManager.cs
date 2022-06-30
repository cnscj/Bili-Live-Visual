using XLibrary.Package;
using FairyGUI;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace THGame.UI
{
    public class UIPackageManager : MonoSingleton<UIPackageManager>
    {
        public static string dependKeyName = "name";
        public static float clearCacheDuration = 1f;        //轮询频率
        public float residentTimeS = 15f;                  //默认包驻留时间
        private float m_cacheTimeTemp;

        private PackageLoader m_loader = new PackageLoader();
        private Dictionary<string, PackageInfo> m_packageMap = new Dictionary<string, PackageInfo>();
        private List<PackageInfo> m_invalidPackages = new List<PackageInfo>();
        private HashSet<string> m_refPackages = new HashSet<string>();
        private Action<PackageInfo> m_onAddedCallback;
        public UIPackageManager()
        {

        }
        public void SetLoader(PackageLoader loader)
        {
            m_loader = loader ?? m_loader;
        }

        public void OnAdded(Action<PackageInfo> func)
        {
            m_onAddedCallback = func;
        }

        //XXX:通过URL加载的也需要交由这里管理,需通过设置加载器加载
        public PackageInfo AddPackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
            {
                Debug.LogError(string.Format("[PackageManager]packageName 不能为空"));
                return null;
            }

            PackageInfo packageInfo = null;
            if (!m_packageMap.TryGetValue(packageName, out packageInfo))
            {
                if (m_loader != null)
                {
                    UIPackage package = m_loader.Load(packageName);
                    if (package != null)
                    {
                        foreach (var depList in package.dependencies)
                        {
                            foreach (var depPair in depList)
                            {
                                if (dependKeyName.Equals(depPair.Key))
                                {
                                    var depPackageInfo = AddPackage(depPair.Value);
                                    if (depPackageInfo != null)
                                    {
                                        if (depPackageInfo.residentTimeS >= 0)
                                        {
                                            Debug.LogWarning(string.Format("[PackageManager]包 {0} 引用了非常驻包 {1} 的资源", packageName, depPair.Value));
                                        }
                                    }
                                }
                            }
                        }

                        packageInfo = new PackageInfo();
                        packageInfo.package = package;
                        packageInfo.residentTimeS = residentTimeS;
                        packageInfo.accessTimestamp = Time.realtimeSinceStartup;
                        m_packageMap.Add(package.name, packageInfo);

                        m_onAddedCallback?.Invoke(packageInfo);
                        Debug.Log(string.Format("[PackageManager]包 {0} 已被成功加载", packageName));
                    }
                }
                else
                {
                    Debug.LogError(string.Format("[PackageManager]没有Loader加载器函数,请先通过SetLoader设置"));
                }
            }
            return packageInfo;
        }

        public void RemovePackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return;

            if (!m_packageMap.ContainsKey(packageName))
                return;

            UIPackage.RemovePackage(packageName);

            m_packageMap.Remove(packageName);
        }

        //包的交叉引用会有问题,这里会造成死递归,这里用Set避免之
        public void RetainPackage(string packageName)
        {
            m_refPackages.Clear();
            __RetainPackage(packageName);
        }

        public void ReleasePackage(string packageName)
        {
            m_refPackages.Clear();
            __ReleasePackage(packageName);
        }
        
        public PackageInfo GetPackageInfo(string packageName)
        {
            if (m_packageMap.ContainsKey(packageName))
            {
                return m_packageMap[packageName];
            }
            return null;
        }

        private void UpdateCache()
        {
            //如果计数小于等于0
            //轮询这些包是否超时,超时释放包
            //常驻包不释放
            foreach (var pair in m_packageMap)
            {
                var packageInfo = pair.Value;
                if (packageInfo.residentTimeS >= 0)  //常驻包不释放
                {
                    if (packageInfo.refCount <= 0)  //引用次数为0时触发倒计时(但也有可能只加载了包,没有组件)
                    {
                        if (packageInfo.accessTimestamp + packageInfo.residentTimeS <= Time.realtimeSinceStartup)   //倒计时就释放
                        {
                            m_invalidPackages.Add(pair.Value);
                        }
                        
                    }
                }
            }

            if (m_invalidPackages.Count > 0)
            {
                foreach (var packageInfo in m_invalidPackages)
                {
                    string packageName = packageInfo.package.name;
                    RemovePackage(packageName);
                    Debug.Log(string.Format("[PackageManager]无效包 {0} 已被释放", packageName));
                }
                m_invalidPackages.Clear();
            }
        }

        private void Update()
        {
            if (clearCacheDuration < 0) return;

            m_cacheTimeTemp += Time.deltaTime;

            if (m_cacheTimeTemp >= clearCacheDuration)
            {
                UpdateCache();
                m_cacheTimeTemp -= clearCacheDuration;
            }
        }

        private void __RetainPackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return;

            PackageInfo packageInfo = null;
            if (m_packageMap.TryGetValue(packageName, out packageInfo))
            {
                if (m_refPackages.Contains(packageName))
                {
                    return;
                }
                m_refPackages.Add(packageName);

                var package = packageInfo.package;
                foreach (var depList in package.dependencies)
                {
                    foreach (var depPair in depList)
                    {
                        if (dependKeyName.Equals(depPair.Key))
                        {
                            __RetainPackage(depPair.Value);

                        }
                    }
                }
                packageInfo.refCount++;
                packageInfo.UpdateTick();
            }
        }
        private void __ReleasePackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return;

            PackageInfo packageInfo = null;
            if (m_packageMap.TryGetValue(packageName, out packageInfo))
            {
                if (m_refPackages.Contains(packageName))
                {
                    return;
                }
                m_refPackages.Add(packageName);

                var package = packageInfo.package;
                foreach (var depList in package.dependencies)
                {
                    foreach (var depPair in depList)
                    {
                        if (dependKeyName.Equals(depPair.Key))
                        {
                            __ReleasePackage(depPair.Value);
                        }
                    }
                }
                packageInfo.refCount--;
                packageInfo.UpdateTick();
            }
        }

    }

}

