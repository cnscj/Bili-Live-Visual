using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class GameObjectPoolObject : MonoBehaviour
    {
        /// <summary>
        /// 对象显示的持续时间，若=0，则不隐藏
        /// </summary>
        public float lifeTime;

        /// <summary>
        /// 与Dispose挂钩
        /// </summary>
        public int postTimes;

        /// <summary>
        /// 在池中的存活时间
        /// </summary>
        public float stayTime = -1f;

        /// <summary>
        /// 所属对象池
        /// </summary>
        [HideInInspector]public GameObjectPool ownPool;

        private float m_stayTick;
        private Coroutine m_releaseCor;

        private void OnDestroy()
        {
            StopCountDown();
        }

        public void TryCountDown()
        {
            if (lifeTime > 0)
            {
                StartCountDown();
            }
        }

        public bool CheckTick()
        {
            if (stayTime >= 0)
            {
                if (Time.realtimeSinceStartup - m_stayTick >= stayTime)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateTick()
        {
            m_stayTick = Time.realtimeSinceStartup;
        }

        public void Release()
        {
            //将对象加入对象池
            StopCountDown();

            if (ownPool != null)
            {
                ownPool.Release(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void StartCountDown()
        {
            StopCountDown();
            m_releaseCor = StartCoroutine(CountDown());
        }
        private void StopCountDown()
        {
            if (m_releaseCor != null) StopCoroutine(m_releaseCor);
            m_releaseCor = null;
        }

        IEnumerator CountDown()
        {
            yield return new WaitForSeconds(lifeTime);
            Release();
        }
    }


}
