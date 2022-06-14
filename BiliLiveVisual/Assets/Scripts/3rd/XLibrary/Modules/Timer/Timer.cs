
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame
{
    public class Timer : MonoSingleton<Timer>
    {
        private int m_id = 0;
        private SortedDictionary<int, Coroutine> m_coroutines = new SortedDictionary<int, Coroutine>();

        #region 针对每一帧执行和下一帧执行的优化
        class TimerNode
        {
            public int id = -1;
            public Action callback;
            public bool isDead = false;

            public void Trigger()
            {
                callback.Invoke();
            }

            public void Reset()
            {
                id = -1;
                callback = null;
                isDead = false;
            }
        }

        const int MAX_TIMER_NODE_POOL_SIZE = 20;
        Stack<TimerNode> m_timerNodePool = new Stack<TimerNode>();
        Dictionary<int, TimerNode> m_eachFrameTimers = new Dictionary<int, TimerNode>();
        Dictionary<int, TimerNode> m_nextFrameTimers = new Dictionary<int, TimerNode>();
        List<TimerNode> m_nodeHelpList = new List<TimerNode>();
        #endregion

        #region 针对每一帧执行和下一帧执行的优化
        void Update()
        {
            CheckTimers(m_eachFrameTimers, false);
            CheckTimers(m_nextFrameTimers, true);
        }

        void CheckTimers(Dictionary<int, TimerNode> dict, bool isOnce)
        {
            if (dict.Count > 0)
            {
                // Trigger()的时候可能会添加新的定时器，影响迭代器，所以遍历部分独立出去
                m_nodeHelpList.Clear();
                foreach (var kvs in dict)
                {
                    m_nodeHelpList.Add(kvs.Value);
                }

                foreach (var tn in m_nodeHelpList)
                {
                    if (!tn.isDead)
                    {
                        tn.Trigger();
                        if (isOnce)
                        {
                            tn.isDead = true;  //只用一次
                        }
                    }

                    if (tn.isDead)
                    {
                        dict.Remove(tn.id);
                        ReturnTimerNode(tn);
                    }
                }
                m_nodeHelpList.Clear();
            }
        }

        internal int ScheduleNextFrame(Action action)
        {
            int id = GetNewTimerId();
            TimerNode tn = GetTimerNode(id, action);
            m_nextFrameTimers.Add(id, tn);
            return id;
        }

        internal int ScheduleEachFrame(Action action)
        {
            int id = GetNewTimerId();
            TimerNode tn = GetTimerNode(id, action);
            m_eachFrameTimers.Add(id, tn);
            return id;
        }

        TimerNode GetTimerNode(int id, Action action)
        {
            TimerNode tn;
            if (m_timerNodePool.Count > 0)
            {
                tn = m_timerNodePool.Pop();
            }
            else
            {
                tn = new TimerNode();
            }
            tn.id = id;
            tn.callback = action;
            return tn;
        }

        void ReturnTimerNode(TimerNode tn)
        {
            if (tn == null) return;

            tn.Reset();

            if (m_timerNodePool.Count < MAX_TIMER_NODE_POOL_SIZE)
            {
                if (m_timerNodePool.Count > 0 && m_timerNodePool.Peek() == tn)
                    return;
                m_timerNodePool.Push(tn);
            }
        }
        #endregion


        int GetNewTimerId()
        {
            return m_id++;
        }

        internal int Schedule(Action action, float interval, int times = -1)
        {
            //过多的协程比Update还要耗性能
            #region 针对每一帧执行和下一帧执行的优化
            if (interval < 0.001f && times <= 0)
                return ScheduleEachFrame(action);

            if (interval < 0.0167f && times == 1)
                return ScheduleNextFrame(action);
            #endregion

            int id = GetNewTimerId();
            IEnumerator co = CreateCoroutine(id, action, interval, times);
            m_coroutines.Add(id, StartCoroutine(co));
            return id;
        }

        internal int ScheduleOnce(Action action, float interval)
        {
            return Schedule(action, interval, 1);
        }

        public int ScheduleDuration(float interval, int duration, Action<float> pollFunc, Action endFunc = null)
        {
            float usedTime = 0;
            int timerId = -1;
            timerId = Schedule(() =>
            {
                if (usedTime > duration)
                {
                    Unschedule(timerId);
                    endFunc?.Invoke();
                    return;
                }
                pollFunc?.Invoke(usedTime);
                usedTime += interval;
            }, interval);

            return timerId;
        }

        internal void Unschedule(int id)
        {
            if (m_coroutines.ContainsKey(id))
            {
                StopCoroutine(m_coroutines[id]);
                m_coroutines.Remove(id);
            }

            if (m_eachFrameTimers.ContainsKey(id))
            {
                m_eachFrameTimers[id].isDead = true;
            }

            if (m_nextFrameTimers.ContainsKey(id))
            {
                m_nextFrameTimers[id].isDead = true;
            }
        }

        internal void UnscheduleAll()
        {
            foreach (var kvs in m_coroutines)
            {
                StopCoroutine(kvs.Value);
            }
            m_coroutines.Clear();

            foreach (var kvs in m_eachFrameTimers)
            {
                kvs.Value.Reset();
            }
            m_eachFrameTimers.Clear();

            foreach (var kvs in m_nextFrameTimers)
            {
                kvs.Value.Reset();
            }
            m_nextFrameTimers.Clear();

            m_nodeHelpList.Clear();

            m_id = 0;
        }


        IEnumerator CreateCoroutine(int id, Action action, float interval, int times)
        {
            if (interval < 0.001f)
            {
                do
                {
                    yield return null;
                    action();
                }
                while (times <= 0 || times-- > 1);
            }
            else
            {
                WaitForSeconds wait = new WaitForSeconds(interval);
                do
                {
                    yield return wait;
                    action();
                }
                while (times <= 0 || times-- > 1);
            }

            m_coroutines.Remove(id);
            action = null;
        }
    }

}
