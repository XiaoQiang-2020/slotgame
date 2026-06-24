/***************************************************
 * 文件名：TimingTask.cs
 * 描  述：系统定时任务记录实体类。
 * 时  间：2017-04-13
 * 作  者：李海波
 * 修  改：
 ***************************************************/

namespace Core
{
    using TimeNotificationAction = System.Action;

    /// <summary>
    /// 系统定时任务记录实体类，用于记录系统定时任务相关数据信息。
    /// </summary>
    public class TimingTask : System.Object, System.IComparable
    {
        /// <summary>
        /// 当前定时任务过期时间。
        /// </summary>
        protected float m_taskExpiredTimer = 0f;

        /// <summary>
        /// 当前定时任务结束标识。
        /// </summary>
        protected bool m_isFinished = false;

        /// <summary>
        /// 当前定时任务是否循环执行启用标识。
        /// </summary>
        protected bool m_isLoopEnabled = false;

        /// <summary>
        /// 当前定时任务延时执行计时参数。
        /// </summary>
        protected float m_invokeDelayTimer = 0f;

        /// <summary>
        /// 当前定时任务通知句柄。
        /// </summary>
        protected TimeNotificationAction m_taskNotificationAction = null;

        public TimingTask()
            : base()
        {
        }

        /// <summary>
        /// 定时任务构造函数。
        /// </summary>        
        /// <param name="delayTimer">延迟执行计时</param>
        /// <param name="timeNotificationAction">通知函数</param>
        /// <param name="isLoop">循环执行标识</param>
        public TimingTask(float delayTimer, TimeNotificationAction timeNotificationAction, bool isLoop = false)
        {           
            m_invokeDelayTimer = delayTimer;

            m_taskExpiredTimer = Timer.Instance().RealtimeSinceStartup + delayTimer;
            m_isFinished = false;
            m_taskNotificationAction = timeNotificationAction;

            m_isLoopEnabled = isLoop;
        }        

        ~TimingTask()
        {
        }

        /// <summary>
        /// 更新当前程序实际运行时间。
        /// </summary>
        /// <param name="delta">时间片段</param>
        public void Update(float delta)
        {
        }

        /// <summary>
        /// 当前定时任务通知处理句柄。
        /// </summary>
        public virtual void DoNotificationHandler()
        {
            if (null != m_taskNotificationAction && IsTaskValidStatus())
            {
                m_taskNotificationAction();
            }
            else
            {
                this.DoTaskFinished();
            }

            // 未结束的循环任务则刷新
            if (!m_isFinished && m_isLoopEnabled)
            {
                this.RefreshLoopTask();
            }
            else // 其他任务则直接结束
            {
                this.DoTaskFinished();
            }
        }

        /// <summary>
        /// 当前定时任务结束接口。
        /// </summary>
        public virtual void DoTaskFinished()
        {
            m_isFinished = true;
        }

        /// <summary>
        /// 检测当前任务是否为有效状态。
        /// </summary>
        /// <returns>若当前任务状态有效则返回true，否则返回false</returns>
        private bool IsTaskValidStatus()
        {
            if (m_isFinished)
            {
                return false;
            }           

            return true;
        }

        /// <summary>
        /// 重置当前任务的过期时间。
        /// </summary>
        public void RefreshLoopTask()
        {
            m_taskExpiredTimer = Timer.Instance().RealtimeSinceStartup + m_invokeDelayTimer;
        }

        public int CompareTo(object obj)
        {
            int flg = 0;
            TimingTask timingTask = obj as TimingTask;
            if (null != timingTask)
            {
                if (this.m_taskExpiredTimer > timingTask.m_taskExpiredTimer)
                {
                    flg = 1;
                }
                else if (this.m_taskExpiredTimer < timingTask.m_taskExpiredTimer)
                {
                    flg = -1;
                }
            }

            return flg;
        }

        public int CompareTo(float time)
        {
            int flg = 0;
            if (this.m_taskExpiredTimer > time)
            {
                flg = 1;
            }
            else if (this.m_taskExpiredTimer < time)
            {
                flg = -1;
            }

            return flg;
        }

        #region Auto Getter/Setter Methods       

        public float TaskExpiredTimer
        {
            get { return m_taskExpiredTimer; }
            set { m_taskExpiredTimer = value; }
        }

        public bool IsFinished
        {
            get { return m_isFinished; }
            set { m_isFinished = value; }
        }

        public bool IsLoopEnabled
        {
            get { return m_isLoopEnabled; }
        }

        #endregion
    }
}
