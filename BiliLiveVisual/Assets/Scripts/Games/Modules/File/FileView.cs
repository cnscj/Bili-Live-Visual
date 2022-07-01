using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using THGame.UI;
using XLibrary;

namespace BLVisual
{
    public class FileView : Window03
    {
        public enum Method
        {
            Open,
            Save,
        }
        FController cType;
        FTextInput rootPath;
        FTextInput filePath;
        FList fileList;
        FButton openBtn;
        FButton saveBtn;

        Method _method;
        Action<string> _onCallback;
        public FileView()
        {
            package = "File";
            component = "FileView";
        }

        protected override void OnInitUI()
        {
            cType = GetController("type");
            rootPath = GetChild<FTextInput>("rootPath");
            filePath = GetChild<FTextInput>("filePath");
            fileList = GetChild<FList>("fileList");
            openBtn = GetChild<FButton>("openBtn");
            saveBtn = GetChild<FButton>("saveBtn");
            rootPath.SetEnabled(false);

            fileList.SetVirtual();
            fileList.SetState((index,data,comp) =>
            {
                FLabel title = comp.GetChild<FLabel>("title");
                FController cType = comp.GetController("type");
                var path = data as string;

                string fileName = Path.GetFileName(path);
                title.SetText(fileName);

                if (Directory.Exists(path))
                {
                    if (path.Substring(path.Length - 3, 3) == "../")
                        title.SetText("../");
   
                    cType.SetSelectedName("folder");
                }
                else if (File.Exists(path))
                {
                    cType.SetSelectedName("file");
                    title.SetText(fileName);
                }
 
                comp.SetData(data);
            });

            fileList.OnClickItem((context) =>
            {
                var data = ((FairyGUI.GObject)context.data).data;
                var path = data as string;
                if (Directory.Exists(path))
                {
                    if (path.Substring(path.Length - 3, 3) == "../")
                    {
                        var lastPath = XPathTools.NormalizePath(Path.GetDirectoryName(path.Substring(0,path.Length - 4)));
                        UpdateList(lastPath); 
                    }
                    else
                    {
                        UpdateList(path);
                    }
                    fileList.ClearSelection();
                }
                else if (File.Exists(path))
                {
                    string fileName = Path.GetFileName(path);
                    filePath.SetText(fileName);
                }
            });

            openBtn.OnClick(() =>
            {
                var selectedData = fileList.GetSelectedData();
                var path = selectedData as string;

                _onCallback?.Invoke(path);
                Close();
            });

            saveBtn.OnClick(() =>
            {
                var savePath = Path.Combine(rootPath.GetText(), filePath.GetText());

                _onCallback?.Invoke(savePath);
                Close();
            });

        }

        private void UpdateList(string path)
        {

            List<string> files = new List<string>();
            TraverseFiles(path, (path) =>
            {
                files.Add(path);
            });
            files.Insert(0,XPathTools.Combine(path, "../"));              //插入一个返回上一层
            fileList.SetDataProvider(files);

            rootPath.SetText(path);
        }


        private void TraverseFiles(string dir, Action<string> callback)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos("*");
            for (int i = 0; i < fileSystemInfos.Length; i++)
            {
                var file = fileSystemInfos[i];
                if (!file.Attributes.HasFlag(FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReparsePoint))
                {
                    string fullPath = file.FullName.Replace('\\', '/');
                    callback?.Invoke(fullPath);
                }
            }
        }

        protected override void OnEnter()
        {
            var dict = GetArgs() as Dictionary<string, object>;
            string path = DictUtil.SafeGetValue<string>(dict, "path", XPlatformTools.GameRootPath);
            _onCallback = DictUtil.SafeGetValue<Action<string>>(dict,"onCallback");
            _method = DictUtil.SafeGetValue<Method>(dict,"method");

            if (_method == Method.Save)
            {
                var name = DictUtil.SafeGetValue<string>(dict, "name", string.Format("{0}", XTimeTools.NowTimeStamp));
                filePath.SetText(name);
            }
            cType.SetSelectedIndex((int)_method);
            UpdateList(path);
        }

        protected override void OnExit()
        {

        }
    }
}