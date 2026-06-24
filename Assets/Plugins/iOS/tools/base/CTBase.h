//
//  CTBase.h
//  youmeng
//
//  Created by 武浩颀 on 2018/11/3.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#ifndef CTBase_h
#define CTBase_h

#import <UIKit/UIKit.h>
#import "CTBaseDelegate.h"

@interface CTBase : NSObject<CTBaseDelegate>

#pragma mark -UIApplicationDelegate

- (BOOL)application:(UIApplication*_Nonnull)application didFinishLaunchingWithOptions:(NSDictionary*_Nullable)launchOptions;
- (BOOL)application:(UIApplication *_Nonnull)application handleOpenURL:(NSURL *_Nullable)url;
- (BOOL)application:(UIApplication *_Nonnull)application openURL:(NSURL *_Nullable)url options:( NSDictionary *_Nullable)options;
- (BOOL)application:(UIApplication*_Nonnull)application openURL:(NSURL*_Nullable)url sourceApplication:(NSString*_Nullable)sourceApplication annotation:(id _Nullable  )annotation;
- (BOOL)application:(UIApplication *_Nonnull)application continueUserActivity:(NSUserActivity *_Nonnull)userActivity restorationHandler:(void (^_Nullable)(NSArray * _Nullable))restorationHandler;
- (void)applicationWillResignActive:(UIApplication *_Nonnull)application;

- (void)applicationDidEnterBackground:(UIApplication *_Nonnull)application;

- (void)applicationWillEnterForeground:(UIApplication *_Nonnull)application ;
- (void)applicationDidBecomeActive:(UIApplication *_Nonnull)application ;
- (void)applicationWillTerminate:(UIApplication *_Nonnull)application ;

@end

#endif /* CTBase_h */
