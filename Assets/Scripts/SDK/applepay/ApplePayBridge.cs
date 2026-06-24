

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ApplePayBridge
{
    #region 单例对象
    private static ApplePayBridge _instance;
    public static ApplePayBridge getInstance()
    {
        if (_instance == null)
        {
            _instance = new ApplePayBridge();
        }
        return _instance;
    }
    #endregion

    #region 私有变量
    /// <summary>
    /// 与IOS交互的类名
    /// </summary>
    private string androidMessageManagerName = "AndroidMessageManager";
    /// <summary>
    /// 交互回调的函数名
    /// </summary>
    private string u3dCallbackName = "onAppPayResult";
    #endregion

    #region 公开变量
    /// <summary>
    /// 回调函数
    /// </summary>
    public System.Action<string> onAppPayResultCallBack = null;
    /// <summary>
    /// LUA回掉函数
    /// </summary>
    // public LuaFunction onAppPayResultCallBackForLua = null;
    #endregion

    #region 私有函数
    private ApplePayBridge()
    {
        var comp = AndroidIOSMessageManager.getInstance();
        androidMessageManagerName = comp.gameObject.name;
        comp.onAppPayResultCallBack += onAppPayResult;
#if !UNITY_IOS || UNITY_IPHONE
        IOSLibraryBridge.getInstance().iosInitAppPayForU3D(androidMessageManagerName, u3dCallbackName);
#endif
    }
    #endregion

    #region 公开接口
    /// <summary>
    /// 调用苹果支付
    /// </summary>
    /// <param name="productID">商品id</param>
    public void iosAppPay(string productID,string itemId, string orderID)
    {
#if UNITY_IOS || UNITY_IPHONE
		IOSLibraryBridge.getInstance().iosAppPay(productID,itemId, orderID);
#endif
    }

    /// <summary>
    /// Lua调用苹果支付
    /// </summary>
    /// <param name="productID"></param>
    /// <param name="orderID"></param>
    /// <param name="luaFunc"></param>
    public void iosAppPayForLua(string productID,string itemId, string orderID, Action luaFunc)
    {
#if UNITY_IOS || UNITY_IPHONE
        // if (onAppPayResultCallBackForLua != null)
        // {
        //     onAppPayResultCallBackForLua.Dispose();
        //     onAppPayResultCallBackForLua = null;
        // }

        // onAppPayResultCallBackForLua = luaFunc;

	   IOSLibraryBridge.getInstance().iosAppPay(productID,itemId, orderID);
#endif
    }

    /// <summary>
    /// 支付回调
    /// </summary>
    /// <param name="json"></param>
    void onAppPayResult(string json)
    {
        if (null != onAppPayResultCallBack)
        {
            onAppPayResultCallBack(json);
        }

        // if (onAppPayResultCallBackForLua != null)
        // {
        //     onAppPayResultCallBackForLua.Call(json);
        // }
    }
    #endregion
}