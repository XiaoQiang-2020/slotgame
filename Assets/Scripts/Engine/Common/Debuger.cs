/***************************************************
 * 文件名：Debuger.cs
 * 描  述：调试模式辅助工具。
 * 时  间：2017-04-14
 * 作  者：李海波
 * 修  改：李智海 2017-05-09
 *         c#调用这里的方法打印log，在console中双击可以直接跳到调用的c#文件对应的行
 *         lua中的调用print方法，最后调用的是这是地的Log方法，在console中双击可以直接跳到对应的lua文件
 ***************************************************/
// #if UNITY_EDITOR
// using System.Reflection;
// using UnityEditor;
// using UnityEditor.Callbacks;
// using System.Collections.Generic;
// using System;
// #endif
namespace Core
{
    using UnityDebug = UnityEngine.Debug;
    using SystemDateTime = System.DateTime;
    using System.Diagnostics;


    enum LogType : byte
    {
        Log = 0,
        Warning = 1,
        Error = 2,
        Exception = 3
    }

    public class Debuger
    {
        /// <summary>
        /// 是否为发布版本
        /// </summary>
        public static bool isDebug = true;

        /// <summary>
        /// 断言处理接口，满足条件下将触发异常。
        /// </summary>
        /// <param name="condition">断言检测条件</param>
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                //throw new System.Exception();
                _Log(LogType.Exception, null);
            }
        }

        /// <summary>
        /// 断言处理接口，满足条件下将触发异常。
        /// </summary>
        /// <param name="condition">断言检测条件</param>
        /// <param name="message">异常信息内容</param>
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                //throw new System.Exception(message);
                _Log(LogType.Exception, message);
            }
        }

        public static void Log(object message)
        {
            if (isDebug)
            {
                //UnityDebug.Log(string.Concat(new object[] { "[", SystemDateTime.Now.Second, ":", SystemDateTime.Now.Millisecond, "]", message }));
                _Log(LogType.Log, message);
            }
        }

        public static void Log(object message, params object[] args)
        {
            if (isDebug)
            {
                //UnityDebug.Log(string.Format(string.Concat(new object[] { "[", SystemDateTime.Now.Second, ":", SystemDateTime.Now.Millisecond, "]", message }), args));
                _Log(LogType.Log, message, args);
            }
        }

        public static void LogWarning(object message)
        {
            if (isDebug)
            {
                //UnityDebug.LogWarning(string.Concat(new object[] { "[", SystemDateTime.Now.Second, ":", SystemDateTime.Now.Millisecond, "]", message.ToString() }));
                _Log(LogType.Warning, message);
            }
        }

        public static void LogWarning(object message, params object[] args)
        {
            if (isDebug)
            {
                //UnityDebug.LogWarning(string.Format(string.Concat(new object[] { "[", SystemDateTime.Now.Second, ":", SystemDateTime.Now.Millisecond, "]", message }), args));
                _Log(LogType.Warning, message, args);
            }
        }

        public static void LogError(object message)
        {
            if (isDebug)
            {
                //UnityDebug.LogError(string.Concat(new object[] { "[", SystemDateTime.Now.Second, ":", SystemDateTime.Now.Millisecond, "]", message }));
                _Log(LogType.Error, message);
            }
        }

        public static void LogError(object message, params object[] args)
        {
            if (isDebug)
            {
                //UnityDebug.LogError(string.Format(string.Concat(new object[] { "[", SystemDateTime.Now.Second, ":", SystemDateTime.Now.Millisecond, "]", message }), args));
                _Log(LogType.Error, message, args);
            }
        }

        private static void _Log(LogType type, object message, params object[] args)
        {
//             StackTrace stackTrace = new StackTrace(true);
//             var stackFrame = stackTrace.GetFrame(stackTrace.FrameCount - 1);
// #if UNITY_EDITOR
//             s_LogStackFrameList.Add(stackFrame);
// #endif

            string msg = SystemDateTime.Now.ToString("<HH:mm:ss.fff>=>");
            msg = string.Concat(msg, message);
            if (args.Length > 0)
            {
                msg = string.Format(msg, args);
            }
            switch (type)
            {
                case LogType.Log:
                    UnityDebug.Log(msg);
                    break;
                case LogType.Warning:
                    UnityDebug.LogWarning(msg);
                    break;
                case LogType.Error:
                    UnityDebug.LogError(msg);
                    break;
                case LogType.Exception:
                    if (message == null)
                    {
                        throw new System.Exception();
                    }
                    else
                    {
                        throw new System.Exception(msg);
                    }
            }
        }

// #if UNITY_EDITOR
//         private static string scriptAssetPath = "Assets/Scripts/Engine/Common/Debuger.cs";
//         private static int s_InstanceID;
//         private static List<int> s_Lines = new List<int> { 137, 140, 143, 148, 152 };
//         private static List<StackFrame> s_LogStackFrameList = new List<StackFrame>();
//         // Console窗口
//         private static object s_ConsoleWindow;
//         private static object s_LogListView;
//         private static FieldInfo s_LogListViewTotalRows;
//         private static FieldInfo s_LogListViewCurrentRow;
//         //LogEntry
//         private static MethodInfo s_LogEntriesGetEntry;
//         private static object s_LogEntry;
//         //instanceId 非UnityEngine.Object的运行时 InstanceID 为零所以只能用 LogEntry.Condition 判断
//         private static FieldInfo s_LogEntryInstanceId;
//         private static FieldInfo s_LogEntryLine;
//         private static FieldInfo s_LogEntryCondition;

//         static Debuger()
//         {
//             s_InstanceID = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptAssetPath).GetInstanceID();
//             UnityEngine.Debug.LogFormat("s_InstanceID:{0}", s_InstanceID);
//             s_LogStackFrameList.Clear();

//             GetConsoleWindowListView();
//         }

//         private static void GetConsoleWindowListView()
//         {
//             if (s_LogListView == null)
//             {
//                 Assembly unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
//                 Type consoleWindowType = unityEditorAssembly.GetType("UnityEditor.ConsoleWindow");
//                 FieldInfo fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
//                 s_ConsoleWindow = fieldInfo.GetValue(null);
//                 FieldInfo listViewFieldInfo = consoleWindowType.GetField("m_ListView", BindingFlags.Instance | BindingFlags.NonPublic);
//                 s_LogListView = listViewFieldInfo.GetValue(s_ConsoleWindow);
//                 s_LogListViewTotalRows = listViewFieldInfo.FieldType.GetField("totalRows", BindingFlags.Instance | BindingFlags.Public);
//                 s_LogListViewCurrentRow = listViewFieldInfo.FieldType.GetField("row", BindingFlags.Instance | BindingFlags.Public);
//                 //LogEntries
//                 Type logEntriesType = unityEditorAssembly.GetType("UnityEditorInternal.LogEntries");
//                 s_LogEntriesGetEntry = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);
//                 Type logEntryType = unityEditorAssembly.GetType("UnityEditorInternal.LogEntry");
//                 s_LogEntry = Activator.CreateInstance(logEntryType);
//                 s_LogEntryInstanceId = logEntryType.GetField("instanceID", BindingFlags.Instance | BindingFlags.Public);
//                 s_LogEntryLine = logEntryType.GetField("line", BindingFlags.Instance | BindingFlags.Public);
//                 s_LogEntryCondition = logEntryType.GetField("condition", BindingFlags.Instance | BindingFlags.Public);
//             }
//         }

//         /// <summary>
//         /// 返回文件名和行号
//         /// </summary>
//         /// <param name="line"></param>
//         /// <returns></returns>
//         private static string GetListViewRowCount(ref int line)
//         {
//             GetConsoleWindowListView();
//             if (s_LogListView == null)
//                 return null;
//             else
//             {
//                 // 获取console中log的条数
//                 int totalRows = (int)s_LogListViewTotalRows.GetValue(s_LogListView);
//                 // 当前log的所有index号
//                 int row = (int)s_LogListViewCurrentRow.GetValue(s_LogListView);
//                 int logByThisClassCount = 0;
//                 for (int i = totalRows - 1; i >= row; i--)
//                 {
//                     s_LogEntriesGetEntry.Invoke(null, new object[] { i, s_LogEntry });
//                     string tempCondition = s_LogEntryCondition.GetValue(s_LogEntry) as string;
//                     //UnityEngine.Debug.LogFormat("i = {0} tempCondition = {1}", i, tempCondition);
//                     //判断是否是由Loger打印的日志
//                     if (tempCondition.Contains(">=>"))
//                         logByThisClassCount++;
//                 }
//                 //UnityEngine.Debug.LogFormat("logByThisClassCount = {0}", logByThisClassCount);

//                 //同步日志列表，ConsoleWindow 点击Clear 会清理
//                 while (s_LogStackFrameList.Count > totalRows)
//                 {
//                     s_LogStackFrameList.RemoveAt(0);
//                 }
//                 if (s_LogStackFrameList.Count >= logByThisClassCount)
//                 {
//                     //return s_LogStackFrameList[s_LogStackFrameList.Count - logByThisClassCount];

//                     s_LogEntriesGetEntry.Invoke(null, new object[] { row, s_LogEntry });
//                     string condition = s_LogEntryCondition.GetValue(s_LogEntry) as string;
//                     condition = condition.Substring(0, condition.IndexOf('\n'));
//                     //UnityEngine.Debug.LogFormat("condition ={0}", condition);
//                     int index = condition.IndexOf(".lua:", StringComparison.Ordinal);

//                     if (index >= 0)
//                     {
//                         // 是lua中的print调用过来的
//                         int start = condition.IndexOf("[", StringComparison.Ordinal);
//                         int end = condition.IndexOf("]:", StringComparison.Ordinal);
//                         string sub = condition.Substring(start +1, end - start-1);
//                         //UnityEngine.Debug.LogFormat("sub ={0}", sub);
//                         string[] strArray = sub.Split(':');
//                         string fileName = strArray[0];
//                         Int32.TryParse(strArray[1], out line);
//                         //UnityEngine.Debug.LogFormat("fileName ={0}, line= {1}", fileName, line);
//                         string filter = fileName.Substring(0, fileName.Length - 4);
//                         index = filter.LastIndexOf("/", StringComparison.Ordinal);
//                         if (index > 0)
//                         {
//                             filter = filter.Substring(index + 1, filter.Length - 1 - index);
//                         }
//                         //UnityEngine.Debug.LogFormat("filter ={0}", filter);
//                         string[] searchPaths = AssetDatabase.FindAssets(filter);
//                         for (int i = 0; i < searchPaths.Length; i++)
//                         {
//                             string path = AssetDatabase.GUIDToAssetPath(searchPaths[i]);
//                             //UnityEngine.Debug.LogFormat("path ={0}", path);
//                             if (path.EndsWith(fileName, StringComparison.Ordinal) ||
//                                 path.EndsWith(fileName + ".lua", StringComparison.Ordinal))
//                             {
//                                 return path;
//                             }
//                         }
//                     }
//                     else
//                     {
//                         // 一般debuger方法的调用方式
//                         var stackFrame = s_LogStackFrameList[s_LogStackFrameList.Count - logByThisClassCount];
//                         //UnityEngine.Debug.LogFormat("stackFrame.GetFileName ={0}，stackFrame.GetFileLineNumber = {1}", stackFrame.GetFileName(), stackFrame.GetFileLineNumber());
//                         string path = stackFrame.GetFileName();
//                         line = stackFrame.GetFileLineNumber();
//                         path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
//                         return path;
//                     }

//                 }
//                 return null;
//             }
//         }

//         [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
//         public static bool OnOpenAsset(int instanceID, int line)
//         {
//             //UnityEngine.Debug.LogFormat("instanceID:{0}，line{1}", instanceID, line);
//             if (instanceID == s_InstanceID && s_Lines.Contains(line))
//             {
//                 int newLine = 0;
//                 var assetPath = GetListViewRowCount(ref newLine);
//                 if (!string.IsNullOrEmpty(assetPath))
//                 {
//                     //UnityEngine.Debug.LogFormat("assetPath ={0}, newLine = {1}", assetPath, newLine);
//                     //AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath), newLine);
//                     if (assetPath.EndsWith(".cs", StringComparison.Ordinal))
//                     {
//                         AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath), newLine);
//                     }
//                     else
//                     {
//                         AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(assetPath), newLine);
//                     }
//                     return true;
//                 }
//             }

//             return false;
//         }
// #endif
    }
}