using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame.UI
{
    public static class ItemUtil
    {
        public static bool IsStone(ItemData data)
        {
            return data != null ? (data.GetCategory() == ItemCategory.Prop && data.GetType() == PropType.Stone) : false ;
        }
    }

}
