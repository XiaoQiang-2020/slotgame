//
//  CTCommonTools.m
//  Unity-iPhone
//
//  Created by 武浩颀 on 2018/10/13.
//

#import "CTCommonTools.h"

@implementation CTCommonTools{
    
}

#pragma SDK_CALL_TOOLS
+ (bool) doSdkCall:(id)target param:(const char*) strParam
{
    if (NULL == target) {
        NSLog(@"CTCommonTools:doSdkCall:target =NULL");
        return false;
    }
    if (nullptr == strParam) {
        return false;
    }
    if (0 == strlen(strParam)) {
        return false;
    }
    NSString *nsParam = [NSString stringWithUTF8String:strParam];
    if (NULL == nsParam) {
        return false;
    }
    @try {
        NSData* nsData = [nsParam dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:nsData
                                                             options:kNilOptions
                                                               error:nil];
        
        NSString* nsMethod = dict[@"method"];
        if (NULL == nsMethod) {
            NSLog(@"doSdkCall: no method in param");
            return false;
        }
        NSDictionary* ditContent = [dict objectForKey:@"content"];
        NSString* nsContent = [CTCommonTools stringWithDictionary:ditContent];
        nsContent = NULL == nsContent ? @"{}" :nsContent;
        NSLog(@"method=%@\n content=%@",nsMethod, nsContent);
        
        SEL methodSel = NSSelectorFromString([NSString stringWithFormat:@"%@:",nsMethod]);
        if (NULL == methodSel) {
            NSLog(@"doSdkCall: UMUtils no method%@",nsMethod);
            return false;
        }
        
        void (*setter)(id,SEL,NSString*);
        setter = (void(*)(id,SEL, NSString*))[target methodForSelector:methodSel];
        setter(target,methodSel,nsContent);
        
    } @catch (NSException *exception) {
        NSLog(@"CTCommonTools:doSdkCall:%@",exception.name);
        return false;
    }
    return true;
}

#pragma NSDictionary_TOOLS
+ (NSDictionary *)dictionaryWithJsonString:(NSString *)jsonString {
    
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
+ (NSString *)stringWithDictionary:(NSDictionary *)dict
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
#pragma mark - PATH
+(bool)stringIsEmpty:(NSString *)strParam{
    if (NULL == strParam) {
        return true;
    }
    return strParam.length == 0;
}

+ (NSString*)cachePath{
    NSString * path = NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES).firstObject;
    return path;
}

+ (NSString*)resPath:(NSString *)path{
    path = NULL ==path ? @"" :path;
    return [NSString stringWithFormat:@"Data/IOS_Res/%@",path];
}

+ (NSString*)bunlePath:(NSString*)path ofType:(NSString*)type {
    if (NULL == path || NULL == type) {
        return @"";
    }
    path = [NSString stringWithFormat:@"Data/IOS_Res/%@",path];
    return [[NSBundle mainBundle] pathForResource:path ofType:type];
}


@end
