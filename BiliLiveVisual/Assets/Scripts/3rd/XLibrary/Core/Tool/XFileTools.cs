/*************************
 * 
 * 文件操作类
 * 
 * 
 **************************/
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace XLibrary
{
	public static class XFileTools
	{

		public static bool Exists(string path)
		{
			if (string.IsNullOrEmpty(path))
				return false;
			return File.Exists(path);
		}


		public static bool WriteAllText(string path, string content)
		{
			try
			{
				File.WriteAllText(path, content);
				return true;
			}
			catch (System.Exception)
			{
				Debug.LogWarning(string.Format("error Write %s", path));
				return false;
			}
		}

		public static bool AppendAllText(string path, string content)
		{
			try
			{
				File.AppendAllText(path, content);
				return true;
			}
			catch (System.Exception)
			{
				Debug.LogWarning(string.Format("error WriteAppend %s", path));
				return false;
			}
		}

		public static string ReadAllText(string fullPath)
		{
			return File.ReadAllText(fullPath);

		}

		public static void Move(string srcPath, string destPath, bool isMoveMeta = true)
		{
			if (Exists(srcPath))
			{
				File.Move(srcPath, destPath);

				// 移动meta文件，如果存在
				if (isMoveMeta)
				{
					string metaPath = srcPath + ".meta";
					if (Exists(metaPath))
					{
						string destMetaPath = destPath + ".meta";
						Move(metaPath, destMetaPath);
					}
				}
			}
		}

		public static void MoveEx(string srcPath, string destPath)
		{
			if (Exists(srcPath))
			{
				if (Exists(destPath))
				{
					File.Delete(destPath);
				}
				File.Move(srcPath, destPath);
			}
		}

		public static void Create(string path)
		{
			if (!Exists(path))
				File.Create(path);
		}

		public static void Delete(string path)
		{
			if (Exists(path))
				File.Delete(path);
		}

		public static void Copy(string srcPath, string dstPath, bool overwrite = false)
		{
			if (Exists(srcPath))
			{
				File.Copy(srcPath, dstPath, overwrite);
			}
		}

		public static long GetLength(string filePath)
        {
			FileInfo fileInfo = new FileInfo(filePath);
			return fileInfo.Length;
		}

		public static string GetMD5(string filePath)
		{
			try
			{
				FileStream file = new FileStream(filePath, FileMode.Open);
				MD5 md5 = new MD5CryptoServiceProvider();
				byte[] retVal = md5.ComputeHash(file);
				file.Close();

				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < retVal.Length; i++)
				{
					sb.Append(retVal[i].ToString("x2"));
				}
				return sb.ToString();
			}
			catch (System.Exception ex)
			{
				Debug.Log(ex);
				return "";
			}
		}

		/// <summary>
		/// 将文件转换为byte数组
		/// </summary>
		/// <param name="path">文件地址</param>
		/// <returns>转换后的byte数组</returns>
		public static byte[] File2Bytes(string path)
		{
			if (!File.Exists(path))
			{
				return new byte[0];
			}

			FileInfo fi = new FileInfo(path);
			byte[] buff = new byte[fi.Length];

			FileStream fs = fi.OpenRead();
			fs.Read(buff, 0, Convert.ToInt32(fs.Length));
			fs.Close();

			return buff;
		}

		/// <summary>
		/// 将byte数组转换为文件并保存到指定地址
		/// </summary>
		/// <param name="buff">byte数组</param>
		/// <param name="savepath">保存地址</param>
		public static void Bytes2File(byte[] buff, string savepath)
		{
			if (File.Exists(savepath))
			{
				File.Delete(savepath);
			}

			FileStream fs = new FileStream(savepath, FileMode.CreateNew);
			BinaryWriter bw = new BinaryWriter(fs);
			bw.Write(buff, 0, buff.Length);
			bw.Close();
			fs.Close();
		}
	}

}