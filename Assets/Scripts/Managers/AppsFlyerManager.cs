namespace Game
{
    using System.Collections.Generic;
    using AppsFlyerSDK;
    using UnityEngine;

    public class AppsFlyerManager : MonoSingleton<AppsFlyerManager>, IAppsFlyerConversionData
    {
        private bool m_isInitialized;

        public void InitializeSdk()
        {
#if (UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            if (m_isInitialized)
            {
                return;
            }

            if (gameObject.GetComponent<DontDestroyObject>() == null)
            {
                gameObject.AddComponent<DontDestroyObject>();
            }

            StartAppsFlyer();
#else
            Core.Debuger.Log("AppsFlyerManager: skip AppsFlyer init on unsupported platform.");
#endif
        }

#if (UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
        private void StartAppsFlyer()
        {
            string devKey = AppSDKConfig.SF_APP_KEY;
            string appId = AppSDKConfig.SF_APP_ID;

            if (string.IsNullOrEmpty(devKey))
            {
                Core.Debuger.LogWarning("AppsFlyerManager: SF_APP_KEY is empty, skip AppsFlyer init.");
                return;
            }

#if UNITY_IOS || UNITY_IPHONE
            if (string.IsNullOrEmpty(appId))
            {
                Core.Debuger.LogWarning("AppsFlyerManager: SF_APP_ID is empty, skip AppsFlyer init on iOS.");
                return;
            }
#endif

            AppsFlyer.setIsDebug(false);
            AppsFlyer.initSDK(devKey, appId, this);
#if UNITY_IOS && !UNITY_EDITOR
            // 请求ATT权限，并给SDK最多60秒等待时间
            AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
            // 在你的UI流程中适当位置调用原生ATT请求对话框
            // 可以参考 Unity 的 iOS Support package
#endif
            AppsFlyer.startSDK();

            m_isInitialized = true;
            Core.Debuger.Log("AppsFlyerManager: AppsFlyer SDK started.");
        }
#endif

        public void LogPurchaseEvent(string productId, string currency, double revenue, string receiptId = null)
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { AFInAppEvents.CURRENCY, currency },
                { AFInAppEvents.REVENUE, revenue.ToString() },
                { AFInAppEvents.CONTENT_ID, productId }
            };

            if (!string.IsNullOrEmpty(receiptId))
            {
                eventValues.Add(AFInAppEvents.RECEIPT_ID, receiptId);
            }

            SendEvent(AFInAppEvents.PURCHASE, eventValues);
        }

        public void LogCustomEvent(string eventName)
        {
            SendEvent(eventName, null);
        }

        public void SendEvent(string eventName, Dictionary<string, string> eventValues = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Core.Debuger.LogWarning("AppsFlyerManager: eventName is empty, skip AppsFlyer event.");
                return;
            }

            AppsFlyer.sendEvent(eventName, eventValues);
        }

        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("onConversionDataFail", error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }
    }
}
