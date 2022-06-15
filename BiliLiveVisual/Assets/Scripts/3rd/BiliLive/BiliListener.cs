using System;
using LitJson;
using UnityEngine;

public class BiliLiveListener
{
    public Action<BiliLiveRoomInfo> onRoomInfo;
    public Action<BiliLiveHostInfo> onHostInfo;
    public Action<BiliLiveDanmakuData.DanmuMsg> onDataDanmuMsg;
    public Action<BiliLiveDanmakuData.SendGift> onDataSendGift;
    public Action<BiliLiveDanmakuData.ComboSend> onDataComboSend;
    public Action<BiliLiveDanmakuData.GuardBuy> onDataGuardBuy;
    public Action<BiliLiveDanmakuData.SuperChatMessage> onDataSuperChatMessage;
    public Action<BiliLiveDanmakuData.WatchedChange> onDataWatchedChange;

    public BiliLiveListener()
    {
        onRoomInfo = OnRoomInfo;
        onHostInfo = OnHostInfo;
        onDataDanmuMsg = OnDataDanmuMsg;
        onDataSendGift = OnDataSendGift;
        onDataComboSend = OnDataComboSend;
        onDataGuardBuy = OnDataGuardBuy;
        onDataSuperChatMessage = OnDataSuperChatMessage;
        onDataWatchedChange = OnDataWatchedChange;
    }
    public virtual void Dispatch(BiliLiveDanmakuData.Raw data)
    {
        if (data == null)
            return;

        switch (data.cmd)
        {
            case BiliLiveDanmakuCmd.DANMU_MSG:
                onDataDanmuMsg?.Invoke((BiliLiveDanmakuData.DanmuMsg)data);
                break;
            case BiliLiveDanmakuCmd.SEND_GIFT:
                onDataSendGift?.Invoke((BiliLiveDanmakuData.SendGift)data);
                break;
            case BiliLiveDanmakuCmd.GUARD_BUY:
                onDataGuardBuy?.Invoke((BiliLiveDanmakuData.GuardBuy)data);
                break;
            case BiliLiveDanmakuCmd.SUPER_CHAT_MESSAGE:
                onDataSuperChatMessage?.Invoke((BiliLiveDanmakuData.SuperChatMessage)data);
                break;
            case BiliLiveDanmakuCmd.WATCHED_CHANGE:
                onDataWatchedChange?.Invoke((BiliLiveDanmakuData.WatchedChange)data);
                break;
        }        
    }
    public virtual BiliLiveDanmakuData.Raw Parse(string jsonStr)
    {
        BiliLiveDanmakuData.Raw outData = null;
        try
        {
            var jsonData = JsonMapper.ToObject(jsonStr);
            var cmd = jsonData["cmd"].ToString();

            if (cmd == BiliLiveDanmakuCmd.DANMU_MSG)    //弹幕
            {
                var info = jsonData["info"];
                outData = new BiliLiveDanmakuData.DanmuMsg
                {
                    cmd = cmd,
                    uid = int.Parse(info[2][0].ToString()),
                    nick = info[2][1].ToString(),
                    content = info[1].ToString()
                };
            }
            else if (cmd == BiliLiveDanmakuCmd.SEND_GIFT)   //礼物
            {
                var data = jsonData["data"];
                outData = new BiliLiveDanmakuData.SendGift
                {
                    cmd = cmd,
                    uid = int.Parse(data["uid"].ToString()),
                    uname = data["uname"].ToString(),
                    action = data["action"].ToString(),
                    giftName = data["giftName"].ToString(),
                };
            }
            else if (cmd == BiliLiveDanmakuCmd.COMBO_SEND)  //连击礼物
            {
                var data = jsonData["data"];
                outData = new BiliLiveDanmakuData.ComboSend
                {
                    uid = int.Parse(data["uid"].ToString()),
                    uname = data["uname"].ToString(),
                    action = data["action"].ToString(),
                    combo_num = int.Parse(data["combo_num"].ToString()),
                    combo_total_coin = int.Parse(data["combo_total_coin"].ToString()),
                    gift_name = data["gift_name"].ToString(),
                    total_num = int.Parse(data["total_num"].ToString()),
                };

            }
            else if (cmd == BiliLiveDanmakuCmd.GUARD_BUY)    //上船
            {
                var data = jsonData["data"];
                outData = new BiliLiveDanmakuData.GuardBuy
                {
                    cmd = cmd,
                    uid = int.Parse(data["uid"].ToString()),
                    username = data["username"].ToString(),
                    guard_level = int.Parse(data["guard_level"].ToString()),
                    price = int.Parse(data["price"].ToString()),
                    gift_id = int.Parse(data["gift_id"].ToString()),
                    gift_name = data["gift_name"].ToString(),
                    start_time = int.Parse(data["start_time"].ToString()),
                    end_time = int.Parse(data["end_time"].ToString()),
                };
            }
            else if (cmd == BiliLiveDanmakuCmd.SUPER_CHAT_MESSAGE)   //醒目留意
            {
                var data = jsonData["data"];
                var user_info = data["user_info"];

                outData = new BiliLiveDanmakuData.SuperChatMessage
                {
                    cmd = cmd,
                    uid = int.Parse(data["uid"].ToString()),
                    uname = user_info["uname"].ToString(),
                    face = user_info["face"].ToString(),
                    face_frame = user_info["face_frame"].ToString(),
                    message = data["message"].ToString(),
                    start_time = int.Parse(data["start_time"].ToString()),
                    end_time = int.Parse(data["end_time"].ToString()),
                    time = int.Parse(data["time"].ToString()),
                    price = int.Parse(data["price"].ToString()),
                };
            }
            else if (cmd == BiliLiveDanmakuCmd.WATCHED_CHANGE)  //观看数变化
            {
                var data = jsonData["data"];
                outData = new BiliLiveDanmakuData.WatchedChange
                {
                    cmd = cmd,
                    num = int.Parse(data["num"].ToString()),
                    text_small = data["text_small"].ToString(),
                    text_large = data["text_large"].ToString(),
                };
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return outData;
    }

    public virtual void Clear()
    {
        onRoomInfo = null;
        onHostInfo = null;
        onDataDanmuMsg = null;
        onDataSendGift = null;
        onDataComboSend = null;
        onDataGuardBuy = null;
        onDataSuperChatMessage = null;
        onDataWatchedChange = null;
    }

 
    //////////////////////////////////

    protected virtual void OnRoomInfo(BiliLiveRoomInfo info)
    {
        
    }
    protected virtual void OnHostInfo(BiliLiveHostInfo info)
    {

    }

    protected virtual void OnDataDanmuMsg(BiliLiveDanmakuData.DanmuMsg data)
    {
        Debug.LogFormat("{0}:{1}", data.nick, data.content);
    }

    protected virtual void OnDataSendGift(BiliLiveDanmakuData.SendGift data)
    {
        Debug.LogFormat("[{0}{1}{2}]", data.uname, data.action, data.giftName);
    }

    protected virtual void OnDataComboSend(BiliLiveDanmakuData.ComboSend data)
    {
        Debug.LogFormat("[{0}{1}{2}*{3}]", data.uname, data.action, data.gift_name, data.combo_num);
    }

    protected virtual void OnDataGuardBuy(BiliLiveDanmakuData.GuardBuy data)
    {
        Debug.LogFormat("[{0}购买了{1}]", data.username, data.gift_name);
    }

    protected virtual void OnDataSuperChatMessage(BiliLiveDanmakuData.SuperChatMessage data)
    {
        Debug.LogFormat("[{0}的SC:{1}]", data.uname, data.message);
    }

    protected virtual void OnDataWatchedChange(BiliLiveDanmakuData.WatchedChange data)
    {
        Debug.LogFormat("[{0}]", data.text_large);
    }
}

