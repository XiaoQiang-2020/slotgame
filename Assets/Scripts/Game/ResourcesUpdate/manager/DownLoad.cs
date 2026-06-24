using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;
namespace HotUpdate
{
    public class DownLoad
    {
        public static int OneReadLength = 16 * 1024;
#if UNITY_IOS
        public static int ReadWriteTimeOut = 10 * 1000;
        public static int TimeOutWait = 10* 1000;
        public static int ConnectionLimit = 200;
#else
        public static int ReadWriteTimeOut = 5 * 1000;
        public static int TimeOutWait = 2 * 1000;
        public static int ConnectionLimit = 100;
#endif

        /// <summary>
        /// 设置下次参数
        /// </summary>
        /// <param name="oneReadLength">一次下载长度</param>
        /// <param name="readWriteTimeOut">读取超时</param>
        /// <param name="timeOutWait">响应超时</param>
        /// <param name="connectionLimit">最大连接数</param>
        public static void SetDownLoadData(int oneReadLength, int readWriteTimeOut, int timeOutWait, int connectionLimit)
        {
            OneReadLength = oneReadLength;
            ReadWriteTimeOut = readWriteTimeOut;
            TimeOutWait = timeOutWait;
            ConnectionLimit = connectionLimit;
        }
        /// <summary>
        /// 单个文件下载
        /// </summary>
        /// <param name="downUnit">单个文件</param>
        /// <param name="callBack">下载回调</param>
        public static void DownLoadOneUnit(DownLoadUnit downUnit,Action<string> callBack)
        {
            //if (downUnit == null) if (null != callBack){ callBack("");}
            string error = "";
            long startPos = 0;
            string tempFile = downUnit.savePath + ".temp";
            FileStream fs = null;
            HttpWebRequest request = null;
            WebResponse respone = null;
            Stream ns = null;
            try
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                //else
                {
                    string direName = Path.GetDirectoryName(tempFile);
                    if (!Directory.Exists(direName)) Directory.CreateDirectory(direName);
                    fs = new FileStream(tempFile, FileMode.Create);
                }
                if(downUnit.downUrl.StartsWith("https",StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(downUnit.downUrl) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(downUnit.downUrl) as HttpWebRequest;
                }

                request.ReadWriteTimeout = ReadWriteTimeOut;
                request.ServicePoint.ConnectionLimit = ConnectionLimit;
                request.Timeout = TimeOutWait;
#if UNITY_IOS
                  request.KeepAlive =false;
#endif

                if (startPos > 0)
                {
                    request.AddRange((int)startPos);
                }
                respone = request.GetResponse();
                ns = respone.GetResponseStream();
                long totalSize = respone.ContentLength;
                long curSize = startPos;
                // 获取当前下载文件大小

                downUnit.size = totalSize;
                if (curSize >= totalSize)
                {
                    if (fs != null)
                    {
                        fs.Flush();
                        fs.Close();
                        fs = null;
                    }
                    if (File.Exists(downUnit.savePath)) File.Delete(downUnit.savePath);
                    File.Move(tempFile, downUnit.savePath);
                }
                else
                {
                    byte[] bytes = new byte[OneReadLength];
                    int readSize = ns.Read(bytes, 0, OneReadLength);
                    while (readSize > 0)
                    {
                        fs.Write(bytes, 0, readSize);
                        curSize += readSize;
                        readSize = ns.Read(bytes, 0, OneReadLength);
                        if (curSize >= totalSize)
                        {
                            break;
                        }
                    }
                    if (curSize >= totalSize)
                    {
                        if (fs != null)
                        {
                            fs.Flush();
                            fs.Close();
                            fs = null;
                        }
                        if (File.Exists(downUnit.savePath)) File.Delete(downUnit.savePath);
                        File.Move(tempFile, downUnit.savePath);
                    }
                    else
                    {
                        error = "download fail size error downsize = "+curSize +"  totalSize = "+totalSize;
                    }

                }
            }
            catch (TimeoutException timeOut)
            {
                error = "timeOut";
                Debug.LogError("下载出错:" + timeOut.ToString());
                // Log.LogDebug.PrintException("下载出错：aaaaaaaaaaa="+timeOut.ToString());
            }
            catch (Exception ex)
            {
                error = downUnit.id + "下载出错:" + ex.ToString();
                // Log.LogDebug.PrintException(downUnit.id +"下载出错：bbbbbbbbbbb="+ex.ToString());
                //Debug.LogError(error);
            }
            finally
            {
                //UnityEngine.Debug.LogError("=================下载线程应该结束了================");
                if (string.IsNullOrEmpty(error))
                {
                    downUnit.downState = DownLoadStateEnum.DOWNLOAD_SUCCESS;
                    //downUnit = null;
                }
                else
                {
                    if(downUnit.retryTimes >= DownManager.MaxRetryTimes)
                    {
                        downUnit.downState = DownLoadStateEnum.DOWNLOAD_FAIL;
                    }
                    else
                    {

                        downUnit.retryTimes++;
                        downUnit.downState = DownLoadStateEnum.DOWNLOAD_RETRY;
                    }
                }

                if (null != callBack)
                {
                    //UnityEngine.Debug.LogError("====下载完成回调====");
                    callBack(error);
                }

                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                    fs = null;
                }
                if (ns != null)
                {
                    ns.Close();
                    ns = null;
                }
                try{
                    if (respone != null)
                    {

                        respone.Close();
                        respone = null;
                    }
                    if (request != null)
                    {
                        request.Abort();
                        request = null;
                    }
                }
                catch (Exception ex1)
                {
                    Log.LogDebug.PrintException("xxxxxxxxxxxxxxx ex = "+ex1.ToString());
                }
#if UNITY_IOS
                System.GC.Collect();
#endif
            }
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }
    }
    public class DownLoadUnit
    {
        public int id;
        public string savePath;
        public long size;
        public string downUrl;
        public DownLoadStateEnum downState;//0,还没下 1，下载中 2，已经下载
        public bool checkState;//0，还没检查 1，已经检查

        public int retryTimes;

        public ResItem resItem;
        public DownLoadUnit(int id, string savePath, long size, string downUrl,ResItem item)
        {
            this.id = id;
            this.savePath = savePath;
            this.size = size;
            this.downUrl = downUrl;
            this.downState = 0;
            this.checkState = false;
            this.retryTimes = 0;
            this.resItem = item;
        }
    }
}