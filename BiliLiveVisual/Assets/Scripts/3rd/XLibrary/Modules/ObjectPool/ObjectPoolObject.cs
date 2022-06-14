using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class ObjectPoolObject
    {
        /// <summary>
        /// 在池中能存活的最长时间
        /// </summary>
        public float stayTime;

        /// <summary>
        /// 上次使用的tick
        /// </summary>
        private float m_updateTick;


        public void UpdateTick()
        {
            m_updateTick = Time.realtimeSinceStartup;
        }

        public bool CheckRemove()
        {
            if (stayTime > 0)
            {
                if (m_updateTick + stayTime <= Time.realtimeSinceStartup)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
