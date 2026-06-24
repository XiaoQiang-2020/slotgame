using System.Collections;
using System.Security;
using Mono.Xml;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class AppSDKConfig
    {
        #region 私有变量
        private static SecurityParser SP;
        private static SecurityElement SE;
        private static SecurityElement SE_Basic;
        private static SecurityElement SE_Android;
        private static SecurityElement SE_IOS;
        private static string xmlFileName = "SDKConfig";

        private static string appName;
        private static string appPackageName;
        private static string wxAppID;
        private static string wxAppSecret;
        private static string appsflyKey = "";
        private static string appsflyAppId = "";
        private static string umengAppKey;
        private static string buglyAppID;
        private static string buglyAppKey;
        #endregion

        #region 公开属性
        public static string APP_NAME
        {
            get
            {
                if (string.IsNullOrEmpty(appName))
                {
                    CheckAndLoadXMLFile();
                    appName = GetNoodText(SE_Basic, "app_name");
                }

                return appName;
            }
        }

        public static string APP_PACKAGE_NAME
        {
            get
            {
                if (string.IsNullOrEmpty(appPackageName))
                {
                    CheckAndLoadXMLFile();
                    appPackageName = GetNoodText(SE_Basic, "app_package_name");
                }

                return appPackageName;
            }
        }

   

        /// <summary>
        /// AppsFlyer Dev Key.
        /// </summary>
        public static string SF_APP_KEY
        {
            get
            {
                return appsflyKey;
            }
            set
            {
                appsflyKey = value;
            }
        }

        /// <summary>
        /// AppsFlyer iOS Apple App ID. Android can keep this empty.
        /// </summary>
        public static string SF_APP_ID
        {
            get
            {
                return appsflyAppId;
            }
            set
            {
                appsflyAppId = value;
            }
        }

        #endregion

        #region 私有方法
        private static IEnumerator CheckAndLoadXMLFile()
        {
            if (null == SP)
            {
                SP = new SecurityParser();
                string xmlpath = Application.streamingAssetsPath + "/" + xmlFileName + ".xml";
                UnityWebRequest www = UnityWebRequest.Get("file://" + xmlpath);
                yield return www.SendWebRequest();
                string xmlContent = www.downloadHandler.text;
                if (null != xmlContent && !string.IsNullOrEmpty(xmlContent))
                {
                    SP.LoadXml(xmlContent);
                    SE = SP.ToXml();

                    foreach (SecurityElement se in SE.Children)
                    {
                        if (se.Tag.Equals("basic"))
                        {
                            SE_Basic = se;
                        }
                        else if (se.Tag.Equals("android"))
                        {
                            SE_Android = se;
                        }
                        else if (se.Tag.Equals("ios"))
                        {
                            SE_IOS = se;
                        }
                    }
                }
            }
        }

        private static SecurityElement GetPlatformNode()
        {
#if UNITY_ANDROID
            return SE_Android;
#elif UNITY_IOS || UNITY_IPHONE
            return SE_IOS;
#else
            return SE_Android ?? SE_IOS;
#endif
        }

        private static string GetNoodText(SecurityElement targetNood, string tag)
        {
            if (null == targetNood || string.IsNullOrEmpty(tag))
            {
                Core.Debuger.LogError("获取SDK配置表某节点的内容传入参数异常");
                return null;
            }

            foreach (SecurityElement se in targetNood.Children)
            {
                if (se.Tag.Equals(tag))
                {
                    return se.Text;
                }
            }

            return null;
        }
        #endregion
    }
}