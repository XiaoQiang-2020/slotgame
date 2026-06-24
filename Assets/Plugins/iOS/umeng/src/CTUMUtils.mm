//
//  UMUtils.m
//  youmeng
//
//  Created by 武浩颀 on 2018/10/4.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UMCommon/UMCommon.h>
#import <UMAnalytics/MobClick.h>
#import <UMAnalytics/MobClickGameAnalytics.h>
#import "CTUMUtils.h"
#include "CTUMConfig.h"


//typedef void (*CALL_FUNC)(id,SEL,NSString*);

static id _umInstance;
@implementation CTUMUtils{
}

-(id)init
{
    self = [super init];
    if (self) {
        
    }
    return self;
}

-(void)dealloc
{
    
}

+(id) instance
{
    if (NULL == _umInstance) {
        _umInstance = [CTUMUtils alloc];
    }
    return _umInstance;
}
-(void) doSdkCall:(const char*) strParam
{
    if (nullptr == strParam) {
        return;
    }
    if (0 == strlen(strParam)) {
        return;
    }
    NSString *nsParam = [NSString stringWithUTF8String:strParam];
    if (NULL == nsParam) {
        return;
    }
    @try {
        NSData* nsData = [nsParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:nsData
                                                             options:kNilOptions
                                                               error:nil];
        
        NSString* nsMethod = dict[@"method"];
        if (NULL == nsMethod) {
            NSLog(@"doSdkCall: no method in param");
            return;
        }
        NSDictionary* ditContent = [dict objectForKey:@"content"];
        NSString* nsContent = [self stringWithDictionary:ditContent];
        nsContent = NULL == nsContent ? @"{}" :nsContent;
        NSLog(@"method=%@\n content=%@",nsMethod, nsContent);
        
        SEL methodSel = NSSelectorFromString([NSString stringWithFormat:@"%@:",nsMethod]);
        if (NULL == methodSel) {
            NSLog(@"doSdkCall: UMUtils no method%@",nsMethod);
            return;
       }
        
        void (*setter)(id,SEL,NSString*);
        setter = (void(*)(id,SEL, NSString*))[self methodForSelector:methodSel];
        setter(self,methodSel,nsContent);
        
    } @catch (NSException *exception) {
        NSLog(@"doSdkCall:%@",exception.name);
    }
}

//-(void)doTest:(id)user sel:(SEL)sel content:(NSString*) strParam{
//    NSLog(@"doTest:%@",strParam);
//}

-(void)doTest:(NSString*) strParam{
    NSLog(@"doTest:%@",strParam);
}

#pragma SDK_INTERFACE_COMMON

-(void)doInit:(NSString*) strParam{
    NSLog(@"doInit:%@",strParam);
    NSLog(@"deviceIDForIntegration:%@",[UMConfigure deviceIDForIntegration]);
    NSString* appKey = [NSString stringWithUTF8String:CTUMConfig::UM_APP_KEY.c_str()];
    NSString* channel = [NSString stringWithUTF8String:CTUMConfig::UM_APP_CHANEL_ID.c_str()];
    [UMConfigure initWithAppkey:appKey channel:channel];
    [MobClick setScenarioType:E_UM_GAME];
    
//
//    NSString* verion = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleShortVersionString"];
    
}

-(void)setScenarioType:(NSString*) strParam{
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];
        int value = [[dict objectForKey:@"scenario_type"] intValue];
        [MobClick setScenarioType:(eScenarioType)value];
    } @catch (NSException *exception) {
        NSLog(@"setScenarioType:exception:%@",exception.description);
    }
}

-(void)setDebugMode:(NSString*)strParam{
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];
        BOOL value = [[dict objectForKey:@"is_debug"] boolValue];
        [UMConfigure setLogEnabled:value];
        
    } @catch (NSException *exception) {
        NSLog(@"setDebugMode:exception:%@",exception.description);
    }
}

-(void)doSignIn:(NSString*)strParam{
    NSLog(@"doSignIn:%@",strParam);
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];
        NSString* strProvider = [dict objectForKey:@"provider"];
        strProvider = NULL!= strProvider?strProvider:@"default";
        NSString* strUID = [dict objectForKey:@"id"];
        strUID = NULL!= strUID?strUID:@"";
        if (0 == strUID.length) {
            NSLog(@"doSignIn:need uid");
            return;
        }
        [MobClick profileSignInWithPUID:strUID provider:strProvider];
    } @catch (NSException *exception) {
        NSLog(@"doSignIn:exception:%@",exception.description);
    }
}

-(void)doSignOff:(NSString*)strParam{
    NSLog(@"doSignOff:%@",strParam);
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSLog(@"doSignOff:%@",strParam);
        [MobClick profileSignOff];
    } @catch (NSException *exception) {
        NSLog(@"doSignIn:exception:%@",exception.description);
    }
}

-(void)doPageStart:(NSString*)strParam{
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];
        NSString* strValue = [dict objectForKey:@"content"];
        strValue = NULL!= strValue?strValue:@"default";
        [MobClick beginLogPageView:strValue];
    } @catch (NSException *exception) {
        NSLog(@"doPageStart:exception:%@",exception.description);
    }
}
-(void)doPageEnd:(NSString*)strParam{
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];
        NSString* strValue = [dict objectForKey:@"content"];
        strValue = NULL!= strValue?strValue:@"default";
        [MobClick endLogPageView:strValue];
    } @catch (NSException *exception) {
        NSLog(@"doPageEnd:exception:%@",exception.description);
    }
}

-(void)setEvent:(NSString*)strParam{
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];
        NSString* strEventId = [dict objectForKey:@"event_id"];
        if (NULL == strEventId) {
            NSLog(@"setEvent: no event_id");
            return;
        }
        NSString* strEventValue = [dict objectForKey:@"event_value"];
//        NSString* keyValues = [dict objectForKey:@"key_value_map"];
        NSDictionary* dictValues = [dict objectForKey:@"key_value_map"];

        if (NULL != strEventValue && NULL != dictValues) {
            [MobClick event:strEventId attributes:dictValues counter:[strEventValue intValue]];
        }else if (NULL == strEventValue && NULL != dictValues){
            [MobClick event:strEventId attributes:dictValues];
        }else{
            [MobClick event:strEventId];
        }
    } @catch (NSException *exception) {
        NSLog(@"setEvent:exception:%@",exception.description);
    }
}

#pragma SDK_INTERFACE_GAME
-(void)setUserLevel:(NSString*)strParam{
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];
        NSString* strValue = [dict objectForKey:@"level"];
        if (NULL == strValue) {
            NSLog(@"setUserLevel:no level");
            return;
        }
        [MobClickGameAnalytics setUserLevel:strValue];
    } @catch (NSException *exception) {
        NSLog(@"doPageEnd:exception:%@",exception.description);
    }
}
-(void)setLevel:(NSString*)strParam{
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];
        NSString* strCmd = [dict objectForKey:@"cmd"];
        if (NULL == strCmd) {
            NSLog(@"setLevel:no cmd");
            return;
        }
        NSString* strValue = [dict objectForKey:@"level"];
        if (NULL == strValue) {
            NSLog(@"setLevel:no level");
            return;
        }
        int cmd = [strCmd intValue];
        if (0 == cmd) {
            [MobClickGameAnalytics startLevel:strValue];
        }else if (1 == cmd){
            [MobClickGameAnalytics finishLevel:strValue];
        }else{
            [MobClickGameAnalytics failLevel:strValue];
        }
    } @catch (NSException *exception) {
        NSLog(@"setLevel:exception:%@",exception.description);
    }
}

-(void)doPlay:(NSString*)strParam{
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];
        double money = [[dict objectForKey:@"money"] doubleValue];
        int source = 1;
        NSString* strValue = [dict objectForKey:@"source"];
        if (NULL != strValue) {
            source = [strValue intValue];
        }
        int number = 0;
        strValue = [dict objectForKey:@"number"];
        if (NULL != strValue) {
            number = [strValue intValue];
        }
        double price = 0;
        strValue = [dict objectForKey:@"price"];
        if (NULL != strValue) {
            price = [strValue doubleValue];
        }
        NSString* strId = [dict objectForKey:@"item_id"];
        if (NULL == strId) {
            [MobClickGameAnalytics pay:money source:source coin:price];
        }else{
            [MobClickGameAnalytics pay:money source:source item:strId amount:number price:price];
        }
        
    } @catch (NSException *exception) {
        NSLog(@"setLevel:exception:%@",exception.description);
    }
}

-(void)doBuy:(NSString*)strParam{
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];

        int number = 0;
        NSString* strValue = [dict objectForKey:@"number"];
        if (NULL != strValue) {
            number = [strValue intValue];
        }
        double price = 0;
        strValue = [dict objectForKey:@"price"];
        if (NULL != strValue) {
            price = [strValue doubleValue];
        }
        NSString* strId = [dict objectForKey:@"item_id"];
        if (nullptr == strId) {
            NSLog(@"doBuy:no item_id");
            return;
        }
        [MobClickGameAnalytics buy:strId amount:number price:price];
        
    } @catch (NSException *exception) {
        NSLog(@"setLevel:exception:%@",exception.description);
    }
}

-(void)doUse:(NSString*)strParam{
    @try {
        strParam = NULL == strParam ? @"{}" : strParam;
        NSData* jsData = [strParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsData
                                                             options:kNilOptions
                                                               error:nil];
        
        int number = 0;
        NSString* strValue = [dict objectForKey:@"number"];
        if (NULL != strValue) {
            number = [strValue intValue];
        }
        double price = 0;
        strValue = [dict objectForKey:@"price"];
        if (NULL != strValue) {
            price = [strValue doubleValue];
        }
        NSString* strId = [dict objectForKey:@"item_id"];
        if (nullptr == strId) {
            NSLog(@"doBuy:no item_id");
            return;
        }
        [MobClickGameAnalytics use:strId amount:number price:price];
        
    } @catch (NSException *exception) {
        NSLog(@"doUse:exception:%@",exception.description);
    }
}

#pragma TOOLS
-(NSDictionary *)dictionaryWithJsonString:(NSString *)jsonString {
    
    if (jsonString == nil) {
        
        return nil;
        
    }
    
    NSData *jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    
    NSError *err;
    
    NSDictionary *dic = [NSJSONSerialization JSONObjectWithData:jsonData
                         
                                                        options:NSJSONReadingMutableContainers
                         
                                                          error:&err];
    
    if(err) {
        
        NSLog(@"json解析失败：%@",err);
        
        return nil;
        
    }
    return dic;
}
-(NSString *)stringWithDictionary:(NSDictionary *)dict
{
    if (NULL == dict) {
        return NULL;
    }
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:NSJSONWritingPrettyPrinted error:&error];
    NSString *jsonString;
    
    if (!jsonData) {
        NSLog(@"%@",error);
    }else{
        jsonString = [[NSString alloc]initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    
    NSMutableString *mutStr = [NSMutableString stringWithString:jsonString];
    NSRange range2 = {0,mutStr.length};
    //去掉字符串中的换行符
    [mutStr replaceOccurrencesOfString:@"\n" withString:@"" options:NSLiteralSearch range:range2];
    return mutStr;
}

@end
