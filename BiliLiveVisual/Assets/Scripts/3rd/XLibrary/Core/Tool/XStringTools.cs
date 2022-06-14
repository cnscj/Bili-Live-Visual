/*************************
 * 
 * 字符串操作类
 * 
 **************************/
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace XLibrary
{
	public static class XStringTools
	{
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="srcStr"></param>
        /// <param name="destStr"></param>
        /// <param name="isIgnoreCase"></param>
        /// <returns></returns>
        public static string Replace(string inputStr,string srcStr,string destStr,bool isIgnoreCase = false)
        {
            RegexOptions options = RegexOptions.None;
            if (isIgnoreCase) options |= RegexOptions.IgnoreCase;

            return Regex.Replace(inputStr, srcStr, destStr, options);
        }

		/// <summary>
		/// 切分文件名，文件后缀
		/// </summary>
		/// <param name="path">全路径</param>
		/// <param name="fileName">文件名</param>
		/// <param name="ext">后缀名</param>
		/// <param name="trimExt">去除后缀文件名</param>
		public static void SplitFileName(string path, out string fileName, out string ext, out string trimExt)
		{
			fileName = "";
			ext = "";
			trimExt = "";
			int refSlash = path.LastIndexOf('/');
			if (refSlash >= 0)
				fileName = path.Substring(refSlash + 1).ToLower();
			int refPoint = path.LastIndexOf('.');
			if (refPoint >= 0)
				ext = path.Substring(refPoint + 1).ToLower();
			if (!string.IsNullOrEmpty(fileName))
				trimExt = fileName.Replace("." + ext, "");
		}

		public static string ToMD5(byte[] data)
		{
            MD5 md5 = MD5.Create();
            data = md5.ComputeHash(data);

            StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; ++i)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			return sBuilder.ToString();
		}

        public static string ToMD5(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            return ToMD5(Encoding.UTF8.GetBytes(str));
        }


        /// <summary>
        /// 提取字符串包含的Key
        /// </summary>
        /// <param name="path">文件路径/文件名</param>
        /// <returns></returns>
        public static string SplitPathKey(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            int indexOf_ = fileName.IndexOf('_');
            string pathKey = (indexOf_ == -1) ? fileName : fileName.Remove(indexOf_);

            return pathKey;
        }

        /// <summary>
        /// 提取字符串除最后一个_之前的内容
        /// </summary>
        /// <param name="path">文件路径/文件名</param>
        /// <returns></returns>
        public static string SplitPathName(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            int indexOf_ = fileName.LastIndexOf('_');
            string pathKey = (indexOf_ == -1) ? fileName : fileName.Remove(indexOf_);

            return pathKey;
        }

        /// <summary>
        /// 提取字符串包含的ID
        /// </summary>
        /// <param name="path">文件路径/文件名</param>
        /// <returns></returns>
        public static long SplitPathId(string path)
		{
            string fileName = Path.GetFileNameWithoutExtension(path);
            int indexOf_ = fileName.IndexOf('_');
            string pathId = (indexOf_ == -1) ? fileName : fileName.Remove(indexOf_);
            long iPathId;
            return !long.TryParse(pathId, out iPathId) ? -1 : iPathId;
        }


        /// <summary>
        /// 提取字符串包含的模块名
        /// </summary>
        /// <param name="path">文件路径/文件名</param>
        /// <returns></returns>
        public static string SplitPathModule(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            int indexOf_ = fileName.IndexOf('_');
            string pathModule = (indexOf_ == -1) ? fileName : fileName.Remove(indexOf_);
            
            return pathModule;
        }

        /// <summary>
        /// 移除括号及括号内容
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string RemoveBracket(string str)
        {
            var normalStr = str.Replace("（", "(").Replace("）", ")");
            var resultStr = Regex.Replace(normalStr, @"\([^\(]*\)", "");
            return resultStr;
        }
    }
}