
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
namespace Game
{
    public class WWWDownloader
    {
        public class DownloadUnit
        {
            public string downUrl;
            public string svaePath;
            public System.Action<bool, string> csharpFunc;


            public override string ToString()
            {
                return string.Format("downUrl: {0}, svaePath: {1}, ", downUrl, svaePath);
            }

        };

        static Dictionary<string, DownloadUnit> downDic = new Dictionary<string, DownloadUnit>();


        public static UnityWebRequest DownloadSync(string url)
        {
            return DownloadSync(url, null);
        }

        public static UnityWebRequest DownloadSync(string url, WWWForm form)
        {
            UnityWebRequest www = null;
            if (null != form)
            {
                www = UnityWebRequest.Post(url, form);
            }
            else
            {
                www = UnityWebRequest.Get(url);
            }

            YieldToStop(www);
            return www;
        }

        private static void YieldToStop(UnityWebRequest www)
        {
            var @enum = DownloadEnumerator(www);
            while (@enum.MoveNext()) ;
        }
        private static IEnumerator DownloadEnumerator(UnityWebRequest www)
        {
            while (!www.isDone) ;
            yield return www;
        }

        public static bool IsUrl(string url, string strRegex = null)
        {
            try
            {
                if (null == strRegex || 0 == strRegex.Length)
                {
                    strRegex = @"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?";
                }
                bool result = System.Text.RegularExpressions.Regex.IsMatch(url, strRegex);
                return result;
            }
            catch (Exception e)
            {
                Core.Debuger.LogError(e.Message);
                return false;
            }
        }

        public static void AddDownload(string url, System.Action<bool, string> func, bool canConver = false)
        {
            AddDownload(url, func,  canConver);
        }

        static IEnumerator StartDownload(DownloadUnit unit)
        {
            UnityWebRequest www = UnityWebRequest.Get(unit.downUrl);

            yield return www.SendWebRequest();

            bool succeed = false;
            string str = "error";
            if (www.result == UnityWebRequest.Result.Success)
            {
                succeed = true;
                str = unit.svaePath;
                byte[] data = www.downloadHandler.data;
                Game.FileUtils.writeBytes(unit.svaePath, data);
                yield return new WaitUntil(() =>
                {
                    bool b = Game.FileUtils.isFileExist(unit.svaePath);
                    return b;
                });
            }
            else
            {
                succeed = false;
                str = www.error;
            }

            Core.Debuger.Log(unit.ToString());
            if (unit.csharpFunc != null)
            {
                unit.csharpFunc(succeed, str);
            }

            downDic.Remove(unit.downUrl);
        }

        /// <summary>
        /// http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <returns>保存在本地的路径</returns>
        public static string HttpDownload(string url)
        {
            //保存路径
            string savePath = Game.CommonTool.md5(url);
            savePath = Game.FileUtils.getInstance().getWritablePath() + "/" + Game.GlobalVar.TEMPT_ROOT + "/" + savePath;
            if (System.IO.File.Exists(savePath))
            {
                System.IO.File.Delete(savePath);    //存在则删除
            }
            try
            {
                FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = System.Net.WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                fs.Close();
                responseStream.Close();
                return savePath;
            }
            catch (Exception )
            {
                Debug.LogError("下载微信防屏蔽分享出错了");
                return null;
            }

        }
        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="path">文件的路径</param>
        /// <returns></returns>
        public static string GetJsonStr(string path)
        {
            return Game.FileUtils.getInstance().GetStringFromFile(path);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        private static string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(string result)
        {
            return DecodeBase64(Encoding.UTF8, result);
        }

        #region----------------带参数的www请求------------------
        /// <summary>
        /// 单纯的上报日志
        /// </summary>
        /// <param name="userName">自己的用户名</param>
        /// <param name="logData">上报的日志内容</param>
        public static void WWWUploadLog(string userName, string logData)
        {
            if (!Core.Debuger.isDebug) return;
            string url = "http://192.168.8.129:8080/xGame/LogUpload.action";
            WWWForm form = new WWWForm();
            form.AddField("username", userName);
            form.AddField("info", logData);
            WWWDownLoadSomething(url, form);
        }
        public static void WWWDownLoadSomething(string url, WWWForm form)
        {
            Core.Coroutine.Instance().StartCoroutine(WWWDownLoad(url, form));
        }
        static IEnumerator WWWDownLoad(string url, WWWForm form)
        {
            UnityWebRequest request =  UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debugger.LogError("=========成功==========");
            }
            else
            {
                //Debugger.LogError("=========失败==========" + www.error);
            }
        }
        #endregion----------------带参数的www请求------------------
    }
}