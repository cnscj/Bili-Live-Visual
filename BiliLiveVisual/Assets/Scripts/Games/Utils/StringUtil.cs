using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BLVisual
{
    public static class StringUtil
    {
        //字符串去除重复
        public static string StringEliminateDuplicate(string str)
        {
            var strArray = str.Distinct().ToArray(); //字符去重
            return string.Join(string.Empty, strArray); //字符成串
        }
    }
}