//
//  ApplicationUtils.h
//  Unity-iPhone
//
//  Created by 武浩颀 on 2018/10/14.
//

#ifndef ApplicationUtils_h
#define ApplicationUtils_h

#import <UIKit/UIKit.h>
@interface ApplicationUtils : NSObject

+ (id _Nonnull ) instance;

#pragma UIApplicationDelegate

+ (BOOL)application:(UIApplication*_Nonnull)application didFinishLaunchingWithOptions:(NSDictionary*_Nullable)launchOptions;
+ (BOOL)application:(UIApplication *_Nonnull)application handleOpenURL:(NSURL *_Nullable)url;
+ (BOOL)application:(UIApplication *_Nonnull)application openURL:(NSURL *_Nullable)url options:( NSDictionary *_Nullable)options;
+ (BOOL)application:(UIApplication*_Nonnull)application openURL:(NSURL*_Nullable)url sourceApplication:(NSString*_Nullable)sourceApplication annotation:(id _Nullable  )annotation;
+ (BOOL)application:(UIApplication *_Nonnull)application continueUserActivity:(NSUserActivity *_Nonnull)userActivity restorationHandler:(void (^_Nullable)(NSArray * _Nullable))restorationHandler;
+ (void)applicationWillTerminate:(UIApplication*_Nonnull)application;

@end

#endif /* ApplicationUtils_h */
