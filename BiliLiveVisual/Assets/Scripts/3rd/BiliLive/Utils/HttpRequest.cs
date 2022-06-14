using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class HttpRequest
{
    /// <summary>
    /// 发送Get请求
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="dic">请求参数定义</param>
    /// <returns></returns>
    public static string Get(string url, Dictionary<string, string> dic = null)
    {
        string result = "";
        StringBuilder builder = new StringBuilder();
        builder.Append(url);
        if (dic != null)
        {
            if (dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
        }
        
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(builder.ToString());
        //添加参数
        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        Stream stream = resp.GetResponseStream();
        try
        {
            //获取内容
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
        }
        finally
        {
            stream.Close();
        }
        return result;
    }


    /// <summary>
    /// 指定Post地址使用Get 方式获取全部字符串
    /// </summary>
    /// <param name="url">请求后台地址</param>
    /// <returns></returns>
    public static string Post(string url, Dictionary<string, string> dic = null)
    {
        string result = "";
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded";
        #region 添加Post 参数
        StringBuilder builder = new StringBuilder();
        if (dic != null)
        {
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
        }
        
        byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
        req.ContentLength = data.Length;
        using (Stream reqStream = req.GetRequestStream())
        {
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();
        }
        #endregion
        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        Stream stream = resp.GetResponseStream();
        //获取响应内容
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        {
            result = reader.ReadToEnd();
        }
        return result;
    }


    /// <summary>
    /// 使用post方法异步请求
    /// </summary>
    /// <param name="url">目标链接</param>
    /// <param name="json">发送的参数字符串，只能用json</param>
    /// <returns>返回的字符串</returns>
    public static async Task<string> PostAsyncJson(string url, string json)
    {
        HttpClient client = new HttpClient();
        HttpContent content = new StringContent(json);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        HttpResponseMessage response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }

    /// <summary>
    /// 使用post方法异步请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    public async static Task<string> PostAsync(string url, Dictionary<string, string> dic = null)
    {
        HttpClient httpClient = new HttpClient();
        List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
        if (dic != null)
        {
            foreach(var pair in dic)
            {
                data.Add(new KeyValuePair<string, string>(pair.Key, pair.Value));
            }
        }
        var content = new FormUrlEncodedContent(data);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
        var resp = await httpClient.PostAsync(url, content);
        resp.EnsureSuccessStatusCode();

        string result = string.Empty;
        result = await resp.Content.ReadAsStringAsync();
        return result;
    }

    /// <summary>
    /// 使用get方法异步请求
    /// </summary>
    /// <param name="url">目标链接</param>
    /// <returns>返回的字符串</returns>
    public static async Task<string> GetAsync(string url, Dictionary<string, string> dic)
    {
        HttpClient hpc = new HttpClient();
        string para = "?";
        foreach (var item in dic)
        {
            para += string.Format("{0}={1}&", item.Key, item.Value);
        }
        para = para.TrimEnd('&');

        if (dic.Count == 0)
        {
            para = para.TrimEnd('?');
        }

        hpc.BaseAddress = new Uri(url);

        string getResponse = "";
        getResponse = await hpc.GetAsync(para).Result.Content.ReadAsStringAsync();
        return getResponse;
    }

}
