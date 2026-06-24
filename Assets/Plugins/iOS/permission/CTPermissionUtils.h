//
//  CTPermissionUtils.h
//  youmeng
//
//  Created by 武浩颀 on 2018/11/5.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#ifndef CTPermissionUtils_h
#define CTPermissionUtils_h

#import "CTBase.h"

@interface CTPermissionUtils : CTBase

#pragma mark - 相册权限
/** 是否开启相册权限 */
- (void)getPhotoPermissions:(void(^)(BOOL authorized))completion;

#pragma makr - 相机权限
/** 是否开启相机权限*/
- (void)getCameraPermissions:(void(^)(BOOL authorized))completion;

@end

#endif /* CTPermissionUtils_h */
