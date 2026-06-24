
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using SystemEncoding = System.Text.Encoding;


namespace Game
{
    public enum BuildPlatform
    {
        WebGL,
        Standalones,
        IOS,
        Android,
        WP8,
        uwp
    }

    public class FileUtils
    {
        static FileUtils _instance;
        private List<string> _searchPathArray = new List<string>();
        // 查找过的文件名，已经验证过是存在的。key是名，value是完成的路径
        private Dictionary<string, string> _pathCache = new Dictionary<string, string>();
        static public FileUtils getInstance()
        {
            if (_instance == null)
            {
                _instance = new FileUtils();
                if (Game.GlobalVar.IS_RES_MODE_DEBUG)
                {
                    _instance.addSearchPath(Application.dataPath + "/" + Game.GlobalVar.RES_ROOT);
                }
                else
                {
                    _instance.addSearchPath(Application.persistentDataPath + "/" + Game.GlobalVar.RES_ROOT);
                    _instance.addSearchPath(Application.streamingAssetsPath + "/" + Game.GlobalVar.RES_ROOT);
                }
            }
            return _instance;
        }
        static public void destroyInstance()
        {
            if (_instance != null)
            {
                _instance._searchPathArray.Clear();
                _instance._pathCache.Clear();
                _instance = null;
            }
        }

        public void ClearCache()
        {
            _pathCache.Clear();
        }

        public List<string> getSearchPaths()
        {
            return _searchPathArray;
        }

        public void setSearchPaths(List<string> searchPaths)
        {
            _searchPathArray = searchPaths;
        }

        public void addSearchPath(string path)
        {
            addSearchPath(path, false);
        }

        public void addSearchPath(string path, bool front)
        {
            fixedPath(ref path);
            if (front)
            {
                var index = _searchPathArray.IndexOf(path);
                if (index == -1)
                    _searchPathArray.Insert(0, path);
                else if (index > 0)
                {
                    _searchPathArray.Remove(path);
                    _searchPathArray.Insert(0, path);
                }
            }
            else
            {
                var index = _searchPathArray.IndexOf(path);
                if (index == -1)
                    _searchPathArray.Add(path);
                else if (index > _searchPathArray.Count - 1)
                {
                    _searchPathArray.Remove(path);
                    _searchPathArray.Add(path);
                }
            }
        }

        public void removeSearchPath(string path)
        {
            int index = _searchPathArray.IndexOf(path);

            if (index >= 0)
            {
                _searchPathArray.RemoveAt(index);
            }
        }

        /// <summary>
        /// 可写目录
        /// </summary>
        /// <returns></returns>
        public string getWritablePath()
        {
            return pathCombine(Application.persistentDataPath, Game.GlobalVar.RES_ROOT);  //UnityEngine.Application.persistentDataPath + "/root/";
        }

        /// <summary>
        /// 获得文件存在的路径，目录从_searchPathArray中查找。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string getFullPath(string fileName)
        {
            if (_pathCache.ContainsKey(fileName)) return _pathCache[fileName];
            if (isFileExist(fileName))
            {
                _pathCache.Add(fileName, fileName);
                return fileName;
            }
            for (int i = 0; i < _searchPathArray.Count; i++)
            {
                string path = _searchPathArray[i];
                if (isRoot(path, fileName))
                    continue;
                fixedPath(ref path);
                //var p = path + fileName;
                string p = pathCombine(path, fileName);
                if (isFileExist(p))
                {
                    _pathCache.Add(fileName, p);
                    return p;
                }
            }
            return "";
        }

        public string getAssetBundleFilePath(string path)
        {
            return getFullPath(Game.GlobalVar.RES_ROOT + "/" + Game.GlobalVar.RES_ASSETBUNDLE_ROOT + "/" + path);
        }

        public AssetBundle getAssetBundleFromMemory(string path)
        {
            var p = getAssetBundleFilePath(path);
            if (string.IsNullOrEmpty(p)) return null;
            byte[] bytes = getBytes(p);
            if (bytes != null && bytes.Length > 0)
            {
                try
                {
                    return AssetBundle.LoadFromMemory(bytes);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString() + "-->" + path);
                }
            }
            return null;

        }

        public AssetBundleCreateRequest getAssetBundleFromMemoryAsync(string path)
        {
            path = path.ToLower();
            var p = getAssetBundleFilePath(path);
            if (string.IsNullOrEmpty(p)) return null;
            byte[] bytes = getBytes(p);
            if (bytes != null && bytes.Length > 0)
            {
                return AssetBundle.LoadFromMemoryAsync(bytes);
            }
            return null;
        }



        public static string GetMd5HashFromFile(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                    return "";
                FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError("GetMd5HashFromFile fail,error: " + e.Message);
            }
            return "";
        }

        /// <summary>
        /// 存储texture2d成本场文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color32"></param>
        /// <returns></returns>
        public static Texture2D writeTexture2D(string path, int width, int height, Color32[] color32)
        {
            var tx = new Texture2D(width, height);
            tx.SetPixels32(color32);
            tx.Apply();
            writeTexture2D(path, tx);
            return tx;
        }

        /// <summary>
        /// 保存图片到目录
        /// </summary>
        /// <param name="path">.jpg|.png</param>
        /// <param name="tx"></param>
        /// <returns>bool</returns>
        public static bool writeTexture2D(string path, Texture2D tx)
        {
            if (Path.GetExtension(path) == ".jpg")
            {
                return writeBytes(path, tx.EncodeToJPG());
            }
            else if (Path.GetExtension(path) == ".png")
            {
                return writeBytes(path, tx.EncodeToPNG());
            }
            else
            {
                throw new Exception("saving path is not jpg|png.");
            }
        }


        /// <summary>
        /// 从文件读字符串
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string getString(string fileName)
        {
            if (!isFileExist(fileName))
            {
                return null;
            }
            return File.ReadAllText(fileName);
        }

        public static string getString(string path, string fileName)
        {
            return getString(pathCombine(path, fileName));
        }
        public byte[] getBytes(string path, string fileName)
        {
            return getBytes(pathCombine(path, fileName));
        }

        /// <summary>
        /// 从文件读二进制
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] getBytes(string fileName)
        {
            if (!isFileExist(fileName))
            {
                return null;
            }
            return File.ReadAllBytes(fileName);
        }

      
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool isFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public string getFullPathForWww(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            var url = getFullPath(path);
            //if (path == url && url.IndexOf(Application.streamingAssetsPath, StringComparison.Ordinal) == -1)
            //{
            //    url = pathCombine(Application.streamingAssetsPath, path);
            //}
            //Debug.LogFormat("path = {0}, full path = {1}", path, url);
            System.Uri u = new System.Uri(url);
            return u.AbsoluteUri;
        }

        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool isDirectoryExist(string dir)
        {
            return Directory.Exists(dir);
        }

        /// <summary>
        /// 移动文件
        /// 将文件src改成target
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool Move(string src, string target)
        {
            try
            {
                if (isFileExist(target))
                {
                    removeFile(target);
                }
                string path = Path.GetDirectoryName(target);
                createDirectory(path);
                File.Move(src, target);
                return true;

            }
            catch (IOException e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="oldFile">旧文件名</param>
        /// <param name="newFile">新文件名</param>
        /// <returns></returns>
        public bool renameFile(string path, string oldFile, string newFile)
        {
            string _old = pathCombine(path, oldFile);   //path + oldFile;
            string _new = pathCombine(path, newFile);   //path + newFile;
            try
            {
                if (isFileExist(_old))
                {
                    removeFile(_new);
                }
                File.Move(_old, _new);
                return true;
            }
            catch (IOException e)
            {
                Debug.LogError(e.ToString());
            }
            Debug.LogError("can't found " + _old);
            return false;
        }

        public void movePath(string oldPath, string newPath)
        {
            ForEachDirectory(oldPath, (path) =>
            {
                var p = path.Replace(oldPath, newPath);
                var dir = Path.GetDirectoryName(newPath);
                createDirectory(dir);
                File.Move(path, p);
            });
        }

        public Int64 getLastWriteTime(string path)
        {
            path = getFullPath(path);
            if (string.IsNullOrEmpty(path))
                return 0;
            FileInfo fi = new FileInfo(path);
            return fi.LastWriteTime.Ticks;
        }
        public Texture2D getTexture2DMarkReadable(string path, bool markReadable)
        {
            path = getFullPath(path);
            if (string.IsNullOrEmpty(path) && !File.Exists(path))
                return null;
            var mData = getBytes(path);

            // loadimage有一定的机率失败，所以改成尝试3次
            int count = 3;
            int time = 0;
            Texture2D tx = new Texture2D(4, 4);
            while(time < count)
            {
                bool b = tx.LoadImage(mData, markReadable);
                Core.Debuger.Log(string.Format("getTexture2D: time:{0} b:{1}", time,b));
                if (tx.width > 4)
                {
                    return tx;
                }
                time++;
            }
            UnityEngine.Object.Destroy(tx);
            return null;
        }

        public Texture2D getTexture2D(string path)
        {
            return getTexture2DMarkReadable(path, true);
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>字符串</returns>
        public string GetStringFromFile(string filePath)
        {
            filePath = getFullPath(filePath);
            if (string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
            {
                return null;
            }
            var mData = getBytes(filePath);
            string str = SystemEncoding.UTF8.GetString(mData);
            return str;
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool removeDirectory(string dir)
        {
            if (isDirectoryExist(dir))
            {
                Directory.Delete(dir, true);
                return true;
            }
            return false;
        }
        public static bool removeFile(string file)
        {
            if (isFileExist(file))
            {
#if !UNITY_EDITOR
            if (file.IndexOf(Application.streamingAssetsPath) == -1)
#endif
                {
                    File.Delete(file);
                    return true;
                }
            }
            return false;
        }



        public static bool writeFileWithCode(string filepath, string data, Encoding code)
        {
            try
            {
                string path = Path.GetDirectoryName(filepath);
                createDirectory(path);

                if (code != null)
                {
                    File.WriteAllText(filepath, data, code);
                }
                else
                {
                    File.WriteAllText(filepath, data);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("writeFIle fail. " + filepath);
                throw e;
            }
        }
        public static bool writeString(string filepath, string data)
        {
            var utf8withoutBom = new System.Text.UTF8Encoding(false);
            return writeFileWithCode(filepath, data, utf8withoutBom);
        }

        public static bool writeBytes(string filePath, byte[] bytes)
        {
            try
            {
                string path = Path.GetDirectoryName(filePath);
                createDirectory(path);

                File.WriteAllBytes(filePath, bytes);
                return true;
            }
            catch (IOException e)
            {
                Debug.LogError("writeFIle fail. " + filePath);
                throw e;
            }
        }
        private void Write(FileStream fs, byte[] data)
        {
            fs.Write(data, 0, data.Length);
        }
        public bool writeFileStream(string path, List<byte[]> dataes)
        {
            createDirectory(Path.GetDirectoryName(path));
            using (FileStream fs = new FileStream(path, System.IO.FileMode.Append))
            {
                for (int i = 0; i < dataes.Count; ++i)
                    Write(fs, dataes[i]);
            }
            return true;
        }
        public bool writeFileStream(string dir, string filename, List<byte[]> dataes)
        {
            return writeFileStream(Path.Combine(dir, filename), dataes);
        }

        public static void createDirectory(string path)
        {
            if (!isDirectoryExist(path))
                Directory.CreateDirectory(path);
        }

        public void clearPath(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                return;
            }
            FileInfo[] files = info.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                files[i].Delete();
            }
            DirectoryInfo[] diries = info.GetDirectories();
            for (int j = 0; j < diries.Length; j++)
            {
                diries[j].Delete(true);
            }
        }

        /// <summary>
        /// 遍历文件夹下所有文件。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callBack"></param>
        public void ForEachDirectory(string path, Action<string> callBack)
        {
            ForEachDirectory(path, "*", callBack);
        }

        /// <summary>
        /// 遍历文件夹下所有文件。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="callBack"></param>
        public void ForEachDirectory(string path, string searchPattern, Action<string> callBack)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                return;
            }
            FileInfo[] files = info.GetFiles(searchPattern, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                callBack(files[i].FullName);
            }

        }

        /// <summary>
        /// 获得目录下所有文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> getAllFileInPath(string path)
        {
            return getAllFileInPathWithSearchPattern(path, null);
        }

        /// <summary>
        /// 获得目录下所有后缀为{searchPattern}的文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public List<string> getAllFileInPathWithSearchPattern(string path, string searchPattern)
        {
            List<string> list = new List<string>();
            ForEachDirectory(path, searchPattern, (string file) =>
            {
                list.Add(file);
            });

            return list;
        }

        /// <summary>
        /// 文件名fileName，是否已经包含这个path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool isRoot(string path, string fileName)
        {
            bool ret = false;
            if (Path.GetDirectoryName(fileName).IndexOf(path, StringComparison.Ordinal) > -1)
            {
                ret = true;
            }
            return ret;
        }

        public static void fixedPath(ref string path)
        {
            path = path.Replace('\\', '/');
            //if (!path.EndsWith("/", StringComparison.Ordinal))
            //{
            //    path = path + "/";
            //}
        }

        public static string pathCombine(string path1, string path2)
        {
            fixedPath(ref path1);
            fixedPath(ref path2);

            if (!path1.EndsWith("/", StringComparison.Ordinal))
            {
                path1 = path1 + "/";
            }
            string path = path1 + path2;
            return path;
        }

        public static string getRuntimePlatform()
        {
            string pf = "";
#if UNITY_EDITOR
            switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
            {
                case UnityEditor.BuildTarget.StandaloneLinux64:
                // case UnityEditor.BuildTarget.StandaloneLinuxUniversal‌:
                // case UnityEditor.BuildTarget.StandaloneOSXIntel:
                    pf = BuildPlatform.IOS.ToString();
                    break;
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    pf = BuildPlatform.Standalones.ToString();
                    //pf = BuildPlatform.Android.ToString();
                    break;
                case UnityEditor.BuildTarget.WebGL:
                    pf = BuildPlatform.WebGL.ToString();
                    break;
#if UNITY_5
                case UnityEditor.BuildTarget.iOS:
#else
            case UnityEditor.BuildTarget.iOS:
#endif
                    pf = BuildPlatform.IOS.ToString();
                    break;
                case UnityEditor.BuildTarget.Android:
                    pf = BuildPlatform.Android.ToString();
                    break;
                case UnityEditor.BuildTarget.WSAPlayer:
                    pf = BuildPlatform.uwp.ToString();
                    break;
                default:
                    Debug.LogError("Internal error. Bundle Manager dosn't support for platform " + UnityEditor.EditorUserBuildSettings.activeBuildTarget);
                    pf = BuildPlatform.Standalones.ToString();
                    break;
            }
#else
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXPlayer:
                pf = BuildPlatform.Standalones.ToString();
                break;
            case RuntimePlatform.WebGLPlayer:
                pf = BuildPlatform.WebGL.ToString();
                break;
            case RuntimePlatform.IPhonePlayer:
                //IOS
                pf = BuildPlatform.IOS.ToString();
                break;
            case RuntimePlatform.Android:
                //安卓
                pf = BuildPlatform.Android.ToString();
                break;

            case RuntimePlatform.WSAPlayerARM:
            case RuntimePlatform.WSAPlayerX64:
            case RuntimePlatform.WSAPlayerX86:
                //Win10
               pf = BuildPlatform.Standalones.ToString();
                break;
            default:
                Debug.LogError("Platform " + Application.platform + " is not supported by BundleManager.");
                pf = BuildPlatform.Standalones.ToString();
                break;
        }
#endif
            return pf.ToLower();

        }

        /// <summary>
        /// 加密lua文件
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static byte[] encodeLuaFile(byte[] bs)
        {
            // 第一步先异或
            byte[] keys = System.Text.Encoding.UTF8.GetBytes("i_love_you");
            for (int i = 0; i < bs.Length; i++)
            {
                bs[i] = (byte)(bs[i] ^ keys[i % keys.Length]);
            }

            // 第二步再两两交换
            int doubleNum = bs.Length / 2;
            byte temp = 0;
            int index = 0;
            for (int i = 0; i < doubleNum; i++)
            {
                index = i * 2;
                temp = bs[index];
                bs[index] = bs[index + 1];
                bs[index + 1] = temp;
            }

            // 返回
            return bs;
        }

        /// <summary>
        /// 解密lua文件
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static byte[] decodeLuaFile(byte[] bs)
        {
            // 第一步先两两交换
            int doubleNum = bs.Length / 2;
            byte temp = 0;
            int index = 0;
            for (int i = 0; i < doubleNum; i++)
            {
                index = i * 2;
                temp = bs[index];
                bs[index] = bs[index + 1];
                bs[index + 1] = temp;
            }

            // 第二步再异或
            byte[] keys = System.Text.Encoding.UTF8.GetBytes("i_love_you");
            for (int i = 0; i < bs.Length; i++)
            {
                bs[i] = (byte)(bs[i] ^ keys[i % keys.Length]);
            }

            // 返回
            return bs;
        }

        /// <summary>
        /// 复文本到剪切板，pc
        /// </summary>
        public static void CopyStringToClipBoard(string content)
        {
            GUIUtility.systemCopyBuffer = content;
        }


        /// <summary>
        /// 拷贝所有文件
        /// </summary>
        /// <param name="sourcePath">源路径</param>
        /// <param name="targetPath">目标路径</param>
        public static void CopyAllFile(string sourcePath, string targetPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sourcePath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo file in fileinfo)
                {
                    if (file is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(targetPath + "/" + file.Name))
                        {
                            Directory.CreateDirectory(targetPath + "/" + file.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyAllFile(file.FullName, targetPath + "/" + file.Name);    //递归调用复制子文件夹
                    }
                    else
                    {
                        if (file.Extension == ".meta")
                            continue;
                        File.Copy(file.FullName, targetPath + "/" + file.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (Exception)
            {
                Debug.LogError("Error!! copy files from " + sourcePath + " to " + targetPath);
                throw;
            }

        }
    }
}