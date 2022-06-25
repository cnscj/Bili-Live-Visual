
using System;
using System.Collections.Generic;
using LitJson;
using THGame.UI;
using UnityEngine;
using XLibGame;

namespace BLVisual
{
    public class MainUIRecordListTmpl : FWidget
    {
        private static readonly HttpRequester httpRequester = HttpRequester.Create(1f,1);
        private static Dictionary<int, string> faceDict = new Dictionary<int, string>();
        FLabel contentText;
        MainUIUserHeadLoader headLoader;

        BiliLiveDanmakuData.DanmuMsg cacheData;
        HttpRequester.RequestInfo requestInfo;
        public MainUIRecordListTmpl()
        {
            package = "MainUI";
            component = "RecordListTmpl";
        }
        protected override void OnInitUI()
        {
            contentText = GetChild<FLabel>("contentText");
            headLoader = GetChild<MainUIUserHeadLoader>("headLoader");
            OnClick(() =>
            {
                Debug.LogFormat("{0}({1}):{2}", cacheData.nick, cacheData.uid, cacheData.content);
            });
        }

        public void SetMsgData(BiliLiveDanmakuData.DanmuMsg msgData)
        {
            contentText.SetText(string.Format("[color={0}]{1}[/color]", msgData.color, msgData.content));

            httpRequester.Cancel(requestInfo);
            if (!faceDict.TryGetValue(msgData.uid, out var face))
            {
                faceDict[msgData.uid] = ""; //标记下,免得一直刷
                requestInfo = httpRequester.Request(HttpRequestMethod.Get, new HttpParams()
                {
                    url = string.Format("https://tenapi.cn/bilibili/?uid={0}", msgData.uid),
                    onCallback = (ret) =>
                    {
                        if (ret == null)
                            return;

                        if (!ret.IsSuccess())
                            return;

                        try
                        {
                            var jsonStr = ret.ToString();
                            var jsonData = JsonMapper.ToObject(jsonStr);

                            var code = int.Parse(jsonData["code"].ToString());
                            if (code == 200)
                            {
                                var data = jsonData["data"];
                                if (data != null)
                                {
                                    var avatarObj = data["avatar"];
                                    if (avatarObj != null)
                                    {
                                        var avatar = avatarObj.ToString();
                                        faceDict[msgData.uid] = avatar;
                                        headLoader.SetHeadData(avatar);
                                    }

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            //地址被Ban了
                        }
                    },
                    onFailed = (code) =>
                    {
                        faceDict.Remove(msgData.uid);
                    }
                });
            }
            else
            {
                headLoader.SetHeadData(face);
            }

            cacheData = msgData;
        }
        protected override void OnExit()
        {
            httpRequester.Cancel(requestInfo);
        }
    }
}