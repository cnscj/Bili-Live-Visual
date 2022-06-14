using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using XLibrary.Package;

/*
 * GComponent 1615行插入 UIPropertyManager.GetInstance().RegisterGObject(this,target,value);
 * GComponent 101行插入 UIPropertyManager.GetInstance().UnregisterGObject(this);
 * */
namespace THGame.UI
{
    public class CustomPropertyManager : Singleton<CustomPropertyManager>
    {
        public class CustomData
        {
            private Dictionary<string, string> m_propertys;

            public void AddProperty(string key, string value, bool isReplace = true)
            {
                var dict = GetPropertys();
                if (dict.ContainsKey(key))
                {
                    if (!isReplace)
                    {
                        return;
                    }
                }
                dict[key] = value;
            }

            public string GetProperty(string key)
            {
                if (m_propertys == null)
                    return null;

                if (m_propertys.TryGetValue(key, out var value))
                {
                    return value;
                }

                return null;
            }

            public bool HasProperty(string key)
            {
                if (m_propertys == null)
                    return false;

                return m_propertys.ContainsKey(key);
            }

            public void Clear()
            {
                m_propertys?.Clear();
            }

            private Dictionary<string, string> GetPropertys()
            {
                m_propertys = m_propertys ?? new Dictionary<string, string>();
                return m_propertys;
            }
        }

        private Dictionary<string, CustomData> m_masterDict;

        public bool IsEnabled { get; set; }
        public bool IsUseId { get; set; }
       
        public CustomPropertyManager()
        {
            IsEnabled = true;
        }

        public void RegisterGObject(GObject gObj, string path, string value)
        {
            if (!IsEnabled)
                return;

            if (gObj == null)
                return;

            if (string.IsNullOrEmpty(path))
                return;

            string[] arr = path.Split('.');
            int cnt = arr.Length;
            GObject finalGObj = gObj;
            GComponent finalGComp = finalGObj.asCom;
            string key = path;
            for (int i = 0; i < cnt; ++i)
            {
                key = arr[i];

                var obj = finalGComp.GetChild(arr[i]);
                if (obj == null)
                    break;

                finalGObj = obj;
                finalGComp = finalGObj.asCom;
            }

            var masterKey = GetMasterKey(finalGObj);

            AddProperty(masterKey, key, value);
        }

        public void UnregisterGObject(GObject gObj)
        {
            if (!IsEnabled)
                return;

            if (gObj == null)
                return;

            if (m_masterDict == null)
                return;

            var masterKey = GetMasterKey(gObj);
            RemoveMaster(masterKey);
        }

        public void AddProperty(string master, string key, string value, bool isReplace = true)
        {
            if (string.IsNullOrEmpty(master))
                return;

            if (string.IsNullOrEmpty(key))
                return;

            var masterDict = GetMasterDict();
            CustomData customData = null;
            if (!masterDict.TryGetValue(master, out customData))
            {
                customData = new CustomData();
                masterDict[master] = customData;
            }

            customData.AddProperty(key, value, isReplace);
        }

        public string GetProperty(string master, string key)
        {
            if (string.IsNullOrEmpty(master))
                return null;

            if (string.IsNullOrEmpty(key))
                return null;

            if (m_masterDict == null)
                return null;

            if (m_masterDict.TryGetValue(master, out var customData))
            {
                return customData.GetProperty(key);
            }

            return null;
        }

        public bool HasProperty(string master, string key)
        {
            if (string.IsNullOrEmpty(master))
                return false;

            if (string.IsNullOrEmpty(key))
                return false;

            if (m_masterDict == null)
                return false;

            if (m_masterDict.TryGetValue(master, out var customData))
            {
                return customData.HasProperty(key);
            }

            return false;
        }

        public string GetProperty(GObject gObj, string key)
        {
            var masterKey = GetMasterKey(gObj);
            return GetProperty(masterKey, key);
        }

        public void RemoveMaster(string master)
        {
            if (string.IsNullOrEmpty(master))
                return;

            if (m_masterDict == null)
                return;

            if (m_masterDict.TryGetValue(master, out var customData))
            {
                m_masterDict.Remove(master);
            }
        }
        public void RemoveMaster(GObject gObj)
        {
            var masterKey = GetMasterKey(gObj);
            RemoveMaster(masterKey);
        }

        public void Clear()
        {
            m_masterDict?.Clear();
        }

        private string GetMasterKey(GObject gObj)
        {
            if (gObj == null)
                return string.Empty;

            if (IsUseId) return string.Format("{0}", gObj.id);
            else return string.Format("{0}", gObj.GetHashCode());
        }

        private Dictionary<string, CustomData> GetMasterDict()
        {
            m_masterDict = m_masterDict ?? new Dictionary<string, CustomData>();
            return m_masterDict;
        }
    }

}
