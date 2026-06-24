using System.Threading;
using System;
namespace HotUpdate
{
    public class DownThread : EasyThread
    {
        /// <summary>
        /// 是否循环
        /// </summary>
        public bool m_IsLoop = true;
        /// <summary>
        /// 下载路径
        /// </summary>
        public string m_downPath = "";
        public DownLoadUnit m_downUnit = null;
        /// <summary>
        /// 当前线程上下文
        /// </summary>
        public SynchronizationContext m_currentContext;
        /// <summary>
        /// 下载结束(可能有异常)
        /// </summary>
        public Action<int> m_downFinish;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DownThread(int workerId, ThreadPriority priority = ThreadPriority.Normal) : base(workerId, priority)
        {

        }
        public override void Run()
        {
            LoopRun();
        }

        void LoopRun()
        {
            while (m_IsLoop)
            {
                DownLoad.DownLoadOneUnit(m_downUnit, (string str) =>
                {
                    m_currentContext.Post(Porgress, str);
                    Suspend();
                });
                Thread.Sleep(100);
            }
        }
        /// <summary>s
        /// 下载进度 下载完一个文件进度加1
        /// </summary>
        /// <param name="state"></param>
        public void Porgress(object state)
        {
            ThreadManager.Instance.CheckDownUnitFinish(m_downUnit.id);
            string strState = state.ToString();
            if (strState.Equals("timeOut"))
            {
               //ThreadManager.Instance.CheckWeakNet();
            }
            //if (m_downFinish != null)
            //{
            //    m_downFinish(m_downUnit.id);
            //}
        }
        public override void Stop()
        {
            m_IsLoop = false;
            base.Stop();
        }
    }
}