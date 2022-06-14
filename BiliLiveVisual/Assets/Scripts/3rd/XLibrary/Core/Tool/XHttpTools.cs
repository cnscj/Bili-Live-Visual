/*************************
 * 
 * Http:Post Get
 * 
 **************************/
using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
namespace XLibrary
{
    public static class XHttpTools
    {
        /// <summary>
        /// 后台发送POST请求
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns></returns>
        public static string Post(string url, string data, string headermessage = "")
        {
            //创建post请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            AddMessage(headermessage, request);
            byte[] payload = Encoding.UTF8.GetBytes(data);
            request.ContentLength = payload.Length;
            //发送post的请求
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            try
            {
                //接受返回来的数据
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string value = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                response.Close();
                return value;
            }
            catch (WebException e)
            {
                WebResponse wr = e.Response;
                using (StreamReader reader = new StreamReader(wr.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    string value = reader.ReadToEnd();
                    return "H" + value;
                }
            }
        }
        private static void AddMessage(string headermessage, HttpWebRequest request)
        {

            if (headermessage != "")
            {
                if (headermessage.Contains("&"))
                {
                    string[] headermessagecell = headermessage.Split('&');
                    foreach (var item in headermessagecell)
                    {
                        string[] itemkeyandvalue = item.Split('=');
                        request.Headers.Add(itemkeyandvalue[0], itemkeyandvalue[1]);
                    }
                }
                else
                {
                    string[] itemkeyandvalue = headermessage.Split('=');
                    request.Headers.Add(itemkeyandvalue[0], itemkeyandvalue[1]);
                }
            }
        }

        /// <summary>
        /// 后台发送POST请求
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns></returns>
        public static string FormPost(string url, string data, string headermessage = "", string token = "")
        {
            byte[] postData = Encoding.UTF8.GetBytes(headermessage);
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            if (token != "")
            {
                try
                {
                    webClient.Headers.Add("token", token);
                    string responseData = webClient.UploadString(url, "POST", headermessage);
                    return responseData;
                }

                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    return "";
                }
            }
            else
            {
                try
                {
                    byte[] responseData = webClient.UploadData(url, "POST", postData);
                    return Encoding.UTF8.GetString(responseData);
                }

                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    return "";
                }
            }
        }

        /// <summary>
        /// 后台发送GET请求
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns></returns>
        public static string Get(string url, string data)
        {
            try
            {
                //创建Get请求
                url = url + (data == "" ? "" : "?") + data;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                //接受返回来的数据
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = streamReader.ReadToEnd();
                streamReader.Close();
                stream.Close();
                response.Close();
                return retString;
            }

            catch (Exception)
            {
                return "";
            }
        }
    }
}
