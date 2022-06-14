using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
namespace XLibGame
{
    public class GameObjectPool : MonoBehaviour
    {
        public enum ActiveOperate
        {
            Enabled,        //失活
            Sight,          //视野
            Custom,         //自定义
        }
        /// <summary>
        /// 所属对象池管理器
        /// </summary>
        public GameObjectPoolManager mgrObj;
        /// <summary>
        /// 每个对象池的名称，当唯一id
        /// </summary>
        public string poolName;
        /// <summary>
        /// 对象预设
        /// </summary>
        public GameObject prefab;
        /// <summary>
        /// 如超过gc时间仍空闲,则移除池(s)
        /// </summary>
        public float idleTime = 180f;
        /// <summary>
        /// 检查时间,检查所有空闲对象(s)
        /// </summary>
        public float checkTime = 60f;
        /// <summary>
        /// 默认在池中停留时间
        /// </summary>
        public int stayTime = 60;
        /// <summary>
        /// 对象池中存放最大数量
        /// </summary>
        public int maxCount = int.MaxValue;
        /// <summary>
        /// 对象池中存放最小数量
        /// </summary>
        public int minCount = 0;
        /// <summary>
        /// 默认初始容量
        /// </summary>
        public int defaultCount = 0;


        /// <summary>
        /// 满池时不会生成
        /// </summary>
        public bool fixedSize = false;
        /// <summary>
        ///释放模式
        /// </summary>
        public ActiveOperate activeOperate = ActiveOperate.Enabled;
        public Action<GameObject, bool> activeCustomFunc;

        /// <summary>
        /// 队列，存放对象池中没有用到的对象，即可分配对象
        /// </summary>
        protected LinkedList<GameObjectPoolObject> m_idleQueue;
        protected float m_lastIdleTick;
        protected float m_lastCheckTick;

        protected int m_disposeTimes;

        public GameObjectPool()
        {
            m_idleQueue = new LinkedList<GameObjectPoolObject>();
        }

        public GameObjectPoolObject GetPoolObject(float lifeTime = 0)
        {
            UpdateIdleTick();

            bool isAlreadyInPool = false;
            GameObjectPoolObject poolObj;
            if (m_idleQueue.Count > 0)
            {
                //池中有待分配对象
                poolObj = m_idleQueue.Last.Value;
                m_idleQueue.RemoveLast();
                isAlreadyInPool = true;
            }
            else
            {
                if (prefab == null) return null;
                if (fixedSize) return null;
                    
                //池中没有可分配对象了，新生成一个
                poolObj = CreatePoolObject();
                if (poolObj == null)
                    return null;
            }

            poolObj.lifeTime = lifeTime;
            poolObj.stayTime = stayTime;
            poolObj.postTimes = m_disposeTimes;
            poolObj.ownPool = this;

            var returnObj = poolObj.gameObject;
            SetGameObjectActive(returnObj, isAlreadyInPool);
            poolObj.TryCountDown();

            return poolObj;
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="lifeTime">对象存在的时间</param>
        /// <returns>生成的对象</returns>
        public GameObject GetOrCreate(float lifeTime = 0)
        {
            var poolObj = GetPoolObject(lifeTime);
            if (poolObj != null)
            {
                return poolObj.gameObject;
            }
            else
            {
                if (prefab != null)
                {
                    return Instantiate(prefab);
                }
            }
            return null;
        }

        /// <summary>
        /// “删除对象”放入对象池
        /// </summary>
        /// <param name="obj">对象</param>
        public void Release(GameObject gobj)
        {
            if (gobj == null)
                return;
      
            if (m_idleQueue.Count > maxCount)
            {
                //当前池中object数量已满，直接销毁
                Object.Destroy(gobj);
                return;
            }

            GameObjectPoolObject poolObj = gobj.GetComponent<GameObjectPoolObject>();
            if (poolObj != null)
            {
                if (poolObj.postTimes < m_disposeTimes)
                {
                    Object.Destroy(gobj);
                    return;
                }
            }
            else
            {
                poolObj = CreatePoolObject(gobj);
            }

            poolObj.UpdateTick();

            //放入对象池，入队
            m_idleQueue.AddLast(poolObj);

            gobj.transform.SetParent(transform, false); //不改变Transform
            SetGameObjectDeactive(gobj);
            
            UpdateIdleTick();
        }


        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            while(m_idleQueue.Count > 0)
            {
                var go = m_idleQueue.Last.Value;
                m_idleQueue.RemoveLast();

                Object.Destroy(go);
            }
            m_disposeTimes++;
        }

        /// <summary>
        /// 销毁该对象池
        /// </summary>
        public void Destroy()
        {
            mgrObj?.DestroyPool(poolName);
        }

        /// <summary>
        /// 根据池原有初始化
        /// </summary>
        public void Init()
        {
            for (int i = 0; i < defaultCount && i < maxCount; i++)
            {
                if (i < transform.childCount)
                {
                    GameObject availableGameObject = transform.GetChild(i).gameObject;
                    Release(availableGameObject);   //放回池中待利用
                }
                else
                {
                    if (prefab != null)
                    {
                        GameObject availableGameObject = Object.Instantiate(prefab);
                        Release(availableGameObject);   //放回池中待利用
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private GameObjectPoolObject CreatePoolObject(GameObject go = null)
        {
            GameObject returnObj = null;
            if (go != null)
            {
                returnObj = go;
            }
            else
            {
                if (prefab != null)
                {
                    returnObj = Instantiate(prefab);
                }else
                {
                    return null;
                }
            }
            returnObj.transform.SetParent(gameObject.transform);
            returnObj.SetActive(false);

            //使用PrefabInfo脚本保存returnObj的一些信息
            GameObjectPoolObject poolObj = returnObj.GetComponent<GameObjectPoolObject>();
            if (poolObj == null)
            {
                poolObj = returnObj.AddComponent<GameObjectPoolObject>();

            }
            return poolObj;
        }

        private void UpdateIdleTick()
        {
            m_lastIdleTick = Time.realtimeSinceStartup;
        }
        private void UpdateCheckTick()
        {
            m_lastCheckTick = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 将自己加入到对象池管理中去
        /// </summary>
        private void Awake()
        {
            Init();
            UpdateIdleTick();
            UpdateCheckTick();
        }

        /// <summary>
        // 移除掉无效的自己
        /// </summary>
        private void Start()
        {
            if (string.IsNullOrEmpty(poolName))
            {
                Destroy();
            }
        }

        /// <summary>
        /// 被销毁清空自己
        /// </summary>
        private void OnDestroy()
        {
            m_idleQueue.Clear();
        }

        private void Update()
        {

            UpdatePool();
            UpdatePoolObjects();
        }

        private void SetGameObjectActive(GameObject gobj, bool isAlreadyInPool)
        {
            if (gobj == null)
                return;

            switch (activeOperate)
            {
                case ActiveOperate.Enabled:
                    gobj.SetActive(true);
                    break;
                case ActiveOperate.Sight:
                    {
                        if (isAlreadyInPool)
                        {
                            var position = gobj.transform.position;
                            position.z -= -1000f;
                            gobj.transform.position = position;
                        }
                        else
                        {
                            gobj.SetActive(true);
                        }
                    }
                    break;
                case ActiveOperate.Custom:
                    activeCustomFunc?.Invoke(gobj, true);
                    break;
            }
        }

        private void SetGameObjectDeactive(GameObject gobj)
        {
            if (gobj == null)
                return;

            switch (activeOperate)
            {
                case ActiveOperate.Enabled:
                    gobj.SetActive(false);
                    break;
                case ActiveOperate.Sight:
                    {
                        var position = gobj.transform.position;
                        position.z += -1000f;
                        gobj.transform.position = position;
                    }
                    break;
                case ActiveOperate.Custom:
                    activeCustomFunc?.Invoke(gobj, false);
                    break;
            }
        }

        private void UpdatePool()
        {
            if (idleTime <= 0)
                return;

            if (m_idleQueue.Count > minCount)
            {
                if (m_lastIdleTick + idleTime <= Time.realtimeSinceStartup)
                {
                    for (LinkedListNode<GameObjectPoolObject> iterNode = m_idleQueue.Last; iterNode != null;)
                    {
                        var nextIter = iterNode.Previous;

                        var poolObj = iterNode.Value;
                        if (poolObj != null)
                        {
                            var returnObj = poolObj.gameObject;
                            Object.Destroy(returnObj);

                            iterNode = iterNode.Previous;
                            m_idleQueue.Remove(iterNode);
                        }

                        iterNode = nextIter;

                        if (m_idleQueue.Count <= minCount)
                            break;
                    }
                }
            }
        }

        private void UpdatePoolObjects()
        {
            if (checkTime <= 0)
                return;

            if (m_lastCheckTick + checkTime <= Time.realtimeSinceStartup)
            {
                if (m_idleQueue.Count > 0)
                {
                    for (LinkedListNode<GameObjectPoolObject> iterNode = m_idleQueue.First; iterNode != null;)
                    {
                        var nextIter = iterNode.Next;

                        var poolObj = iterNode.Value;
                        if (poolObj != null)
                        {
                            if (poolObj.CheckTick())
                            {
                                var poolObjGo = poolObj.gameObject;
                                Object.Destroy(poolObjGo);

                                m_idleQueue.Remove(iterNode);
                            }
                            else
                            {
                                //因为节点是按时间排序,因此第一个不满足的之后都是不满足
                                break;
                            }
                        }

                        iterNode = nextIter;
                    }
                }
                UpdateCheckTick();
            }
        }
    }
}
