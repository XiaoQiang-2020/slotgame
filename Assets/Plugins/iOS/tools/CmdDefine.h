//
//  CmdDefine.h
//  Unity-iPhone
//
//  Created by 武浩颀 on 2018/10/13.
//

#ifndef CmdDefine_h
#define CmdDefine_h

#if defined (__cplusplus)
extern "C"
{
#endif

    //===============系统
    static int CMD_DIVICE_UNIQUE_CODE = 1003;//唯一标识
    static int CMD_DIVECE_ANDROID_ID = 1004 ;// 安卓设备号
    static int CMD_DIVICE_MAC_ADDRESS = 1005 ;// MAC地址
    
    
    static int CMD_VIBRATOR = 1010 ;// 震动
    static int CMD_BATTERY = 1011 ;//电量
    static int CMD_SIGNAL = 1012 ;//信号强度
    static int CMD_SYSTEM_AUTHORIZATION = 1013 ;//系统权限
    static int CMD_SYSTEM_COPYTOBOARD=1014;//复制到剪切板
    static  int CMD_SYSTEM_PHONE_CALL_STATE = 1015; // 获取来去电状态
    static int CMD_SYSTEM_APP_LINK=1016;//applin
    
    //===============微信
    static int CMD_WX_INIT = 1200 ;// 微信SDK初始化
    static int CMD_WX_OPEN = 1201 ;// 打开微信
    static int CMD_WX_IS_SUPPORT_CIRCLE = 1202 ;// 是否支持朋友圈
    static int CMD_WX_IS_INSTALLED = 1203 ;// 是否安装了微信
    static int CMD_WX_IS_SUPPORT_PAY = 1204 ;// 是否支持支付
    static int CMD_WX_PAY = 1205 ;// 支付
    static int CMD_WX_SHARE = 1206 ;// 分享
    static int CMD_WX_UNREGISTER = 1207 ;// 注销
    static int CMD_WX_GET_AUTH = 1208 ;// 获取授权
    
    //-- 百度定位
    static int  CMD_BAIDU_LOCATION = 1006 ;// 百度定位
    static int CMD_LBS_INFO = 1007 ;// 获取基站信息
    
    static int CMD_ALIPAY = 1001; // aplipay
    
    static int CMD_UM_INTERFACE = 1105; // 友盟接口
    
    // 20000 - 20050 权限相关
    static int CMD_REQUSET_MUST_AUTHORIZE = 20000; // 请求必须权限
    static int CMD_REQUSET_AUTHORIZE = 20001; // 获取权限
    
    // 20100 - 20150 webview相关
    static int CMD_OPEN_WEBVIEW = 20100; // 打开webview
    
    // 20160 -20180 相机相册相关
    static int CMD_OPEN_CAMAR = 20160; // 打开相机
    static int CMD_OPEN_PHOTO = 20161; // 打开相册
    
    
    
    void OS_TO_LUA(int cmd,const char* strParam);
    void OS_TO_LUA_JSON(const char* strParam);
    id CTGetGLViewController() ;
    
#if defined (__cplusplus)
}
#endif

#endif /* CmdDefine_h */
