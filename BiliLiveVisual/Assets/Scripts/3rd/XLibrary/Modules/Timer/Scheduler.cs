using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame
{
    /// <summary>
    /// 定时任务系统功能有
    /// 1、添加定时任务
    /// 2、删除定时任务
    /// 3、替换定时任务
    /// </summary>
    public class Scheduler : MonoSingleton<Scheduler>
    {
        // 任务 ID
        private int taskId;
        // taskId 列表
        private List<int> taskIdList = new List<int>();
        // 回收任务结束不用了的 taskId 的缓存列表
        private List<int> recycleTaskIdList = new List<int>();

        // 定时任务列表
        private List<TimerTask> taskTimeList = new List<TimerTask>();
        // 临时 定时任务列表 （作为缓存列表）
        private List<TimerTask> tmpTaskTimeList = new List<TimerTask>();

        // 获取任务id 的锁
        private static readonly string objLock = "lock";

        // 帧计数
        int frameCounter = 0;
        // 帧定时任务列表
        private List<TimerFrameTask> taskFrameTimeList = new List<TimerFrameTask>();
        // 帧临时 定时任务列表 （作为缓存列表）
        private List<TimerFrameTask> tmpTaskFrameTimeList = new List<TimerFrameTask>();



        // Update is called once per frame
        void Update()
        {
            // 检测定时任务是否到达可执行
            CheckTimerTask();

            // 检测定时帧任务是否到达可执行
            CheckTimerFrameTask();

            // 回收任务ID
            RecycleTaskId();
        }

        /// <summary>
        /// 检测定时任务是否到达可执行
        /// </summary>
        private void CheckTimerTask()
        {
            // 加入临时定时任务列表的定时任务 (这样做保证同帧不干扰下面遍历任务列表)
            for (int tempIndex = 0; tempIndex < tmpTaskTimeList.Count; tempIndex++)
            {
                taskTimeList.Add(tmpTaskTimeList[tempIndex]);
            }
            tmpTaskTimeList.Clear();

            // 遍历检测定时任务是否满足执行条件
            for (int index = 0; index < taskTimeList.Count; index++)
            {
                TimerTask task = taskTimeList[index];
                if (GetCurrentTimeTick() < task.destTime)
                {
                    continue;
                }
                else
                {
                    Action callback = task.callback;

                    // 异常捕获，避免回调函数出错
                    try
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                    }
                    catch (Exception e)
                    {

                        Debug.Log(e.Message);
                    }


                    // 任务循环只剩一次
                    if (task.loopCount == 1)
                    {
                        // 把结束任务不用的 taskid 缓存到 回收任务 id 列表中
                        recycleTaskIdList.Add(task.taskId);
                        taskTimeList.RemoveAt(index);
                        index--;
                    }
                    else
                    {
                        // 不是无限循环，循环次数 -1
                        if (task.loopCount != 0)
                        {
                            task.loopCount -= 1;
                        }
                        // 目标时间更新一个 delay 时间
                        task.destTime += task.delay;
                    }
                }

            }
        }

        /// <summary>
        /// 检测定时帧任务是否到达可执行
        /// </summary>
        private void CheckTimerFrameTask()
        {
            // 帧计数(会不会越界)
            frameCounter += 1;

            // 加入临时定时任务列表的定时任务 (这样做保证同帧不干扰下面遍历任务列表)
            for (int tempIndex = 0; tempIndex < tmpTaskFrameTimeList.Count; tempIndex++)
            {
                taskFrameTimeList.Add(tmpTaskFrameTimeList[tempIndex]);
            }
            tmpTaskFrameTimeList.Clear();

            // 遍历检测定时任务是否满足执行条件
            for (int index = 0; index < taskFrameTimeList.Count; index++)
            {
                TimerFrameTask task = taskFrameTimeList[index];
                if (frameCounter < task.destFrame)
                {
                    continue;
                }
                else
                {
                    Action callback = task.callback;

                    // 异常捕获，避免回调函数出错
                    try
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                    }
                    catch (Exception e)
                    {

                        Debug.Log(e.Message);
                    }


                    // 任务循环只剩一次
                    if (task.loopCount == 1)
                    {
                        // 把结束任务不用的 taskid 缓存到 回收任务 id 列表中
                        recycleTaskIdList.Add(task.taskId);
                        taskFrameTimeList.RemoveAt(index);
                        index--;
                    }
                    else
                    {
                        // 不是无限循环，循环次数 -1
                        if (task.loopCount != 0)
                        {
                            task.loopCount -= 1;
                        }
                        // 目标时间更新一个 delay 时间
                        task.destFrame += task.delay;
                    }
                }

            }
        }

        /// <summary>
        /// 取得当前的时间节拍
        /// </summary>
        /// <returns></returns>
        private float GetCurrentTimeTick()
        {
            return Time.realtimeSinceStartup * 1000;
        }


        #region TimeTask    

        /// <summary>
        /// 添加定时的任务
        /// </summary>
        /// <param name="callback">任务回调</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="loopCount">循环次数（0 为 无限循环）</param>
        /// <param name="timerUnit">时间单位（ms,s,m,h,Day）</param>
        /// <returns>返回任务id</returns>
        public int AddTimerTask(Action callback, float delay, int loopCount = 1, TimerUnit timerUnit = TimerUnit.Millisecond)
        {
            if (timerUnit != TimerUnit.Millisecond)
            {
                switch (timerUnit)
                {
                    case TimerUnit.Second:
                        delay = delay * 1000;
                        break;
                    case TimerUnit.Minute:
                        delay = delay * 1000 * 60;
                        break;
                    case TimerUnit.Hour:
                        delay = delay * 1000 * 60 * 60;
                        break;
                    case TimerUnit.Day:
                        delay = delay * 1000 * 60 * 60 * 24;
                        break;
                    default:
                        Debug.Log("Add Tssk Unit Type Error...");

                        break;
                }
            }
            int tid = GetTaskId();

            float destTime = GetCurrentTimeTick() + delay;
            TimerTask timerTask = new TimerTask(tid, callback, destTime, delay, loopCount);

            // 添加新定时任务的时候，先添加到临时列表（避免同帧遍历任务列表时干扰遍历列表）
            tmpTaskTimeList.Add(timerTask);

            // tid 添加到使用列表中
            taskIdList.Add(tid);

            return tid;
        }

        /// <summary>
        /// 删除指定任务
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <returns>返回是否删除</returns>
        public bool DeleteTimerTask(int taskId)
        {
            bool exist = false;
            for (int i = 0; i < taskTimeList.Count; i++)
            {
                TimerTask task = taskTimeList[i];

                if (task.taskId == taskId)
                {
                    taskTimeList.RemoveAt(i);
                    for (int j = 0; j < taskIdList.Count; j++)
                    {
                        if (taskIdList[j] == taskId)
                        {
                            taskIdList.RemoveAt(j);
                            break;
                        }
                    }

                    exist = true;
                    break;
                }
            }

            // 从临时缓存任务列表中查找
            if (exist == false)
            {
                for (int i = 0; i < tmpTaskTimeList.Count; i++)
                {
                    TimerTask task = tmpTaskTimeList[i];
                    if (task.taskId == taskId)
                    {

                        tmpTaskTimeList.RemoveAt(i);
                        for (int j = 0; j < taskIdList.Count; j++)
                        {
                            if (taskIdList[j] == taskId)
                            {
                                taskIdList.RemoveAt(j);
                                break;
                            }
                        }

                        exist = true;
                        break;
                    }
                }
            }

            return exist;
        }

        /// <summary>
        /// 替换存在的任务
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <param name="callback">任务回调</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="loopCount">循环次数（0 为 无限循环）</param>
        /// <param name="timerUnit">时间单位（ms,s,m,h,Day）</param>
        /// <returns>返回是否替换成功</returns>
        public bool ReplaceTimerTask(int taskId, Action callback, float delay, int loopCount = 1, TimerUnit timerUnit = TimerUnit.Millisecond)
        {
            if (timerUnit != TimerUnit.Millisecond)
            {
                switch (timerUnit)
                {
                    case TimerUnit.Second:
                        delay = delay * 1000;
                        break;
                    case TimerUnit.Minute:
                        delay = delay * 1000 * 60;
                        break;
                    case TimerUnit.Hour:
                        delay = delay * 1000 * 60 * 60;
                        break;
                    case TimerUnit.Day:
                        delay = delay * 1000 * 60 * 60 * 24;
                        break;
                    default:
                        Debug.Log("Add Tssk Unit Type Error...");

                        break;
                }
            }


            float destTime = GetCurrentTimeTick() + delay;
            TimerTask newTimerTask = new TimerTask(taskId, callback, destTime, delay, loopCount);

            bool isReplace = false;
            // 从任务列表中查找对应的 taskId 进行任务替换
            for (int i = 0; i < taskTimeList.Count; i++)
            {
                TimerTask task = taskTimeList[i];

                if (task.taskId == taskId)
                {
                    taskTimeList[i] = newTimerTask;

                    isReplace = true;
                    break;
                }
            }

            // 从临时缓冲任务列表中替换
            if (isReplace == false)
            {
                for (int i = 0; i < tmpTaskTimeList.Count; i++)
                {
                    TimerTask task = tmpTaskTimeList[i];
                    if (task.taskId == taskId)
                    {

                        tmpTaskTimeList[i] = newTimerTask;
                        isReplace = true;
                        break;
                    }
                }
            }


            return isReplace;
        }
        #endregion

        #region TimerFrameTask

        /// <summary>
        /// 添加定时帧任务
        /// </summary>
        /// <param name="callback">任务回调</param>
        /// <param name="delay">延迟帧数</param>
        /// <param name="loopCount">循环次数（0 为 无限循环）</param>
        /// <returns>返回任务id</returns>
        public int AddTimerFrameTask(Action callback, int delay, int loopCount = 1)
        {

            int tid = GetTaskId();

            int destTime = frameCounter + delay;
            TimerFrameTask timerTask = new TimerFrameTask(tid, callback, destTime, delay, loopCount);

            // 添加新定时帧任务的时候，先添加到临时列表（避免同帧遍历任务列表时干扰遍历列表）
            tmpTaskFrameTimeList.Add(timerTask);

            // tid 添加到使用列表中
            taskIdList.Add(tid);

            return tid;
        }

        /// <summary>
        /// 删除指定任务
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <returns>返回是否删除</returns>
        public bool DeleteTimerFrameTask(int taskId)
        {
            bool exist = false;
            for (int i = 0; i < taskFrameTimeList.Count; i++)
            {
                TimerFrameTask task = taskFrameTimeList[i];

                if (task.taskId == taskId)
                {
                    taskFrameTimeList.RemoveAt(i);
                    for (int j = 0; j < taskIdList.Count; j++)
                    {
                        if (taskIdList[j] == taskId)
                        {
                            taskIdList.RemoveAt(j);
                            break;
                        }
                    }

                    exist = true;
                    break;
                }
            }

            // 从临时缓存任务列表中查找
            if (exist == false)
            {
                for (int i = 0; i < tmpTaskFrameTimeList.Count; i++)
                {
                    TimerFrameTask task = tmpTaskFrameTimeList[i];
                    if (task.taskId == taskId)
                    {

                        tmpTaskFrameTimeList.RemoveAt(i);
                        for (int j = 0; j < taskIdList.Count; j++)
                        {
                            if (taskIdList[j] == taskId)
                            {
                                taskIdList.RemoveAt(j);
                                break;
                            }
                        }

                        exist = true;
                        break;
                    }
                }
            }

            return exist;
        }

        /// <summary>
        /// 替换存在的任务
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <param name="callback">任务回调</param>
        /// <param name="delay">延迟帧数</param>
        /// <param name="loopCount">循环次数（0 为 无限循环）</param>
        /// <returns>返回是否替换成功</returns>
        public bool ReplaceTimerFrameTask(int taskId, Action callback, int delay, int loopCount = 1)
        {

            int destTime = frameCounter + delay;
            TimerFrameTask newTimerFrameTask = new TimerFrameTask(taskId, callback, destTime, delay, loopCount);

            bool isReplace = false;
            // 从任务列表中查找对应的 taskId 进行任务替换
            for (int i = 0; i < taskFrameTimeList.Count; i++)
            {
                TimerFrameTask task = taskFrameTimeList[i];

                if (task.taskId == taskId)
                {
                    taskFrameTimeList[i] = newTimerFrameTask;

                    isReplace = true;
                    break;
                }
            }

            // 从临时缓冲任务列表中替换
            if (isReplace == false)
            {
                for (int i = 0; i < tmpTaskFrameTimeList.Count; i++)
                {
                    TimerFrameTask task = tmpTaskFrameTimeList[i];
                    if (task.taskId == taskId)
                    {

                        tmpTaskFrameTimeList[i] = newTimerFrameTask;
                        isReplace = true;
                        break;
                    }
                }
            }


            return isReplace;
        }

        #endregion

        #region Tools

        /// <summary>
        /// 回收任务结束，不用的taskId，以备下次使用
        /// </summary>
        private void RecycleTaskId()
        {
            for (int i = 0; i < recycleTaskIdList.Count; i++)
            {
                int taskId = recycleTaskIdList[i];
                for (int j = 0; j < taskIdList.Count; j++)
                {
                    if (taskIdList[j] == taskId)
                    {
                        taskIdList.RemoveAt(j);
                        break;
                    }
                }
            }


            recycleTaskIdList.Clear();
        }

        /// <summary>
        /// 获得任务ID
        /// </summary>
        /// <returns>返回一个任务ID</returns>
        private int GetTaskId()
        {
            lock (objLock)
            {
                taskId += 1;

                while (true)
                {
                    // 防止越界
                    if (taskId == int.MaxValue)
                    {
                        taskId = 0;
                    }

                    // 遍历循环检查taskId是否被使用
                    bool isUsed = false;
                    for (int i = 0; i < taskIdList.Count; i++)
                    {
                        if (taskId == taskIdList[i])
                        {
                            isUsed = true;
                            break;
                        }
                    }
                    // 如果该 taskId 没有被使用，跳出循环
                    if (isUsed == false)
                    {
                        break;
                    }
                    else
                    {
                        taskId += 1;
                    }
                }
            }

            return taskId;
        }

        #endregion

        #region 任务模型

        /// <summary>
        /// 任务模型
        /// </summary>
        class TimerTask
        {
            public int taskId;
            public Action callback;
            public float destTime;
            public float delay;
            public int loopCount;

            public TimerTask(int taskId, Action callback, float destTime, float delay, int loopCount)
            {
                this.taskId = taskId;
                this.callback = callback;
                this.destTime = destTime;
                this.delay = delay;
                this.loopCount = loopCount;
            }
        }

        /// <summary>
        /// 帧任务模型
        /// </summary>
        class TimerFrameTask
        {
            public int taskId;
            public Action callback;
            public int destFrame;
            public int delay;
            public int loopCount;

            public TimerFrameTask(int taskId, Action callback, int destFrame, int delay, int loopCount)
            {
                this.taskId = taskId;
                this.callback = callback;
                this.destFrame = destFrame;
                this.delay = delay;
                this.loopCount = loopCount;
            }
        }

        #endregion


    }

    /// <summary>
    /// 时间单位枚举
    /// </summary>
    public enum TimerUnit
    {
        Millisecond,
        Second,
        Minute,
        Hour,
        Day
    }
}