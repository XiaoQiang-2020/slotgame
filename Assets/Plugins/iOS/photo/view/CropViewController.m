//
//  CropViewController.m
//  youmeng
//
//  Created by 武浩颀 on 2018/11/3.
//  Copyright © 2018年 武浩颀. All rights reserved.
//

#import "CropViewController.h"
#import "TKImageView/TKImageView.h"
#import "CTCommonTools.h"

@interface CropViewController()
@property(nonatomic, assign) BOOL isCrop;
@property(nonatomic, strong) TKImageView* tkView;
@end


#define CROP_SCREEN_WIDTH       [UIScreen mainScreen].bounds.size.width
#define CROP_SCREEN_HEIGHT      [UIScreen mainScreen].bounds.size.height
#define CROP_DEFAULT_OFFSET     100
@implementation CropViewController

- (void)viewDidLoad{
    [super viewDidLoad];
    [self createTKView];
    [self createOperate];
}

- (void)viewDidAppear:(BOOL)animated{
    [super viewDidAppear:animated];
//    [self createTKView];
//    [self createOperate];
}

- (void)createTKView{
    _tkView = [[TKImageView alloc] initWithFrame:CGRectMake(0, 0, CROP_SCREEN_WIDTH, CROP_SCREEN_HEIGHT - CROP_DEFAULT_OFFSET)];
    [self.view addSubview:_tkView];
    //需要进行裁剪的图片对象
    _tkView.toCropImage = _image;
    //是否显示中间线
    _tkView.showMidLines = YES;
    //是否需要支持缩放裁剪
    _tkView.needScaleCrop = NO;
    //是否显示九宫格交叉线
    _tkView.showCrossLines = YES;
    _tkView.cornerBorderInImage = NO;
    _tkView.cropAreaCornerWidth = 44;
    _tkView.cropAreaCornerHeight = 44;
    _tkView.minSpace = 30;
    _tkView.cropAreaCornerLineColor = [UIColor whiteColor];
    _tkView.cropAreaBorderLineColor = [UIColor whiteColor];
    _tkView.cropAreaCornerLineWidth = 6;
    _tkView.cropAreaBorderLineWidth = 1;
    _tkView.cropAreaMidLineWidth = 20;
    _tkView.cropAreaMidLineHeight = 6;
    _tkView.cropAreaMidLineColor = [UIColor whiteColor];
    _tkView.cropAreaCrossLineColor = [UIColor whiteColor];
    _tkView.cropAreaCrossLineWidth = 0.5;
    _tkView.initialScaleFactor = .8f;
    _tkView.cropAspectRatio = 1;
    _tkView.maskColor = [UIColor clearColor];
    
    self.isCrop = YES;
}

- (void)createOperate{
    UIView  *editorView = [[UIView alloc] initWithFrame:CGRectMake(0, CROP_SCREEN_HEIGHT - CROP_DEFAULT_OFFSET, CROP_SCREEN_WIDTH, CROP_DEFAULT_OFFSET)];
    editorView.backgroundColor = [UIColor blackColor];
    editorView.alpha = 0.8;
    [self.view addSubview:editorView];
    
    UIButton *cancleBtn = [UIButton buttonWithType:UIButtonTypeCustom];
    cancleBtn.frame = CGRectMake(((CROP_SCREEN_WIDTH / 3.0) - 50)/2.0, (120 - 50)/2.0, 50, 50);

    NSString* path = [CTCommonTools resPath:@"image/photo_cancel"];
    [cancleBtn setImage:[UIImage imageNamed:path] forState:UIControlStateNormal];
    [cancleBtn addTarget:self action:@selector(cancel:) forControlEvents:UIControlEventTouchUpInside];
    
    [editorView addSubview:cancleBtn];
    
//    UIButton *clipBtn = [UIButton buttonWithType:UIButtonTypeCustom];
//    clipBtn.frame = CGRectMake(((CROP_SCREEN_WIDTH) - 50)/2.0, (120 - 50)/2.0, 50, 50);
//    [cancleBtn setTitle:@"OK" forState:UIControlStateNormal];
////    [clipBtn setImage:[UIImage imageNamed:@"clipPhoto"] forState:UIControlStateNormal];
////    [clipBtn setImage:[UIImage imageNamed:@"backPhoto"] forState:UIControlStateSelected];
//
//    [clipBtn addTarget:self action:@selector(clip:) forControlEvents:UIControlEventTouchUpInside];
//    [editorView addSubview:clipBtn];
    
    UIButton *sureBtn = [UIButton buttonWithType:UIButtonTypeCustom];
    sureBtn.frame = CGRectMake(((CROP_SCREEN_WIDTH/3.0) - 50)/2.0 + (CROP_SCREEN_WIDTH * 2.0/3.0), (120 - 50)/2.0, 50, 50);
    path = [CTCommonTools resPath:@"image/photo_ok"];
    [sureBtn setImage:[UIImage imageNamed:path] forState:UIControlStateNormal];
    [sureBtn addTarget:self action:@selector(ok:) forControlEvents:UIControlEventTouchUpInside];
    
    [editorView addSubview:sureBtn];
}

#pragma mark - IBAction

- (void)cancel:(UIButton*)btn{
    [self dismissViewControllerAnimated:YES completion:nil];
}

- (void)clip:(UIButton*)btn{
    
}

- (void)ok:(UIButton*)btn{
    
    UIImage* image = NULL;
    if (self.isCrop) {
        image = [_tkView currentCroppedImage];
    }else{
        image = self.image;
    }
    
    if (self.delegate && [self.delegate respondsToSelector:@selector(doResult:)]) {
        [_delegate doResult:image];
    }
    
    if (self.isSaveToPhoto) {
        UIImageWriteToSavedPhotosAlbum(image, self, nil, nil);
    }
    
    [self dismissViewControllerAnimated:YES completion:nil];
    [self.picker dismissViewControllerAnimated:YES completion:nil];
    [self.controller dismissViewControllerAnimated:YES completion:nil];
}

- (void)didReceiveMemoryWarning{
    [super didReceiveMemoryWarning];
}




@end
