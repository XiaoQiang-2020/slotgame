namespace Game
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityGameObject = UnityEngine.GameObject;
    using UnityTransform = UnityEngine.Transform;
    using Utils;

    public class AppGameManager : MonoSingleton<AppGameManager>
    {
        public const string NativeCallbackObjectName = "AppGameManager";

        /// <summary>
        /// 初始化运行时宿主相关节点。
        /// </summary>
        private IEnumerator Init()
        {
            if (InitUIController.Instance.uiLaunch != null)
            {
                InitUIController.Instance.uiLaunch.SetProgess(0f);
            }

            // 旧资源解压、消息中心、Lua 初始化流程已不作为新项目启动链的一部分。
            InitSceneRootNodes();
            yield return new WaitForFixedUpdate();
        }

        #region 私有属性

        /// <summary>
        /// 场景根节点。
        /// </summary>
        private UnityTransform m_sceneRoot = null;

        /// <summary>
        /// UI 根节点。
        /// </summary>
        private UnityTransform m_uiRoot = null;

        /// <summary>
        /// 分享 UI 根节点。
        /// </summary>
        private UnityTransform m_uiShareRoot = null;

        /// <summary>
        /// 游戏运行速度。
        /// </summary>
        private float m_gameSpeed = 1.0f;

        /// <summary>
        /// 宿主环境是否已初始化。
        /// </summary>
        private bool m_isHostInitialized = false;

        /// <summary>
        /// 是否正在解压资源。保留给旧模块引用。
        /// </summary>
        public static bool hasUncompress = false;

        #endregion

        #region 启动宿主

        /// <summary>
        /// 初始化运行时宿主环境。由 AppBoot 编排调用。
        /// </summary>
        public void InitializeHost()
        {
            if (m_isHostInitialized)
            {
                return;
            }

            m_isHostInitialized = true;

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Input.multiTouchEnabled = false;

            if (gameObject.GetComponent<DontDestroyObject>() == null)
            {
                gameObject.AddComponent<DontDestroyObject>();
            }

            Core.Coroutine.Instance().SetupTargetBehaviour(this);
            SystemStartup.Instance.OnIinit();
            AppsFlyerManager.Instance.InitializeSdk();
        }

        /// <summary>
        /// 请求启动权限。权限结果返回给 AppBoot，再由 AppBoot 决定是否进入 patch 流程。
        /// </summary>
        public void RequestStartupAuthorize(Action<bool> callback)
        {
            InitializeHost();
            Core.Coroutine.Instance().StartCoroutine(RequestStartupAuthorizeAsync(callback));
        }

        private IEnumerator RequestStartupAuthorizeAsync(Action<bool> callback)
        {
            yield return new WaitForSeconds(0.01f);
            StartupAuthorize.getInstance().Init();
            StartupAuthorize.getInstance().RequestCallback = (bool bResult) =>
            {
                if (bResult)
                {
                    SystemStartup.Instance.OnStartup(() =>
                    {
                        Core.Coroutine.Instance().StartCoroutine(Init());
                        callback?.Invoke(true);
                    });
                    return;
                }

                callback?.Invoke(false);
                Core.Debuger.LogError("RequestStartupAuthorize ERROR");
            };
        }

        #endregion

        #region Unity 生命周期

        protected void Awake()
        {
            gameObject.name = NativeCallbackObjectName;
            InitializeHost();
        }

        private void Start()
        {
        }

        private void Update()
        {
            // 旧网络消息中心刷新入口已停用。
            // MessageCenter.Instance().Update(Time.deltaTime * m_gameSpeed);

            // 旧 Lua 帧刷新入口已停用。
            // LuaManager.Instance().ListenToFrameUpdate(Time.deltaTime * m_gameSpeed);
        }

        private void OnApplicationPause(bool isPause)
        {
            // LuaManager.Instance().LuaOnApplicationPause(isPause);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            // LuaManager.Instance().LuaOnApplicationFocus(hasFocus);
        }

        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            OnEditorApplicationQuit();
#endif
            // LuaManager.Instance().LuaQuitGame();
        }

        private void OnDestroy()
        {
        }

        #endregion

        /// <summary>
        /// 初始化场景中的根节点引用。
        /// </summary>
        private void InitSceneRootNodes()
        {
            UnityGameObject sceneNode = UnityGameObject.Find(GlobalVar.GAME_OBJECT_SCENE);
            if (sceneNode != null)
            {
                Destroy(sceneNode);
            }

            sceneNode = new UnityGameObject(GlobalVar.GAME_OBJECT_SCENE);
            sceneNode.transform.SetParent(transform.parent);
            sceneNode.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
            sceneNode.AddComponent<DontDestroyObject>();
            m_sceneRoot = sceneNode.transform;

            UnityGameObject uiNode = UnityGameObject.Find(GlobalVar.GAME_OBJECT_CANVAS);
            if (uiNode != null)
            {
                m_uiRoot = uiNode.transform;
            }
            else
            {
                Core.Debuger.LogError("场景中没有 UI 根节点");
            }

            UnityGameObject uiShareNode = UnityGameObject.Find(GlobalVar.GAME_OBJECT_SHARE_CANVAS);
            if (uiShareNode != null)
            {
                m_uiShareRoot = uiShareNode.transform;
            }
            else
            {
                Core.Debuger.LogError("场景中没有分享 UI 根节点");
            }
        }

        #region 编辑器接口

        public static System.Action EditorApplicationQuitHandler;

        private void OnEditorApplicationQuit()
        {
#if UNITY_EDITOR
            if (EditorApplicationQuitHandler != null)
            {
                EditorApplicationQuitHandler();
            }

            // SocketManager.Instance.Close();
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        #endregion

        #region 公开属性

        /// <summary>
        /// 场景根节点。
        /// </summary>
        public UnityTransform SceneRoot
        {
            get
            {
                return m_sceneRoot;
            }
        }

        /// <summary>
        /// 场景 UI 根节点。
        /// </summary>
        public UnityTransform UiRoot
        {
            get
            {
                return m_uiRoot;
            }
        }

        /// <summary>
        /// 场景分享 UI 根节点。
        /// </summary>
        public UnityTransform UiShareRoot
        {
            get
            {
                return m_uiShareRoot;
            }
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 设置游戏运行速度。
        /// </summary>
        public void SetGameSpeed(float speed)
        {
            if (speed <= 0)
            {
                return;
            }

            m_gameSpeed = speed;
            Time.timeScale = speed;
        }

        /// <summary>
        /// 退出程序。
        /// </summary>
        public void ApplicationQuit()
        {
            OnApplicationQuit();
            Application.Quit();
        }

        #endregion
    }
}