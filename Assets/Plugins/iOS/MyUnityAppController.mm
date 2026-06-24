//
//  MyUnityAppController.m
//  Unity-iPhone
//
//  Created by SpanishScream on 2017/8/17.
//
//
#import "UnityAppController.h"
#import "MyUnityAppController.h"
#import "WXApiManager.h"
#import "WXApiObject.h"
#import <CoreLocation/CoreLocation.h>
#import "UnityBridgeOC.h"
#import "OpenInstallSDK.h"
#import "OpenInstallUnity3DCallBack.h"
#import "umeng/src/CTUMUtils.h"
#import "applink/CTAppLink.h"
#import "tools/base/ApplicationUtils.h"

IMPL_APP_CONTROLLER_SUBCLASS(MyUnityAppController)

@implementation MyUnityAppController

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
        [OpenInstallSDK initWithDelegate:[OpenInstallUnity3DCallBack defaultManager]];
    [super application:application didFinishLaunchingWithOptions:launchOptions];
//    // 获取定位
//    [self getLocation];
    // 微信注册
	[WXApi registerApp:@"wx9bba9f7bba61e428" enableMTA:YES];

    //七鱼注册
	[[QYSDK sharedSDK] registerAppId:@"cf86f70d70cb0a6be4b2d116dd08d679" appName:@"大圣南京麻将"];
    [[[QYSDK sharedSDK] conversationManager] setDelegate:self];

    
    [self receivePhoneCall];
    NSString* strParam = [NSString stringWithFormat:@"{\"method\":\"doInit\"}"];
    [[CTUMUtils instance] doSdkCall:[strParam UTF8String]];

    NSURL* url = [launchOptions objectForKey:UIApplicationLaunchOptionsURLKey];
    [CTAppLink appLink:[url absoluteString]];

    return [ApplicationUtils application:application didFinishLaunchingWithOptions:launchOptions];
}

- (void)applicationWillTerminate:(UIApplication*)application
{
    [super applicationWillTerminate:application];

    //七鱼注销
    [[QYSDK sharedSDK] logout:^(){}];
    [ApplicationUtils applicationWillTerminate:application];
}

#pragma mark - 微信登录回调
- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url
{
    NSString *host = [url host];
    if([UnityBridgeOC sharedInstance].isDebug)
    {
        NSLog(@"------openURL url = %@" , url);
        NSLog(@"------openURL1 host = %@" , host);
    }
    if ([host isEqualToString:@"oauth"]|| [host containsString:@"wechat"])
    {
        // 微信
        return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
    }
    else if([host isEqualToString:@"pay"])
    {
        return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
    }

    [CTAppLink appLink:[url absoluteString]];
    return [ApplicationUtils application:application handleOpenURL:url];
}
- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(nonnull NSDictionary *)options
{
    NSString *host = [url host];
    if([UnityBridgeOC sharedInstance].isDebug)
    {
        NSLog(@"------openURL url = %@" , url);
        NSLog(@"------openURL1 host = %@" , host);
    }
    if ([host isEqualToString:@"oauth"]|| [host containsString:@"wechat"])
    {
        // 微信
        return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
    }
    else if([host isEqualToString:@"pay"])
    {
        return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
    }
    
    return [ApplicationUtils application:app openURL:url options:options];
}
- (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation
{
    //判断是否通过OpenInstall URL Scheme 唤起App
    if  ([OpenInstallSDK handLinkURL:url]){//必写
        return YES;
    }
    [super application:application openURL:url sourceApplication:sourceApplication annotation:annotation];

    NSString *host = [url host];
    if([UnityBridgeOC sharedInstance].isDebug)
    {
        NSLog(@"------openURL url = %@" , url);
        NSLog(@"------openURL1 host = %@" , host);
    }
    if ([host isEqualToString:@"oauth"]|| [host containsString:@"wechat"])
    {
        // 微信
        return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
    }
    else if([host isEqualToString:@"pay"])
    {
        return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
    }

    return [ApplicationUtils application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
}
- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray * _Nullable))restorationHandler
{
    if ([OpenInstallSDK continueUserActivity:userActivity]) {

        return YES;
    }
    [super application:application continueUserActivity:userActivity restorationHandler:restorationHandler];
    if (![ApplicationUtils application:application continueUserActivity:userActivity restorationHandler:restorationHandler]) {
        return NO;
    }
    return YES;
}

#pragma mark - 监听来电
// /** Device call state: No activity. */
// public static final int CALL_STATE_IDLE = 0;
// /** Device call state: Ringing. A new call arrived and is
//  *  ringing or waiting. In the latter case, another call is
//  *  already active. */
// public static final int CALL_STATE_RINGING = 1;
// /** Device call state: Off-hook. At least one call exists
//   * that is dialing, active, or on hold, and no calls are ringing
//   * or waiting. */
// public static final int CALL_STATE_OFFHOOK = 2;
- (void)receivePhoneCall {
    self.callCenter = [[CTCallCenter alloc] init];
    self.callCenter.callEventHandler = ^(CTCall * call) {
        int clientState = 0;
        if([call.callState isEqualToString:CTCallStateDisconnected]) {
            NSLog(@"Call has been disconnected");//电话被挂断(我们用的这个)
        } else if([call.callState isEqualToString:CTCallStateConnected]) {
            NSLog(@"Call has been connected");//电话被接听
            clientState = 2;
        } else if([call.callState isEqualToString:CTCallStateIncoming]) {
            NSLog(@"Call is incoming");//来电话了
            clientState = 1;
        } else if([call.callState isEqualToString:CTCallStateDialing]) {
            NSLog(@"Call is Dialing");//拨号
            clientState = 1;
        } else {
            NSLog(@"Nothing is done");
        }
        NSString* strContent =  [NSString stringWithFormat:@"{\"callState\":\"%d\"}",clientState];
        [[NSNotificationCenter defaultCenter] postNotificationName:@"SYSTEM_CALL_PHONE_BACK" object:strContent];
    };
}

#pragma mark - 定位
- (void)getLocation
{
    self.locationManager = [[CLLocationManager alloc] init];
    // 设置代理
    self.locationManager.delegate = self;
    // 设置定位精确度到米
    self.locationManager.desiredAccuracy =  kCLLocationAccuracyBest;
    // 设置过滤器为无
    self.locationManager.distanceFilter = kCLDistanceFilterNone;

    //如果iOS是8.0以上版本
    if([[UIDevice currentDevice]systemVersion].doubleValue > 8.0)
    {
        if([self.locationManager respondsToSelector:@selector(requestWhenInUseAuthorization)])
        {
            // 位置管理对象中有requestAlwaysAuthorization这个方法
            // 运行
            [self.locationManager requestWhenInUseAuthorization];
        }
    }
    // 开始定位
    [self.locationManager startUpdatingLocation];
}

#pragma mark - 定位代理方法
// 定位成功后的回调
-(void) locationManager:(CLLocationManager *)manager didUpdateLocations:(NSArray<CLLocation *> *)locations
{
    if([UnityBridgeOC sharedInstance].isDebug)
    {
        NSLog(@"ios - locationManager 定位成功后的回调");
    }
    CLLocation *location = [locations lastObject];
    double latitude = location.coordinate.latitude;
    double longitude = location.coordinate.longitude;

    NSMutableDictionary *dict = [NSMutableDictionary dictionary];
    [dict setValue:[NSNumber numberWithBool:true] forKey:@"result"];
    [dict setValue:@"定位成功" forKey:@"errorStr"];
    [dict setValue:[NSNumber numberWithInt:1006] forKey:@"cmd"];
    [dict setValue:[NSNumber numberWithDouble:latitude] forKey:@"latitude"];
    [dict setValue:[NSNumber numberWithDouble:longitude] forKey:@"longitude"];

    NSData *data = [NSJSONSerialization dataWithJSONObject:dict options:kNilOptions error:nil];
    NSString *jsonString = [[NSString alloc]initWithData:data encoding:NSUTF8StringEncoding];
    const char* constStr = [jsonString UTF8String];

    if([UnityBridgeOC sharedInstance].isDebug)
    {
        NSLog(@"ios - locationManager: %@", jsonString);
        NSLog(@"ios - u3dGameObjectName: %@", [UnityBridgeOC sharedInstance].u3dGameObjectName);
        NSLog(@"ios - u3dCallbackName_onLocation: %@", [UnityBridgeOC sharedInstance].u3dCallbackName_onLocation);
    }
    UnitySendMessage([[UnityBridgeOC sharedInstance].u3dGameObjectName UTF8String],
                     [[UnityBridgeOC sharedInstance].u3dCallbackName_onLocation UTF8String],
                     constStr);
    [self.locationManager stopUpdatingLocation];
}

// 定位失败后的回调
-(void) locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error
{
    if([UnityBridgeOC sharedInstance].isDebug)
    {
        NSLog(@"ios - locationManager 定位失败后的回调");
    }
    NSMutableDictionary *dict = [NSMutableDictionary dictionary];
    [dict setValue:[NSNumber numberWithBool:false] forKey:@"result"];
    [dict setValue:@"定位失败" forKey:@"errorStr"];
    [dict setValue:[NSNumber numberWithInt:1006] forKey:@"cmd"];
    NSData *data = [NSJSONSerialization dataWithJSONObject:dict options:kNilOptions error:nil];
    NSString *jsonString = [[NSString alloc]initWithData:data encoding:NSUTF8StringEncoding];
    const char* constStr = [jsonString UTF8String];

    if([UnityBridgeOC sharedInstance].isDebug)
    {
        NSLog(@"ios - locationManager: %@", jsonString);
    }
    UnitySendMessage([[UnityBridgeOC sharedInstance].u3dGameObjectName UTF8String],
                     [[UnityBridgeOC sharedInstance].u3dCallbackName_onLocation UTF8String],
                     constStr);
}

#pragma mark - 七鱼未读消息代理方法
- (void)lhzonUnreadCountChanged:(NSInteger)count
{
    if([UnityBridgeOC sharedInstance].isDebug)
    {
        NSLog(@"ios - onUnreadCountChanged 七鱼未读消息: %ld", (long)count);
    }
    if (count > 0)
    {
        NSString *countStr = [NSString stringWithFormat:@"%ld", (long)count];

        NSString *u3dGameObjectName = [UnityBridgeOC sharedInstance].u3dGameObjectName;
        NSString *u3dCallbackName_onQYUnreadCountChanged = [UnityBridgeOC sharedInstance].u3dCallbackName_onQYUnreadCountChanged;
        if(u3dGameObjectName!=nil && [u3dGameObjectName length]>0 &&
           u3dCallbackName_onQYUnreadCountChanged!=nil && [u3dCallbackName_onQYUnreadCountChanged length]>0)
        {
            UnitySendMessage([u3dGameObjectName UTF8String],
                             [u3dCallbackName_onQYUnreadCountChanged UTF8String],
                             [countStr UTF8String]);
        }
    }
}

/**
 *  会话列表变化；非平台电商用户，只有一个会话项，平台电商用户，有多个会话项
 */
- (void)onSessionListChanged:(NSArray<QYSessionInfo*> *)sessionList
{

}

/**
 *  收到消息
 */
- (void)onReceiveMessage:(QYMessageInfo *)message
{

}

@end
