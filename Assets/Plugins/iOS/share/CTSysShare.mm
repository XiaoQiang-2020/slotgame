//
//  CTSysShare.m
//  Unity-iPhone
//
//  Created by 武浩颀 on 2018/10/25.
//

#import <Foundation/Foundation.h>
#import "CTSysShare.h"
#import "CTCommonTools.h"
#import "CmdDefine.h"

#define CT_SHARE_SAFE_ID(value) (value = NULL == value ? @"": value);

@implementation CTSysShare{
    
}

enum CT_SHARE_TYPE{
    E_TEXT       = 1,
    E_IMAGE      = 2,
    E_VIDEO      = 3,
    E_AUDIO      = 4,
    E_EMOJI      = 5,
    E_WEB_URL    = 6,
    E_FILE       = 7,
};

+(void)systemShare:(const char *)strParam{
    if (nullptr == strParam) {
        return;
    }
    try {
        NSString* strNSString = [NSString stringWithUTF8String:strParam];
        NSDictionary* dict = [CTCommonTools dictionaryWithJsonString:strNSString];
        int actionType = [[dict objectForKey:@"actiontype"] intValue];
        NSString* strTitle = [dict objectForKey:@"title"];
        CT_SHARE_SAFE_ID(strTitle)
        NSString* strContent = [dict objectForKey:@"description"];
        CT_SHARE_SAFE_ID(strContent);
        NSString* strUrl = NULL;
        NSString *thumbPath     = dict[@"thumbPath"];
        CT_SHARE_SAFE_ID(thumbPath);
        NSData *thumeData = [NSData dataWithContentsOfFile:thumbPath];
        NSArray* activityItems = NULL;
        
        switch (actionType) {
            case CT_SHARE_TYPE::E_TEXT :
                {
                    strContent = [dict objectForKey:@"text"];
                    CT_SHARE_SAFE_ID(strContent);
                    strTitle = [NSString stringWithFormat:@"%@\n%@",strTitle,strContent];
                    NSURL* url = [NSURL URLWithString:@"content://"];
                    activityItems = @[strTitle,url];
                }
                break;
            case CT_SHARE_TYPE::E_IMAGE :
                {
                    strUrl = [dict objectForKey:@"imgPath"];
                    CT_SHARE_SAFE_ID(strUrl);
//                    strUrl= [[NSBundle mainBundle] pathForResource:@"app_icon" ofType:@"png"];
                    NSData *imageData = [NSData dataWithContentsOfFile:strUrl];
                    if (NULL == imageData) {
                        [CTSysShare doCallback:NO];
                        return;
                    }
                    strTitle = [NSString stringWithFormat:@"%@\n%@",strTitle,strContent];
                    activityItems = @[strTitle,imageData];
                }
                break;
            case CT_SHARE_TYPE::E_VIDEO :
                {
                    strUrl = [dict objectForKey:@"videoUrl"];
                    CT_SHARE_SAFE_ID(strUrl);
                    if ([CTCommonTools stringIsEmpty:strUrl]) {
                        [CTSysShare doCallback:NO];
                        return;
                    }
                    NSURL* url = [NSURL fileURLWithPath:strUrl];
                    activityItems = @[url];
                }
                break;
            case CT_SHARE_TYPE::E_AUDIO :
                {
                    strUrl = [dict objectForKey:@"musicUrl"];
                    CT_SHARE_SAFE_ID(strUrl);
                    if ([CTCommonTools stringIsEmpty:strUrl]) {
                        [CTSysShare doCallback:NO];
                        return;
                    }
                    NSURL* url = [NSURL fileURLWithPath:strUrl];
                    activityItems = @[url];
                }
                break;
            case CT_SHARE_TYPE::E_EMOJI :
                {
                    strUrl = [dict objectForKey:@"emojiPath"];
                    CT_SHARE_SAFE_ID(strUrl);
                    NSData *imageData = [NSData dataWithContentsOfFile:strUrl];
                    if (NULL == imageData) {
                        [CTSysShare doCallback:NO];
                        return;
                    }
                    strTitle = [NSString stringWithFormat:@"%@\n%@",strTitle,strContent];
                    activityItems = @[strTitle,imageData];
                }
                break;
            case CT_SHARE_TYPE::E_WEB_URL :
                {
                    strUrl = [dict objectForKey:@"webpageUrl"];
                    CT_SHARE_SAFE_ID(strUrl)
                    if ([CTCommonTools stringIsEmpty:strUrl]) {
                        [CTSysShare doCallback:NO];
                        return;
                    }
                    strTitle = [NSString stringWithFormat:@"%@\n%@",strTitle,strContent];
                    NSURL* url = [NSURL URLWithString:strUrl];
                    activityItems = @[strTitle,url];
                }
                break;
            case CT_SHARE_TYPE::E_FILE :
                {
                    strUrl = [dict objectForKey:@"filePath"];
                    CT_SHARE_SAFE_ID(strUrl);
                    if ([CTCommonTools stringIsEmpty:strUrl]) {
                        [CTSysShare doCallback:NO];
                        return;
                    }
                    NSURL* url = [NSURL fileURLWithPath:strUrl];
                    activityItems = @[url];
                }
                break;
                
            default:
                {
                    NSLog(@"CTSysShare:systemShare:not support%d",actionType);
                }
                break;
        }
        
        [CTSysShare doShare:activityItems];
        
    } catch (NSException* exception) {
        NSLog(@"CTSysShare:systemShare:%@",exception.description);
    }

}

+(void)doShare:(NSArray*) activityItems{
    if (NULL == activityItems or 0 == activityItems.count) {
        [CTSysShare doCallback:NO];
        return;
    }
    
    UIActivityViewController *activityVC = [[UIActivityViewController alloc]initWithActivityItems:activityItems
                                                                            applicationActivities:nil];
    //不出现在活动项目
    activityVC.excludedActivityTypes = @[UIActivityTypePrint, UIActivityTypeCopyToPasteboard,UIActivityTypeAssignToContact,UIActivityTypeSaveToCameraRoll,UIActivityTypeAddToReadingList];
    
    //给activityVC的属性completionHandler写一个block。
    //用以UIActivityViewController执行结束后，被调用，做一些后续处理。
    UIActivityViewControllerCompletionWithItemsHandler myBlock = ^(UIActivityType activityType, BOOL completed, NSArray * returnedItems, NSError * activityError)
    {
        if (completed)
        {
            NSLog(@"completed");
        }
        else
        {
            NSLog(@"cancel");
        }
        [CTSysShare doCallback:completed];
    };
    
    // 初始化completionHandler，当post结束之后（无论是done还是cancell）该blog都会被调用
    activityVC.completionWithItemsHandler = myBlock;
    UIViewController * rootVc = [UIApplication sharedApplication].keyWindow.rootViewController;
    [rootVc presentViewController:activityVC animated:TRUE completion:nil];
}

+(void)doCallback:(BOOL) isSuccess{
    NSString* strResult = isSuccess ? @"true":@"false";
    strResult = [NSString stringWithFormat:@"{\"resultStatus\":\"%@\"}",strResult];
    OS_TO_LUA(CMD_WX_SHARE, [strResult UTF8String]);
}

@end
