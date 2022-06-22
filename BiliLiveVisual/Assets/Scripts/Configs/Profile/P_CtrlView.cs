

using System.Collections.Generic;
using THGame.UI;
namespace BLVisual
{
    public static class P_CtrlView
    {
        public static readonly List<Dictionary<string, object>> FuncArray = new List<Dictionary<string, object>>()
        {
            new Dictionary<string, object>(){
                ["text"] = "投票检测",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {
                    UIManager.OpenView<MainUIVoteStatisticsWnd>();
                }),
            },
            new Dictionary<string, object>(){
                ["text"] = "弹幕设置",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {

                }),
            },
            new Dictionary<string, object>(){
                ["text"] = "黑名单",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {

                }),
            },
        };
    }
}