using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame
{
    public class GameObjectPoolManager : MonoSingleton<GameObjectPoolManager>
    {
        /// <summary>
        /// 存放所有的对象池
        /// </summary>
        Dictionary<string, GameObjectPool> m_poolDic;
        /// <summary>
        /// 对象池在场景中的父控件
        /// 将对象池的对象都放在了一个单独的gameobject下
        /// </summary>
        Transform m_parentTrans;//默认存放路径


        protected GameObjectPoolManager()
        {
            m_poolDic = new Dictionary<string, GameObjectPool>();
        }

        private void Awake()
        {
            m_parentTrans = gameObject.transform;
        }

        /// <summary>
        /// 初始化所有池
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        /// 创建一个新的对象池
        /// </summary>
        /// <param name="poolName">对象池名称，唯一id</param>
        /// <returns>对象池对象</returns>
        public GameObjectPool NewPool(string poolName, GameObject prefab = null, int maxCount = 30, int minCount = 0,int defaultCount = 0)
        {
            if (string.IsNullOrEmpty(poolName))
            {
                return null;
            }

            if (m_poolDic.ContainsKey(poolName))
            {
                return m_poolDic[poolName];
            }
            GameObject obj = new GameObject(poolName);
            GameObjectPool pool = obj.AddComponent<GameObjectPool>();

            if (m_parentTrans)
            {
                pool.mgrObj = this;
                pool.poolName = poolName;
                pool.prefab = prefab;
                pool.maxCount = maxCount;
                pool.minCount = minCount;
                pool.defaultCount = defaultCount;
                obj.transform.SetParent(m_parentTrans);

                pool.Init();
            }

            m_poolDic[poolName] = pool;

            return pool;
        }


        /// <summary>
        /// 添加一个对象池
        /// </summary>
        /// <param name="pool"></param>
        public bool AddPool(GameObjectPool pool)
        {
            if (pool == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(pool.poolName))
            {
                return false;
            }

            if (pool.prefab == null)
            {
                return false;
            }

            //如果已存在,创建也失败
            if (m_poolDic.ContainsKey(pool.poolName))
            {
                return false;
            }

            m_poolDic[pool.name] = pool;
            return true;
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <param name="poolName"></param>
        public void DestroyPool(string poolName)
        {
            if (m_poolDic.TryGetValue(poolName, out var pool))
            {
                UnityEngine.Object.Destroy(pool.gameObject);
                m_poolDic.Remove(poolName);
            }
        }

        /// <summary>
        /// 取得对象池
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public GameObjectPool GetPool(string poolName)
        {
            if (m_poolDic.TryGetValue(poolName, out var pool))
            {
                return pool;
            }
            return null;
        }

        /// <summary>
        /// 是否存在对象池
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public bool HasPool(string poolName)
        {
            return m_poolDic.ContainsKey(poolName);
        }

        /// <summary>
        /// 从对象池中取出新的对象
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="lifeTime">对象显示时间</param>
        /// <returns>新对象</returns>
        public GameObject GetOrCreateGameObject(string poolName, float lifeTime = 0f)
        {
            if (m_poolDic.ContainsKey(poolName))
            {
                return m_poolDic[poolName].GetOrCreate(lifeTime);
            }
            return null;
        }

        /// <summary>
        /// 将对象存入对象池中
        /// </summary>
        /// <param name="go">对象</param>
        public void ReleaseGameObject(GameObject go,int stayTime = -1)
        {
            if (go == null)
                return;

            var gameObjectPoolObjectCom = go.GetComponent<GameObjectPoolObject>();
            if (gameObjectPoolObjectCom)
            {
                gameObjectPoolObjectCom.Release();
            }
            else
            {
                Destroy(go);
            }
        }

        /// <summary>
        /// 将对象存入对象池中
        /// </summary>
        /// <param name="go">对象</param>
        public void ReleaseGameObject(string poolName, GameObject go, bool isForce = true)
        {
            if (go == null)
                return;

            var pool = GetPool(poolName);
            if (pool)
            {
                pool.Release(go);
            }
            else
            {
                if (isForce)
                {
                    //新创建一个GameGojectPool
                    pool = NewPool(poolName);
                    pool.Release(go);
                }
                else
                {
                    Destroy(go);
                }
            }

        }

        /// <summary>
        /// 销毁所有对象池操作
        /// </summary>
        public void DisposeAll()
        {
            foreach (var poolPair in m_poolDic)
            {
                var pool = poolPair.Value;
                pool.Destroy();
            }
            m_poolDic.Clear();
            UnityEngine.Object.Destroy(m_parentTrans);
        }

    }
}
