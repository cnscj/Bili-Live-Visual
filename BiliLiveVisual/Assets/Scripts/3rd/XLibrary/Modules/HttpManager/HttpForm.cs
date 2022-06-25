
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace XLibGame
{
    public class HttpForm
    {
        private Dictionary<string, object> m_params;

        public HttpForm()
        {

        }

        public HttpForm(Dictionary<string, object> form)
        {
            m_params = new Dictionary<string, object>();
            if (form != null)
            {
                foreach (var pair in form)
                {
                    Add(pair.Key, pair.Value);
                }
            }

        }

        public HttpForm(HttpForm form) : this(form.m_params)
        {

        }

        public object this[string key]
        {
            get
            {
                if (m_params != null && m_params.ContainsKey(key))
                    return m_params[key];
                return null;
            }
            set
            {
                m_params = m_params ?? new Dictionary<string, object>();
                m_params[key] = value;
            }
        }


        public void Add(string key, object data)
        {
            m_params = m_params ?? new Dictionary<string, object>();
            if (m_params.ContainsKey(key))
                return;

            m_params.Add(key, data);
        }

        public void Remove(string key)
        {
            if (m_params == null)
                return;

            if (string.IsNullOrEmpty(key))
                return;

            m_params.Remove(key);
        }

        public void Clear()
        {
            if (m_params == null)
                return;

            m_params.Clear();
        }

        public int Count()
        {
            if (m_params == null)
                return 0;
            return m_params.Count;
        }

        public string ToGetData(string url = null)
        {
            string ret = string.IsNullOrEmpty(url) ? "" : url;
            StringBuilder data = new StringBuilder();

            string args = ToString();
            data.Append(args);

            if (url.IndexOf("?") == -1)
            {
                url += "?";
            }
            else
            {
                url += "&";
            }

            ret = url + data.ToString();

            return ret;
        }

        public WWWForm ToPostData(string url = null)
        {
            WWWForm ret = new WWWForm();
            if (m_params != null && m_params.Count > 0)
            {
                foreach (var pair in m_params)
                {
                    if (pair.Value is byte[])
                        ret.AddBinaryData(pair.Key, pair.Value as byte[]);
                    else
                        ret.AddField(pair.Key, pair.Value.ToString());
                }
            }
            return ret;
        }

        public override string ToString()
        {
            StringBuilder data = new StringBuilder();

            if (m_params != null && m_params.Count > 0)
            {
                foreach (var pair in m_params)
                {
                    data.Append(pair.Key + "=");
                    data.Append(pair.Value.ToString() + "&");
                }
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }

    }
}