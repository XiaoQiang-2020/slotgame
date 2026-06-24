/***************************************************
 * 文件名：Singleton.cs
 * 描  述：单例的定义。
 * 时  间：2017-04-10
 * 作  者：李海波
 * 修  改：
 ***************************************************/
namespace Core
{
    /// <summary>
    /// 单例对象模板框架类，用于快速建立对象实例。
    /// </summary>
    public abstract class Singleton<T> : System.Object where T : Singleton<T>, new()
    {
        /// <summary>
        /// 单例对象实例，保护类成员外部不可直接访问。
        /// </summary>
        private volatile static T m_instance = null;

        /// <summary>
        /// 单例模板对象内部的对象锁。
        /// </summary>
        protected static System.Object m_locker = new System.Object();

        /// <summary>
        /// 保护访问类型的默认无参构造函数，所有继承自单例模式的类均需提供一个公有默认构造函数。
        /// </summary>
        protected Singleton()
        {
        }

        /// <summary>
        /// 默认析构函数
        /// </summary>
        ~Singleton()
        {
        }

        /// <summary>
        /// 单例对象实例的初始化接口，在构建时调用一次。
        /// </summary>
        /// <returns>若初始化成功则返回true，否则返回false</returns>
        protected virtual bool Initialize() { return true; }

        /// <summary>
        /// 单例对象实例的清理接口，在销毁时调用一次。
        /// </summary>
        protected virtual void Cleanup() { }

        /// <summary>
        /// 启动单例对象实例构建接口，用于实例对象的连锁加载流程处理。
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// 实例构建及获取接口，内部通过对象锁屏蔽多线程对调用的影响。
        /// </summary>
        /// <returns>返回已构建的对象实例</returns>
        public static T Instance()
        {
            if (null == m_instance)
            {
                // 线程锁对实例构建进行线程保护
                lock (m_locker)
                {
                    if (null == m_instance)
                    {
                        m_instance = new T();
                        if (false == m_instance.Initialize())
                        {
                            Debuger.LogError("当前单例对象实例进行初始化操作失败！");
                            m_instance = null;
                        }
                    }
                }
            }

            return m_instance;
        }

        /// <summary>
        /// 实例析构及清理接口，内部通过对象锁屏蔽多线程对调用的影响。
        /// </summary>
        public static void DestroyInstance()
        {
            if (null != m_instance)
            {
                // 线程锁对实例销毁进行线程保护
                lock (m_locker)
                {
                    if (null != m_instance)
                    {
                        m_instance.Cleanup();
                        m_instance = null;
                    }
                }
            }
        }
    }
}

