using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS || UNITY_IPHONE
using System.Runtime.InteropServices;
#endif

public class SdkBridge
{
#if UNITY_IOS || UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void CallMethodByCmd(string json, int cmd);
#endif

    #region Singleton

    private static SdkBridge _instance;

    public static SdkBridge getInstance()
    {
        if (_instance == null)
        {
            _instance = new SdkBridge();
        }
        return _instance;
    }

    #endregion

    private string androidMessageManagerName = Game.AppGameManager.NativeCallbackObjectName;
    private string u3dCallbackName = "UnitySendMessageCall";
    private string json = "";

    #region Fields

#if UNITY_ANDROID && !UNITY_EDITOR
    private const string AndroidClassName = "com.u3d.plugins.UnityAppActivity";
    private AndroidJavaClass m_androidJavaClass;
#endif

    private Dictionary<int, Action> m_DicCallbackCmd = new Dictionary<int, Action>();

    #endregion

    private SdkBridge()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        m_androidJavaClass = new AndroidJavaClass(AndroidClassName);
#endif
        var comp = AndroidIOSMessageManager.getInstance();
        androidMessageManagerName = comp.gameObject.name;
        comp.onFromAndroidCallback += sdkCallback;
        //#if !UNITY_IOS || UNITY_IPHONE
        //        IOSLibraryBridge.getInstance().iosInitAppPayForU3D(androidMessageManagerName, u3dCallbackName);
        //#endif
    }

    private void sdkCallback(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        int cmd = -1;
        LitJson.JsonData jd = LitJson.JsonMapper.ToObject(json);
        int.TryParse(jd["cmd"].ToString(), out cmd);
        if (m_DicCallbackCmd.ContainsKey(cmd) && m_DicCallbackCmd[cmd] != null)
        {
            Core.Debuger.Log(string.Format("LOG FORM C# ----- SdkBridge.sdkCallback() : To lua cmd:{0} data:{1}", cmd, json));
            m_DicCallbackCmd[cmd]();
        }
        else
        {
            Core.Debuger.LogWarning("LOG FORM C# ----- SdkBridge.sdkCallback() : There is no listener for cmd " + cmd);
        }
    }

    public void CallSdkForLua(string argument, int cmd, Action luaFunc)
    {
        if (m_DicCallbackCmd.ContainsKey(cmd))
        {
            if (m_DicCallbackCmd[cmd] != null)
            {
                // m_DicCallbackCmd[cmd].Dispose();
                m_DicCallbackCmd[cmd] = null;
            }
            m_DicCallbackCmd[cmd] = luaFunc;
        }
        else
        {
            m_DicCallbackCmd.Add(cmd, luaFunc);
        }

        Core.Debuger.Log(string.Format("LOG FORM C# ----- SdkBridge.CallSdkForLua() : Add listener for cmd:{0} arguement:{1}", cmd, argument));
        json = PackageJson(argument);

#if UNITY_EDITOR
        Core.Debuger.Log("no supoort platorm");
#elif UNITY_ANDROID
        m_androidJavaClass.CallStatic("CallMethodByCmd", json, cmd);
#elif UNITY_IOS || UNITY_IPHONE
        CallMethodByCmd(json, cmd);
#endif
    }

    public string PackageJson(string param)
    {
        LitJson.JsonData data = new LitJson.JsonData();
        data["u3dObjName"] = androidMessageManagerName;
        data["u3dMethodName"] = u3dCallbackName;
        data["jsonInfo"] = param;
        json = data.ToJson();
        return json;
    }
}