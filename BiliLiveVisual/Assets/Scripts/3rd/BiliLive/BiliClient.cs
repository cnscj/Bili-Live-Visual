
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using LitJson;
using System.Text;

public class BiliLiveClient
{
    public Action<string> onRoomMsg;
    public Action<string> onDanmakuMsg;

    int _roomId;
    bool _isRunning;

    BiliLiveRoomInfo _roomInfo;
    BiliLiveHostInfo _hostInfo;

    WebSocket _websocket = new WebSocket();
    IntervalTimer _heartbeatTimer = new IntervalTimer(BiliLiveDef.HEART_BEAT_PACKET_SEND_INTERVAL);
    Queue<string> _msgQueue = new Queue<string>();

    public BiliLiveClient(int roomId)
    {
        _roomId = roomId;
        _heartbeatTimer.onEvent = OnTimerEvent;
        _websocket.onMessage = OnWebsocketMessage;
        onDanmakuMsg = OnDanmakuMsg;
    }

    public BiliLiveClient() : this(-1)
    {

    }

    public BiliLiveRoomInfo GetRoomInfo()
    {
        return _roomInfo;
    }
    public BiliLiveHostInfo GetHostInfo()
    {
        return _hostInfo;
    }
    public int GetRoomId()
    {
        return _roomId;
    }
    public bool IsRunning()
    {
        return _isRunning;
    }

    public async Task Start(int roomId)
    {
        _roomId = roomId;
        await Close();
        await Start();
    }

    public async Task Start()
    {
        if (_isRunning)
        {
            return;
        }
        
        var isRoomInitSucc = await InitRoomInfo();
        var isHostInitSucc = await InitHostServer();

        if (isRoomInitSucc && isHostInitSucc)
        {
            _isRunning = true;
            await ConnectRoom();
            KeepConnect();
            await KeepReceive();
        }
    }

    public async Task Close()
    {
        StopKeepConnect();
        await DisconnectRoom();
        _isRunning = false;
    }

    ////////////////////////

    private async Task<bool> InitRoomInfo()
    {
        try
        {
            var jsonStr = await HttpRequest.GetAsync(BiliLiveDef.ROOM_INIT_URL, new Dictionary<string, string> { ["room_id"] = _roomId.ToString() });
            onRoomMsg?.Invoke(jsonStr);

            var jsonData = JsonMapper.ToObject(jsonStr);
            var codeStr = jsonData["code"].ToString();
            if (codeStr == "0")
            {
                var room_info = jsonData["data"]["room_info"];

                _roomInfo.longRoomId = int.Parse(room_info["room_id"].ToString());
                _roomInfo.shortRoomId = int.Parse(room_info["short_id"].ToString());

                _roomInfo.roomTitle = room_info["title"].ToString();
                _roomInfo.roomOwnerUid = int.Parse(room_info["uid"].ToString());

                _roomInfo.finalRoomId = (_roomInfo.shortRoomId != 0) ? _roomInfo.shortRoomId : _roomInfo.longRoomId;

                Debug.LogFormat("room_id={0},short_id={1},title={2}", _roomInfo.longRoomId, _roomInfo.shortRoomId, _roomInfo.roomTitle);

                return true;
            }
            else
            {
                Debug.LogError(jsonData["message"]);
            }
        }
        catch(Exception)
        {
            Debug.LogError("Parse Error");
        }
        return false;
    }

    private async Task<bool> InitHostServer()
    {
        try
        {
            var jsonStr = await HttpRequest.GetAsync(BiliLiveDef.DANMAKU_SERVER_CONF_URL, new Dictionary<string, string> { ["id"] = _roomId.ToString(),["type"] = "0" });
            var jsonData = JsonMapper.ToObject(jsonStr);

            var codeStr = jsonData["code"].ToString();
            if (codeStr == "0")
            {
                var data = jsonData["data"];
                _hostInfo.token = data["token"].ToString();

                var host_list = data["host_list"];
                _hostInfo.hostList = new BiliLiveHostInfoHostData[host_list.Count];

                for(int i = 0;i < host_list.Count;i++)
                {
                    var host_data = host_list[i];
                    var hostData = new BiliLiveHostInfoHostData();
                    hostData.host = host_data["host"].ToString();
                    hostData.port = int.Parse(host_data["port"].ToString());
                    hostData.wsPort = int.Parse(host_data["ws_port"].ToString());
                    hostData.wssPort = int.Parse(host_data["wss_port"].ToString());

                    _hostInfo.hostList[i] = hostData;
                }

                //Debug.LogFormat("token={0}", _hostInfo.token);
                return true;
            }
            else
            {
                Debug.LogError(jsonData["message"]);
            }
        }
        catch (Exception)
        {
            Debug.LogError("Parse Error");
        }
        return false;
    }

    private async Task<bool> ConnectRoom()
    {
        //建立WebSocket链接,这里应该对每个进行尝试
        bool ret = true;
        foreach(var hostData in _hostInfo.hostList)
        {
            await ConnectWebscoket("wss", hostData.host, hostData.wssPort);
            await SendAuthPacket();

            if (ret) break;
        }
        return ret;
    }

    private void StopKeepConnect()
    {
        _heartbeatTimer.Stop();
    }

    private async Task DisconnectRoom()
    {
        await _websocket.Disconnect();
    }

    private void KeepConnect()
    {
        _heartbeatTimer.Start();
    }

    private async Task KeepReceive()
    {
        while (_isRunning)
        {
            while(_msgQueue.Count > 0)
            {
                var jsonStr = _msgQueue.Dequeue();
                onDanmakuMsg?.Invoke(jsonStr);
            }
            await Task.Delay(100);//休眠线程
        }
    }

    private async Task ConnectWebscoket(string proto,string host,int port)
    {
        string url = string.Format("{0}://{1}:{2}/sub", proto, host, port);
        await _websocket.Connect(url);
    }

    //发送认证包
    private async Task SendAuthPacket()
    {
        var authArgs = new Dictionary<string, object>();
        //XXX:应该不是房主自己,这里传太多参数会导致链接失败

        //authArgs["uid"] = 0;//_roomInfo.roomOwnerUid;
        //authArgs["protover"] = 3;
        //authArgs["platform"] = "web";
        //authArgs["type"] = 2;   
        authArgs["roomid"] = _roomInfo.finalRoomId;
        authArgs["key"] = _hostInfo.token;

        var data = MakePackData(authArgs, (uint)BiliLiveCode.WS_OP_USER_AUTHENTICATION);
        await _websocket.Send(data);
    }

    //心跳包,空的用来维持链接
    private async Task SendHeartbeatPacket()
    {
        var emptyArgs = new Dictionary<string, object>();

        var data = MakePackData(emptyArgs, (uint)BiliLiveCode.WS_OP_HEARTBEAT);
        await _websocket.Send(data);
    }

    private byte[] MakePackData(Dictionary<string, object> args, uint operation)
    {
        var jsonStr = JsonMapper.ToJson(args);
        var body = Encoding.UTF8.GetBytes(jsonStr);

        return PackageData(body, operation);
    }

    private void ParsePacketData(byte[] data)
    {
        if (data == null || data.Length <= 0)
            return;

        //解析一个数据包,多余数据裁掉
        var isSucc = UnpackageData(data, out var outHeader, out var outBody);
        if (!isSucc) return;

        //粘包问题,截取剩下的再次解析
        if (outHeader.pack_len > 0 && data.Length > outHeader.pack_len)
        {
            int subLen = (int)(data.Length - outHeader.pack_len);
            var subData = ByteHelper.SubBytes(data, (int)outHeader.pack_len, subLen);
            ParsePacketData(subData);
        }
        

        if (outHeader.operation == (uint)BiliLiveCode.WS_OP_HEARTBEAT_REPLY)
        {
            //好像什么都没返回
        }
        else if (outHeader.operation == (uint)BiliLiveCode.WS_OP_MESSAGE)
        {
            if (outHeader.ver == (uint)BiliLiveCode.WS_BODY_PROTOCOL_VERSION_NORMAL) //JSON明文
            {
                var str = Encoding.UTF8.GetString(outBody, 0, (int)outHeader.pack_len - (int)outHeader.raw_header_size);
                ParseDanmakuMsg(str);
            }
            else if (outHeader.ver == (uint)BiliLiveCode.WS_BODY_PROTOCOL_VERSION_DEFLATE)
            {
                //需要剥离头部信息
                var newData = ZipUtility.Decompress_Deflate(outBody);
                ParsePacketData(newData);
            }
        }
        else if (outHeader.operation == (uint)BiliLiveCode.WS_OP_CONNECT_SUCCESS)
        {
            try
            {
                var str = Encoding.UTF8.GetString(outBody, 0, (int)outHeader.pack_len - (int)outHeader.raw_header_size);
                var jsonData = JsonMapper.ToObject(str);
                var code = int.Parse(jsonData["code"].ToString());
                if (code != 0)
                {
                    Debug.LogError("Connect Error");
                }
            }
            catch (Exception)
            {
                Debug.LogError("Parse Error");
            }
        }
    }

    //弹幕类解析
    private void ParseDanmakuMsg(string jsonStr)
    {
        //这里先送到缓存区,随后交由主线程处理
        _msgQueue.Enqueue(jsonStr);
    }

    private void OnDanmakuMsg(string jsonStr)
    {
        try
        {
            var jsonData = JsonMapper.ToObject(jsonStr);
            var cmd = jsonData["cmd"].ToString();

            if (cmd == BiliLiveDanmakuCmd.DANMU_MSG)    //弹幕
            {
                var info = jsonData["info"];
                var uid = int.Parse(info[2][0].ToString());
                var nick = info[2][1].ToString();
                var content = info[1].ToString();

                Debug.LogFormat("{0}:{1}", nick, content);

            }
            else if (cmd == BiliLiveDanmakuCmd.SEND_GIFT)   //礼物
            {
                var data = jsonData["data"];
                var uid = int.Parse(data["uid"].ToString());
                var uname = data["uname"].ToString();
                var action = data["action"].ToString();
                var giftName = data["giftName"].ToString();

                Debug.LogFormat("[{0}{1}{2}]", uname, action, giftName);
            }
            else if (cmd == BiliLiveDanmakuCmd.COMBO_SEND)  //连击礼物
            {
                var data = jsonData["data"];
                var uid = int.Parse(data["uid"].ToString());
                var uname = data["uname"].ToString();
                var action = data["action"].ToString();
                var combo_num = int.Parse(data["combo_num"].ToString());
                var combo_total_coin = int.Parse(data["combo_total_coin"].ToString());
                var gift_name = data["gift_name"].ToString();
                var total_num = int.Parse(data["total_num"].ToString());

                Debug.LogFormat("[{0}{1}{2}*{3}]", uname, action, gift_name, combo_num);
            }
            else if(cmd == BiliLiveDanmakuCmd.GUARD_BUY)    //上船
            {
                var data = jsonData["data"];
                var uid = int.Parse(data["uid"].ToString());
                var username = data["username"];
                var guard_level = int.Parse(data["guard_level"].ToString());
                var price = int.Parse(data["price"].ToString());
                var gift_id = int.Parse(data["gift_id"].ToString());
                var gift_name = data["gift_name"].ToString();
                var start_time = int.Parse(data["start_time"].ToString());
                var end_time = int.Parse(data["end_time"].ToString());

                Debug.LogFormat("[{0}购买了{1}]", username, gift_name);

            }
            else if(cmd == BiliLiveDanmakuCmd.SUPER_CHAT_MESSAGE)   //醒目留意
            {
                var data = jsonData["data"];
                var user_info = data["user_info"];

                var uid = int.Parse(data["uid"].ToString());
                var uname = user_info["uname"].ToString();
                var face = user_info["face"].ToString();
                var face_frame = user_info["face_frame"].ToString();
                var message = data["uname"].ToString();
                var start_time = data["start_time"].ToString();
                var end_time = data["end_time"].ToString();
                var time = data["time"].ToString();
                var price = int.Parse(data["price"].ToString());

                Debug.LogFormat("[{0}的SC:{1}]", uname, message);

            }
            else if (cmd == BiliLiveDanmakuCmd.WATCHED_CHANGE)  //观看数变化
            {
                var data = jsonData["data"];
                var num = int.Parse(data["num"].ToString());
                var text_small = data["text_small"].ToString();
                var text_large = data["text_large"].ToString();

                Debug.LogFormat("[{0}]", text_large);
            }

        }
        catch (Exception)
        {
            Debug.LogError("Parse Error");
        }
    }

    //
    private BiliLiveHeader DecodePacketHeader(byte[] data)
    {
        if (data == null || data.Length < BiliLiveDef.PACKAGE_HEADER_TOTAL_LENGTH)
        {
            return new BiliLiveHeader();
        }
        
        var headerData = new byte[BiliLiveDef.PACKAGE_HEADER_TOTAL_LENGTH];
        object blHeader = new BiliLiveHeader();

        Array.Copy(data, 0, headerData, 0, headerData.Length);
        ByteHelper.ByteArrayToStructureEndian(headerData, ref blHeader, 0);
        var header = (BiliLiveHeader)blHeader;

        return header;
    }
    //封装一个数据包
    private byte[] PackageData(byte[] body, uint operation)
    {
        var blHeader = new BiliLiveHeader();
        blHeader.raw_header_size = (ushort)System.Runtime.InteropServices.Marshal.SizeOf(blHeader);
        blHeader.pack_len = (uint)(blHeader.raw_header_size + ((body != null) ? body.Length : 0));
        blHeader.ver = 1;
        blHeader.operation = operation;
        blHeader.seq_id = 1;

        var header = ByteHelper.StructureToByteArrayEndian(blHeader);

        return ByteHelper.CombineBytes(header,body);
    }

    //解析一个数据包
    private bool UnpackageData(byte[] data, out BiliLiveHeader outHeader, out byte[] outBody)
    {
        if (data == null || data.Length < BiliLiveDef.PACKAGE_HEADER_TOTAL_LENGTH)
        {
            outHeader = new BiliLiveHeader();
            outBody = null;
            return false;
        }

        var headerData = new byte[BiliLiveDef.PACKAGE_HEADER_TOTAL_LENGTH];
        ByteHelper.SpliteBytes(data, headerData, null);

        object blHeader = new BiliLiveHeader();
        ByteHelper.ByteArrayToStructureEndian(headerData, ref blHeader, 0);
        var header = (BiliLiveHeader)blHeader;

        int bodyLen = (int)(header.pack_len - header.raw_header_size);
        byte[] body = default;
        if (bodyLen > 0)
        {
            body = new byte[bodyLen];
            ByteHelper.SpliteBytes(data, headerData, body);
        }

        outHeader = header;
        outBody = body;

        return true;
    }

    
    //
    private async void OnTimerEvent()
    {
        await SendHeartbeatPacket();
    }

    private void OnWebsocketMessage(byte[] data)
    {
        //var outHeader = DecodePacketHeader(data);
        //Debug.LogFormat("len={0},dataLen={1},op={2},ver={3},", outHeader.pack_len, data.Length, outHeader.operation, outHeader.ver, outHeader.seq_id);

        ParsePacketData(data);
    }
}
