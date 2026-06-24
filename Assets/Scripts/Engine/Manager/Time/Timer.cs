/***************************************************
 * 文件名：Timer.cs
 * 描  述：游戏定时器模块实现类。
 * 时  间：2017-04-13
 * 作  者：李海波
 * 修  改：
 ***************************************************/

namespace Core
{
    
    using SystemDateTime = System.DateTime;
    using SystemTimeZone = System.TimeZone;

    using TimingTaskCollectList = System.Collections.Generic.List<TimingTask>;

    /// <summary>
    /// 系统定时器管理工具类，用于记录系统时间相关数据信息。
    /// </summary>
    public class Timer : Singleton<Timer>
    {
        /// <summary>
        /// 国际标准计时本地实时时间。
        /// </summary>
        protected long m_realtimeSinceWorld = 0;

        /// <summary>
        /// 游戏启动后流逝的实时时间。
        /// </summary>
        protected float m_realtimeSinceStartup = 0;

        /// <summary>
        /// 帧间隔毫秒计时剩余量
        /// </summary>
        private double m_residualTime = 0;

        /// <summary>
        /// 定时任务统计列表。
        /// </summary>
        private TimingTaskCollectList m_timingTaskCollectList = null;

        public Timer()
            : base()
        {
        }

        ~Timer()
        {
        }

        #region Implements Virtual Methods

        /// <summary>
        /// 系统定时器的初始化回调接口。
        /// </summary>
        /// <returns>若系统定时器初始化成功则返回true，否则返回false</returns>
        protected override bool Initialize()
        {
            m_timingTaskCollectList = new TimingTaskCollectList();
            return true;
        }

        /// <summary>
        /// 系统定时器的清理回调接口。
        /// </summary>
        protected override void Cleanup()
        {
            m_timingTaskCollectList.Clear();
            m_timingTaskCollectList = null;
        }

        /// <summary>
        /// 启动系统定时器实例对象。
        /// </summary>
        public override void Start()
        {
            this.ReadjustSystemRealtime();
        }

        #endregion

        #region 定时器刷新相关接口

        /// <summary>
        /// 更新当前程序实际运行时间。
        /// </summary>
        /// <param name="delta">时间片段</param>
        public void Update(float delta)
        {
            m_residualTime += delta * GameConst.STANDARD_THOUSAND_DIGIT;

            // 取整毫秒
            int value = (int)m_residualTime;
            m_residualTime -= value;
            m_realtimeSinceWorld += value;

            m_realtimeSinceStartup += delta;

            UpdateTimingTaskHandler();
        }

        public void LateUpdate()
        {

        }

        /// <summary>
        /// 获取标准系统时间。
        /// </summary>
        /// <returns>返回系统标准时间</returns>
        public SystemDateTime GetSystemTime()
        {
            return SystemDateTime.Now;
        }

        /// <summary>
        /// 重置系统时间功能。
        /// </summary>
        public void ReadjustSystemRealtime()
        {
            // UNIX时间纪元
            SystemDateTime startTime = new SystemDateTime(1970, 1, 1, 0, 0, 0, 0);

            // 当前时间
            SystemDateTime nowTime = SystemDateTime.Now;

            // 计算当前时间相对于时间纪元的间隔毫秒数
            m_realtimeSinceWorld = (long)System.Math.Round((nowTime - startTime).TotalMilliseconds, System.MidpointRounding.AwayFromZero);

            m_realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;

            m_residualTime = 0f;
        }

        #endregion

        #region 定时任务统计分派相关接口

        /// <summary>
        /// 注册定时任务通知对象实例。
        /// </summary>
        /// <param name="timingTask">定时任务对象</param>
        public void RegisterTimingTask(TimingTask timingTask)
        {
            m_timingTaskCollectList.Add(timingTask);
            m_timingTaskCollectList.Sort();
            return;
        }

        /// <summary>
        /// 定时任务自刷新调度接口。
        /// </summary>
        protected void UpdateTimingTaskHandler()
        {
            bool needCheckTime = true;
            while (m_timingTaskCollectList.Count > 0 && needCheckTime)
            {
                TimingTask task = m_timingTaskCollectList[0];
                if (task.CompareTo(m_realtimeSinceStartup) > 0)
                {
                    needCheckTime = false;
                }
                else
                {
                    task.DoNotificationHandler();

                    // 任务结束 删除
                    if (task.IsFinished)
                    {
                        m_timingTaskCollectList.Remove(task);
                    }
                    else // 未结束 排序
                    {
                        m_timingTaskCollectList.Sort();
                    }
                }
            }
        }

        /// <summary>
        /// 清理当前已经注册的全部定时任务实例。
        /// </summary>
        public void CleanAllTimingTask()
        {
            m_timingTaskCollectList.Clear();
        }

        #endregion

        #region Auto Getter/Setter Methods

        public long RealtimeSinceWorld
        {
            get { return m_realtimeSinceWorld; }
        }

        public float RealtimeSinceStartup
        {
            get { return m_realtimeSinceStartup; }
        }

        #endregion
    }
}

