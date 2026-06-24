//
//  MyUnityAppController.h
//  Unity-iPhone
//
//  Created by SpanishScream on 2017/8/17.
//
//

#import "UnityAppController.h"
#import <CoreLocation/CoreLocation.h>
#import "QYSDK.h"
#import <CoreTelephony/CTCallCenter.h>
#import <CoreTelephony/CTCall.h>

#define VERSION [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleShortVersionString"]
#define APP_NAME [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleDisplayName"]

@interface MyUnityAppController : UnityAppController<CLLocationManagerDelegate,QYConversationManagerDelegate>
//{
//    CLLocationManager *_locationManager;
//}
@property (nonatomic, strong) CLLocationManager * locationManager;
@property (nonatomic, strong) CTCallCenter *callCenter;

- (void)getLocation;


@end
