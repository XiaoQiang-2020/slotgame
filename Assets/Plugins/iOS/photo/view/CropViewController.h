//
//  CropViewController.h
//  youmeng
//
//  Created by 武浩颀 on 2018/11/3.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#ifndef CropViewController_h
#define CropViewController_h

#import <UIKit/UIKit.h>

@protocol CTCropDelegate<NSObject>

- (void) doResult:(UIImage*)image;

@end

@interface CropViewController: UIViewController

@property(nonatomic, strong) UIImage* image;
@property(nonatomic, strong) UIImagePickerController *picker;
@property(nonatomic, strong) UIViewController* controller;
@property(nonatomic, weak) id<CTCropDelegate> delegate;
@property(nonatomic, assign) BOOL isSaveToPhoto;

@end


#endif /* CropViewController_h */
