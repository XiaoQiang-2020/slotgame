//
//  CTApplicationDelegate.h
//  Unity-iPhone
//
//  Created by 武浩颀 on 2018/10/14.
//

#ifndef CTApplicationDelegate_h
#define CTApplicationDelegate_h
#import <Foundation/Foundation.h>

@protocol CTApplicationDelegate<NSObject>

@required
+(id) instance;

@optional
-(BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions;
-(BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url;
-(BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary *)options;
-(BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation;
-(BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray * _Nullable))restorationHandler;
-(void)applicationWillTerminate:(UIApplication*)application;


@end


#endif /* CTApplicationDelegate_h */
