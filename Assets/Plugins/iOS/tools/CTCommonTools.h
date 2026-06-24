//
//  CTCommonTools.h
//  Unity-iPhone
//
//  Created by 武浩颀 on 2018/10/13.
//

#ifndef CTCommonTools_h
#define CTCommonTools_h
#import <UIKit/UIKit.h>

@interface CTCommonTools : NSObject
+ (bool) doSdkCall:(id)target param:(const char*) strParam;
+ (NSString *)stringWithDictionary:(NSDictionary *)dict;
+ (NSDictionary *)dictionaryWithJsonString:(NSString *)jsonString;
+(bool)stringIsEmpty:(NSString*) strParam;
+ (NSString*)cachePath;
+ (NSString*)resPath:(NSString*)path;

@end

#endif /* CTCommonTools_h */
