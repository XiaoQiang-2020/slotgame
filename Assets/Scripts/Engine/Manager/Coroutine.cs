namespace Core
{
    using SystemIEnumerator = System.Collections.IEnumerator;

    using UnityCoroutine = UnityEngine.Coroutine;
    using UnityBehaviour = UnityEngine.MonoBehaviour;

    /// <summary>
    /// 游戏线程模块实现类，处理游戏异步线程调度相关逻辑处理。
    /// </summary>
    public class Coroutine : Core.Singleton<Coroutine>, Core.ICoroutine
    {
        protected UnityBehaviour m_behaviourTarget = null;

        #region Implements Virtual Methods

        /// <summary>
        /// 行为监控器的初始化回调接口。
        /// </summary>
        /// <returns>若行为监控器初始化成功则返回true，否则返回false</returns>
        protected override bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// 行为监控器的清理回调接口。
        /// </summary>
        protected override void Cleanup()
        {
            this.StopAllCoroutines();

            m_behaviourTarget = null;
        }

        #endregion

        /// <summary>
        /// 设置目标行为处理实体。
        /// </summary>
        /// <param name="behaviour">行为对象</param>
        public bool SetupTargetBehaviour(UnityBehaviour behaviour)
        {
            if (null == behaviour)
            {
                return false;
            }
            m_behaviourTarget = behaviour;
            return true;
        }

        #region ICoroutine Interface Methods

        /// <summary>
        /// 检测当前协程实例是否处于可用状态。
        /// </summary>
        /// <returns>若当前协程实例可用则返回true，否则返回false</returns>
        public bool IsCoroutineEnable()
        {
            if (null == m_behaviourTarget)
            {
                return false;
            }

            return m_behaviourTarget.enabled;
        }

        /// <summary>
        /// 通过指定方法引用在当前运行行为对象上开始一个协程处理。
        /// </summary>
        /// <param name="routine">方法引用</param>
        /// <returns>若启动协程处理成功则返回对应的协程引用，否则返回null</returns>
        public UnityCoroutine StartCoroutine(SystemIEnumerator routine)
        {
            if (null != m_behaviourTarget)
            {
                return m_behaviourTarget.StartCoroutine(routine);
            }

            return null;
        }

        /// <summary>
        /// 通过指定方法名在当前运行行为对象上开始一个协程处理。
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <returns>若启动协程处理成功则返回对应的协程引用，否则返回null</returns>
        public UnityCoroutine StartCoroutine(string methodName)
        {
            if (null != m_behaviourTarget)
            {
                return m_behaviourTarget.StartCoroutine(methodName);
            }

            return null;
        }

        /// <summary>
        /// 通过指定方法名及附属参数在当前运行行为对象上开始一个协程处理。
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="value">附属参数</param>
        /// <returns>若启动协程处理成功则返回对应的协程引用，否则返回null</returns>
        public UnityCoroutine StartCoroutine(string methodName, object value)
        {
            if (null != m_behaviourTarget)
            {
                return m_behaviourTarget.StartCoroutine(methodName, value);
            }

            return null;
        }

        /// <summary>
        /// 停止在当前运行行为对象上的全部协程调度函数。
        /// </summary>
        public void StopAllCoroutines()
        {
            if (null != m_behaviourTarget)
            {
                m_behaviourTarget.StopAllCoroutines();
            }
        }

        /// <summary>
        /// 通过指定协程引用停止在当前运行行为对象上的对应全部协程。
        /// </summary>
        /// <param name="routine">协程引用</param>
        public void StopCoroutine(UnityCoroutine routine)
        {
            if (null != m_behaviourTarget)
            {
                m_behaviourTarget.StopCoroutine(routine);
            }
        }

        /// <summary>
        /// 通过指定方法引用停止在当前运行行为对象上的对应全部协程。
        /// </summary>
        /// <param name="routine">引用目标</param>
        public void StopCoroutine(SystemIEnumerator routine)
        {
            if (null != m_behaviourTarget)
            {
                m_behaviourTarget.StopCoroutine(routine);
            }
        }

        /// <summary>
        /// 通过指定方法名停止在当前运行行为对象上的对应全部协程。
        /// </summary>
        /// <param name="methodName">方法名称</param>
        public void StopCoroutine(string methodName)
        {
            if (null != m_behaviourTarget)
            {
                m_behaviourTarget.StopCoroutine(methodName);
            }
        }

        #endregion
    }
}
