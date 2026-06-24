//
//  CTAppLink.h
//  youmeng
//
//  Created by 武浩颀 on 2018/10/8.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#ifndef CTAppLink_h
#define CTAppLink_h

#import <Foundation/Foundation.h>

@interface CTAppLink : NSObject

+(void)appLink:(NSString*)url;

+(NSString*)getLinkData;
+(void)resetLinkData;

@end


#endif /* CTAppLink_h */
