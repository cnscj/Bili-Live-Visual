public class BiliLiveDef
{

    public const string ROOM_INIT_URL = "https://api.live.bilibili.com/xlive/web-room/v1/index/getInfoByRoom";                    //房间信息API
    public const string DANMAKU_SERVER_CONF_URL = "https://api.live.bilibili.com/xlive/web-room/v1/index/getDanmuInfo";           //弹幕信息API

    public const int HEART_BEAT_PACKET_SEND_INTERVAL = 30 * 1000;    //心跳包发送间隔
    public const int PACKAGE_HEADER_TOTAL_LENGTH = 16;              //头部字节大小
}

public enum BiliLiveCode
{
    WS_OP_HEARTBEAT = 2, //心跳
    WS_OP_HEARTBEAT_REPLY = 3, //心跳回应 
    WS_OP_MESSAGE = 5, //弹幕,消息广播等全部信息
    WS_OP_USER_AUTHENTICATION = 7,//用户进入房间
    WS_OP_CONNECT_SUCCESS = 8, //进房回应

    WS_BODY_PROTOCOL_VERSION_NORMAL = 0,//普通消息
    WS_BODY_PROTOCOL_VERSION_HEARTBEAT = 1,//Body 内容为房间人气值
    WS_BODY_PROTOCOL_VERSION_DEFLATE = 2,//需要用zlib.inflate解压出一个新的数据包
    WS_BODY_PROTOCOL_VERSION_BROTLI = 3,//brotli压缩信息
    WS_HEADER_DEFAULT_VERSION = 1,
    WS_HEADER_DEFAULT_OPERATION = 1,
    WS_HEADER_DEFAULT_SEQUENCE = 1,

    WS_PACKAGE_HEADER_TOTAL_LENGTH = 16,
    WS_PACKAGE_OFFSET = 0,
    WS_HEADER_OFFSET = 4,
    WS_VERSION_OFFSET = 6,
    WS_OPERATION_OFFSET = 8,
    WS_SEQUENCE_OFFSET = 12,

    WS_AUTH_OK = 0,
    WS_AUTH_TOKEN_ERROR = -101
}

//弹幕CMD
public class BiliLiveDanmakuCmd
{
    public const string DANMU_MSG = "DANMU_MSG";              //弹幕消息
    public const string WELCOME_GUARD = "WELCOME_GUARD";      //欢迎xxx老爷
    public const string ENTRY_EFFECT = "ENTRY_EFFECT";        //欢迎舰长进入房间
    public const string WELCOME = "WELCOME";                  //欢迎xxx进入房间
    public const string INTERACT_WORD = "INTERACT_WORD";      //进入了房间

    public const string SUPER_CHAT_MESSAGE_JPN = "SUPER_CHAT_MESSAGE_JPN";    //日文翻译SC
    public const string SUPER_CHAT_MESSAGE = "SUPER_CHAT_MESSAGE";            //原文SC留言

    public const string SEND_GIFT = "SEND_GIFT";                //投喂礼物
    public const string COMBO_SEND = "COMBO_SEND";              //连续投喂

    public const string GUARD_BUY = "GUARD_BUY";                //上舰长
    public const string USER_TOAST_MSG = "USER_TOAST_MSG";      //续费了舰长
    public const string NOTICE_MSG = "NOTICE_MSG";              //在本房间续费了舰长

    public const string WATCHED_CHANGE = "WATCHED_CHANGE";      //观看人数变化
    public const string ONLINE_RANK_COUNT = "ONLINE_RANK_COUNT"; // 高能榜数量更新

    public const string ROOM_REAL_TIME_MESSAGE_UPDATE = "ROOM_REAL_TIME_MESSAGE_UPDATE";    //粉丝关注变动
}

//房间数据
public struct BiliLiveRoomInfo
{
    public int finalRoomId;

    public int longRoomId;
    public int shortRoomId;
    public string roomTitle;
    public int roomOwnerUid;
}

public struct BiliLiveHostInfoHostData
{
    public string host;
    public int port;
    public int wsPort;
    public int wssPort;
}

//Host信息
public struct BiliLiveHostInfo
{
    public string token;
    public BiliLiveHostInfoHostData[] hostList;
}

//弹幕数据结构
public static class BiliLiveDanmakuData
{
    public class Raw
    {
        public string cmd;
    }

    public class DanmuMsg : Raw
    {
        public int uid;
        public string nick;
        public string color;
        public string content;
    }

    public class SendGift : Raw
    {
        public int uid;
        public string uname;
        public string action;
        public string giftName;
    }
    public class ComboSend : Raw
    {
        public int uid;
        public string uname;
        public string action;
        public int combo_num;
        public int combo_total_coin;
        public string gift_name;
        public int total_num;
    }

    public class GuardBuy : Raw
    {
        public int uid;
        public string username;
        public int guard_level;
        public int price;
        public int gift_id;
        public string gift_name;
        public int start_time;
        public int end_time;
    }

    public class SuperChatMessage : Raw
    {
        public int uid;
        public string uname;
        public string face;
        public string face_frame;
        public string message;
        public int start_time;
        public int end_time;
        public int time;
        public int price;
        public string message_font_color;
        public string background_price_color;
        public string background_bottom_color;
        public string background_color;
        public string background_color_end;
        public string background_color_start;

    }

    public class WatchedChange : Raw
    {
        public int num;
        public string text_small;
        public string text_large;
    }

}


//数据包头部数据
//https://github.com/lovelyyoshino/Bilibili-Live-API/blob/master/API.WebSocket.md
//'>I2H2I'结构体定义(>表示这是一个大端序，二进制高位在左边。I表示无符号整型数字，占4个字节，H 表示无符号短整型，占用2个字节。2H是HH的简化写法)
public struct BiliLiveHeader
{
    public uint pack_len;               //数据包长度(包含body)
    public ushort raw_header_size;      //数据包头部长度（固定为 16）
    public ushort ver;                  //协议版本（见下文）
    public uint operation;              //操作类型（见下文）
    public uint seq_id;                 //数据包序列（固定为 1）
}
