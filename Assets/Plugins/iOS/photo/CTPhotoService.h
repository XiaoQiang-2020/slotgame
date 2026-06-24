//
//  PhotoService.h
//  youmeng
//
//  Created by 武浩颀 on 2018/11/3.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#ifndef PhotoService_h
#define PhotoService_h

#import <UIKit/UIKit.h>
#import "CTBase.h"

@interface CTPhotoService : CTBase

+ (void) setCmd:(int) value;
+ (void) openCamera:(const char* ) strParam;
+ (void) openAlbum:(const char*) strParam;

@end


#endif /* PhotoService_h */
