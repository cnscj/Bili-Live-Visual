using System;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class HttpRequester : MonoBehaviour
    {
        public class RequestInfo
        {
            public HttpRequestMethod method;
            public HttpParams args;

            public bool isStop;
            public int corId;
        }

        public static HttpRequester Create(float interval = 0.1f, int requestCount = 1)
        {
            GameObject gObj = new GameObject();
            var httpRequester = gObj.GetComponent<HttpRequester>() ?? gObj.AddComponent<HttpRequester>();
            httpRequester.interval = interval;
            httpRequester.requestCount = requestCount;

            return httpRequester;
        }

        public float interval = 0.1f;
        public int requestCount = 1;
        
        private Queue<RequestInfo> _requeseQueue = new Queue<RequestInfo>();
        private float _lastTick;

        protected HttpRequester()
        {

        }

        public RequestInfo Request(HttpRequestMethod method, HttpParams args)
        {
            var request = new RequestInfo();
            request.method = method;
            request.args = args;

            _requeseQueue.Enqueue(request);
            return request;
        }

        public void Cancel(RequestInfo info)
        {
            if (info == null)
                return;

            info.isStop = true;
            HttpManager.GetInstance().Stop(info.corId);
        }

        public void Clear()
        {
            while (_requeseQueue.Count > 0)
            {
                var request = _requeseQueue.Dequeue();
                Cancel(request);
            }
        }

        public void Destroy()
        {
            GameObject.Destroy(gameObject);
        }

        private void UpdateRequest()
        {
            if (Time.realtimeSinceStartup < _lastTick + interval)
                return;

            int count = 0;
            while(_requeseQueue.Count > 0)
            {
                var request = _requeseQueue.Dequeue();
                if (request.isStop)
                    continue;

                switch(request.method)
                {
                    case HttpRequestMethod.Get:
                        request.corId = HttpManager.GetInstance().Get(request.args);
                        break;
                    case HttpRequestMethod.Post:
                        request.corId = HttpManager.GetInstance().Post(request.args);
                        break;
                }

                count++;
                if (requestCount > 0 && count >= requestCount)
                    break;
            }

            _lastTick = Time.realtimeSinceStartup;
        }

        private void Update()
        {
            UpdateRequest();
        }
    }
}
