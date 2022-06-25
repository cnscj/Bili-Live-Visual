using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace XLibGame
{
    public static class HttpUtil
    {
        public static string Unicode2String(string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase).Replace(
              source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }

        public static string Bytes2String(byte[] data)
        {
            return System.Text.Encoding.Default.GetString(data);
        }

        public static Texture Bytes2Texture(byte[] data)
        {
            Texture2D texture = new Texture2D(100,100);
            texture.LoadImage(data);

            return texture;
        }

    }

}
