#import <Foundation/Foundation.h>

@interface SDKWX : NSObject

+(void)openWX;
+(bool)isWXAppInstalled:(NSDictionary *)jsonDict;
+(void)initWXForU3D:(NSDictionary *)jsonDict;
+(void)sendShareInfoToWX:(NSDictionary *)jsonDict;
+(void)getWechatAuth:(NSDictionary *)jsonDict;
+(void)WXPay:(NSDictionary *)jsonDict;
@end