
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 通用全局预置变量定义类，声明游戏内通用全局预置变量的值。
    /// </summary>
    public class GlobalVar
    {

        /// <summary>
        /// 调用UnityResources.UnloadUnusedAssets间隔帧数
        /// </summary>
        public static int UNLOADED_UNUSED_ASSET_FRAME_INTERVAL = 200;



        /// <summary>
        /// 场景基础节点
        /// </summary>
        public static string GAME_OBJECT_UICAMERA = "UICamera";
        public static string GAME_OBJECT_EVENT = "EventSystem";
        public static string GAME_OBJECT_CANVAS = "Canvas";
        public static string GAME_OBJECT_SHARE_CANVAS = "ShareCanvas";
        public static string GAME_OBJECT_SCENE = "SceneRoot";

        /// <summary>
        /// 层级
        /// </summary>
        public static int PLATFORM_OBJECT_LAYER_UGUI = 11;           // ugui层
        public static int PLATFORM_OBJECT_LAYER_UGUI_MODEL = 12;     // ugui中显示的模型层

        /// <summary>
        /// ui相机和canvas的设置
        /// </summary>
        public static int UI_CAMERA_FIELD_VIEW = 60;
        public static float UI_CAMERA_NEAR_CLIP_PLANE = 0.01f;
        public static float UI_CAMERA_FAR_CLIP_PLANE = 1000;
        public static float UI_CAMERA_DEPTH = 1;
        public static float UI_CAVAS_PLANE_DISTANCE = 100;
        public static int UI_CAVAS_SORTING_ORDER = 0;


        // 路径相关
        public static string RES_ROOT = "Res";                          // 所有资源的根目录
        public static string TEMPT_ROOT = "temp";                       // 计划用来存储玩家头像等用户相关资源文件  相对：application.persistentPath + Res
        public static string UI_PIC_ROOT = "UIPic";             // ui图片存放位置
        public static string RES_ASSETBUNDLE_ROOT = "assetbundle";
        public static string RES_GAMES_ROOT = RES_ASSETBUNDLE_ROOT + "/games";
        public static string RES_LIST_FILE = "res_list_all.txt";
        public static string LUA_PATH_ONE = "lua/tolua";
        public static string LUA_PATH_TWO = "lua/framework";
        public static string LUA_PATH_THREE = "lua/games";
        public static string LUA_PATH_FOUR = "lua";
        public static string LUA_PATH_FIVE = "lua/tolua/protobuf";
        public static string RES_TEMP = "Temp/Res";

#if UNITY_STANDALONE_WIN
        public static string osDir = "win";
#elif UNITY_STANDALONE_OSX
        public static string osDir = "mac";
#elif UNITY_ANDROID
        public static string osDir = "android";
#elif UNITY_IPHONE
        public static string osDir = "ios";
#else
        public static string osDir = "win";
#endif
        // 微信信息
        public static string APPSFLYER_APP_ID = AppSDKConfig.SF_APP_ID;
        public static string APPSFLYER_APP_KEY = AppSDKConfig.SF_APP_KEY;


        // ZeroBraneStudio
        public static string zbsDir = "D:/Program Files (x86)/ZeroBraneStudio/lualibs/mobdebug";
        public static bool openZbsDebugger = false;



        /**********************************************************/
        /* 这面这些变量是读配置表的
        /**********************************************************/
        /// <summary>
        /// 标准屏幕尺寸
        /// </summary>
        public static int STANDORD_SCREEN_WIDTH = 1080;     // GlobarVarConfig.Instance().ScreenWidth;
        public static int STANDORD_SCREEN_HEIGHT = 1920;     // GlobarVarConfig.Instance().ScreenHeight;
                
        /// <summary>
        /// 是否使用assetbundle资源
        /// </summary>
        public static bool IS_RES_MODE_DEBUG = false;      // GlobarVarConfig.Instance().ResDebugMode;
        /// <summary>
        /// 开启LuaIDE调用器
        /// </summary>
        public static bool IS_OPEN_LUAIDE_DEBUG = false;    // GlobarVarConfig.Instance().LuaIdeMode;
        /// <summary>
        /// 消息加密开关
        /// </summary>
        public static bool IS_OPEN_NET_MSG_ENCRYPT = false; // GlobarVarConfig.Instance().MsgEncryptMode;
        /// <summary>
        /// 消息加密密钥
        /// </summary>
        public static string NET_MSG_ENCRYPTION_KEY = "";   // GlobarVarConfig.Instance().MsgEncryptKey;
        /// <summary>
        /// lua文件加密
        /// </summary>
        public static bool IS_OPEN_LUA_FILE_ENCRYPT = false;    // GlobarVarConfig.Instance().LuaEncryptMode;
        /// <summary>
        /// 是否打开Lua Socket库
        /// </summary>
        public static bool IS_OPEN_LUA_SOCKET = false;      // GlobarVarConfig.Instance().LuaSocketMode;
        /// <summary>
        /// 如果开启更新模式
        /// </summary>
        public static bool IS_OPEN_RES_UPDATE = true;       // GlobarVarConfig.Instance().NeedUpdate;   // 更新模式-默认关闭
        /// <summary>
        /// 资源服务器地址
        /// </summary>
        public static string RES_SERVER_URL = "http://192.168.7.6:35173/gameZip/"; //GlobarVarConfig.Instance().UpdateUri;
        //版本号
        public static string APP_VERSION="";
        public static bool  IS_NEED_RES_ECTRACT=false;
        /// <summary>
        /// 用静态构造函数，读取配置
        /// 来初始化上面的部分变量
        /// </summary>
        static GlobalVar()
        {
            string configefilename = "app_basic_config";
            TextAsset configfile = Resources.Load<TextAsset>(configefilename);
            if(configfile == null || string.IsNullOrEmpty(configfile.text))
            {
                Core.Debuger.Log("GlobalVar(): Recource下不存在基本的配置文件：app_basic_config.json, 或内容为空！");
                return;
            }
            try
            {
                LitJson.JsonData jd = LitJson.JsonMapper.ToObject(configfile.text);
                int.TryParse(jd["standord_screen_width"].ToString(), out STANDORD_SCREEN_WIDTH);
                int.TryParse(jd["standord_screen_height"].ToString(), out STANDORD_SCREEN_HEIGHT);
#if UNITY_EDITOR
                bool.TryParse(jd["is_res_mode_debug"].ToString(), out IS_RES_MODE_DEBUG);
#elif UNITY_ANDROID
                IS_RES_MODE_DEBUG=false;
#elif UNITY_IPHONE
                IS_RES_MODE_DEBUG=false;
#endif
                bool.TryParse(jd["is_open_luaide_debug"].ToString(), out IS_OPEN_LUAIDE_DEBUG);
                bool.TryParse(jd["is_open_net_msg_encrypt"].ToString(), out IS_OPEN_NET_MSG_ENCRYPT);
                NET_MSG_ENCRYPTION_KEY = jd["net_msg_encryption_key"].ToString();
                bool.TryParse(jd["is_open_lua_file_encrypt"].ToString(), out IS_OPEN_LUA_FILE_ENCRYPT);
                bool.TryParse(jd["is_open_lua_socket"].ToString(), out IS_OPEN_LUA_SOCKET);
                bool.TryParse(jd["is_open_res_update"].ToString(), out IS_OPEN_RES_UPDATE);
                RES_SERVER_URL = jd["res_server_url"].ToString();
                APP_VERSION = jd["app_version"].ToString();
                if(PlayerPrefs.HasKey("app_version"))
                {
                    if(APP_VERSION!=PlayerPrefs.GetString("app_version"))
                    {
                        PlayerPrefs.SetString("app_version",APP_VERSION);
                        IS_NEED_RES_ECTRACT=true;
                    }
                }
                else
                {
                    PlayerPrefs.SetString("app_version",APP_VERSION);
                     IS_NEED_RES_ECTRACT=true;
                }
            }
            catch(LitJson.JsonException jx)
            {
                Core.Debuger.LogError("GlobalVar(): Recource下的配置文件：app_basic_config.json, 不是合法的json格式或数据不误，请检查！");
                Core.Debuger.LogError(jx.Message);
            }

        }
    }
}