namespace BLVisual
{
    public static class EventType
    {
        public static readonly int TEST_EVENT = UIDUtil.GetEventUid();

        public static readonly int BILILIVE_START = UIDUtil.GetEventUid();
        public static readonly int BILILIVE_ROOM_INFO_UPDATE = UIDUtil.GetEventUid();
        public static readonly int BILILIVE_DANMU_MSG = UIDUtil.GetEventUid();
        public static readonly int BILILIVE_SUPER_CHAT_MESSAGE = UIDUtil.GetEventUid();
        public static readonly int BILILIVE_GUARD_BUY_MESSAGE = UIDUtil.GetEventUid();

        public static readonly int BILILIVE_STOP = UIDUtil.GetEventUid();

        public static readonly int DANMUSHOWLAYER_CHANGE_DANMU_ARGS = UIDUtil.GetEventUid();
    }

}
