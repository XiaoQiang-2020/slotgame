//
//  CTAppLink.m
//  youmeng
//
//  Created by 武浩颀 on 2018/10/8.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "CTAppLink.h"

static NSString* _linkData = NULL;
static NSString* LINK_KEY = @"stevesoft";

@implementation CTAppLink{
    
}

+(void)appLink:(NSString*)url{
    if (NULL == url) {
        return;
    }
    if (![url hasPrefix:LINK_KEY]) {
        return;
    }
    
    [CTAppLink cacheLinkData:url];
}

+(void)cacheLinkData:(NSString*)url{
    if (NULL == url) {
        return;
    }
    NSRange range = [url rangeOfString:@"//"];
    NSString* strParam = [url substringFromIndex:range.location+ range.length];
    NSMutableDictionary* dict = [NSMutableDictionary dictionary];
    NSArray*arrAll = [strParam componentsSeparatedByString:@"?"];
     [dict setValue:arrAll.firstObject forKey:@"url"];
    NSArray* arrParam = [arrAll.lastObject componentsSeparatedByString:@"&"];

   
    for (int i=0; i< arrParam.count; i++) {
        NSArray* arrTemp = [arrParam[i] componentsSeparatedByString:@"="];
        [dict setValue:arrTemp.lastObject forKey:arrTemp.firstObject];
    }
    _linkData = [CTAppLink stringWithDictionary:dict];
    NSLog(@"cacheLinkData:%@",_linkData);
    
}

+(NSString *)stringWithDictionary:(NSDictionary *)dict
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

+(NSString*)getLinkData{
    return NULL == _linkData ? @"{}":_linkData;
}
+(void)resetLinkData{
    _linkData = @"{}";
}

@end
