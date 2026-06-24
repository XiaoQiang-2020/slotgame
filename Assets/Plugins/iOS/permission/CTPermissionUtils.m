//
//  CTPermissionUtils.m
//  youmeng
//
//  Created by 武浩颀 on 2018/11/5.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#import "CTPermissionUtils.h"

#import <Photos/Photos.h>

#import "CmdDefine.h"

#define PERMISSION_CALL(call,result) if(call){call(result);}
#define PERMISSION_DEFATULT_CONTENT @"您可以进入系统\"设置>隐私>%@\",允许访问您的%@"
#define PERMISSION_DEFATULT_TIELE @"%@权限未开启"
#define PERMISSION_PHOTO_NAME  @"相册"
#define PERMISSION_CAMERA_NAME  @"相机"


@interface CTPermissionUtils()
@end

static id _permissonInstance = nil;
@implementation CTPermissionUtils

+ (id)instance{
    if (NULL == _permissonInstance) {
        _permissonInstance = [[CTPermissionUtils alloc] init];
    }
    return _permissonInstance;
}

#pragma mark - 相册权限
/** 是否开启相册权限 */
- (void)getPhotoPermissions:(void(^)(BOOL authorized))completion{
    @try {
        PHAuthorizationStatus status = [PHPhotoLibrary authorizationStatus];
        if (status == PHAuthorizationStatusAuthorized) {
            [self doResult:completion result:YES name:@"相册"];
            return;
        }
        if (status == PHAuthorizationStatusNotDetermined) {
            [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    if (status == PHAuthorizationStatusAuthorized) {
                        PERMISSION_CALL(completion, YES)
                        [self doResult:completion result:YES name:PERMISSION_PHOTO_NAME];
                    }else{
                        PERMISSION_CALL(completion, NO)
                        // 权限被拒绝
                        [self doResult:completion result:NO name:PERMISSION_PHOTO_NAME];
                    }
                });
            }];
            return;
        }
        // 权限被拒绝
        [self doResult:completion result:NO name:PERMISSION_PHOTO_NAME];
    } @catch (NSException *exception) {
        NSLog(@"CTPermissionUtils:getPhotoPermissions exception:%@",exception.description);
    }
}

#pragma mark - 相机权限
/** 是否开启相机权限*/
- (void)getCameraPermissions:(void(^)(BOOL authorized))completion{
    @try {
        AVAuthorizationStatus status = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
        if (status == AVAuthorizationStatusAuthorized) {
            [self doResult:completion result:YES name:PERMISSION_CAMERA_NAME];
            return;
        }
        if (status == AVAuthorizationStatusNotDetermined) {
            [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    [self doResult:completion result:granted name:PERMISSION_CAMERA_NAME];
                });
            }];
            return;
        }
        // 权限被拒绝
        [self doResult:completion result:NO name:PERMISSION_CAMERA_NAME];
    } @catch (NSException *exception) {
        NSLog(@"CTPermissionUtils:getCameraPermissions exception:%@",exception.description);
    }
}

#pragma mark - 跳转
- (void)doResult:(void(^)(BOOL authorized)) completion result:(BOOL)result name:(NSString*)name{
    if (NULL == completion) {
        NSLog(@"CTPermissionUtils:doResult completion = null");
        return;
    }
    PERMISSION_CALL(completion, result)
    if (!result) {
        [self gotoSetting:name];
    }
}
- (void)gotoSetting:(NSString*)name{
    NSString* content = [NSString stringWithFormat:PERMISSION_DEFATULT_CONTENT,name,name];
    NSString* title = [NSString stringWithFormat:PERMISSION_DEFATULT_TIELE,name];
    [self gotoSettting:content title:title];
}

- (void) gotoSettting:(NSString*)content title:(NSString*)title{
    if (NULL == content || NULL == title) {
        return;
    }
    UIAlertController* alertControll = [UIAlertController alertControllerWithTitle:title message:content preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction* cancelAction = [UIAlertAction actionWithTitle:@"取消" style:UIAlertActionStyleDefault handler:nil];
    UIAlertAction* okAction = [UIAlertAction actionWithTitle:@"确定" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
    }];
    [alertControll addAction:cancelAction];
    [alertControll addAction:okAction];
    dispatch_async(dispatch_get_main_queue(), ^{
        [CTGetGLViewController() presentViewController:alertControll animated:YES completion:nil];
    });
}


@end

