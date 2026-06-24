#import "UnityBridgeOC.h"
#import "MyUnityAppController.h"
#import "WXApiManager.h"
#import "tools/CmdDefine.h"
#import "tools/CTCommonTools.h"

#import "QiyuViewController.h"
#import <StoreKit/StoreKit.h>
#import "AppPayViewController.h"
#import "GSKeyChainDataManager.h"
#import "Reachability.h"
#import "SDKWX.h"
#import "SDKSys.h"
#import <AudioToolbox/AudioToolbox.h>
#import "applink/CTAppLink.h"
#import "umeng/src/CTUMUtils.h"
#import "alipay/src/CTAlipayUtils.h"
#import "photo/CTPhotoService.h"
#import <AdSupport/AdSupport.h>
@implementation UnityBridgeOC

const static NSString * defautObjectName = @"AppGameManager";
const static NSString * defautMethodName = @"UnitySendMessageCall";

+(instancetype)sharedInstance {
    static dispatch_once_t onceToken;
    static UnityBridgeOC *instance;
    dispatch_once(&onceToken, ^{
        instance = [[UnityBridgeOC alloc] init];
    });
    return instance;
}

- (instancetype)init
{
    self = [super init];
    if (self) {
        self.isDebug = true;
        self.u3dGameObjectName = @"";
        self.u3dCallbackName_onResp = @"";
        self.u3dCallbackName_onReq = @"";
        self.u3dCallbackName_onLocation = @"";
        self.u3dCallbackName_onQYUnreadCountChanged = @"";
        self.u3dCallbackName_onAppPayResult = @"";

        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(receivePhoneCall:)  name:@"SYSTEM_CALL_PHONE_BACK" object:nil];
    }
    return self;
}
-(void)dealloc{
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

// 在注册通知的控制器中写入该方法
-(void) receivePhoneCall:(NSNotification*)notification{
    NSString *objectString = [notification object];
    if (NULL == objectString) {
        return;
    }
    int cmd = 1015;// CMD_SYSTEM_PHONE_CALL_STATE 目前访问限制 写死
    NSString* parmJson= [NSString stringWithFormat:@"{\"resultStatus\":%@, \"cmd\":%d}",objectString,cmd];
    NSLog(@"receivePhoneCall:%@",parmJson);
    const char * josChar =[parmJson UTF8String];
    UnitySendMessage([defautObjectName UTF8String],
                     [defautMethodName UTF8String],
                     josChar);
}
-(void) setIdentifyCode:(NSString *)code
{
    [GSKeyChainDataManager saveUUID:code];
}

-(NSString *) getIdentifyCode
{
    NSString* code = [GSKeyChainDataManager readUUID];
    return code;
}


@end

#if defined (__cplusplus)
extern "C"
{
#endif
//==============================delete============start==================

    // 一个初始化方法 给u3d用的
    void initWXForU3D(const char * _wechatAppID,const char * _u3dGameObjectName, const char *_u3dCallbackName_onResp, const char * _u3dCallbackName_onReq)
    {
        return;
        [UnityBridgeOC sharedInstance].u3dGameObjectName = [NSString stringWithUTF8String:_u3dGameObjectName];
        [UnityBridgeOC sharedInstance].u3dCallbackName_onResp = [NSString stringWithUTF8String:_u3dCallbackName_onResp];
        [UnityBridgeOC sharedInstance].u3dCallbackName_onReq = [NSString stringWithUTF8String:_u3dCallbackName_onReq];
              //注册在所有调用之前
        NSString * wechatAPPid = [NSString stringWithUTF8String:_wechatAppID];
        [WXApi registerApp:wechatAPPid enableMTA:YES] ;
    }


    void openWX()
    {
        return [WXApiManager WXOpen];
    }

    bool isWXAppInstalled()
    {
        return false;
       return [WXApiManager WXIsAppInstalled];
    }


    void getWechatAuth()
    {
        [WXApiManager WXGetAuth];
    }


    void sendShareInfoToWX(int actiontype, int target, const char *jsonInfo)
    {
        return;
        NSString *jsonStr = [NSString stringWithUTF8String:jsonInfo];
        if([UnityBridgeOC sharedInstance].isDebug)
        {
            NSLog(@"ios - SendShareInfoToWX: %@",jsonStr);
        }

        NSData *jsonData = [jsonStr dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *jsonDict = [NSJSONSerialization JSONObjectWithData:jsonData options:kNilOptions error:nil];

        NSLog(@"%d, %d", actiontype, target);
        [WXApiManager WXSendShareInfo:actiontype target:target dictInfo:jsonDict];
    }

    #pragma mark - 震动
    void getShock()
    {
         AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
    }

    void getCopy(const char *copyContent)
    {
        return;
//        NSString *copyContentStr = [NSString stringWithUTF8String:copyContent];
//        UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
//        pasteboard.string = copyContentStr;

    }

    int getWifiSignalStrength()
    {
        UIApplication *app =[UIApplication sharedApplication];
        // iphoneX状态栏和其他iPhone设备不同，变化比较大 //判断是否是iPhoneX
        if([[app valueForKeyPath:@"_statusBar"] isKindOfClass: NSClassFromString(@"UIStatusBar_Modern")])
        {
            NSString *wifiEntry =[[[ [app valueForKey:@"statusBar"] valueForKey:@"_statusBar"] valueForKey:@"_currentAggregatedData"] valueForKey:@"_wifiEntry"];
            int signalStrength =[[wifiEntry valueForKey:@"_displayValue"]intValue];
            //NSLog(@"iphonx ------%d",signalStrength);
            return signalStrength;

        }
        else
        {

            UIApplication *app = [UIApplication sharedApplication];

            NSArray *subviews = [[[app valueForKey:@"statusBar"]valueForKey:@"foregroundView"] subviews];
            NSString *dataNetworkItemView = nil;
            for (id subview in subviews) {
                if([subview isKindOfClass:[NSClassFromString(@"UIStatusBarDataNetworkItemView") class]]) {
                    dataNetworkItemView = subview;
                    break;
                }
            }

            int signalStrength = [[dataNetworkItemView valueForKey:@"_wifiStrengthBars"] intValue];

            return signalStrength;
        }

    }

       float getBatteryLevel()
    {
            [UIDevice currentDevice].batteryMonitoringEnabled = YES;
        float deviceLevel = [UIDevice currentDevice].batteryLevel*100;

        return deviceLevel;
    }

        //获取ios设备唯一id
    const char * getPhoneUDID(NSString * )
    {
        const char * ret=[@"" UTF8String];
            return ret;
    }
    //==============================delete============end==================






    #pragma mark - 获取网络状态
    char * getNetWorkState()
    {
            NSString *network = @"";
            switch ([[Reachability reachabilityForInternetConnection]currentReachabilityStatus]) {
                case NotReachable:
                    network = @"无网络";
                    break;
                case ReachableViaWiFi:
                    network = @"wifi";
                    break;
                case ReachableViaWWAN:
                    network = @"4G";
                    break;
                default:
                    break;
            }
            if ([network isEqualToString:@""]) {
                network = @"NO DISPLAY";
            }
            NSLog(@"%@",network);

        const char * constChars = [network UTF8String];
        char* chars = (char*)malloc(strlen(constChars)+1);
        strcpy(chars, constChars);

        if([UnityBridgeOC sharedInstance].isDebug)
        {
            NSLog(@"ios getNetWorkState: %@ -- %s",network, chars);
        }

        return chars;
    }

     #pragma mark - 切换到横屏
    void ChangeRootViewControllerLandscape()
    {
        // [[UIDevice  currentDevice] performSelector:@selector(setOrientation:)withObject:(id)UIDeviceOrientationLandscapeLeft];
        if ([[UIDevice currentDevice] respondsToSelector:@selector(setOrientation:)]) {
            SEL selector = NSSelectorFromString(@"setOrientation:");
            NSInvocation *invocation = [NSInvocation invocationWithMethodSignature:[UIDevice instanceMethodSignatureForSelector:selector]];
            [invocation setSelector:selector];
            [invocation setTarget:[UIDevice currentDevice]];
            int val = UIDeviceOrientationLandscapeLeft;
            [invocation setArgument:&val atIndex:2];
            [invocation invoke];
        }

    }

    #pragma mark 切换到竖屏
    void ChangeRootViewControllerVertialScreen()
    {
        if ([[UIDevice currentDevice] respondsToSelector:@selector(setOrientation:)]) {
            SEL selector = NSSelectorFromString(@"setOrientation:");
            NSInvocation *invocation = [NSInvocation invocationWithMethodSignature:[UIDevice instanceMethodSignatureForSelector:selector]];
            [invocation setSelector:selector];
            [invocation setTarget:[UIDevice currentDevice]];
            int val = UIDeviceOrientationPortrait;
            [invocation setArgument:&val atIndex:2];
            [invocation invoke];
        }

    }
    #pragma mark - ios定位相关
    // 一个初始化方法 给u3d用的
    void getLocation(const char * _u3dGameObjectName, const char *_u3dCallbackName_onLocation)
    {

         [UnityBridgeOC sharedInstance].u3dGameObjectName = [NSString stringWithUTF8String:_u3dGameObjectName];
         [UnityBridgeOC sharedInstance].u3dCallbackName_onLocation = [NSString stringWithUTF8String:_u3dCallbackName_onLocation];
        if([UnityBridgeOC sharedInstance].isDebug)
        {
            NSLog(@"ios - getLocation:");
            NSLog(@"ios - u3dGameObjectName: %@", [UnityBridgeOC sharedInstance].u3dGameObjectName);
            NSLog(@"ios - u3dCallbackName_onLocation: %@", [UnityBridgeOC sharedInstance].u3dCallbackName_onLocation);
        }

        MyUnityAppController *controller = (MyUnityAppController*)[UIApplication sharedApplication].delegate;
        [controller getLocation];
    }

    #pragma mark - 七鱼
    // 一个初始化方法 给u3d用的
    void initQYForU3D(const char * _u3dGameObjectName, const char *_u3dCallbackName_onQYUnreadCountChanged)
    {
        [UnityBridgeOC sharedInstance].u3dGameObjectName = [NSString stringWithUTF8String:_u3dGameObjectName];
        [UnityBridgeOC sharedInstance].u3dCallbackName_onQYUnreadCountChanged = [NSString stringWithUTF8String:_u3dCallbackName_onQYUnreadCountChanged];

        QYCustomUIConfig * qyConfig=[[QYSDK sharedSDK] customUIConfig];
qyConfig.autoShowKeyboard=NO;
    }

    // 打开七鱼客服页面
    void openQiyu(const char *userID, const char *userName, const char *gameID)
    {
        NSString *userIDStr     = [NSString stringWithUTF8String:userID];
        NSString *userNameStr   = [NSString stringWithUTF8String:userName];
        NSString *gameIDStr     = [NSString stringWithUTF8String:gameID];

        QiyuViewController *qyVC = [[QiyuViewController alloc]init];
        qyVC.userID   = userIDStr;
        qyVC.userName = userNameStr;
        qyVC.gameID   = gameIDStr;
        [UnityGetGLViewController() presentViewController:qyVC animated:NO completion:nil];

    }

    int u3dGetUnReadCount(){
        int count=0;
     QYConversationManager *qyMgr=   [[QYSDK sharedSDK] conversationManager];
count= (int)qyMgr.allUnreadCount;
        return count;
    }
    #pragma mark - 苹果内购
    //
    void initAppPayForU3D(const char * _u3dGameObjectName, const char *_u3dCallbackName_onAppPayResult)
    {
        [UnityBridgeOC sharedInstance].u3dGameObjectName = [NSString stringWithUTF8String:_u3dGameObjectName];
        [UnityBridgeOC sharedInstance].u3dCallbackName_onAppPayResult = [NSString stringWithUTF8String:_u3dCallbackName_onAppPayResult];

    }

  // 支付
    void AppPay(const char *productID,const char *itemId,const char *cusID)
    {
        NSString *productIDStr = [NSString stringWithUTF8String:productID];
        NSString *itemIdStr=[NSString stringWithUTF8String:itemId];
         NSString *cusIDStr=[NSString stringWithUTF8String:cusID];

                if([UnityBridgeOC sharedInstance].isDebug)
        {
            NSLog(@"productIDStr:%@-itemIdStr:%@-cusIDStr:%@",productIDStr,itemIdStr,cusIDStr);
        }
        if([SKPaymentQueue canMakePayments])
        {
            SKProductsRequest *request = [[SKProductsRequest alloc]initWithProductIdentifiers:[NSSet setWithObject:productIDStr]];
            AppPayViewController *payVC = [[AppPayViewController alloc]init];
            //            payVC.cusID = [NSString stringWithUTF8String:cusID];//自定义订单号，赋值
            [payVC setCusID:[NSString stringWithUTF8String:cusID]];
            request.delegate =[AppPayViewController appPayManager];
            [request start];
        }
        else
        {
            NSLog(@"用户不允许内购");//没有开起支付

             NSString* parmJson= [NSString stringWithFormat:@"{\"result\":\"false\", \"bodyString\":\"\",\"cusID\" : \"\" , \"msg\" : \"用户不允许内购\" , \"item_id\" : \"%@\"}",itemIdStr];
            const char * jsonData =[parmJson UTF8String];
            UnitySendMessage([[UnityBridgeOC sharedInstance].u3dGameObjectName UTF8String],
                             [[UnityBridgeOC sharedInstance].u3dCallbackName_onAppPayResult UTF8String],
                             jsonData);
        }
    }


    //===============Test
//    static int TestCmd = 1000; //测试
//    static int UDIDTest = 1001;
//
//    //===============系统
//    static int CMD_DIVICE_UNIQUE_CODE = 1003;//唯一标识
//    static int CMD_DIVECE_ANDROID_ID = 1004 ;// 安卓设备号
//    static int CMD_DIVICE_MAC_ADDRESS = 1005 ;// MAC地址
//
//
//    static int CMD_VIBRATOR = 1010 ;// 震动
//    static int CMD_BATTERY = 1011 ;//电量
//    static int CMD_SIGNAL = 1012 ;//信号强度
//    static int CMD_SYSTEM_AUTHORIZATION = 1013 ;//系统权限
//    static int CMD_SYSTEM_COPYTOBOARD=1014;//复制到剪切板
//    static  int CMD_SYSTEM_PHONE_CALL_STATE = 1015; // 获取来去电状态
//    static int CMD_SYSTEM_APP_LINK=1016;//applin
//
//    //===============微信
//    static int CMD_WX_INIT = 1200 ;// 微信SDK初始化
//    static int CMD_WX_OPEN = 1201 ;// 打开微信
//    static int CMD_WX_IS_SUPPORT_CIRCLE = 1202 ;// 是否支持朋友圈
//    static int CMD_WX_IS_INSTALLED = 1203 ;// 是否安装了微信
//    static int CMD_WX_IS_SUPPORT_PAY = 1204 ;// 是否支持支付
//    static int CMD_WX_PAY = 1205 ;// 支付
//    static int CMD_WX_SHARE = 1206 ;// 分享
//    static int CMD_WX_UNREGISTER = 1207 ;// 注销
//    static int CMD_WX_GET_AUTH = 1208 ;// 获取授权
//
////-- 百度定位
//static int  CMD_BAIDU_LOCATION = 1006 ;// 百度定位
//static int CMD_LBS_INFO = 1007 ;// 获取基站信息

    static NSString * const KEY_USER_WAWAIDTENFIY = @"com.blackwhalegame.hyj.sichuan";

    void CallMethodByCmd(const char *jsonInfo, int cmd)
    {
        NSString *jsonStr = [NSString stringWithUTF8String:jsonInfo];
        /* ====================日志输出*/
        NSLog(@"收到的 cmd: %d",cmd);
        if([UnityBridgeOC sharedInstance].isDebug)
        {
            NSLog(@"ios - SendShareInfoToWX: %@",jsonStr);
        }
        NSData *jsonData = [jsonStr dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *jsonDict = [NSJSONSerialization JSONObjectWithData:jsonData options:kNilOptions error:nil];
        NSString *u3dObjNameTemp     = jsonDict[@"u3dObjName"];
        const char * parmU3dObjName =[u3dObjNameTemp UTF8String];
        NSString *u3dMethodNameTemp     = jsonDict[@"u3dMethodName"];
        const char * parmU3dMethodName =[u3dMethodNameTemp UTF8String];
        NSString* strContent = [jsonDict objectForKey:@"jsonInfo"];
        
        
//        if(cmd == TestCmd)
//        {
//            // NSString *feedbackInfo = @"收到ios回馈的信息";
//            const char* msg = "{\"cmd\" : 1000, \"bodyString\" : \"\" ,\"cusID\" : \"\" , \"msg\" : \"用户不允许内购\"}";
//            // UnitySendMessage([className UTF8String],[methodName UTF8String],[feedbackInfo UTF8String]);
//            UnitySendMessage([[UnityBridgeOC sharedInstance].u3dGameObjectName UTF8String],
//                             [[UnityBridgeOC sharedInstance].u3dCallbackName_onAppPayResult UTF8String],
//                             msg);
//        }
//        else if (cmd == UDIDTest)
//        {
//            NSString* code = [[UnityBridgeOC sharedInstance] getIdentifyCode];
//            if (code == nil || [code isEqualToString:@""])
//            {
//                code = [[UIDevice currentDevice].identifierForVendor UUIDString];
//            }
//            [[UnityBridgeOC sharedInstance] setIdentifyCode:code];
//            NSString* cmdMsg = @"{\"cmd\" : 1001, \"bodyString\" :\"";
//            NSString* extStr = @"\"}";
//            NSString* msg = [[NSString alloc] initWithFormat:@"%@%@%@", cmdMsg, code, extStr];
//            UnitySendMessage([[UnityBridgeOC sharedInstance].u3dGameObjectName UTF8String],
//                             [[UnityBridgeOC sharedInstance].u3dCallbackName_onAppPayResult UTF8String],
//                             [msg UTF8String]);
//        }
        //=====================微信=====================start
        if (cmd == CMD_WX_INIT)
        {
            [SDKWX initWXForU3D:jsonDict];
        }
        else if (cmd == CMD_WX_IS_INSTALLED)
        {
            bool isWxInstall=[SDKWX isWXAppInstalled:jsonDict];
            NSString* isWXAppInstalledTemp=nil;
            if (isWxInstall) {
                isWXAppInstalledTemp=@"true";
            }else   {
                isWXAppInstalledTemp=@"false";
            }
            NSString* parmJson= [NSString stringWithFormat:@"{\"resultStatus\":\"%@\", \"cmd\":%d,\"new_method_for_share\":true}",isWXAppInstalledTemp,cmd];
            const char * parmIsWXAppInstalledTemp =[parmJson UTF8String];
            NSLog(@"CMD_WX_IS_INSTALLED: %@ %@",isWXAppInstalledTemp,parmJson);
            UnitySendMessage(
                             parmU3dObjName,
                             parmU3dMethodName,
                             parmIsWXAppInstalledTemp
            );
        }
        else if (cmd == CMD_WX_SHARE)
        {
            [SDKWX sendShareInfoToWX:jsonDict];
        }
        else if(cmd == CMD_WX_OPEN)
        {
            [SDKWX openWX];
        }
        else if(cmd == CMD_WX_IS_SUPPORT_CIRCLE)
        {

        }
        else if(cmd == CMD_WX_IS_SUPPORT_PAY)
        {

        }
        else if(cmd == CMD_WX_PAY)
        {
            [SDKWX WXPay:jsonDict];
        }
        else if(cmd == CMD_WX_UNREGISTER)
        {

        }
        else if(cmd == CMD_WX_GET_AUTH)
        {
            [SDKWX getWechatAuth:jsonDict];
        }
        //=====================微信=====================end
        //=====================系统=====================start
        else if(cmd == CMD_DIVICE_UNIQUE_CODE)
        {
            NSString * strUUID = [SDKSys getPhoneUDID:KEY_USER_WAWAIDTENFIY];
            NSString * adId = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
            NSString * parmJson = [NSString stringWithFormat:@"{\"resultStatus\":\"%@\", \"cmd\":\"%d\",\"strADID\":\"%@\"}",strUUID,cmd,adId];
            const char * jsonData = [parmJson UTF8String];
            NSLog(@"UUID: %@ %@",strUUID,parmJson);
            UnitySendMessage(
                             parmU3dObjName,
                             parmU3dMethodName,
                             jsonData
                             );

        }
        else if(cmd == CMD_DIVECE_ANDROID_ID)
        {

        }
        else if(cmd == CMD_DIVICE_MAC_ADDRESS)
        {

        }
        else if(cmd == CMD_SYSTEM_COPYTOBOARD)
        {
            bool parmResult= [SDKSys getCopy:jsonDict];

            NSString* isTrue=nil;
            if (parmResult) {
                isTrue=@"true";
            }else   {
                isTrue=@"false";
            }

            NSString* parmJson= [NSString stringWithFormat:@"{\"resultStatus\":\"%@\", \"cmd\":%d}",isTrue,cmd];
            const char * parmRet =[parmJson UTF8String];
            NSLog(@"CMD_SYSTEM_COPYTOBOARD: %d %@",parmResult,parmJson);
            UnitySendMessage(
                             parmU3dObjName,
                             parmU3dMethodName,
                             parmRet
            );
        }
        else if(cmd == CMD_VIBRATOR)
        {
             [SDKSys getShock];
        }
        else if(cmd == CMD_BATTERY)
        {
          float level=    [SDKSys getBatteryLevel];
            NSString* parmJson= [NSString stringWithFormat:@"{\"resultStatus\":%f, \"cmd\":%d}",level,cmd];
            const char * parmIsWXAppInstalledTemp =[parmJson UTF8String];
            NSLog(@"CMD_BATTERY: %@",parmJson);
            UnitySendMessage(
                             parmU3dObjName,
                             parmU3dMethodName,
                             parmIsWXAppInstalledTemp
                             );
        }
        else if(cmd == CMD_SIGNAL)
        {
         int signal=    [SDKSys getWifiSignalStrength];
            NSString* parmJson= [NSString stringWithFormat:@"{\"resultStatus\":%d, \"cmd\":%d}",signal,cmd];
            const char * parmIsWXAppInstalledTemp =[parmJson UTF8String];
            NSLog(@"CMD_SIGNAL: %@",parmJson);
            UnitySendMessage(
                             parmU3dObjName,
                             parmU3dMethodName,
                             parmIsWXAppInstalledTemp
                             );
        }
        else if(cmd == CMD_SYSTEM_AUTHORIZATION)
        {

        }
        //=====================系统=====================end
        //=====================百度定位=====================start
        else if(cmd == CMD_BAIDU_LOCATION)
        {
getLocation(parmU3dObjName,parmU3dMethodName);
        }
        else if(cmd == CMD_LBS_INFO)
        {

        }
        else if(cmd == CMD_SYSTEM_APP_LINK){
            NSString* linkData =  [CTAppLink getLinkData];
            NSString* content = [NSString stringWithFormat:@"{\"content\":%@,\"cmd\":%d}",linkData,cmd];
            UnitySendMessage(
                             parmU3dObjName,
                             parmU3dMethodName,
                             [content UTF8String]
                             );
           [CTAppLink resetLinkData];
        }
        else if (cmd == CMD_UM_INTERFACE){
            [[CTUMUtils instance] doSdkCall:[strContent UTF8String]];
        }
        else if (cmd == CMD_ALIPAY){
//            strContent = @"123456";
            NSString* strTemp = [NSString stringWithFormat:@"{\"content\":%@,\"method\":\"%@\"}",strContent,@"doPay"];
            [[CTAlipayUtils instance] doSdkCall:[strTemp UTF8String]];
        }
        else if (cmd == CMD_OPEN_WEBVIEW){

        }
        else if (cmd == CMD_OPEN_CAMAR){
            [CTPhotoService setCmd:cmd];
            [CTPhotoService openCamera:[strContent UTF8String]];
        }
        else if (cmd == CMD_OPEN_PHOTO){
            [CTPhotoService setCmd:cmd];
            [CTPhotoService openAlbum:[strContent UTF8String]];
        }
        //=====================百度定位=====================end
        else
        {
            NSLog(@"ceshishis");
        }
    }


#if defined (__cplusplus)
}
#endif
