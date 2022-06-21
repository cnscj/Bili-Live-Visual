using System.Collections;
using System.Collections.Generic;

namespace BLVisual
{
    public static class H_Descation
    {
        public static Dictionary<int, string> formaMap = new Dictionary<int, string>()
        {
            [10001] = "{0} Test",

            [10101] = "{0} Test",

            [10201] = "显示醒目留言",
            [10202] = "显示弹幕",
            [10203] = "隐藏醒目留言",
            [10204] = "隐藏弹幕",

            [10301] = "{0}",
            [10302] = "{0}的SC:{1}",
            [10303] = "{0}{1}{2}", //投喂礼物
            [10304] = "{0}{1}{2}",  //上舰

            [10401] = "投票倒计时:{0}",

            [10501] = "{0}/s",
        };
    }
}