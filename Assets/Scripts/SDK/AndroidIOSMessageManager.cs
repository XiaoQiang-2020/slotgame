
using UnityEngine;
using App.Boot;
public class AndroidIOSMessageManager : MonoBehaviour
{
    //private AndroidJavaClass CSharpCallJava;
    //private string AndroidClassName = "com.u3d.plugins.UnityAppActivity";

    #region 单例对象
    private static AndroidIOSMessageManager _instance;
    public static AndroidIOSMessageManager getInstance()
    {
        if (_instance == null)
        {
            GameObject go = AppBoot.Instance.gameObject;
            _instance = go.GetComponent<AndroidIOSMessageManager>();
            if (_instance == null)
            {
                _instance = go.AddComponent<AndroidIOSMessageManager>();
                // Debug.Log("单li执行构造函数添加AndroidIOSMessageManager");
                
            }
            // Debug.Log("guashangjiaoben " + _instance.name);
        }
        // Debug.Log("返回单例 " + _instance.name);
        return _instance;
    }
    #endregion

    #region MonoBehaviour方法
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
       // InitJavaClass();
    }
    #endregion

//    private void InitJavaClass()
//    {
//#if UNITY_ANDROID
//        CSharpCallJava = new AndroidJavaClass(AndroidClassName);
//#endif
//    }

//    public void CallJavaClass(string json)
//    {

//#if UNITY_ANDROID
//        alipayUtil.CallStatic("AlipayToPayOrder", jsonData,);
//#endif
//    }

    #region Android系统相关
    //接收安卓系统电量回调函数
    public System.Action<string> onReceiveBatteryReceiverCallBack;
    //接收安卓系统网络强度回调函数
    public System.Action<int> onNetworkSignalListenerCallBack;

    /// <summary>
    /// 接收来自Android端的安卓系统电量通知
    /// </summary>
    /// <param name="json">数据信息</param>
    private void onReceiveBatteryReceiver(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        if (null != onReceiveBatteryReceiverCallBack)
        {
            onReceiveBatteryReceiverCallBack(json);
        }
    }

    /// <summary>
    /// 接收来自Android端的安卓系统网络信号通知
    /// </summary>
    /// <param name="intNetworkSignal">数据信息</param>
    private void onNetworkSignalListener(string intNetworkSignal)
    {
        int networkSignal;
        if (int.TryParse(intNetworkSignal, out networkSignal))
        {
            onNetworkSignalListenerCallBack(networkSignal);
        }
    }
    #endregion

    #region WeChat SDK
    //调起微信授权回调函数
    public System.Action<string> onRespFormAndroidWechatCallBack;
    //请求微信回调函数
    public System.Action<string> onReqFormAndroidWechatCallBack;
    //请求微信支付回调函数
    public System.Action<string> onPayResqFormAndroidWechatCallBack;

    /// <summary>
    /// 接收来息android下的微信sdk的回调用
    /// APP发送到微信请求的响应结果将回调到onResp方法
    /// </summary>
    /// <param name="json">主要是android下的BaseResp对象的json输出</param>
    private void onRespFormAndroidWechat(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        if (null != onRespFormAndroidWechatCallBack)
        {
            onRespFormAndroidWechatCallBack(json);
        }
    }

    /// <summary>
    /// 接收来息android下的微信sdk的回调用
    /// 微信发送给APP的请求将回调到onReq方法
    /// </summary>
    /// <param name="json">主要是android下的BaseReq对象的json输出</param>
    private void onReqFormAndroidWechat(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        if (null != onReqFormAndroidWechatCallBack)
        {
            onReqFormAndroidWechatCallBack(json);
        }
    }

    /// <summary>
    /// 支付返回
    /// </summary>
    /// <param name="json">主要是android下的BaseResp对象的json输出</param>
    private void onPayRespFormAndroidWechat(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        if (null != onPayResqFormAndroidWechatCallBack)
        {
            onPayResqFormAndroidWechatCallBack(json);
        }
    }
    #endregion

    #region BaiDu SDK
    //接收到百度定位信息后的回调函数
    public System.Action<string> onGetPositionFromAndroidBaiduCallBack;

    /// <summary>
    /// 接收来息android下的baidu sdk的回调用
    /// 返回当前设置位置的经纬度
    /// </summary>
    /// <param name="json"></param>
    private void onGetPositionFromAndroidBaidu(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        if (null != onGetPositionFromAndroidBaiduCallBack)
        {
            onGetPositionFromAndroidBaiduCallBack(json);
        }
    }
    #endregion

    #region QiYu SDK
    //接收到七鱼信息后的回调函数
    public System.Action<int> onGetNewMessageCountAndroidQiyuCallBack;

    /// <summary>
    /// 接收来息android下的 baidu sdk 的回调用
    /// 返回当前设置位置的经纬度
    /// </summary>
    /// <param name="json"></param>
    private void onGetNewMessageCountAndroidQiyu(string strCount)
    {
        if (string.IsNullOrEmpty(strCount))
            return;

        if (onGetNewMessageCountAndroidQiyuCallBack != null)
        {
            int count;
            if (int.TryParse(strCount, out count))
            {
                onGetNewMessageCountAndroidQiyuCallBack(count);
            }
        }
    }
    #endregion

    #region Apple Pay SDK
    //调起苹果支付后的回调函数
    public System.Action<string> onAppPayResultCallBack;

    /// <summary>
    /// 接收来息android下的内购的回调用
    /// </summary>
    /// <param name="json"></param>
    void onAppPayResult(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        if (null != onAppPayResultCallBack)
        {
            onAppPayResultCallBack(json);
        }
    }
    #endregion


    #region Alipay SDK
    //调起支付宝支付后的回调函数
    public System.Action<string> onToPayFromAndroidAlipayCallBack;

    /// <summary>
    /// 接收来自android下的alipay sdk的回调用
    /// </summary>
    /// <param name="json">订单信息同步支付结果</param>
    void onToPayFromAndroidAlipay(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        if (null != onToPayFromAndroidAlipayCallBack)
        {
            onToPayFromAndroidAlipayCallBack(json);
        }
    }
    #endregion

    #region 所有的sdk的回调
    public System.Action<string> onFromAndroidCallback;
    
    private void UnitySendMessageCall(string json)
    {
        if(string.IsNullOrEmpty(json))
        {
            Debug.Log("UnitySendMessageCall json 参数是空的");
            return;
        }
        if(null != onFromAndroidCallback)
        {
            onFromAndroidCallback(json);
        }
    }
    #endregion



}