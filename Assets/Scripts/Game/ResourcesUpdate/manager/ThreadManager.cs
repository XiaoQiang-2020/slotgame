
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
namespace HotUpdate
{
    public class ThreadManager
    {
        static ThreadManager _instance = null;
        /// <summary>
        /// 线程管理类
        /// </summary>
        public static ThreadManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new ThreadManager();
                }
                return _instance;
            }
        }
        /// <summary>
        /// 最大的线程数
        /// </summary>
        private int MaxThreadNum = 5;
        /// <summary>
        /// 检测线程固定编号
        /// </summary>
        private const int CHECK_THREAD_ID = 10001;
        /// <summary>
        /// 创建的下载线程标识
        /// </summary>
        private int ThreadID = CHECK_THREAD_ID;
        /// <summary>
        /// 空闲下载线程
        /// </summary>
        public Action<int> FreeDownThread;
        /// <summary>
        /// 线程集合
        /// </summary>
        public List<EasyThread> ThreadList = new List<EasyThread>();

        public CheckThread MyCheckThread;

        /// <summary>
        /// 初始化线程管理
        /// </summary>
        public void Init()
        {
            ThreadID = CHECK_THREAD_ID;
            StartCheackThread();
        }
        /// <summary>
        /// 启动监控线程
        /// </summary>
        public void StartCheackThread()
        {
            MyCheckThread = new CheckThread(CHECK_THREAD_ID);
            MyCheckThread.Start();
            ThreadList.Add(MyCheckThread);
        }
        /// <summary>
        /// 1,检测是否有空闲的线程
        /// 2,在不超过最大线程数的情况下创建一个新线程
        /// </summary>
        public void CheckFreeThread(SynchronizationContext m_currentContext)
        {
            EasyThread easytd = null;
            DownThread downtd = null;
            for (int i = 0; i < ThreadList.Count; i++)
            {
                easytd = ThreadList[i];

                if (easytd.m_workerId == CHECK_THREAD_ID) continue;
                downtd = easytd as DownThread;
                if (null == downtd) continue;

                if (downtd.GetCurrentThreadState() == ThreadStateDefine.Free)
                {
                    if (null != FreeDownThread)
                    {
                        FreeDownThread(downtd.m_workerId);
                    }
                    return;
                }
                else if (downtd.GetCurrentThreadState() == ThreadStateDefine.Suspended)
                {
                    if (null != FreeDownThread)
                    {
                        FreeDownThread(downtd.m_workerId);
                    }
                    return;
                }
            }
            if (ThreadList.Count <= MaxThreadNum)
            {
                ThreadID++;
                DownThread m_thread = new DownThread(ThreadID);
                m_thread.m_currentContext = m_currentContext;
                m_thread.m_downFinish += MyCheckThread.DownFinish;
                ThreadList.Add(m_thread);
                if (null != FreeDownThread)
                {
                    FreeDownThread(m_thread.m_workerId);
                }
            }
        }

        void DownFinish(int downUnitId)
        {
            //Debugger.LogError("下完了" + downUnitId);
        }
        /// <summary>
        /// 根据id启动一个线程
        /// </summary>
        /// <param name="ThreadId"></param>
        public void StartThreadByID(int threadId)
        {
            for (int i = 0; i < ThreadList.Count; i++)
            {
                if (ThreadList[i].m_workerId == CHECK_THREAD_ID) continue;
                if (threadId == ThreadList[i].m_workerId)
                {
                    DownThread m_thread = ThreadList[i] as DownThread;
                    if (m_thread.GetCurrentThreadState() == ThreadStateDefine.Free)
                    {
                        m_thread.Start();
                    }
                    else if (m_thread.GetCurrentThreadState() == ThreadStateDefine.Suspended)
                    {
                        m_thread.Resume();
                    }
                    break;
                }
            }
        }
        public DownThread GetThreadByID(int threadId)
        {
            DownThread downThread = null;
            for (int i = 0; i < ThreadList.Count; i++)
            {
                if (ThreadList[i].m_workerId == CHECK_THREAD_ID) continue;
                downThread = ThreadList[i] as DownThread;
                if (threadId == downThread.m_workerId)
                {
                    return downThread;
                }
            }
            return downThread;
        }
        /// <summary>
        /// 暂停所有线程
        /// </summary>
        public void SuspendAllThread()
        {
            EasyThread easyThread = null;
            for (int i = 0; i < ThreadList.Count; i++)
            {
                easyThread = ThreadList[i];
                if (easyThread.m_workerId == CHECK_THREAD_ID)
                {
                    CheckThread checkThread = easyThread as CheckThread;
                    checkThread.Suspend();
                }
                else
                {
                    DownThread downThread = easyThread as DownThread;
                    downThread.Suspend();
                }
            }
        }

        /// <summary>
        /// 所有的线程都处于空闲状态
        /// </summary>
        /// <returns></returns>
        public bool AllThreadFree()
        {
            int index = 0;
            EasyThread easyThread = null;
            for (int i = 0; i < ThreadList.Count; i++)
            {
                easyThread = ThreadList[i];
                if (easyThread.m_workerId != CHECK_THREAD_ID)
                {
                    DownThread downThread = easyThread as DownThread;
                    if (downThread.GetCurrentThreadState() == ThreadStateDefine.Suspended)
                    {
                        index++;
                    }
                }
            }
            return (index >= MaxThreadNum);
        }


        /// <summary>
        /// 启动所有暂停的线程
        /// </summary>
        public void ResumeAllThread()
        {
            EasyThread easyThread = null;
            for (int i = 0; i < ThreadList.Count; i++)
            {
                easyThread = ThreadList[i];
                if (easyThread.m_workerId == CHECK_THREAD_ID)
                {
                    CheckThread checkThread = easyThread as CheckThread;
                    checkThread.Resume();
                }
                else
                {
                    DownThread downThread = easyThread as DownThread;
                    downThread.Resume();
                }
            }
        }
        public void CheckDownUnitFinish(int id)
        {
            if (DownManager.Instance)
                DownManager.Instance.CheckDownUnitFinish(id);
        }
        public void CheckWeakNet()
        {
            if (DownManager.Instance)
                DownManager.Instance.WeakNetState();
        }

        /// <summary>
        /// 清除所有线程
        /// </summary>
        public void ClearAllThread()
        {
            EasyThread easyThread = null;
            for (int i = 0; i < ThreadList.Count; i++)
            {
                easyThread = ThreadList[i];
                if (easyThread.m_workerId == CHECK_THREAD_ID)
                {
                    CheckThread checkThread = easyThread as CheckThread;
                    checkThread.Stop();
                }
                else
                {
                    DownThread downThread = easyThread as DownThread;
                    downThread.Stop();
                }
            }
            ThreadList.Clear();
        }
    }
}