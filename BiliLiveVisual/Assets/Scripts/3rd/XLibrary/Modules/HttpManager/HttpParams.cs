using System;

namespace XLibGame
{
    public class HttpParams
    {
        public string url;
        public HttpForm formData;
        public byte[] bodyData;
        public int timeout;
        public Action<HttpResult> onCallback;
        public Action<object> onSuccess;
        public Action<string> onFailed;
    }
}