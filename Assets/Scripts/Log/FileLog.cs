/***************************************************
 * 文件名：FileLog.cs
 * 描  述：日志文件书写
 * 时  间：2018-08-01 14:04:15
 * 作  者：李红轩
 * 修  改：
 ***************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading;
namespace Log
{
    public class FileLog
    {
        /// <summary>
        /// 文件1
        /// </summary>
        public static string FileName1 = "";
        /// <summary>
        /// 文件2
        /// </summary>
        public static string FileName2 = "";
        /// <summary>
        /// 当前书写的这个文件
        /// </summary>
        private static string CurrentFilePath;
        /// <summary>
        /// 当前书写文件编号
        /// </summary>
        private static int CurrentFileIndex;
        /// <summary>
        /// 下一个书写日志
        /// </summary>
        private static string NextFilePath;
        /// <summary>
        /// 书写最大值
        /// </summary>
        public static int MaxLength = 1 * 1024 * 1000;
        public static int MaxLogFileNum = 5;
        public static string BaseName = "log_";
        public static List<string> logFilePaths = new List<string>();

        /// <summary>
        /// 当前写入数据流
        /// </summary>
        static StreamWriter opensw;
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            CreateFile();
            GetCurrentFilePath();
        }

        /// <summary>
        /// 创建日志文件
        /// </summary>
        public static void CreateFile()
        {
            string fileName = "";
            FileStream fs = null;
            for (int i = 0; i < MaxLogFileNum; i++)
            {
                fileName = GetLogPath(BaseName + i);
                if (!File.Exists(fileName))
                {
                    try
                    {
                        fs = File.Create(fileName);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Create log file error: " + ex);
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
                logFilePaths.Add(fileName);
            }
        }

        /// <summary>
        /// 获取当前写入文件路径
        /// </summary>
        public static void GetCurrentFilePath()
        {
            bool haveFile = false;
            for (int i = 0; i < logFilePaths.Count; i++)
            {
                FileInfo info = new FileInfo(logFilePaths[i]);
                if (info.Length < MaxLength)
                {
                    CurrentFilePath = logFilePaths[i];
                    CurrentFileIndex = i;
                    haveFile = true;
                    return;
                }
            }
            if (!haveFile)
            {
                CurrentFilePath = logFilePaths[0];
                CurrentFileIndex = 0;
            }
        }

        /// <summary>
        /// 获取下一个写入文件
        /// </summary>
        /// <returns></returns>
        public static string GetNextFilePath()
        {
            string nextPath = "";
            if (CurrentFileIndex < logFilePaths.Count - 1)
            {
                CurrentFileIndex = CurrentFileIndex + 1;
            }
            else
            {
                CurrentFileIndex = 0;
            }
            nextPath = logFilePaths[CurrentFileIndex];
            return nextPath;
        }


        static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="fileContext"></param>
        public static void WriteFile(string fileContext)
        {
            if (CurrentFilePath == null) return;
            try
            {
                // LogWriteLock.EnterWriteLock();
                if(opensw == null)
                {
                    opensw = new StreamWriter(File.Open(CurrentFilePath, FileMode.Append));
                    opensw.AutoFlush = true;
                    if(opensw.BaseStream.Length == 0)
                    {
                        opensw.WriteLine(OutputSystemInfo());
                    }
                }
                if (opensw.BaseStream.Length > MaxLength)
                {
                    opensw.Dispose();
                    NextFilePath = GetNextFilePath();
                    if (File.Exists(NextFilePath))
                    {
                        File.Delete(NextFilePath);
                    }
                    opensw = new StreamWriter(File.Open(NextFilePath, FileMode.Append));
                    opensw.AutoFlush = true;
                    CurrentFilePath = NextFilePath;
                    opensw.WriteLine(OutputSystemInfo());
                }
                opensw.WriteLine(fileContext);
            }
            catch (Exception )
            {
                // LOG回调里面调LOG会导致死循环
                //LogDebug.PrintException("=========ex=========" + ex);
            }
            // finally
            // {

            //     LogWriteLock.ExitWriteLock();
            // }
        }


        /// <summary>
        /// 设置及获取对应发布平台的日记存放路径
        /// </summary>
        /// <param name="logName"></param>
        private static string GetLogPath(string logName)
        {
            string m_logPath = "";
            string m_logFile = "Log";
            switch (Application.platform)
            {

                case RuntimePlatform.Android:
                    {
                        m_logPath = string.Format("{0}/{1}", Application.persistentDataPath, m_logFile);
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                    {
                        m_logPath = string.Format("{0}/{1}", Application.persistentDataPath, m_logFile);
                    }
                    break;
                case RuntimePlatform.WindowsPlayer:
                    {
                        m_logPath = string.Format("{0}/../{1}", Application.dataPath, m_logFile);
                    }
                    break;
                case RuntimePlatform.WindowsEditor:
                    {
                        m_logPath = string.Format("{0}/../{1}", Application.dataPath, m_logFile);
                    }
                    break;
                case RuntimePlatform.OSXEditor:
                    {

                    }
                    break;
            }
            if (!Directory.Exists(m_logPath))
            {
                Directory.CreateDirectory(m_logPath);
            }
            m_logPath = m_logPath +"/" +logName;
            return m_logPath;
        }
        /// <summary>
        /// 设备信息
        /// </summary>
        /// <returns></returns>
        private static string OutputSystemInfo()
        {
            string str2 = string.Format("logger Start, time: {0}, version: {1}.", DateTime.Now.ToString(), Application.unityVersion);

            string systemInfo = SystemInfo.operatingSystem + " "
                                + "deviceModel：" + SystemInfo.deviceModel + " "
                                + "processorType：" + SystemInfo.processorType + " " + SystemInfo.processorCount + " "
                                + "memorySize:" + SystemInfo.systemMemorySize + " "
                                + "Graphics: " + SystemInfo.graphicsDeviceName + " vendor: " + SystemInfo.graphicsDeviceVendor
                                + " memorySize: " + SystemInfo.graphicsMemorySize + " " + SystemInfo.graphicsDeviceVersion;
            string sysInfo = string.Format("{0}\n{1}", str2, systemInfo);
            return sysInfo;
        }

        // 正常退出时，主动关掉log，非正常退出时由系统关闭文件流
        public static void FlushAndCloseLogWriter()
        {
            if(opensw != null)
            {
                opensw.Flush();
                opensw.Dispose();
                opensw = null;
                CurrentFilePath = null;
            }
        }

    }

}
