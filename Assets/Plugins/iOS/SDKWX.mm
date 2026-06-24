#import "SDKWX.h"
#import "WXApiManager.h"
@implementation SDKWX

#pragma mark - 没有安装微信
+ (bool)isWXAppInstalled:(NSDictionary *)jsonDict
{
    /*
     {
     "u3dObjName":"AppGameManager",
     "u3dMethodName":"UnitySendMessageCall",
     "jsonInfo":""
     }
     */

    return [WXApiManager WXIsAppInstalled];
}
#pragma mark - 初始化微信
+(void)initWXForU3D:(NSDictionary *)jsonDict
{
    /*
     {
     "u3dObjName":"u3dCallbackName_onAppPayResultu3dCallbackName_onAppPayResult",
     "u3dMethodName":"UnitySendMessageCall",
     "jsonInfo":"{\"wechatAppId\":\"wx9bba9f7bba61e428\"}"
     }
     */
    NSString *jsonInfoTemp     = jsonDict[@"jsonInfo"];
    NSData *jsonDataTemp = [jsonInfoTemp dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *jsonDictTemp = [NSJSONSerialization JSONObjectWithData:jsonDataTemp options:kNilOptions error:nil];
    NSString *wechatAppId     = jsonDictTemp[@"wechatAppId"];
    [WXApi registerApp:wechatAppId enableMTA:YES];
}
#pragma mark - 微信信息分享
+(void) sendShareInfoToWX:(NSDictionary *)jsonDict
{
    /*
     {
     "u3dObjName":"AppGameManager",
     "u3dMethodName":"UnitySendMessageCall",
     "jsonInfo":"{\"target\":1,\"thumbPath\":\"\",\"imgPath\":\"\\/var\\/mobile\\/Containers\\/Data\\/Application\\/49AD9B8B-4008-4E17-8E19-E8AE1C319C54\\/Documents\\/ScreenShot.png\",\"actiontype\":2,\"description\":\"ShareImage test\"}"
     }
     */
    NSString *jsonInfoTemp     = jsonDict[@"jsonInfo"];
    const char * parmJsonInfo =[jsonInfoTemp UTF8String];

    NSData *jsonDataTemp = [jsonInfoTemp dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *jsonDictTemp = [NSJSONSerialization JSONObjectWithData:jsonDataTemp options:kNilOptions error:nil];
    int parmTarget     = [jsonDictTemp[@"target"] intValue];
    int parmActiontype     = [jsonDictTemp[@"actiontype"] intValue];

    NSString *jsonStr = [NSString stringWithUTF8String:parmJsonInfo];
    NSData *jsonData = [jsonStr dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *parmjsonDict = [NSJSONSerialization JSONObjectWithData:jsonData options:kNilOptions error:nil];

    [WXApiManager WXSendShareInfo:parmActiontype target:parmTarget dictInfo:parmjsonDict];
}
#pragma mark - 打开微信
+(void) openWX
{
    [WXApiManager WXOpen];
}
+(void)WXPay:(NSDictionary *)jsonDict
{
    [WXApiManager WXPay:jsonDict];
}

#pragma mark - 微信授权
+(void) getWechatAuth:(NSDictionary *)jsonDict
{
    NSLog(@"getWechatAuth  ");
    [WXApiManager WXGetAuth];
}
@end


