

using System.Collections.Generic;
using THGame.UI;
namespace BLVisual
{
    public static class P_CtrlView
    {
        public static readonly List<Dictionary<string, object>> funcArray = new List<Dictionary<string, object>>()
        {
            new Dictionary<string, object>(){
                ["text"] = "投票检测",
                ["onClick"] = new FairyGUI.EventCallback0(()=>
                {
                    UIManager.OpenView<MainUIVoteStatisticsWnd>();
                }),
            },

        };
    }
}