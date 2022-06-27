using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using THGame.UI;

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

            fileList.SetVirtual();
            fileList.SetState((index,data,comp) =>
            {
                FLabel title = comp.GetChild<FLabel>("title");
                FController cType = comp.GetController("type");
                var path = data as string;
                string fileName = Path.GetFileName(path);

                if (Directory.Exists(path))
                {
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
                var data1 = ((FairyGUI.GObject)context.data);
                var data = ((FairyGUI.GObject)context.data).data;
                var path = data as string;
                if (Directory.Exists(path))
                {
                    UpdateList(path);
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
                if(File.Exists(savePath))
                {
                    //是否覆盖
                    return;
                }
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
            files.Insert(0,"../");              //插入一个返回上一层
            fileList.SetDataProvider(files);

            var folderPath = Path.GetDirectoryName(path);
            rootPath.SetText(folderPath);

        }

        private void TraverseFiles(string dir, Action<string> callback)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }

            var fileList = Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly);

            foreach (string path in fileList)
            {
                string fullPath = path.Replace('\\', '/');

                callback?.Invoke(fullPath);
            }
        }

        protected T SafeGetArgs<T>(string key, T def = default)
        {
            var arg = GetArgs();
            if (arg != null)
            {
                var dict = arg as Dictionary<string, object>;
                if (dict != null)
                {
                    if (dict.TryGetValue(key,out var val))
                    {
                        return (T)val;
                    }
                }
            }
            return def;
        }

        protected override void OnEnter()
        {
            string path = SafeGetArgs<string>("path", "/Volumes/Data");
            _onCallback = SafeGetArgs<Action<string>>("onCallback");
            _method = SafeGetArgs<Method>("method");

            cType.SetSelectedIndex((int)_method);
            UpdateList(path);
        }

        protected override void OnExit()
        {

        }
    }
}