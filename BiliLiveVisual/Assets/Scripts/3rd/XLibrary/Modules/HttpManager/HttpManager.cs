using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using XLibrary.Package;

namespace XLibGame
{
    public class HttpManager : MonoSingleton<HttpManager>
    {
        protected int m_coroutineId;
        private Dictionary<int, Coroutine> m_coroutines;
 
        public HttpManager()
        {
            m_coroutines = new Dictionary<int, Coroutine>();
        }

        public int Post(HttpParams args)
        {
            return StartRequest(HttpRequestMethod.Post, args);
        }

        public int Get(HttpParams args)
        {
            return StartRequest(HttpRequestMethod.Get, args);
        }

        public int Put(HttpParams args)
        {
            return StartRequest(HttpRequestMethod.Put, args);
        }

        public int Delete(HttpParams args)
        {
            return StartRequest(HttpRequestMethod.Delete, args);
        }

        public void Stop(int id)
        {
            if (m_coroutines.TryGetValue(id, out var coroutine))
            {
                StopCoroutine(coroutine);
                m_coroutines.Remove(id);
            }
        }
        public int Ping(string ipAddr, Action<float> callfunc)
        {
            var newId = ++m_coroutineId;
            var coroutine = StartCoroutine(OnPingCoroutine(newId, ipAddr, callfunc));
            m_coroutines.Add(newId, coroutine);

            return newId;
        }

        public void Clear()
        {
            foreach(var coroutine in m_coroutines.Values)
            {
                StopCoroutine(coroutine);
            }
            m_coroutines.Clear();
        }

        IEnumerator OnPingCoroutine(int id, string ipAddr, Action<float> callfunc)
        {
            Ping ping = new Ping(ipAddr);
            int addTime = 0;
            int requestCount = 20 * 10;         // 0.1秒 请求 1 次，所以请求次数是 n秒 x 10
                                               // 等待请求返回
            while (!ping.isDone)
            {
                yield return new WaitForSeconds(0.1f);
                // 链接失败
                if (addTime > requestCount)
                {
                    addTime = 0;
                    callfunc?.Invoke(ping.time);
                    yield break;
                }
                addTime++;
            }

            m_coroutines.Remove(id);

            // 链接成功
            if (ping.isDone)
            {
                callfunc?.Invoke(ping.time);
                yield return null;
            }
        }

        private int StartRequest(HttpRequestMethod requestMethod, HttpParams args)
        {
            if (args == null)
                return -1;

            if (string.IsNullOrEmpty(args.url))
                return -1;

            var newId = m_coroutineId++;
            Coroutine coroutine = null;
            switch(requestMethod)
            {
                case HttpRequestMethod.Post:
                    coroutine = StartCoroutine(OnPostCoroutine(newId, args));
                    break;
                case HttpRequestMethod.Get:
                    coroutine = StartCoroutine(OnGetCoroutine(newId, args));
                    break;
                case HttpRequestMethod.Put:
                    coroutine = StartCoroutine(OnPutCoroutine(newId, args));
                    break;
                case HttpRequestMethod.Delete:
                    coroutine = StartCoroutine(OnDeleteCoroutine(newId, args));
                    break;
            }
            m_coroutines[newId] = coroutine;
            return newId;
        }

        IEnumerator OnPostCoroutine(int id, HttpParams args)
        {
            var wwwForm = (args.formData != null) ? args.formData.ToPostData(args.url) : null;
            var request = UnityWebRequest.Post(args.url, wwwForm);
            InitRequest(request, args);

            yield return request.SendWebRequest();
            OnRequestCallback(id, request, args);
        }

        IEnumerator OnGetCoroutine(int id, HttpParams args)
        {
            var newUrl = (args.formData != null) ? args.formData.ToGetData(args.url): args.url;
            var request = UnityWebRequest.Get(newUrl);
            InitRequest(request, args);

            yield return request.SendWebRequest();
            OnRequestCallback(id, request, args);
        }

        IEnumerator OnPutCoroutine(int id, HttpParams args)
        {
            var newUrl = args.url;
            var request = UnityWebRequest.Put(newUrl, args.bodyData);
            InitRequest(request, args);

            yield return request.SendWebRequest();
            OnRequestCallback(id, request, args);
        }

        IEnumerator OnDeleteCoroutine(int id, HttpParams args)
        {
            var newUrl = args.url;
            var request = UnityWebRequest.Delete(newUrl);
            InitRequest(request, args);

            yield return request.SendWebRequest();
            OnRequestCallback(id, request, args);
        }

        private void InitRequest(UnityWebRequest webRequest, HttpParams args)
        {
            if (args.timeout > 0)
            {
                webRequest.timeout = args.timeout;
            }

        }

        void OnRequestCallback(int id, UnityWebRequest webRequest, HttpParams args)
        {
            var result = new HttpResult(webRequest);

            if (!result.IsSuccess())
            {
                args.onFailed?.Invoke(webRequest.error);
            }
            else
            {
                args.onSuccess?.Invoke(webRequest.downloadHandler.data);
            }
            args.onCallback?.Invoke(result);
            m_coroutines.Remove(id);
        }


    }
}
