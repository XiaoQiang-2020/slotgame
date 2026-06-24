//
//  UMUtils.h
//  youmeng
//
//  Created by 武浩颀 on 2018/10/4.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#ifndef UMUtils_h
#define UMUtils_h

#import <Foundation/Foundation.h>

@interface CTUMUtils : NSObject


+(id) instance;
-(void) doSdkCall:(const char* ) strParam;

@end

#endif /* UMUtils_h */
