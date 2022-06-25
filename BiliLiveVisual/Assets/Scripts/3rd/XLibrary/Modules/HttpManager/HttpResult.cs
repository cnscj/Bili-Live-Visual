
using UnityEngine.Networking;
using XLibrary;

namespace XLibGame
{
    public class HttpResult
    {
        UnityWebRequest _webRequest;

        public bool IsHttpError => _webRequest.isHttpError;
        public bool IsNetworkError => _webRequest.isNetworkError;
        public bool IsDone => _webRequest.isDone;
        public string Error => _webRequest.error;

        public HttpResult(UnityWebRequest webRequest)
        {
            _webRequest = webRequest;
        }
        public bool IsSuccess()
        {
            return !(IsHttpError || IsNetworkError || !IsDone);
        }
        public override string ToString()
        {
            return _webRequest.downloadHandler.text;
        }
        public byte[] ToBytes()
        {
            return _webRequest.downloadHandler.data;
        }
    }
}
