//
//  NSObject+WWKeyChianUtils.h
//  Unity-iPhone
//
//  Created by sdf on 2018/9/5.
//

#import <Foundation/Foundation.h>

#import <Foundation/Foundation.h>

@interface WWKeyChianUtils : NSObject

+ (NSMutableDictionary *)getKeychainQuery:(NSString *)service;

+ (void)save:(NSString *)service data:(id)data;

+ (id)load:(NSString *)service;

+ (void)delete:(NSString *)service;

@end
