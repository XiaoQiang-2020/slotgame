//
//  ApplicationUtils.m
//  Unity-iPhone
//
//  Created by 武浩颀 on 2018/10/14.
//


#import "ApplicationUtils.h"
#import "CTNotificationCollect.h"
#import "CTCommonTools.h"

static id _instance;

@interface ApplicationUtils()

@property(nonatomic, strong) NSMutableArray* configs;

@end

@implementation ApplicationUtils{
    
}

- (id)init
{
    self = [super init];
    if (self) {
        self.configs = [NSMutableArray arrayWithCapacity:10];
        
    }
    return self;
}

- (void)dealloc
{
    _configs = NULL;
    _instance = NULL;
}

- (void)startup{
    NSLog(@"ApplicationUtils:startup");
    [self initConfig];
}

-(void)initConfig
{
    NSLog(@"ApplicationUtils:config START");
    NSString* strPath = [CTCommonTools resPath:@"ct_class_config.json"];
    strPath= [[NSBundle mainBundle] pathForResource:strPath ofType:nil];
    if (NULL == strPath) {
        printf("ApplicationUtils:config=> path error");
        return;
    }
    printf("ApplicationUtils:confi path:%s",[strPath UTF8String]);
    
    NSString* strContent = [NSString stringWithContentsOfFile:strPath encoding:NSUTF8StringEncoding error:NULL];
    if (NULL == strContent) {
        printf("ApplicationUtils:config=> no content");
        return;
    }
    NSDictionary* dict = [CTCommonTools dictionaryWithJsonString:strContent];
    if (NULL == dict) {
        printf("ApplicationUtils:config=> not json=%s",[strContent UTF8String]);
        return;
    }
    
    id newObj = NULL;
    NSArray* arr = dict[@"interfaces"];
    for (id value in arr) {
        Class obj = NSClassFromString(value);
        if (NULL == obj) {
            printf("ApplicationUtils:config:no %s",[value UTF8String]);
            continue;
        }
        try {
            newObj = [obj instance];
            [self.configs addObject:newObj];
        } catch (NSException* e) {
            NSLog(@"ApplicationUtils:config:exception %@",e.description);
        }
    }
    NSLog(@"ApplicationUtils:config END");
}

- (NSArray*)objects{
    return self.configs;
}

+ (id) instance
{
    if (NULL == _instance) {
        _instance = [[ApplicationUtils alloc] init];
    }
    return _instance;
}

#pragma UIApplicationDelegate

+ (BOOL)application:(UIApplication*_Nonnull)application didFinishLaunchingWithOptions:(NSDictionary*_Nullable)launchOptions
{
    [[ApplicationUtils instance] startup];
    for (id object in [[ApplicationUtils instance] objects]) {
        if (![object application:application didFinishLaunchingWithOptions:launchOptions]) {
            return NO;
        }
    }
    return YES;
}
+ (BOOL)application:(UIApplication *_Nonnull)application handleOpenURL:(NSURL *_Nullable)url
{
    for (id object in [[ApplicationUtils instance] objects]) {
        if (![object application:application handleOpenURL:url]) {
            return NO;
        }
    }
    return YES;
}

+ (BOOL)application:(UIApplication *_Nonnull)application openURL:(NSURL *_Nullable)url options:( NSDictionary *_Nullable)options
{
    for (id object in [[ApplicationUtils instance] objects]) {
        if (![object application:application openURL:url options:options]) {
            return NO;
        }
    }
    return YES;
}
+ (BOOL)application:(UIApplication*_Nonnull)application openURL:(NSURL*_Nullable)url sourceApplication:(NSString*_Nullable)sourceApplication annotation:(id _Nullable)annotation
{
    for (id object in [[ApplicationUtils instance] objects]) {
        if (![object application:application openURL:url sourceApplication:sourceApplication annotation:annotation]) {
            return NO;
        }
    }
    return YES;
}
+ (BOOL)application:(UIApplication *_Nonnull)application continueUserActivity:(NSUserActivity *_Nonnull)userActivity restorationHandler:(void (^_Nullable)(NSArray * _Nullable))restorationHandler;
{
    for (id object in [[ApplicationUtils instance] objects]) {
        if (![object application:application continueUserActivity:userActivity restorationHandler:restorationHandler]) {
            return NO;
        }
    }
    return YES;
}

+ (void)applicationWillTerminate:(UIApplication*_Nonnull)application{
    for (id object in [[ApplicationUtils instance] objects]) {
        [object applicationWillTerminate:application];
    }
}

@end
