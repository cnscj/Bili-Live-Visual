using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLVisual
{
    public static class DictUtil
    {
        public static T SafeGetValue<T>(Dictionary<string,object> dict, string key, T def = default)
        {
            if (dict != null)
            {
                if (dict.TryGetValue(key, out var val))
                {
                    return (T)val;
                }
            }
            
            return def;
        }
    }
}