#import "SDKSys.h"
#import "WXApiManager.h"

#import "WWKeyChianUtils.h"
#import <AudioToolbox/AudioToolbox.h>
@implementation SDKSys

#pragma mark - 震动
+(void) getShock
{
     AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
}
#pragma mark - 复制
+(bool) getCopy:(NSDictionary *)jsonDict
{
     /*
     {
     "u3dObjName":"AppGameManager",
     "u3dMethodName":"UnitySendMessageCall",
     "jsonInfo":"{\"Data\":\"test\"}"
     }
     */
 NSString *jsonInfoTemp     = jsonDict[@"jsonInfo"];
    const char * parmJsonInfo =[jsonInfoTemp UTF8String];

    NSData *jsonDataTemp = [jsonInfoTemp dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *jsonDictTemp = [NSJSONSerialization JSONObjectWithData:jsonDataTemp options:kNilOptions error:nil];
    NSString* parmData     =jsonDictTemp[@"Data"];

if(parmData!=nil)
{
            UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
        pasteboard.string = parmData;
        return true;
}else   {
    return false;
}

}
#pragma mark - 获取电量
+(float) getBatteryLevel
{
            [UIDevice currentDevice].batteryMonitoringEnabled = YES;
        float deviceLevel = [UIDevice currentDevice].batteryLevel*100;
        return deviceLevel;
}
#pragma mark - 获取wifi信号强度
+(int) getWifiSignalStrength
{
    return 3;
}


#pragma mark - 获取ios设备唯一id
+(NSString *) getPhoneUDID:(NSString *) id
{
                   NSString * strUUID = (NSString *)[WWKeyChianUtils load:id];

        //首次执行该方法时，uuid为空
        if ([strUUID isEqualToString:@""] || !strUUID)
        {
            //生成一个uuid的方法
            CFUUIDRef uuidRef = CFUUIDCreate(kCFAllocatorDefault);

            strUUID = (NSString *)CFBridgingRelease(CFUUIDCreateString (kCFAllocatorDefault,uuidRef));

            //将该uuid保存到keychain
            [WWKeyChianUtils save:id data:strUUID];

            CFRelease(uuidRef);
        }
        strUUID = [strUUID stringByReplacingOccurrencesOfString:@"-" withString:@""];
        return strUUID;
}
@end


