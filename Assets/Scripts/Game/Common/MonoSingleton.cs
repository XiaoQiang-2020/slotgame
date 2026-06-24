/***************************************************
 * 文件名：LuaLoader.cs
 * 描  述：Mono 单例类。
 * 时  间：2017-04-13
 * 作  者：李海波
 * 修  改：
 ***************************************************/

namespace Game
{
    using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
    using UnityGameObject = UnityEngine.GameObject;

    public abstract class MonoSingleton<T> : UnityMonoBehaviour where T : MonoSingleton<T>
    {
        private static T m_instance = null;

        public static T Instance
        {
            get
            {
                if (null == m_instance)
                {
                    m_instance = UnityGameObject.FindObjectOfType(typeof(T)) as T;
                    if (null == m_instance)
                    {
                        m_instance = new UnityGameObject("Singleton of " + typeof(T).ToString()).AddComponent<T>();
                        m_instance.Initialize();
                    }
                }
                return m_instance;
            }
        }

        private void Awake()
        {
            if (null == m_instance)
            {
                m_instance = this as T;
            }
        }

        /// <summary>
        /// 单例对象初始化函数。
        /// </summary>
        protected virtual bool Initialize() { return true; }

        /// <summary>
        /// 单例对象清理函数。
        /// </summary>
        protected virtual void Cleanup() { }

        private void OnApplicationQuit()
        {
            if (null != m_instance)
            {
                m_instance.Cleanup();
                m_instance = null;
            }
        }
    }
}

