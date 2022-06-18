using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLVisual
{
    public static class Language
    {
        public static string GetString(int id, params object[] args)
        {
            if (H_Descation.formaMap.TryGetValue(id, out var format))
            {
                return string.Format(format, args);
            }
            return default;
        }
    }
}
