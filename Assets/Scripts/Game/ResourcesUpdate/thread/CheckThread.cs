using System.Collections.Generic;
using System.Threading;
using System;
namespace HotUpdate
{
    public class CheckThread : EasyThread
    {    /// <summary>
         /// 循环检测
         /// </summary>
        public bool is_Loop = true;

        public CheckThread(int workerId, ThreadPriority priority = ThreadPriority.Normal) : base(workerId, priority)
        {

        }
        public override void Run()
        {
            CheckThreadState();
        }
        /// <summary>
        /// 检测所有线程的状态
        /// </summary>
        /// <param name="id"></param>
        void CheckThreadState()
        {
            SynchronizationContext m_currentContext = new SynchronizationContext();
            while (is_Loop)
            {  ThreadManager.Instance.CheckFreeThread(m_currentContext);
              
                Thread.Sleep(10);
                // 监控线程检查进度
                //if (m_downUnitId != 0)
                //{
                //    ThreadManager.Instance.CheckDownUnitFinish(m_downUnitId);
                //}
                if (isStopCheck)
                {
                    Suspend();
                }
            }
        }
        private int m_downUnitId = 0;
        public void DownFinish(int downUnitId)
        {
            UnityEngine.Debug.LogError(Thread.CurrentThread.Name);
            m_downUnitId = downUnitId;
            // 子线程直接发给主线程进度
            ThreadManager.Instance.CheckDownUnitFinish(m_downUnitId);
        }
        bool isStopCheck = false;
        public void SuspendCheckThread()
        {
            isStopCheck = true;
        }
        public void ResumeCheckThread()
        {
            isStopCheck = false;
            Resume();
        }
        /// <summary>
        /// 停止检测
        /// </summary>
        public override void Stop()
        {
            is_Loop = false;
            base.Stop();
        }
    }
}