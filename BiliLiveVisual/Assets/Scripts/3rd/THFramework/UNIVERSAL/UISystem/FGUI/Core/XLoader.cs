
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class XLoader : GLoader
    {
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
            url = UITextureManager.GetInstance().ParseFormatPath(url);
            string srcUrl = url;
            UITextureManager.GetInstance().GetOrCreateNTexture(srcUrl, true, (ntexture) =>
            {
                bool isError = false;
                do
                {
                    if (isDisposed)
                    {
                        isError = true;
                        break;
                    }

                    string curUrl = url;
                    if (string.Compare(srcUrl, curUrl, true) != 0)
                    {
                        isError = true;
                        break;
                    }

                } while (false);
                

                if (isError)
                    return;

                NTexture ntex = ntexture;
                if (ntexture != null)
                    onExternalLoadSuccess(ntexture);
                else
                    onExternalLoadFailed();

            }, null);
        }

        protected override void FreeExternal(NTexture texture)
        {
            //释放外部载入的资源
            UITextureManager.GetInstance().ReleaseNTexture(texture);
        }
    }

}
