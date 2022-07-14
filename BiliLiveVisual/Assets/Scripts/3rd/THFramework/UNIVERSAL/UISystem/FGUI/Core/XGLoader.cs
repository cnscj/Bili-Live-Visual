
using System;
using FairyGUI;
namespace THGame.UI
{
    public class XGLoader : GLoader
    {
        NTexture _ntexture;
        protected override void LoadExternal()
        {
            /*
            开始外部载入，地址在url属性
            载入完成后调用OnExternalLoadSuccess
            载入失败调用OnExternalLoadFailed

            注意：如果是外部载入，在载入结束后，调用OnExternalLoadSuccess或OnExternalLoadFailed前，
            比较严谨的做法是先检查url属性是否已经和这个载入的内容不相符。
            如果不相符，表示loader已经被修改了。
            这种情况下应该放弃调用OnExternalLoadSuccess或OnExternalLoadFailed。
            */
            ReleaseNTexture();
            url = UITextureManager.GetInstance().ParseFormatPath(url);
            string srcUrl = url;
            GetOrCreateNTexture(srcUrl, true, (ntexture) =>
            {
                bool isError;
                do
                {
                    isError = true;
                    if (isDisposed)
                    {
                        break;
                    }

                    string curUrl = url;
                    if (string.Compare(srcUrl, curUrl, false) != 0)
                    {
                        break;
                    }
                    isError = false;
                } while (false);
                

                if (isError)
                {
                    UITextureManager.GetInstance().ReleaseNTexture(ntexture);
                    return;
                }


                NTexture ntex = ntexture;
                if (ntexture != null)
                {
                    onExternalLoadSuccess(ntexture);
                    _ntexture = ntexture;
                }
                else
                {
                    onExternalLoadFailed();
                }


            }, (code) =>
            {
                onExternalLoadFailed();
            });
        }

        protected override void FreeExternal(NTexture texture)
        {
            //释放外部载入的资源
            //NOTE:20220713,Dispose没有跑这里,按谷主的说法是个BUG,待修复
            ReleaseNTexture();
        }

        public override void Dispose()
        {
            base.Dispose();
            ReleaseNTexture();
        }

        void GetOrCreateNTexture(string key, bool isAsync, Action<NTexture> onSuccess, Action<int> onFailed)
        {
            UITextureManager.GetInstance().GetOrCreateNTexture(key, isAsync, onSuccess, onFailed);
        }

        void ReleaseNTexture()
        {
            if (_ntexture != null)
            {
                UITextureManager.GetInstance().ReleaseNTexture(_ntexture);
                _ntexture = null;
            }
        }
    }

}
