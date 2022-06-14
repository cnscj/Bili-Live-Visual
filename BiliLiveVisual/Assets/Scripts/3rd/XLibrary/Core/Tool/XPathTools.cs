using System;
using System.IO;
using UnityEngine;

namespace XLibrary
{
    public static class XPathTools
    {
        //平台统一路径
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            return path.Replace(@"\", "/");
        }

        //连接路径
        public static string Combine(params string[] path)
        {
            string combinePath = Path.Combine(path);
            return NormalizePath(combinePath);
        }

        //取根目录
        public static string GetFileRootPath(string assetPath)
        {
            if (Directory.Exists(assetPath))
            {
                return assetPath;
            }
            else
            {
                string fileRootPath = Path.GetDirectoryName(assetPath);
                return fileRootPath;
            }
        }

        //取得全路径
        public static string GetFullPath(string relaPath)
        {
            if (string.IsNullOrEmpty(relaPath))
                return null;

            int length = "Assets".Length;
            return Application.dataPath + relaPath.Substring(length);

        }

        //取得相对路径
        public static string GetRelativePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return null;
            return fullPath.Replace(Application.dataPath, "Assets");
        }

        //获取上层目录
        public static string GetLastPath(string curPath)
        {
            return Path.GetDirectoryName(curPath);
        }

        //获取上层目录
        public static string GetParentPath(string curPath)
        {
            curPath = NormalizePath(curPath);
            int lastIndex = curPath.LastIndexOf("/", System.StringComparison.Ordinal);
            if (lastIndex >= 0)
            {
                string lastPath = curPath.Substring(0, lastIndex);
                return lastPath;
            }
            else
            {
                return "/";
            }
        }

        //遍历并获取一个唯一路径
        public static string GetUniquePath(string savePath)
        {
            string fileNameNotEx = Path.GetFileNameWithoutExtension(savePath);
            string fileEx = Path.GetExtension(savePath);
            string fileRootPath = Path.GetDirectoryName(savePath);

            string finalPath = savePath;
            for (int i = 0; XFileTools.Exists(finalPath); i++)
            {
                finalPath = Combine(fileRootPath, string.Format("{0}({1}){2}", fileNameNotEx, i, fileEx));
            }

            return finalPath;
        }

        //截取相对路径
        public static string SubRelativePath(string relaPath, string filePath)
        {
            relaPath = NormalizePath(relaPath);
            filePath = NormalizePath(filePath);
            int startPos = filePath.IndexOf(relaPath, StringComparison.Ordinal);
            if (startPos >= 0)
            {
                return filePath.Substring(startPos + relaPath.Length + 1);
            }
            return filePath;
        }
    }
}

