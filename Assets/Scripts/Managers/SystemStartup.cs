/***************************************************
 * 文件名：SystemStartup.cs
 * 描  述：
 * 时  间：2018-11-06 14:08:29
 * 作  者：尼尔
 * 修  改：
 ***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class SystemStartup : MonoSingleton<SystemStartup>
    {
        public enum STARTUP_STAGE {
            ENONE       = 0,
            ESTARTUPING = 1,
            ESUCCESS   = 2,
            EEND        = 3,
        };

        STARTUP_STAGE m_stage = STARTUP_STAGE.ENONE;

        Action m_callback;
        bool m_isInit = false;
        bool m_isStart = false;

        public bool IsInit() {
            return m_isInit;
        }

        public void OnIinit() {
            if (m_isInit)
            {
                Core.Debuger.LogWarning("SystemStartup:OnIinit has init");
                return;
            }
            m_isInit = true;
            m_stage = STARTUP_STAGE.ESTARTUPING;
            // LuaManager.Instance().InitBind((bool bResult) =>
            // {
            //     SystemStartup.Instance.m_stage = STARTUP_STAGE.ESUCCESS;
            // });
           m_stage = STARTUP_STAGE.ESUCCESS;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void OnStartup(Action callback) {
            if (null == callback)
            {
                Core.Debuger.LogError("SystemStartup: callback = null");
            }

            m_callback = callback;
            m_isStart = true;
        }

        private void Awake()
        {
            gameObject.AddComponent<DontDestroyObject>();
        }

        private void Start()
        {

        }

        private void Update()
        {
            if (!m_isStart)
            {
                return;
            }
            if (m_stage < STARTUP_STAGE.ESUCCESS)
            {
                return;
            }
            if (STARTUP_STAGE.ESUCCESS == m_stage)
            {
                if (null != m_callback)
                {
                    m_callback();
                }
                m_stage = STARTUP_STAGE.EEND;
            }
        }
        private void FixedUpdate()
        {

        }

        private void LateUpdate()
        {

        }
    }
}



