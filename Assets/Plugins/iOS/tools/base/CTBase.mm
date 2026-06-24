//
//  CTBase.m
//  youmeng
//
//  Created by 武浩颀 on 2018/11/3.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#import <Foundation/Foundation.h>

#include "CTBase.h"

@interface CTBase()
@end

@implementation CTBase{
    
}

+ (id)instance{
    NSLog(@"CTBase:instance");
    return NULL;
}

- (BOOL)application:(UIApplication*_Nonnull)application didFinishLaunchingWithOptions:(NSDictionary*_Nullable)launchOptions{
    return YES;
}
- (BOOL)application:(UIApplication *_Nonnull)application handleOpenURL:(NSURL *_Nullable)url{
    return YES;
}
- (BOOL)application:(UIApplication *_Nonnull)application openURL:(NSURL *_Nullable)url options:( NSDictionary *_Nullable)options{
    return YES;
}
- (BOOL)application:(UIApplication*_Nonnull)application openURL:(NSURL*_Nullable)url sourceApplication:(NSString*_Nullable)sourceApplication annotation:(id _Nullable  )annotation{
    return YES;
}
- (BOOL)application:(UIApplication *_Nonnull)application continueUserActivity:(NSUserActivity *_Nonnull)userActivity restorationHandler:(void (^_Nullable)(NSArray * _Nullable))restorationHandler{
    return YES;
}
- (void)applicationWillResignActive:(UIApplication *_Nonnull)application{
    
}

- (void)applicationDidEnterBackground:(UIApplication *_Nonnull)application{
    
}

- (void)applicationWillEnterForeground:(UIApplication *_Nonnull)application {
    
}
- (void)applicationDidBecomeActive:(UIApplication *_Nonnull)application {
    
}
- (void)applicationWillTerminate:(UIApplication *_Nonnull)application {
    
}

@end
