using System;
using System.Threading;

namespace HotUpdate
{
    public class EasyThread
    {

        /// <summary>
        /// 线程执行函数
        /// </summary>
        public Action m_CallBack;
        /// <summary>
        /// 当前线程
        /// </summary>
        Thread m_Thread;
        /// <summary>
        /// 线程编号
        /// </summary>
        public int m_workerId = 0;
        /// <summary>
        ///线程阻塞
        /// </summary>
        ManualResetEvent m_waitEvent;
        /// <summary>
        /// 当前状态
        /// </summary>
        ThreadStateDefine m_currentStatus;

        /// <summary>
        /// 构造传事件
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="workerId"></param>
        /// <param name="priority"></param>
        public EasyThread(Action action, int workerId, ThreadPriority priority = ThreadPriority.Normal)
        {
            m_CallBack = action;
            m_Thread = new Thread(Run);
            m_Thread.Priority = priority;
            m_workerId = workerId;
        }
        /// <summary>
        /// 构造不传事件
        /// </summary>
        /// <param name="workerId"></param>
        /// <param name="priority"></param>
        public EasyThread(int workerId, ThreadPriority priority = ThreadPriority.Normal)
        {
            m_Thread = new Thread(Run);
            m_Thread.Priority = priority;
            m_workerId = workerId;
            m_currentStatus = ThreadStateDefine.Free;
            m_waitEvent = new ManualResetEvent(false);
        }
        /// <summary>
        /// 启动线程
        /// </summary>
        public virtual void Start(Action action = null)
        {
            m_currentStatus = ThreadStateDefine.Running;
            if (action != null)
            {
                m_CallBack = action;
            }
            m_Thread.Start();
        }
        /// <summary>
        /// 线程干的事情
        /// </summary>
        public virtual void Run()
        {
            if (null != m_CallBack)
            {
                m_CallBack();
            }
        }
        /// <summary>
        /// 线程内部调用(不能乱调用)
        /// </summary>
        public void Suspend()
        {
            m_currentStatus = ThreadStateDefine.Suspended;
            m_waitEvent.Reset();
            m_waitEvent.WaitOne(-1);
        }

        /// <summary>
        /// 恢复 线程
        /// </summary>
        public void Resume()
        {
            m_currentStatus = ThreadStateDefine.Running;
            m_waitEvent.Set();
        }
        /// <summary>
        /// 销毁 线程
        /// </summary>
        public virtual void Stop()
        {
            m_CallBack = null;
            m_Thread.Interrupt();
            m_currentStatus = ThreadStateDefine.Stopped;
            m_waitEvent.Close();
            m_Thread.Abort();
        }
        /// <summary>
        /// 获取当前的线程状态
        /// </summary>
        public ThreadStateDefine GetCurrentThreadState()
        {
            return m_currentStatus;
        }
    }
    public enum ThreadStateDefine
    {
        Free = 0,
        Suspended = 1,
        Running = 2,
        Stopped = 3
    }
}