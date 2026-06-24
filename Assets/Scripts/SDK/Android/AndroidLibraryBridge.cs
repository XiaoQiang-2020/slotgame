


using UnityEngine;
using System;
public class AndroidLibraryBridge
{
    #region 单例对象
    private static AndroidLibraryBridge _instance;
    public static AndroidLibraryBridge getInstance()
    {
        if (_instance == null)
        {
            _instance = new AndroidLibraryBridge();
        }
        return _instance;
    }
    #endregion

    #region 私有变量
    /// <summary>
    /// 与Android通信的类名
    /// </summary>
    private string androidMessageManagerName = "AndroidMessageManager";
    /// <summary>
    /// 接收电量回调函数
    /// </summary>
    private string u3dCallbackName_onReceiveBatteryReceiver = "onReceiveBatteryReceiver";
    /// <summary>
    /// 与Android交互的JAVA类
    /// </summary>
    private AndroidJavaClass myLibraryUtil;
    private string AndroidClassName_MyLibraryUtil = "com.u3d.plugins.MyLibraryUtil";
    #endregion

    #region 公开变量
    /// <summary>
    /// 电量回调函数
    /// </summary>
    public System.Action<string> onBatteryChangedCallback;
    /// <summary>
    /// LUA电量回调函数
    /// </summary>
    public Action onBatteryChangedCallbackForLua;
    #endregion

    #region 构造函数
    private AndroidLibraryBridge()
    {
        myLibraryUtil = new AndroidJavaClass(AndroidClassName_MyLibraryUtil);
        var comp = AndroidIOSMessageManager.getInstance();
        androidMessageManagerName = comp.gameObject.name;
        comp.onReceiveBatteryReceiverCallBack += onReceiveBatteryReceiver;
    }
    #endregion

    #region 私有函数


    /// <summary>
    /// 接收来息android系统的电量广播
    /// </summary>
    /// <param name="json"></param>
    void onReceiveBatteryReceiver(string json)
    {
        if (onBatteryChangedCallback != null)
        {
            onBatteryChangedCallback(json);
        }
        if (onBatteryChangedCallbackForLua != null)
        {
            onBatteryChangedCallbackForLua();
        }
    }
    #endregion

    #region 公开接口
    /// <summary>
    /// 显示一个android提示
    /// </summary>
    /// <param name="content"></param>
    public void ShowToast(string content)
    {
        myLibraryUtil.CallStatic("ShowToast", content);
    }

    /// <summary>
    /// 振动手机
    /// </summary>
    public void SetVibrator()
    {
        myLibraryUtil.CallStatic("SetVibrator");
    }

    /// <summary>
    /// 注册：电量广播接收器
    /// </summary>
    public void RegisterBatteryReceiver()
    {
        myLibraryUtil.CallStatic("RegisterBatteryReceiver", androidMessageManagerName, u3dCallbackName_onReceiveBatteryReceiver);
    }
    public void RegisterBatteryReceiverForLua(Action func)
    {
        if (onBatteryChangedCallbackForLua != null)
        {
            // onBatteryChangedCallbackForLua.Dispose();
            onBatteryChangedCallbackForLua = null;
        }
        onBatteryChangedCallbackForLua = func;
        myLibraryUtil.CallStatic("RegisterBatteryReceiver", androidMessageManagerName, u3dCallbackName_onReceiveBatteryReceiver);
    }

    /// <summary>
    /// 注销：电量广播接收器
    /// </summary>
    public void UnregisterBatteryReceiver()
    {
        myLibraryUtil.CallStatic("UnregisterBatteryReceiver");
        if (onBatteryChangedCallback != null)
        {
            onBatteryChangedCallback = null;
        }
        if (onBatteryChangedCallbackForLua != null)
        {
            // onBatteryChangedCallbackForLua.Dispose();
            onBatteryChangedCallbackForLua = null;
        }
    }

    /// <summary>
    /// 网络信号强度
    /// 信号范围-100 到 0， 0信号最好
    /// </summary>
    public int GetNetSignalStrength()
    {
        return myLibraryUtil.CallStatic<int>("GetNetSignalStrength");
    }

    /// <summary>
    /// 将文本内容放到系统剪贴板里
    /// </summary>
    /// <param name="str"></param>
    public void CopyStringToClipBoard(string str)
    {
        myLibraryUtil.CallStatic("CopyStringToClipboard", str);
    }

    ///// <summary>
    ///// 打电话
    ///// </summary>
    ///// <param name="phone"></param>
    //public void CallPhone(string phone)
    //{
    //    myLibraryUtil.CallStatic("CallPhone", phone);
    //}

    /// <summary>
    /// 获取网络强度
    /// 返回一个json，有两个字段：isWifi 和 strength 和 asu
    ///   isWifi:ture or false, 如果是false忽略strength为：0-没有网络，1-有网络
    ///   strength:0--4，数字越大表示信号越强
    /// </summary>
    /// <returns></returns>
    public string GetNetworkSigleStrength()
    {
        string json = myLibraryUtil.CallStatic<string>("GetNetworkSigleStrength");
        return json;
    }
    #endregion
}