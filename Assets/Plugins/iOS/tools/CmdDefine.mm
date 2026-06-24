//
//  CmdDefine.m
//  Unity-iPhone
//
//  Created by 武浩颀 on 2018/10/14.
//

#include "CmdDefine.h"
#import <UIKit/UIKit.h>

#ifndef UNITY_IOS
#include "AppDelegate.h"
#else
#import "UnityAppController.h"
#endif

void OS_TO_LUA(int cmd,const char* strParam)
{
    if (!strParam) {
        return;
    }
    const char* strFormat = "{\"cmd\" : %d, \"content\" :%s}";
    size_t nLen = strlen(strFormat);
    nLen = nLen+ nLen;
    nLen += strlen(strParam);
    char* strContent = (char*)malloc(nLen);
    sprintf(strContent, strFormat,cmd,strParam);
    printf("OS_TO_LUA:%d=>%s",cmd,strContent);
    UnitySendMessage("AppGameManager", "UnitySendMessageCall", strContent);
    free(strContent);
    
}

void OS_TO_LUA_JSON(const char* strParam)
{
    if (!strParam) {
        return;
    }
    UnitySendMessage("AppGameManager", "UnitySendMessageCall", strParam);
    printf("OS_TO_LUA_JSON:=>%s",strParam);
}

id CTGetGLViewController(){
#ifndef UNITY_IOS
    AppDelegate *app = (AppDelegate*)[UIApplication sharedApplication].delegate;
    return app.window.rootViewController;
#else
    return GetAppController().rootViewController;
#endif
    return NULL;
}


