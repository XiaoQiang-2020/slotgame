//
//  PhotoService.m
//  youmeng
//
//  Created by 武浩颀 on 2018/11/3.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#import "CTPhotoService.h"
#import "CropViewController.h"
#import "CTPermissionUtils.h"
#import "CTCommonTools.h"
#import "CmdDefine.h"

@interface CTPhotoService()<UINavigationControllerDelegate, UIImagePickerControllerDelegate,CTCropDelegate>
@property(nonatomic, assign) bool isCrop;
@property(nonatomic, strong) UIImage* image;
@property(nonatomic,strong) NSString* cachePath;
@property(nonatomic, assign) int cmd;
@end

@implementation CTPhotoService{
}
static id _photoInstance;
- (id)init
{
    self = [super init];
    if (self) {
        
    }
    return self;
}

- (void)dealloc
{
    _photoInstance = NULL;
}

+ (id)instance{
    NSLog(@"PhotoService:instance");
    if (NULL == _photoInstance) {
        _photoInstance = [[CTPhotoService alloc] init];
    }
    return _photoInstance;
}

+ (void) setCmd:(int)value{
    [[CTPhotoService instance] setCmd:value];
}

+ (void)openCamera:(const char *)strParam{
    
    [[CTPermissionUtils instance] getCameraPermissions:^(BOOL authorized) {
        if (!authorized) {
            return ;
        }
        id obj = [CTPhotoService instance];
        UIImagePickerControllerSourceType sourceType = UIImagePickerControllerSourceTypeCamera;
        [obj onExecute:strParam sourceType:sourceType];
    }];
}

+ (void) openAlbum:(const char *)strParam{
    
    [[CTPermissionUtils instance] getPhotoPermissions:^(BOOL authorized) {
        if (!authorized) {
            return ;
        }
        id obj = [CTPhotoService instance];
        UIImagePickerControllerSourceType sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
        [obj onExecute:strParam sourceType:sourceType];
    }];
}

#pragma mark - ToolMethod
- (void)clear{
    self.cachePath = NULL;
    self.image = NULL;
    self.isCrop = true;
}

- (void)onExecute:(const char* )strParam sourceType:(UIImagePickerControllerSourceType) sourceType{
    try {
        strParam = nullptr == strParam ? "{}" : strParam;
        [self clear];
        NSDictionary* dict = [CTCommonTools dictionaryWithJsonString:[NSString stringWithUTF8String:strParam]];
        bool isCrop = true;
        id value = dict[@"isCrop"];
        if (NULL != value) {
            isCrop = [value boolValue];
        }
        [self setIsCrop:isCrop];
        [self setCachePath:dict[@"path"]];
        
        UIImagePickerController  *imagePicker = [[UIImagePickerController alloc] init];
//        UIImagePickerControllerSourceType sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
        imagePicker.sourceType = sourceType;
//        imagePicker.allowsEditing = YES;
        imagePicker.delegate = self;
        [CTGetGLViewController() presentViewController:imagePicker animated:NO completion:NULL];
        
        NSLog(@"[CTCommonTools cachePath]:%@",[CTCommonTools cachePath]);
        
    } catch (NSException* e) {
        NSLog(@"openAlbum exception = %@",e.description);
    }
}



- (void) gotoCrop:(UIImage*)image{
    CropViewController* viewController = [[CropViewController alloc] init];
    viewController.image = image;
    viewController.delegate = self;
    viewController.isSaveToPhoto = NO;
    
//    UIViewController * rootView = (UIViewController*)CTGetGLViewController() ;
//    [rootView.navigationController pushViewController:viewController animated:YES];
    
    [CTGetGLViewController() presentViewController:viewController animated:NO completion:nil];
}

#pragma mark - UIImagePickerControllerDelegate
- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info{
    if (!_isCrop) {
        return;
    }
    
    UIImage* cropImage = info[UIImagePickerControllerOriginalImage];
    [picker dismissViewControllerAnimated:YES completion:NULL];
     [self gotoCrop:cropImage];
}

#pragma mark - CTCropDelegate
- (void)doResult:(UIImage *)image{
    if (NULL == self.cachePath) {
        self.cachePath = [NSString stringWithFormat:@"%@/%ld.png",[CTCommonTools cachePath],time(NULL)];
    }
    NSLog(@"CTPhotoService:doResult:%@",self.cachePath);
    [UIImagePNGRepresentation(image) writeToFile:self.cachePath atomically:YES];
    UIImage *test = [UIImage imageWithContentsOfFile:self.cachePath];
    NSLog(@"image size%@",NSStringFromCGSize(test.size));
    NSMutableDictionary* dict = [NSMutableDictionary dictionary];
    [dict setObject:self.cachePath forKey:@"path"];
    NSString* json = [CTCommonTools stringWithDictionary:dict];
    OS_TO_LUA(self.cmd, [json UTF8String]);
    [self clear];
}

@end
