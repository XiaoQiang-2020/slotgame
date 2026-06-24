/***************************************************
 * 文件名：LogDebug.cs
 * 描  述：日志公共接口
 * 时  间：2018-08-06 10:16:09
 * 作  者：李红轩
 * 修  改：
 ***************************************************/
namespace Log
{
    /// <summary>
    /// 日志调用接口
    /// </summary>
    public class LogDebug
    {
        /// <summary>
        /// 游戏开始初始化debug
        /// </summary>
        public static void Init()
        {
            LoggerManager.Intance.Init();
        }
        /// <summary>
        /// 需要再登陆是写入用户id
        /// </summary>
        /// <param name="userID"></param>
        public static void SetUserID(string userID)
        {
            LoggerManager.Intance.SetUserID(userID);
        }
        public static void SetUploadUrl(string url)
        {
            LoggerManager.Intance.SetUploadUrl(url);
        }
        public static void Print(string msg)
        {
            LoggerManager.Intance.Log(msg);
        }
        public static void PrintError(string msg)
        {
            LoggerManager.Intance.Log(msg, LOG_TYPE.LOG_ERROR);
        }
        public static void PrintException(string msg)
        {
            LoggerManager.Intance.Log(msg, LOG_TYPE.LOG_EXCEPTION);
        }
    }
}
