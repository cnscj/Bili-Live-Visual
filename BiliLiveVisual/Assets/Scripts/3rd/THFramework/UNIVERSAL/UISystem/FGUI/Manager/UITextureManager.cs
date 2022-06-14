
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FairyGUI;
using UnityEngine;
using UnityEngine.Networking;
using XLibrary;
using XLibrary.Package;

namespace THGame.UI
{
    //异步加载图片资源或者预加载资源,一般一张图片一个AB包
    //离散释放,如果一次加载太多又同时释放会卡
    public static class UITextureLoadStatus
    {
        public static readonly int LOAD_ERROR = -1;
        public static readonly int NETWORK_ERROR = -2;
        public static readonly int NETWORK_TIMEOUT = -3;
        public static readonly int URL_ERROR = -4;
        public static readonly int DATA_ERROR = -5;

    }

    public class TextureCache : MonoBehaviour
    {
        public class TextureInfo
        {
            public string key;
            public int usedTimes;
            public bool isResident;
            public float lastVisitTime;
            public bool isAddByManager;

            public string path;
            public Action<TextureInfo> onRelease;

            private float m_disposeTime;    //释放时间

            public Texture texture;

            public void UpdateVisitTime()
            {
                m_disposeTime = -1f;
                lastVisitTime = Time.realtimeSinceStartup;
            }

            public void Retain()
            {
                usedTimes++;
            }

            public void Release()
            {
                usedTimes--;
                usedTimes = Math.Max(0, usedTimes);
            }

            public bool CheckDispose()
            {
                if (isResident)
                    return false;

                if (m_disposeTime > 0f)
                {
                    if (Time.realtimeSinceStartup >= m_disposeTime)
                    {
                        return true;
                    }
                }
                else
                {
                    var newUsedTimes = usedTimes;
                    if (isAddByManager) newUsedTimes--; //1是Cache的引用
                    if (newUsedTimes <= 0)
                    {
                        var timeMs = UnityEngine.Random.Range(1, 6000);
                        m_disposeTime = Time.realtimeSinceStartup + timeMs / 1000;
                    }
                }

                return false;
            }

            public void Dispose()
            {
                onRelease?.Invoke(this);
            }

        }

        public float checkFrequence = 1f;
        private Dictionary<string, TextureInfo> m_texturesDict;
        private Queue<string> m_releaseQueue;
        private float m_lastCheckTic;

        public TextureInfo Add(string key, Texture texture, bool isReplace = false)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (texture == null)
                return null;

            var dict = GetTextureDict();
            if (dict.ContainsKey(key))
            {
                if (!isReplace)
                {
                    return null;
                }
            }

            var textureInfo = new TextureInfo();
            textureInfo.key = key;
            textureInfo.texture = texture;
            textureInfo.UpdateVisitTime();
            dict[key] = textureInfo;

            return textureInfo;
        }

        public TextureInfo GetTextureInfo(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (m_texturesDict == null)
                return null;

            if (!m_texturesDict.ContainsKey(key))
                return null;

            var textureInfo = m_texturesDict[key];
            textureInfo.UpdateVisitTime();
            return textureInfo;
        }

        public Texture Get(string key)
        {
            var textureInfo = GetTextureInfo(key);
            if (textureInfo == null)
                return default;

            textureInfo.Retain();

            return textureInfo.texture;
        }

        public void Release(string key)
        {
            var textureInfo = GetTextureInfo(key);
            if (textureInfo == null)
                return;

            textureInfo.Release();
        }

        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (m_texturesDict == null)
                return false;

            return m_texturesDict.Remove(key);
        }

        public void Dispose(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (m_texturesDict == null)
                return;

            var releaseQueue = GetReleaseQueue();
            releaseQueue.Enqueue(key);
        }

        public void Clear()
        {
            if (m_texturesDict == null)
                return;

            foreach (var key in m_texturesDict.Keys)
            {
                var releaseQueue = GetReleaseQueue();
                releaseQueue.Enqueue(key);
            }
        }

        private void Update()
        {
            UpdateCheck();
            UpdateRemove();
        }

        private void UpdateCheck()
        {
            if (checkFrequence < 0)
                return;

            if (Time.realtimeSinceStartup - m_lastCheckTic < checkFrequence)
                return;

            if (m_texturesDict == null || m_texturesDict.Count <= 0)
                return;

            foreach (var dictPair in m_texturesDict)
            {
                var key = dictPair.Key;
                var textureInfo = dictPair.Value;
                if (textureInfo.CheckDispose())
                {
                    var releaseQueue = GetReleaseQueue();
                    releaseQueue.Enqueue(key);
                }
            }

            m_lastCheckTic = Time.realtimeSinceStartup;
        }

        private void UpdateRemove()
        {
            if (m_releaseQueue == null)
                return;

            while (m_releaseQueue.Count > 0)
            {
                if (m_texturesDict == null || m_texturesDict.Count <= 0)
                    continue;

                var key = m_releaseQueue.Dequeue();
                var textureInfo = GetTextureInfo(key);
                if (textureInfo == null)
                    continue;

                textureInfo.Dispose();

                m_texturesDict.Remove(key);
            }
        }

        private Dictionary<string, TextureInfo> GetTextureDict()
        {
            m_texturesDict = m_texturesDict ?? new Dictionary<string, TextureInfo>();
            return m_texturesDict;
        }

        private Queue<string> GetReleaseQueue()
        {
            m_releaseQueue = m_releaseQueue ?? new Queue<string>();
            return m_releaseQueue;
        }
    }

    //持久化-用于将图片保存到本地 
    //有可能写文件失败,或者写坏了
    public class DataPersistencer : MonoBehaviour
    {
        //启动一个写线程,专门写文件
        public class WriteRequest
        {
            public string path;
            public byte[] data;
        }

        public class FileNode
        {
            public string name;
            public string path;
            public long accessTime;
        }

        public string saveFolderName = "texture_cache";
        public int maxIdleTime = 60;                //空闲时间x关闭写线程
        public long maxCacheCount = 100;            //保留最近访问的n个文件

        private Queue<WriteRequest> m_writeQueue;
        private string m_curWritePath;
        private bool isStopThread;
        private Thread m_runWriteThread;
        private long m_lastWriteTime;
        private string m_saveFolderPath;

        private Dictionary<string, LinkedListNode<FileNode>> m_cacheFileDict;
        private LinkedList<FileNode> m_cacheFileList;


        static void WriteThreadFunc(object obj)
        {
            DataPersistencer pDataPersistencer = obj as DataPersistencer;
            pDataPersistencer.OnWriteFile();
        }

        private void Awake()
        {
            LoadCache();
        }

        public bool IsContain(string name)
        {
            if (m_cacheFileDict == null || m_cacheFileDict.Count <= 0)
            {
                return false;
            }

            var key = GetFileName(name);
            return m_cacheFileDict.ContainsKey(key);
        }

        public void ReadTexture(string name, Action<Texture> callback)
        {
            Read(name, (srcData) =>
            {
                var data = (byte[])srcData;
                var texture = new Texture2D(100, 100);
                var ret = texture.LoadImage(data);
                if (ret)
                {
                    callback?.Invoke(texture);
                }
            });
        }

        public void WriteTexture(string name, Texture texture)
        {
            var texture2d = TextureToTexture2D(texture);
            var data = texture2d.EncodeToPNG();
            Write(name, data);
        }

        public void Read(string name, Action<byte[]> callback)
        {
            if (string.IsNullOrEmpty(name))
                return;

            string loadPath = GetFilePath(name);
            if (string.Compare(loadPath, m_curWritePath) == 0)
                return;


            FileStream fileStream = new FileStream(loadPath, FileMode.Open, FileAccess.Read);
            if (fileStream != null)
            {
                var fileName = GetFileName(name);
                UpdateCache(fileName);

                BinaryReader binaryReader = new BinaryReader(fileStream);

                int fsLen = (int)fileStream.Length;
                byte[] heByte = new byte[fsLen];
                int r = binaryReader.Read(heByte, 0, heByte.Length);

                callback?.Invoke(heByte);

                binaryReader.Close();
                binaryReader.Dispose();
            }

            fileStream.Close();
            fileStream.Dispose();
        }

        public void Write(string name, byte[] data)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (data == null)
                return;

            string savePath = GetFilePath(name);
            WriteRequest request = new WriteRequest();
            request.path = savePath;
            request.data = data;

            GetWriteQueue().Enqueue(request);
            StartWriteThread();
        }

        public void Close()
        {
            StopWriteThread();
        }

        private void LoadCache()
        {
            if (maxCacheCount <= 0)
                return;

            var folderPath = GetFolderPath(); 
            if (Directory.Exists(folderPath))
            {
                DirectoryInfo direction = new DirectoryInfo(folderPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                List<FileInfo> fileList = new List<FileInfo>();
                fileList.AddRange(files);
                fileList.Sort((a, b) =>
                {
                    if (a.CreationTime < b.CreationTime) return 1;
                    else if(a.LastAccessTime == b.LastAccessTime) return 0;
                    else return -1;
                });
                foreach (var fileInfo in fileList)
                {
                    PushCache(fileInfo);
                }

            }
        }
        private void UpdateCache(string name)
        {
            if (maxCacheCount <= 0)
                return;

            if (string.IsNullOrEmpty(name))
                return;

            if (GetCacheDict().TryGetValue(name,out var listNode))
            {
                GetCacheList().Remove(listNode);
                GetCacheList().AddFirst(listNode);
            }
        }
        private void PushCache(FileInfo fileInfo)
        {
            if (maxCacheCount <= 0)
                return;

            if (fileInfo == null)
                return;

            var name = fileInfo.Name;
            if (!GetCacheDict().ContainsKey(name))
            {
                var fileNode = new FileNode();
                fileNode.name = name;
                fileNode.path = fileInfo.FullName;
                fileNode.accessTime = XTimeTools.GetTimeStamp(fileInfo.LastAccessTime);

                var listNode = GetCacheList().AddFirst(fileNode);
                GetCacheDict().Add(name, listNode);

                //如果超过限制,弹出末端文件,并移除之
                if (CheckCacheMax())
                {
                    var lastNode = GetCacheList().Last;
                    var lastFileNode = lastNode.Value;

                    GetCacheDict().Remove(lastFileNode.name);
                    GetCacheList().Remove(lastNode);

                    DoWithFile(lastFileNode);
                }
            }
        }

        private void DoWithFile(FileNode node)
        {
            var filePath = node.path;
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private bool CheckCacheMax()
        {
            return maxCacheCount > 0 && GetCacheDict().Count > maxCacheCount;
        }

        private Dictionary<string, LinkedListNode<FileNode>> GetCacheDict()
        {
            m_cacheFileDict = m_cacheFileDict ?? new Dictionary<string, LinkedListNode<FileNode>>();
            return m_cacheFileDict;
        }

        private LinkedList<FileNode> GetCacheList()
        {
            m_cacheFileList = m_cacheFileList ?? new LinkedList<FileNode>();
            return m_cacheFileList;
        }

        private string GetFolderPath()
        {
            if (string.IsNullOrEmpty(m_saveFolderPath))
            {
                m_saveFolderPath = Path.Combine(XPlatformTools.PersistentRootPath, saveFolderName);
                if (!Directory.Exists(m_saveFolderPath))
                    Directory.CreateDirectory(m_saveFolderPath);
            }

            return m_saveFolderPath;
        }

        private string GetFilePath(string name)
        {
            string newName = GetFileName(name);
            string savePath = Path.Combine(GetFolderPath(), newName);
            return savePath;
        }

        private string GetFileName(string name)
        {
            string newName = XStringTools.ToMD5(name);
            return newName;
        }
        private Queue<WriteRequest> GetWriteQueue()
        {
            m_writeQueue = m_writeQueue ?? new Queue<WriteRequest>();
            return m_writeQueue;
        }

        private void StartWriteThread()
        {
            if (m_runWriteThread == null)
            {
                Thread tw = new Thread(WriteThreadFunc);
                tw.Start(this);
                m_runWriteThread = tw;
            }

            isStopThread = false;
        }

        private void StopWriteThread()
        {
            isStopThread = true;
        }

        private void OnWriteCompleted(string filePath)
        {
            if (maxCacheCount <= 0)
                return;

            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                PushCache(fileInfo);
            }
        }

        private void OnWriteFile()
        {
            while(!isStopThread)
            {
                if (m_writeQueue != null && m_writeQueue.Count > 0)
                {
                    var writeInfo = m_writeQueue.Dequeue();
                    string srcPath = writeInfo.path;
                    string tempPath = string.Format("{0}.temp", srcPath);
                    FileStream fileStream = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write);

                    if (fileStream != null)
                    {
                        m_curWritePath = writeInfo.path;
                        BinaryWriter binaryWriter = new BinaryWriter(fileStream);

                        binaryWriter.Seek(0, SeekOrigin.Begin);
                        binaryWriter.Write(writeInfo.data, 0, writeInfo.data.Length);

                        binaryWriter.Close();
                        binaryWriter.Dispose();
                    }

                    fileStream.Close();
                    fileStream.Dispose();

                    if (File.Exists(srcPath))
                        File.Delete(srcPath);
                    File.Move(tempPath, srcPath);

                    OnWriteCompleted(srcPath);

                    m_curWritePath = null;
                    m_lastWriteTime = XTimeTools.NowTimeStamp;
                }
                else
                {
                    if (maxIdleTime >= 0)
                    {
                        if (XTimeTools.NowTimeStamp - m_lastWriteTime >= maxIdleTime)
                        {
                            break;
                        }
                    }
                    
                    Thread.Sleep(500);
                }
            }
            m_runWriteThread = null;
        }

        private Texture2D TextureToTexture2D(Texture texture)
        {
            Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
            Graphics.Blit(texture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;
        }
    }

    //用于加载网络资源
    public class NetworkCentral : MonoBehaviour
    {
        public class TaskInfo
        {
            public string path;
            public Coroutine coroutine;
            public Dictionary<int,TaskHandler> callbacks;

            public void Success(Texture texture)
            {
                if (callbacks != null)
                {
                    foreach (var handler in callbacks.Values)
                    {
                        handler.onSuccess?.Invoke(texture);
                    }
                }
            }

            public void Failed(int reason)
            {
                if (callbacks != null)
                {
                    foreach (var handler in callbacks.Values)
                    {
                        handler.onFailed?.Invoke(reason);
                    }
                }
            }
        }

        public class TaskHandler
        {
            public int id;
            public TaskInfo taskInfo;
            public Action<Texture> onSuccess;
            public Action<int> onFailed;
        }

        private int m_taskHandlerId;
        private Dictionary<int, TaskHandler> m_taskHandlerDict;
        private Dictionary<string, TaskInfo> m_taskInfoDict;
        private Queue<TaskInfo> m_taskCache;

        public int Load(string url, Action<Texture> onSuccess, Action<int> onFailed = null)
        {
            //Url地址严格大小写
            if (string.IsNullOrEmpty(url))
            {
                onFailed?.Invoke(UITextureLoadStatus.URL_ERROR);
                return -1;
            }

            TaskInfo taskInfo = null;
            if (!GetTaskDict().TryGetValue(url, out taskInfo))
            {
                taskInfo = NewTask(url);
                StartTask(taskInfo);

                GetTaskDict().Add(url, taskInfo);
            }
            TaskHandler taskHandler = NewTaskHandler(taskInfo, onSuccess, onFailed);
            return taskHandler.id;
        }

        public void Stop(int id)
        {
            if (m_taskHandlerDict == null)
                return;

            if (m_taskHandlerDict.TryGetValue(id, out var taskHandler))
            {
                var taskInfo = taskHandler.taskInfo;
                if (taskInfo.callbacks != null)
                {
                    taskInfo.callbacks.Remove(id);

                    if (taskInfo.callbacks.Count <= 0)
                    {
                        StopTask(taskInfo);
                        RemoveTask(taskInfo);
                    }
                }
            }
        }
        
        private TaskHandler NewTaskHandler(TaskInfo taskInfo, Action<Texture> onSuccess, Action<int> onFailed)
        {
            if (taskInfo == null)
                return default;

            var handler = new TaskHandler();
            var id = ++m_taskHandlerId;

            handler.id = id;
            handler.taskInfo = taskInfo;
            handler.onSuccess = onSuccess;
            handler.onFailed = onFailed;

            GetTaskHandler().Add(id, handler);
            taskInfo.callbacks[id] = handler;

            return handler;
        }

        private void RemoveTaskHandler(int id)
        {
            if (m_taskHandlerDict == null)
                return;

            if (m_taskHandlerDict.Count <= 0)
                return;

            if (m_taskHandlerDict.TryGetValue(id, out var handler))
            {
                var taskInfo = handler.taskInfo;
                if (taskInfo != null && taskInfo.callbacks != null)
                {
                    taskInfo.callbacks.Remove(id);
                }

                m_taskHandlerDict.Remove(id);
            }
        }

        private void RemoveAllTaskCallback(TaskInfo taskInfo)
        {
            var callbacks = taskInfo.callbacks;
            if (callbacks != null)
            {
                foreach(var pair in callbacks)
                {
                    m_taskHandlerDict.Remove(pair.Key);
                }
                callbacks.Clear();
            }
        }

        private TaskInfo NewTask(string path)
        {
            var taskInfo = GetTaskQueue().Count > 0 ? GetTaskQueue().Dequeue() : new TaskInfo();

            taskInfo.path = path;
            taskInfo.callbacks = taskInfo.callbacks ?? new Dictionary<int, TaskHandler>();

            return taskInfo;
        }

        private void StartTask(TaskInfo taskInfo)
        {
            taskInfo.coroutine = StartCoroutine(OnGetCoroutine(taskInfo));
        }

        private void StopTask(TaskInfo taskInfo)
        {
            StopCoroutine(taskInfo.coroutine);
        }

        private void RemoveTask(TaskInfo taskInfo)
        {
            if (m_taskInfoDict == null)
                return;

            m_taskInfoDict.Remove(taskInfo.path);
            GetTaskQueue().Enqueue(taskInfo);
        }

        private IEnumerator OnGetCoroutine(TaskInfo taskInfo)
        {
            var newUrl = taskInfo.path;
            var request = UnityWebRequest.Get(newUrl);
            request.timeout = 15;

            yield return request.SendWebRequest();
            OnRequestCallback(taskInfo, request);
        }

        private void OnRequestCallback(TaskInfo taskInfo, UnityWebRequest webRequest)
        {
            bool isSuccess = true;
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                taskInfo.Failed(UITextureLoadStatus.NETWORK_ERROR);
                isSuccess = false;
            }

            if (!webRequest.isDone)
            {
                taskInfo.Failed(UITextureLoadStatus.LOAD_ERROR);
                isSuccess = false;
            }

            if (webRequest.responseCode != 200)
            {
                taskInfo.Failed(UITextureLoadStatus.LOAD_ERROR);
                isSuccess = false;
            }

            if (isSuccess)
            {
                var srcData = webRequest.downloadHandler.data;
                var data = (byte[])srcData;
                var texture = new Texture2D(100, 100);
                var ret = texture.LoadImage(data);
                if (!ret)
                {
                    Debug.LogWarningFormat("[TextureNew]url {0} load error", taskInfo.path);
                    taskInfo.Failed(UITextureLoadStatus.DATA_ERROR);
                }else
                {
                    taskInfo.Success(texture);
                }
            }

            RemoveAllTaskCallback(taskInfo);
            RemoveTask(taskInfo);
        }

        private Dictionary<int, TaskHandler> GetTaskHandler()
        {
            m_taskHandlerDict = m_taskHandlerDict ?? new Dictionary<int, TaskHandler>();
            return m_taskHandlerDict;
        }
        private Dictionary<string, TaskInfo> GetTaskDict()
        {
            m_taskInfoDict = m_taskInfoDict ?? new Dictionary<string, TaskInfo>();
            return m_taskInfoDict;
        }

        private Queue<TaskInfo> GetTaskQueue()
        {
            m_taskCache = m_taskCache ?? new Queue<TaskInfo>();
            return m_taskCache;
        }
    }

    //AB包计数支持释放
    //不支持依赖加载
    public class BundleManager : MonoBehaviour
    {
        public class BundleInfo
        {
            public AssetBundle assetBundle;
            public HashSet<string> dependencies;

            public void Add(string assetName)
            {
                var dict = GetDependencies();
                if (!dict.Contains(assetName))
                    dict.Add(assetName);

            }

            public void Remove(string assetName)
            {
                if (dependencies == null)
                    return;

                dependencies.Remove(assetName);
            }

            public bool IsNotDepends()
            {
                if (dependencies == null)
                    return true;

                return dependencies.Count <= 0;
            }

            private HashSet<string> GetDependencies()
            {
                dependencies = dependencies ?? new HashSet<string>();
                return dependencies;
            }


        }
        public delegate void LoadCallback(UnityEngine.Object obj);
        public class LoadingInfo
        {
            public Dictionary<string, LoadCallback> loadDict;

            public void Add(string assetName, LoadCallback callback)
            {
                if (string.IsNullOrEmpty(assetName))
                    return;

                var dict = GetLoadDict();
                if (dict.ContainsKey(assetName))
                {
                    dict[assetName] += callback;
                }
                else
                {
                    dict.Add(assetName, callback);
                }
            }

            private Dictionary<string, LoadCallback> GetLoadDict()
            {
                loadDict = loadDict ?? new Dictionary<string, LoadCallback>();
                return loadDict;
            }
        }

        private Dictionary<string, BundleInfo> m_loadedBundle;
        private Dictionary<string, LoadingInfo> m_loadingBundle;

        public T LoadSync<T>(string abPath, string assetName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(abPath))
                return default;

            return OnSyncLoad<T>(abPath, assetName);
        }

        public void LoadAsync<T>(string abPath, string assetName, Action<T> onSuccess, Action<int> onFailed) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(abPath))
                return;


            if (TryGetLoadingBundle(abPath, out var loadingInfo))
            {
                //回调合并
                loadingInfo.Add(assetName, (obj) => { onSuccess?.Invoke(obj as T); });
                return;
            }

            loadingInfo = new LoadingInfo();
            loadingInfo.Add(assetName, (obj) => { onSuccess?.Invoke(obj as T); });
            GetLoadingBundleDict().Add(abPath, loadingInfo);

            StartCoroutine(OnAsyncLoad<T>(abPath, assetName));
        }

        public void Unload(string abPath)
        {
            if (string.IsNullOrEmpty(abPath))
                return;

            if (TryGetLoadedBundle(abPath, out var bundleInfo))
            {
                var ab = bundleInfo.assetBundle;
                ab.Unload(false);   //有的纹理可能加载了,但是没受池管理,不理它

                Debug.LogFormat("[TextureManager]The AssetBundle '{0}' had been unload!", abPath);
                GetLoadedBundleDict().Remove(abPath);
            }
        }

        //只有没有依赖了,才开始释放Ab
        public void TryUnload(string abPath)
        {
            if (string.IsNullOrEmpty(abPath))
                return;

            if (TryGetLoadedBundle(abPath, out var bundleInfo))
            {
                if (bundleInfo.IsNotDepends())
                {
                    Unload(abPath);
                }
            }
        }

        public void AddDepend(string abPath, string assetName)
        {
            if (string.IsNullOrEmpty(abPath))
                return;

            if (TryGetLoadedBundle(abPath, out var bundleInfo))
            {
                bundleInfo.Add(assetName);
            }
        }

        public void RemoveDepend(string abPath, string assetName)
        {
            if (string.IsNullOrEmpty(abPath))
                return;

            if (TryGetLoadedBundle(abPath, out var bundleInfo))
            {
                bundleInfo.Remove(assetName);
            }
        }

        private T OnSyncLoad<T>(string abPath, string assetName) where T : UnityEngine.Object
        {

            AssetBundle ab = null;
            if (TryGetLoadedBundle(abPath, out var bundleInfo))
            {
                ab = bundleInfo.assetBundle;
            }
            else
            {
                ab = AssetBundle.LoadFromFile(abPath);
                RecordBundle(abPath, ab);//对这个AB包保持引用
            }

            if (ab != null)
            {
                var asset = ab.LoadAsset<T>(assetName);
                if (asset != null)
                {
                    return asset;
                }
            }
            return default;
        }

        private IEnumerator OnAsyncLoad<T>(string abPath, string assetName) where T : UnityEngine.Object
        {
            AssetBundle ab = null;
            if (TryGetLoadedBundle(abPath, out var bundleInfo))
            {
                ab = bundleInfo.assetBundle;
            }
            else
            {
                var abRequest = AssetBundle.LoadFromFileAsync(abPath);
                yield return abRequest;

                ab = abRequest.assetBundle;
                RecordBundle(abPath, ab);//对这个AB包保持引用
            }

            if (ab != null)
            {
                if (TryGetLoadingBundle(abPath, out var loadingInfo))
                {
                    GetLoadingBundleDict().Remove(abPath);  //防止在遍历中对Dict操作

                    if (loadingInfo.loadDict != null)
                    {
                        foreach (var pair in loadingInfo.loadDict)
                        {
                            var callName = pair.Key;
                            var callback = pair.Value;
                            var assetRequest = ab.LoadAssetAsync<T>(assetName);
                            yield return assetRequest;

                            var asset = assetRequest.asset as T;
                            if (asset != null)
                            {
                                callback?.Invoke(asset);
                            }
                        }
                    }
                }
            }
        }

        private bool RecordBundle(string key, AssetBundle assetBundle)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (assetBundle == null)
                return false;

            var bundleDict = GetLoadedBundleDict();
            if (bundleDict.ContainsKey(key))
                return false;

            var bundleInfo = new BundleInfo();
            bundleInfo.assetBundle = assetBundle;

            bundleDict[key] = bundleInfo;
            return true;
        }

        private bool TryGetLoadedBundle(string key, out BundleInfo bundleInfo)
        {
            bundleInfo = null;
            if (m_loadedBundle == null)
                return false;

            return m_loadedBundle.TryGetValue(key, out bundleInfo);
        }

        private bool TryGetLoadingBundle(string abPath, out LoadingInfo loadingInfo)
        {
            loadingInfo = null;
            if (m_loadingBundle == null)
                return false;

            string key = abPath;
            return m_loadingBundle.TryGetValue(key, out loadingInfo);
        }

        private Dictionary<string, BundleInfo> GetLoadedBundleDict()
        {
            m_loadedBundle = m_loadedBundle ?? new Dictionary<string, BundleInfo>();
            return m_loadedBundle;
        }

        private Dictionary<string, LoadingInfo> GetLoadingBundleDict()
        {
            m_loadingBundle = m_loadingBundle ?? new Dictionary<string, LoadingInfo>();
            return m_loadingBundle;
        }

    }

    //NTex对象池
    public class NTexturePool : MonoBehaviour
    {
        public class PoolGroup
        {
            public class NTextureInfo
            {
                public NTexture ntexture;
                public Action onDispose;

            }

            public float stayTime = 120f;
            private int maxCount = 50;
            private Dictionary<int, NTextureInfo> m_ntextureMapper;
            private LinkedList<NTextureInfo> m_availableTexs;
            private float m_visitTickTime;

            public int Count()
            {
                if (m_availableTexs == null)
                    return 0;

                return m_availableTexs.Count;
            }

            public NTextureInfo Get()
            {
                if (m_availableTexs == null)
                    return null;

                if (m_availableTexs.Count <= 0)
                    return null;

                var ntextureInfo = m_availableTexs.Last.Value;
                m_availableTexs.RemoveLast();

                UpdateTick();
                return ntextureInfo;
            }

            public bool Add(NTexture ntexture)
            {
                if (ntexture == null)
                    return false;

                var ntextureInfo = CreateNTexture(ntexture);
                if (ntextureInfo == null)
                    return false;

                var code = TransNTextureID(ntexture);
                GetNTextureMap().Add(code, ntextureInfo);
                GetAvailableList().AddLast(ntextureInfo);

                UpdateTick();
                return true;
            }

            public void Release(NTexture ntexture)
            {
                if (Count() > maxCount)
                    return;

                var ntextureMap = GetNTextureMap();

                var code = TransNTextureID(ntexture);
                if (ntextureMap.TryGetValue(code, out var ntextureInfo))
                {
                    GetAvailableList().AddLast(ntextureInfo);
                    UpdateTick();
                }
                else
                {
                    Add(ntexture);
                }
            }

            public void Dispose()
            {
                if (m_availableTexs == null)
                    return;

                foreach (var ntextureInfo in m_availableTexs)
                {
                    ntextureInfo.onDispose?.Invoke();
                }
                m_ntextureMapper?.Clear();
                m_availableTexs.Clear();
            }

            public void Update()
            {
                UpdateInvalid();
            }

            public bool CheckDispose()
            {
                if (Time.realtimeSinceStartup - m_visitTickTime < stayTime)
                    return false;

                return true;
            }

            public void UpdateTick()
            {
                m_visitTickTime = Time.realtimeSinceStartup;
            }

            private void UpdateInvalid()
            {
                if (m_availableTexs == null)
                    return;

                for (LinkedListNode<NTextureInfo> iterNode = m_availableTexs.Last; iterNode != null; iterNode = iterNode.Previous)
                {
                    var ntextureInfo = iterNode.Value;
                    if (ntextureInfo.ntexture == null)
                    {
                        m_availableTexs.Remove(iterNode);
                    }

                }
            }

            private NTextureInfo CreateNTexture(NTexture ntexture)
            {
                NTextureInfo ntextureInfo = new NTextureInfo();
                ntextureInfo.ntexture = ntexture;

                return ntextureInfo;
            }

            private LinkedList<NTextureInfo> GetAvailableList()
            {
                m_availableTexs = m_availableTexs ?? new LinkedList<NTextureInfo>();
                return m_availableTexs;
            }

            private Dictionary<int, NTextureInfo> GetNTextureMap()
            {
                m_ntextureMapper = m_ntextureMapper ?? new Dictionary<int, NTextureInfo>();
                return m_ntextureMapper;
            }

            private int TransNTextureID(NTexture ntexture)
            {
                if (ntexture == null)
                    return 0;

                return ntexture.GetHashCode();
            }

        }
        private static Dictionary<int, string> s_nameMap = new Dictionary<int, string>();
        private Dictionary<string, PoolGroup> m_poolGroups;
        private Queue<string> m_removeQueue;

        public PoolGroup.NTextureInfo Get(string key)
        {
            if (m_poolGroups == null)
                return null;

            if (!m_poolGroups.ContainsKey(key))
                return null;

            var poolGroup = m_poolGroups[key];
            if (poolGroup.Count() <= 0)
                return null;

            return poolGroup.Get();
        }

        public bool Add(string key, NTexture ntexture)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (ntexture == null)
                return false;

            var ret = GetOrCreatePoolGroup(key).Add(ntexture);
            if (ret)
            {
                var code = TransNTextureID(ntexture);
                if (s_nameMap.ContainsKey(code))
                    s_nameMap.Remove(code);

                s_nameMap.Add(code, key);
            }
            return ret;
        }
        public bool Add(string key, Texture texture) { return Add(key, new NTexture(texture)); }
        public bool Add(string key, Texture texture, Rect rect) { return Add(key, new NTexture(texture, rect)); }
        public bool Add(string key, Sprite sprite) { return Add(key, new NTexture(sprite)); }

        public void Release(NTexture ntexture)
        {
            if (ntexture == null)
                return;

            if (m_poolGroups == null)
                return;

            int code = TransNTextureID(ntexture);
            string key = GetNTextureKey(code);
            if (string.IsNullOrEmpty(key))
                return;

            if (!m_poolGroups.ContainsKey(key))
                return;

            var poolGroup = m_poolGroups[key];

            poolGroup.Release(ntexture);
            s_nameMap.Remove(code);
        }

        public void Clear()
        {
            if (m_poolGroups == null)
                return;

            foreach (var poolPair in m_poolGroups)
            {
                var poolGroup = poolPair.Value;
                poolGroup.Dispose();
            }
            m_poolGroups.Clear();
            s_nameMap.Clear();
        }

        private PoolGroup GetOrCreatePoolGroup(string key)
        {
            var poolGroups = GetPoolGroups();
            if (!poolGroups.ContainsKey(key))
            {
                poolGroups[key] = new PoolGroup();
            }
            var poolGroup = poolGroups[key];
            return poolGroup;
        }

        private Dictionary<string, PoolGroup> GetPoolGroups()
        {
            m_poolGroups = m_poolGroups ?? new Dictionary<string, PoolGroup>();
            return m_poolGroups;
        }
        private Queue<string> GetRemoveQueue()
        {
            m_removeQueue = m_removeQueue ?? new Queue<string>();
            return m_removeQueue;
        }

        private int TransNTextureID(NTexture ntexture)
        {
            if (ntexture == null)
                return 0;

            return ntexture.GetHashCode();
        }

        private string GetNTextureKey(int code)
        {
            if (s_nameMap.TryGetValue(code, out var key))
            {
                return key;
            }
            return string.Empty;
        }

        private void Update()
        {
            UpdateGroup();
            UpdateRemove();
        }
        private void UpdateGroup()
        {
            if (m_poolGroups == null)
                return;

            foreach (var itPair in m_poolGroups)
            {
                var poolGroup = itPair.Value;
                poolGroup.Update();
                if (poolGroup.CheckDispose())
                {
                    GetRemoveQueue().Enqueue(itPair.Key);
                }
            }
        }

        private void UpdateRemove()
        {
            if (m_removeQueue == null)
                return;

            while (m_removeQueue.Count > 0)
            {
                var itKey = m_removeQueue.Dequeue();
                if (m_poolGroups == null || m_poolGroups.Count <= 0)
                    continue;

                if (m_poolGroups.TryGetValue(itKey, out var poolGroup))
                {
                    poolGroup.Dispose();
                    m_poolGroups.Remove(itKey);
                }
            }
        }
    }


    public class UITextureManager : MonoSingleton<UITextureManager>
    {
        public static readonly string DEFAULT_TEXTURE_KEY = "_DefaultTexture_";

        private TextureCache m_u3dTexCache;
        private NetworkCentral m_networkCentral;
        private BundleManager m_bundleManager;
        private NTexturePool m_ntexturePool;
        private DataPersistencer m_dataPersistencer;

        //Note:小图图集打包,不过应该用处不打就不写了
        private Func<string, Texture> m_customLoaderSync;              //自定义同步加载器
        private Action<string, Action<Texture>> m_customLoaderAsync;   //自定义异步加载器
        private Dictionary<string, string> m_pathDict;
        private Texture m_defaultTexture;

        public Texture DefaultTexture   //默认纹理   
        {
            get
            {
                return m_defaultTexture;
            }

            set
            {
                m_defaultTexture = value;
                AddTexture(DEFAULT_TEXTURE_KEY, m_defaultTexture, true);
            }
        }

        private void OnDestroy()
        {
            m_dataPersistencer?.Close();
        }

        public void SetCustomLoader(Func<string, Texture> syncFunc, Action<string, Action<Texture>> asyncFunc)
        {
            m_customLoaderSync = syncFunc;
            m_customLoaderAsync = asyncFunc;
        }

        public Texture GetTexture(string key)
        {
            if (m_u3dTexCache == null)
                return null;

            return m_u3dTexCache.Get(key);
        }

        public bool AddTexture(string key, Texture texture,bool isReplace = false)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (texture == null)
                return false;

            var cache = GetTextureCache();
            var textureInfo = cache.Add(key, texture, isReplace);
            textureInfo.isAddByManager = true;
            textureInfo.Retain();  //强引用

            return textureInfo != null;
        }

        public void RemoveTexture(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (m_u3dTexCache == null)
                return;

            m_u3dTexCache.Dispose(key);
        }

        public void GetOrCreateNTexture(string key, bool isAsync, Action<NTexture> onSuccess, Action<int> onFailed)
        {
            if (string.IsNullOrEmpty(key))
                return;

            var ntexturePool = GetNTexturePool();
            var ntextureInfo = ntexturePool.Get(key);
            if (ntextureInfo != null)
            {
                onSuccess?.Invoke(ntextureInfo.ntexture);
            }
            else
            {
                var texture = GetTexture(key);
                if (texture != null)
                {
                    ntexturePool.Add(key, texture);
                    ntextureInfo = ntexturePool.Get(key);
                    onSuccess?.Invoke(ntextureInfo.ntexture);
                }
                else
                {
                    LoadTexture(key, isAsync, (textureInfo) =>
                    {
                        texture = textureInfo.texture;
                        textureInfo.Retain();

                        ntexturePool.Add(key, texture);
                        ntextureInfo = ntexturePool.Get(key);
                        ntextureInfo.onDispose = () =>
                        {
                            textureInfo.Release();
                        };
                        onSuccess?.Invoke(ntextureInfo.ntexture);
                    },(reason) =>
                    {
                        onFailed?.Invoke(reason);
                    });
                }
            }
        }

        public string ParseFormatPath(string srcUrl)
        {
            if (m_pathDict == null)
                return srcUrl;

            if (string.IsNullOrEmpty(srcUrl))
                return srcUrl;

            int protocolStartPos = srcUrl.IndexOf("://");
            if (protocolStartPos <= 0)
                return srcUrl;

            int protocolEndPos = protocolStartPos + "://".Length;
            string protocolStr = srcUrl.Substring(0, protocolStartPos);

            if (m_pathDict.TryGetValue(protocolStr,out var format))
            {
                string assetPath = srcUrl.Substring(protocolEndPos, srcUrl.Length - protocolEndPos);
                string buildStr = format;

                string parentPath = Path.GetDirectoryName(assetPath);
                string fineNameNotEx = Path.GetFileNameWithoutExtension(assetPath);

                buildStr = buildStr.Replace("{path}", assetPath);
                buildStr = buildStr.Replace("{folder}", parentPath);
                buildStr = buildStr.Replace("{fileNameNotEx}", fineNameNotEx);
                buildStr = buildStr.Replace("{protocol}", protocolStr);
                return buildStr;
            }
            return srcUrl;
        }

        public void AddFormatPath(string prefix, string format)
        {
            if (string.IsNullOrEmpty(prefix))
                return;

            m_pathDict = m_pathDict ?? new Dictionary<string, string>();
            m_pathDict[prefix] = format;
        }

        public void ReleaseNTexture(NTexture ntexture)
        {
            var ntexturePool = GetNTexturePool();
            ntexturePool.Release(ntexture);
        }

        public void LoadTexture(string path, bool isAsync, Action<TextureCache.TextureInfo> onSuccess, Action<int> onFailed)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (m_u3dTexCache != null)
            {
                var tetureInfo = m_u3dTexCache.GetTextureInfo(path);
                if (tetureInfo != null)
                {
                    OnLoadCallbackSuccess(true, path, tetureInfo.texture, onSuccess);
                    return;
                }
            }

            if (IsUrl(path))
            {
                if (GetDataPersistencer().IsContain(path))
                {
                    GetDataPersistencer().ReadTexture(path, (texture2d) =>
                    {
                        OnLoadCallbackSuccess(true, path, texture2d, onSuccess);
                    });
                }
                else
                {
                    GetNetworkCentral().Load(path, (texture2d) =>
                    {
                        var textureInfo = OnLoadCallbackSuccess(true, path, texture2d, onSuccess);
                        OnDataCacheCallback(path, textureInfo);
                    }, (reason) =>
                    {
                        OnLoadCallbackFailed(reason, onFailed, onSuccess);
                    });
                }
            }
            else
            {
                if (isAsync)
                {
                    if (m_customLoaderAsync != null)
                    {
                        m_customLoaderAsync(path, (texture2d) =>
                        {
                            OnLoadCallbackSuccess(true, path, texture2d, onSuccess);
                        });
                    }
                    else
                    {
                        if (SplitePath(path, out var abPath, out var assetName) >= 0)
                        {
                            GetBundleManager().LoadAsync<Texture>(abPath, assetName, (texture2d) =>
                            {
                                var textureInfo = OnLoadCallbackSuccess(true, path, texture2d, onSuccess);
                                OnManagerCallback(path, textureInfo);
                            },(reason) =>
                            {
                                OnLoadCallbackFailed(reason, onFailed, onSuccess);
                            });
                        }
                    }
                }
                else
                {
                    if (m_customLoaderSync != null)
                    {
                        var texture2d = m_customLoaderSync(path);
                        OnLoadCallbackSuccess(true, path, texture2d, onSuccess);
                    }
                    else
                    {
                        if (SplitePath(path, out var abPath, out var assetName) >= 0)
                        {
                            var texture2d = GetBundleManager().LoadSync<Texture>(abPath, assetName);

                            var textureInfo = OnLoadCallbackSuccess(true, path, texture2d, onSuccess);
                            OnManagerCallback(path, textureInfo);
                        }
                    }
                }
            }
        }

        private TextureCache.TextureInfo OnLoadCallbackSuccess(bool isUseCache, string path, Texture texture2d, Action<TextureCache.TextureInfo> onSuccess)
        {
            if (texture2d == null)
                return null;

            var cache = GetTextureCache();
            TextureCache.TextureInfo textureInfo = null;
            cache.Add(path, texture2d);
            textureInfo = cache.GetTextureInfo(path);
            if (!isUseCache)
            {
                cache.Remove(path);
            }

            onSuccess?.Invoke(textureInfo);

            return textureInfo;
        }

        private void OnLoadCallbackFailed(int reason, Action<int> onFailed, Action<TextureCache.TextureInfo> onSuccess)
        {
            if (DefaultTexture == null)
            {
                onFailed?.Invoke(reason);
            }
            else//使用默认贴图
            {
                var cache = GetTextureCache();
                TextureCache.TextureInfo  textureInfo = cache.GetTextureInfo(DEFAULT_TEXTURE_KEY);
                onSuccess?.Invoke(textureInfo);
            }
        }

        private void OnDataCacheCallback(string path, TextureCache.TextureInfo textureInfo)
        {
            if (textureInfo == null)
                return;

            if (string.IsNullOrEmpty(path))
                return;

            var dataCache = GetDataPersistencer();
            var texture = textureInfo.texture;

            dataCache.WriteTexture(path, texture);
        }

        private void OnManagerCallback(string path, TextureCache.TextureInfo textureInfo)
        {
            if (textureInfo == null)
                return;

            if(string.IsNullOrEmpty(path))
                return;

            textureInfo.path = path;
            textureInfo.onRelease = OnReleaseTexture;
            if (SplitePath(textureInfo.path, out var abPath, out var assetName) >= 0)
            {
                GetBundleManager().AddDepend(abPath, assetName);
            }
        }

        private void OnReleaseTexture(TextureCache.TextureInfo textureInfo)
        {
            //释放,如果ab没有依赖了释放Ab
            if (SplitePath(textureInfo.path, out var abPath, out var assetName) >= 0)
            {
                GetBundleManager().RemoveDepend(abPath, assetName);
                GetBundleManager().TryUnload(abPath);
            }
        }

        private string CombinePath(string abPath,string assetName)
        {
            return string.Format("{0}|{1}", abPath, assetName);
        }

        private int SplitePath(string path,out string abPath,out string assetName)
        {
            abPath = path;
            assetName = null;

            if (string.IsNullOrEmpty(path))
                return -1;

            if (path.IndexOf("|") > 0)
            {
                string[] pathPairs = path.Split('|');
                abPath = pathPairs[0];
                assetName = pathPairs[1];

                return pathPairs.Length;
            }

            return 0;
        }

        private bool IsUrl(string str)
        {
            try
            {
                string Url = @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$";
                return System.Text.RegularExpressions.Regex.IsMatch(str, Url);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public TextureCache GetTextureCache()
        {
            if (m_u3dTexCache == null)
            {
                GameObject cacheGobj = new GameObject("TextureCache");
                cacheGobj.transform.SetParent(transform, false);
                m_u3dTexCache = cacheGobj.AddComponent<TextureCache>();
            }
            return m_u3dTexCache;
        }

        public NetworkCentral GetNetworkCentral()
        {
            if (m_networkCentral == null)
            {
                GameObject newworkGobj = new GameObject("NetworkCentral");
                newworkGobj.transform.SetParent(transform, false);
                m_networkCentral = newworkGobj.AddComponent<NetworkCentral>();
            }
            return m_networkCentral;
        }

        public BundleManager GetBundleManager()
        {
            if (m_bundleManager == null)
            {
                GameObject managerGobj = new GameObject("BundleManager");
                managerGobj.transform.SetParent(transform, false);
                m_bundleManager = managerGobj.AddComponent<BundleManager>();
            }
            return m_bundleManager;
        }

        public NTexturePool GetNTexturePool()
        {
            if (m_ntexturePool == null)
            {
                GameObject managerGobj = new GameObject("NTexturePool");
                managerGobj.transform.SetParent(transform, false);
                m_ntexturePool = managerGobj.AddComponent<NTexturePool>();
            }
            return m_ntexturePool;
        }

        public DataPersistencer GetDataPersistencer()
        {
            if (m_dataPersistencer == null)
            {
                GameObject managerGobj = new GameObject("DataPersistencer");
                managerGobj.transform.SetParent(transform, false);
                m_dataPersistencer = managerGobj.AddComponent<DataPersistencer>();
            }
            return m_dataPersistencer;
        }
    }
}