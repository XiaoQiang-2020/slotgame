#import <Foundation/Foundation.h>

@interface SDKSys : NSObject

+(void) getShock;
+(bool) getCopy:(NSDictionary *)jsonDict;
+(float) getBatteryLevel;
+(int) getWifiSignalStrength;
+(NSString *) getPhoneUDID:(NSString *) id;
@end

