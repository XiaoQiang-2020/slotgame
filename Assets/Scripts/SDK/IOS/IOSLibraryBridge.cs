/***************************************************
 * 文件名： IOSLibraryBridge.cs
 * 描  述：
 * 时  间： 2017-08-24 12:08:24
 * 作  者： 李智海
 * 修  改：
 ***************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class IOSLibraryBridge
{
#if UNITY_IOS || UNITY_IPHONE
	// 杂项
	[DllImport("__Internal")]
	private static extern void getShock ();
	[DllImport("__Internal")]
	private static extern void getCopy (string content);
	[DllImport("__Internal")]
	private static extern float getBatteryLevel ();
	[DllImport("__Internal")]
	private static extern string getNetWorkState ();
	[DllImport("__Internal")]
	private static extern int getWifiSignalStrength ();

    //横竖屏切换
    [DllImport("__Internal")]
	private static extern void ChangeRootViewControllerLandscape();
    [DllImport("__Internal")]
	private static extern void ChangeRootViewControllerVertialScreen();

	// 微信
	[DllImport("__Internal")]
	private static extern void initWXForU3D(string _wechatAppID,string _u3dGameObjectName, string _u3dCallbackName_onResp, string _u3dCallbackName_onReq);
	[DllImport("__Internal")]
	private static extern void openWX();
	[DllImport("__Internal")]
	private static extern bool isWXAppInstalled();
	[DllImport("__Internal")]
	private static extern void getWechatAuth();
	[DllImport("__Internal")]
	private static extern void sendShareInfoToWX(int actiontype, int target, string jsonInfo);

	// 定位
	[DllImport("__Internal")]
	private static extern void getLocation(string _u3dGameObjectName, string _u3dCallbackName_onLocation);

	// 七鱼客服
	[DllImport("__Internal")]
	private static extern void initQYForU3D(string _u3dGameObjectName, string _u3dCallbackName_onQYUnreadCountChanged);
	[DllImport("__Internal")]
	private static extern void openQiyu(string userID, string userName, string gameID);
	[DllImport("__Internal")]
	private static extern int u3dGetUnReadCount();

	// 苹果支付
	[DllImport("__Internal")]
	private static extern void initAppPayForU3D(string _u3dGameObjectName, string _u3dCallbackName_onAppPayResult);
	[DllImport("__Internal")]
	private static extern void AppPay(string productID,string itemId, string cusID);
#endif

    //	string iosMessageManagerName = "AndroidIOSMessageManager";
    //	string u3dCallbackName = "";

    static IOSLibraryBridge _instance;
	public static IOSLibraryBridge getInstance()
	{
		if (_instance == null) {
			_instance = new IOSLibraryBridge();
		}
		return _instance;
	}

	private IOSLibraryBridge()
	{
//		var comp = AndroidIOSMessageManager.getInstance();
//		iosMessageManagerName = comp.name;

	}

	#region 杂项
	/// <summary>
	/// ios震动
	/// </summary>
	public void iosGetShock ()
	{
		#if UNITY_IOS || UNITY_IPHONE
		getShock();
		#endif
	}

	/// <summary>
	/// Ios复制文本到剪切板
	/// </summary>
	public void iosGetCopy (string content)
	{
		#if UNITY_IOS || UNITY_IPHONE
		getCopy(content);
		#endif
	}

	/// <summary>
	/// ios获取电量
	/// </summary>
	public float iosGetBatteryLevel ()
	{
		#if UNITY_IOS || UNITY_IPHONE
		return getBatteryLevel();
		#else
		return 100f;
		#endif
	}

	/// <summary>
	/// ios 获取网络状态
	/// 所有的返回值：unknown 2G 3G 4G Wifi
	/// </summary>
	/// <returns>The get net work state.</returns>
	public string iosGetNetWorkState()
	{
		#if UNITY_IOS || UNITY_IPHONE
		return getNetWorkState();
		#else
		return "unknown";
		#endif
	}

	/// <summary>
	/// ios 获取wifi信号强度
	/// 所有的返回值：unknown 2G 3G 4G Wifi
	/// </summary>
	/// <returns>The get net work state.</returns>
	public int iosGetWifiSignalStrength()
	{
		#if UNITY_IOS || UNITY_IPHONE
		return getWifiSignalStrength();
		#else
		return 0;
		#endif
	}
    #endregion

    #region 横竖屏切换
    public void ChangeScreenToLandscape()
    {
        #if UNITY_IOS || UNITY_IPHONE
		ChangeRootViewControllerLandscape();
        #endif
    }

    public void ChangeScreenToPortrait()
    {
        #if UNITY_IOS || UNITY_IPHONE
		ChangeRootViewControllerVertialScreen();
        #endif
    }

    #endregion



    #region 微信
    public void iosInitWXForU3D(string _wechatAppID,string _u3dGameObjectName, string _u3dCallbackName_onResp, string _u3dCallbackName_onReq)
	{
#if UNITY_IOS || UNITY_IPHONE
		initWXForU3D(_wechatAppID,_u3dGameObjectName, _u3dCallbackName_onResp, _u3dCallbackName_onReq);
#endif
	}

	public void iosOpenWX()
	{
#if UNITY_IOS || UNITY_IPHONE
		openWX();
#endif
	}

	public bool iosIsWXAppInstalled()
	{
#if UNITY_IOS || UNITY_IPHONE
		return isWXAppInstalled();
#else
		return false;
#endif
	}

	public void iosGetWechatAuth()
	{
#if UNITY_IOS || UNITY_IPHONE
		getWechatAuth();
#endif
	}
	public void iosSendShareInfoToWX(int actiontype, int target, string jsonInfo)
	{
#if UNITY_IOS || UNITY_IPHONE
		sendShareInfoToWX(actiontype, target, jsonInfo);
#endif
	}
#endregion

#region 定位
	public void iosGetLocation(string _u3dGameObjectName, string _u3dCallbackName_onLocation)
	{
#if UNITY_IOS || UNITY_IPHONE
		getLocation(_u3dGameObjectName, _u3dCallbackName_onLocation);
#endif
	}
#endregion

#region 七鱼客服
	public void iosInitQYForU3D(string _u3dGameObjectName, string _u3dCallbackName_onQYUnreadCountChanged)
	{
#if UNITY_IOS || UNITY_IPHONE
		initQYForU3D(_u3dGameObjectName, _u3dCallbackName_onQYUnreadCountChanged);
#endif
	}

	public void iosOpenQiyu(string userID, string userName, string gameID)
	{
#if UNITY_IOS || UNITY_IPHONE
		openQiyu(userID, userName, gameID);
#endif
	}

		public int GetUnReadCount()
	{
#if UNITY_IOS || UNITY_IPHONE
		return u3dGetUnReadCount();
#else
		return 0;
#endif
	}
#endregion

#region 苹果支付
    public void iosInitAppPayForU3D(string _u3dGameObjectName, string _u3dCallbackName_onAppPayResult)
	{
#if UNITY_IOS || UNITY_IPHONE
		initAppPayForU3D(_u3dGameObjectName, _u3dCallbackName_onAppPayResult);
#endif
	}

	public void iosAppPay(string productID,string itemId, string orderID)
	{
#if UNITY_IOS || UNITY_IPHONE
		AppPay(productID,itemId, orderID);
#endif
	}
#endregion

}