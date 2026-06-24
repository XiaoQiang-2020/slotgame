/***************************************************
 * 文件名：LoggerManager.cs
 * 描  述：log管理类
 * 时  间：2018-08-01 14:04:15
 * 作  者：李红轩
 * 修  改：
 ***************************************************/
using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
namespace Log
{
    public class LoggerManager:MonoBehaviour
    {
        static LoggerManager instance;
        public static LoggerManager Intance
        {
            get {
                if (instance == null)
                {
                    GameObject o = new GameObject("LoggerManager");
                    instance = o.AddComponent<LoggerManager>();
                }
                return instance;
            }
        }
        /// <summary>
        /// debug的开关   非编辑器下关闭debug
        /// </summary>
#if UNITY_EDITOR
        public static bool isDebug = true;
#else
        public static bool isDebug = false;
#endif


#if UNITY_EDITOR
        public static bool isWriteLog = false;
#else
        public static bool isWriteLog = true;
#endif

        /// <summary>
        /// 玩家id保存的key值
        /// </summary>
        public const string USER_ID_KEY = "user_id_key";
        public const string USER_URL_KEY = "user_url_key";
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            isWriteLog = true;
            if (isWriteLog)
            {
                FileLog.Init();
                PutFileToSever();
            }
        }
        private void OnEnable()
        {
            Application.logMessageReceived += OnLogCallBack;
        }
        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogCallBack;
        }
        /// <summary>
        /// 监听控制台输出日志
        /// </summary>
        /// <param name="condition">日志</param>
        /// <param name="stackTrace">堆栈</param>
        /// <param name="type">类型</param>
        void OnLogCallBack(string condition, string stackTrace, LogType type)
        {
            if (!isWriteLog) return;
            FileLog.WriteFile(condition);
        }
        /// <summary>
        /// 本地保存一次玩家id
        /// </summary>
        /// <param name="userID">玩家id</param>
        public void SetUserID(string userID)
        {
            PlayerPrefs.SetString(USER_ID_KEY, userID);
        }
        /// <summary>
        /// 设置提交url
        /// </summary>
        /// <param name="url"></param>
        public void SetUploadUrl(string url)
        {
            PlayerPrefs.SetString(USER_URL_KEY, url);
        }
        /// <summary>
        /// 获取本地保存的玩家id
        /// </summary>
        /// <returns></returns>
        public string GetUserID()
        {
            return PlayerPrefs.GetString(USER_ID_KEY);
        }
        public string GetUploadUrl()
        {
            return PlayerPrefs.GetString(USER_URL_KEY);
        }
        /// <summary>
        /// 输出基本日志
        /// </summary>
        /// <param name="logType"></param>
        public void Log(string logMsg, LOG_TYPE logType = LOG_TYPE.LOG_NORMAL)
        {
            StringBuilder sb = new StringBuilder();

            string msgHead = "[" + logType+"]";
            string msgTime = "[" + DateTime.Now.ToString()+"]";
            if (logType == LOG_TYPE.LOG_ERROR)
            {

            }
            //string msgUserId = "[" + GetUserID() + "]";
            sb.Append(msgHead);
            sb.Append(msgTime);
            //sb.Append(msgUserId);
            sb.Append(logMsg);

            if (isDebug)
            {
                //控制台输出debug日志
                // 这里会触发LOG回调，回调会判断是否写入文件
                ShowConsoleLog(sb.ToString(), logType);
            }
            else
            {
                if (isWriteLog)
                {
                    //日志写入文件
                    FileLog.WriteFile(sb.ToString());
                }
            }
        }

        /// <summary>
        /// 控制台输出日志
        /// </summary>
        /// <param name="logMsg"></param>
        /// <param name="logType"></param>
        private void ShowConsoleLog(string logMsg, LOG_TYPE logType = LOG_TYPE.LOG_NORMAL)
        {
            switch (logType)
            {
                case LOG_TYPE.LOG_NORMAL:
                    Debug.Log(logMsg);
                    break;
                case LOG_TYPE.LOG_ERROR:
                    Debug.LogError(logMsg);
                    break;
                case LOG_TYPE.LOG_EXCEPTION:
                    Debug.LogError(logMsg);
                    break;
            }
        }
        /// <summary>
        /// 上传日志
        /// </summary>
        public void PutFileToSever()
        {
            // if (string.IsNullOrEmpty(GetUserID()) || string.IsNullOrEmpty(GetUploadUrl())) return;
            // StartCoroutine(PutLogToSvr.PutFileLog(FileLog.logFilePaths[0], GetUserID(), GetUploadUrl()));
        }

        private void OnApplicationQuit()
        {
            FileLog.FlushAndCloseLogWriter();
        }
    }

    /// <summary>
    /// 日志输出类型
    /// </summary>
    public enum LOG_TYPE
    {
        LOG_NORMAL = 0,// 普通
        LOG_ERROR  = 1,// 错误
        LOG_EXCEPTION  = 2,// 异常
    }
}