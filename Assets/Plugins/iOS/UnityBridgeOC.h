//
//  UnityBridgeOC.h
//  Unity-iPhone
//
//  Created by SpanishScream on 2017/8/15.
//
//

#import <Foundation/Foundation.h>

@interface UnityBridgeOC : NSObject

@property (nonatomic,assign) BOOL isDebug;
@property (nonatomic,strong) NSString * u3dGameObjectName;
@property (nonatomic,strong) NSString * u3dCallbackName_onResp;
@property (nonatomic,strong) NSString * u3dCallbackName_onReq;
@property (nonatomic,strong) NSString * u3dCallbackName_onLocation;
@property (nonatomic,strong) NSString * u3dCallbackName_onQYUnreadCountChanged;
@property (nonatomic,strong) NSString * u3dCallbackName_onAppPayResult;

+ (instancetype)sharedInstance;

-(void) setIdentifyCode:(NSString *)code;
-(NSString *) getIdentifyCode;

@end

