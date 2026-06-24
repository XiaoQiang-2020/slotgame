


using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class CommonTool
    {
        /// <summary>
        ///  是否需要转屏
        /// </summary>
        private static bool m_needTurn = true;

        /// <summary>
        /// 是否向左转
        /// </summary>
        private static bool m_isTurnLeft = true;


        /// <summary>
        /// 设置屏幕状态
        /// </summary>
        /// <param name="isNeedTurn">是否需要旋转</param>
        /// <param name="isTurnLeft">是否向左转</param>
        public static void SetScreenState(bool isNeedTurn, bool isTurnLeft)
        {
            m_needTurn = isNeedTurn;
            m_isTurnLeft = isTurnLeft;
        }

        public static int Int(object o)
        {
            return Convert.ToInt32(o);
        }

        public static float Float(object o)
        {
            return (float)Math.Round(Convert.ToSingle(o), 2);
        }

        public static long Long(object o)
        {
            return Convert.ToInt64(o);
        }

        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }


        public static long GetTime()
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 搜索子物体组件-GameObject版
        /// </summary>
        public static T Get<T>(GameObject go, string subnode) where T : Component
        {
            if (go != null)
            {
                Transform sub = go.transform.Find(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件-Transform版
        /// </summary>
        public static T Get<T>(Transform go, string subnode) where T : Component
        {
            if (go != null)
            {
                Transform sub = go.Find(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件-Component版
        /// </summary>
        public static T Get<T>(Component go, string subnode) where T : Component
        {
            return go.transform.Find(subnode).GetComponent<T>();
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        public static GameObject Child(GameObject go, string subnode)
        {
            return Child(go.transform, subnode);
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        public static GameObject Child(Transform go, string subnode)
        {
            Transform tran = go.Find(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(GameObject go, string subnode)
        {
            return Peer(go.transform, subnode);
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(Transform go, string subnode)
        {
            Transform tran = go.parent.Find(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string md5(string source)
        {
            MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Data = _md5.ComputeHash(data, 0, data.Length);
            _md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string md5file(string file)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 _md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = _md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                //throw new Exception("md5file() fail, error:" + ex.Message);
                Debug.LogError("GetMd5HashFromFile fail,error: " + ex.Message);
            }
            return "";
        }

        /// <summary>
        /// 清除所有子节点
        /// </summary>
        public static void ClearChild(Transform go)
        {
            if (go == null) return;
            for (int i = go.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(go.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 取得文本
        /// </summary>
        public static string GetFileText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }

        /// <summary>
        /// 防止初学者不按步骤来操作
        /// </summary>
        /// <returns></returns>
        public static int CheckRuntimeFile()
        {
            if (!Application.isEditor) return 0;
            string streamDir = Application.dataPath + "/StreamingAssets/";
            if (!Directory.Exists(streamDir))
            {
                return -1;
            }
            else
            {
                string[] files = Directory.GetFiles(streamDir);
                if (files.Length == 0)
                {
                    return -1;
                }

                if (!File.Exists(streamDir + "files.txt"))
                {
                    return -1;
                }
            }

            string sourceDir = UnityEngine.Application.dataPath + "/LuaFramework/ToLua/Source/Generate/";
            if (!Directory.Exists(sourceDir))
            {
                return -2;
            }
            else
            {
                string[] files = Directory.GetFiles(sourceDir);
                if (files.Length == 0) return -2;
            }
            return 0;
        }



        /// <summary>
        /// 检查运行环境
        /// </summary>
        public static bool CheckEnvironment()
        {
#if UNITY_EDITOR
            int resultId = CommonTool.CheckRuntimeFile();
            if (resultId == -1)
            {
                Debug.LogError("没有找到框架所需要的资源，单击Game菜单下Build xxx Resource生成！！");
                EditorApplication.isPlaying = false;
                return false;
            }
            else if (resultId == -2)
            {
                Debug.LogError("没有找到Wrap脚本缓存，单击Lua菜单下Gen Lua Wrap Files生成脚本！！");
                EditorApplication.isPlaying = false;
                return false;
            }
            if (SceneManager.GetActiveScene().name == "Test" && !Game.GlobalVar.IS_RES_MODE_DEBUG)
            {
                Debug.LogError("测试场景，必须打开调试模式，Game.GlobalVar.RES_DEBUG_MODE = true！！");
                EditorApplication.isPlaying = false;
                return false;
            }
#endif
            return true;
        }

        /// <summary>
        /// 获取资源更新地址
        /// </summary>
        /// <returns></returns>
        public static string GetResourceUpdateUrl()
        {
            //#if UNITY_EDITOR
            //            string url = UnityEngine.Application.dataPath + "/../../output_res/win/" + Game.GlobalVar.RES_ROOT;
            //            Game.FileUtils.fixedPath(ref url);
            //            return new System.Uri(url).AbsoluteUri;
            //#else
            //            return GlobalVar.RES_SERVER_ADDR + "/" + Game.GlobalVar.RES_ROOT;
            //#endif

            // return GlobalVar.RES_SERVER_ADDR + "/" + Game.GlobalVar.RES_ROOT;
            return GlobalVar.RES_SERVER_URL + "/" + Game.GlobalVar.osDir + "/" + Game.GlobalVar.RES_ROOT;
        }

        /// <summary>
        /// 简单截屏的方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator CaptureScreenshot(string path, System.Action<bool, string> callback)
        {
            // 路径处理
            // 注：path是一个在Application.persistentDataPath下的全路径
            // Application.persistentDataPath这个方法的参数：
            //      在pc呆传全路径
            //      在android和ios上可能传Application.persistentDataPath下的相关路径

            string newPath = path;
#if !UNITY_EDITOR
            newPath = path.Replace(Application.persistentDataPath + "/", "");
#endif
            Debug.Log("CaptureScreenshot path: " + path);
            Debug.Log("CaptureScreenshot newPath: " + newPath);
            ScreenCapture.CaptureScreenshot(newPath);
            float time = Time.time;
            bool b = false;
            yield return new WaitUntil(() =>
            {
                b = System.IO.File.Exists(path);
                float offset = Time.time - time;
                Debug.Log("b = " + b + "  offset = " + offset);
                return b || (offset > 5f);
            });

            //bool b = true;
            //bool has = false;
            //while (b)
            //{
            //    has = System.IO.File.Exists(path);
            //    float offset = Time.time - time;
            //    Debug.Log("has = " + has + "  offset = " + offset);
            //    if (has || offset>10f)
            //    {
            //        b = false;
            //        break;
            //    }
            //    yield return 0;
            //}

            string str = path;
            if (b == false)
            {
                str = "截屏出错！";
            }
            if (callback != null)
            {
                callback(b, str);
            }
        }

        /// <summary>
        /// 合并两个texture
        /// </summary>
        /// <param name="bigTexture">大图</param>
        /// <param name="smallTexture">小图</param>
        /// <param name="targetPos">小图合到大图中的位置</param>
        /// <param name="targetSize">小图拿到大图中的大小</param>
        public static bool MergeTwoTexture(Texture2D bigTexture, Texture2D smallTexture, Vector2 targetPos, Vector2 targetSize, string savePath)
        {
            if (bigTexture == null || smallTexture == null)
            {
                Debug.LogError("传入的纹理为空");
                return false;
            }
            try
            {
                int targetSizeW = (int)targetSize.x;
                int targetSizeH = (int)targetSize.y;
                int targetPosX = (int)targetPos.x;
                int targetPosY = (int)targetPos.y;
                Vector2 bigSize = new Vector2(bigTexture.width, bigTexture.height);
                Vector2 smallSize = new Vector2(smallTexture.width, smallTexture.height);
                int halfWidthOfSmall = smallTexture.width / 2;
                int halfHeightOfSmall = smallTexture.height / 2;
                int halfWidthOfTarget = targetSizeW / 2;
                int halfHeightOfTarget = targetSizeH / 2;
                float scaleW = smallSize.x / targetSize.x;
                float scaleH = smallSize.y / targetSize.y;
                Color color = Color.white;
                int offsetX = 0;
                int offsetY = 0;
                //Texture2D texture = new Texture2D(bigTexture.width, bigTexture.height);
                for (int x = targetPosX - halfWidthOfTarget; x <= targetPosX + halfWidthOfTarget; x++)
                {
                    for (int y = targetPosY - halfHeightOfTarget; y <= targetPosY + halfHeightOfTarget; y++)
                    {
                        if (x >= targetPosX - halfWidthOfTarget && y >= targetPosY - halfHeightOfTarget && x <= targetPosX + halfWidthOfTarget && y <= targetPosY + halfHeightOfTarget)
                        {
                            //Debug.LogError("xxx1 = " + x);
                            offsetX = x - targetPosX;
                            offsetY = y - targetPosY;
                            //Debug.LogError("xxx2 = " + x);
                            color = smallTexture.GetPixel((int)(scaleW * offsetX + halfWidthOfSmall), (int)(offsetY * scaleH + halfHeightOfSmall));
                            //Debug.LogError("xxx3 = " + x);
                            bigTexture.SetPixel(x, y, color);
                            //Debug.LogError("xxx4 = " + x);
                        }
                        //else
                        //{
                        //    texture.SetPixel(x, y, bigTexture.GetPixel(x,y));
                        //}
                    }
                    //Debug.LogError("x = " + x);
                }
                //Debug.LogError("000000");
                bigTexture.Apply();
                //Debug.LogError("111111");
                byte[] bytes;   // = bigTexture.EncodeToPNG();
                string ext = System.IO.Path.GetExtension(savePath);
                //Debugger.Log(string.Format("ext = {0}", ext));
                if (ext.ToLower() == ".png")
                {
                    bytes = bigTexture.EncodeToPNG();
                    //Debug.LogError("png");
                }
                else if (ext.ToLower() == ".jpg")
                {
                    bytes = bigTexture.EncodeToJPG();
                    //Debug.LogError("jpg");
                }
                else
                {
                    bytes = bigTexture.EncodeToEXR();
                    //Debug.LogError("EXR");
                }
                System.IO.File.WriteAllBytes(savePath, bytes);
                UnityEngine.Object.DestroyImmediate(bigTexture, true);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("异常：" + e.Message);
                return false;
            }
        }
        public static IEnumerator CaptureScreenRect(string fileName, Rect rect, int baseImageWidth, int baseImageHeight, bool isNeedTurn, System.Action<Texture2D, string> callback = null)
        {
            yield return new WaitForEndOfFrame();
            string imgPath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            rect.width = rect.width == 0 ? Screen.width : rect.width;
            rect.height = rect.height == 0 ? Screen.height : rect.height;
            Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            tex.ReadPixels(rect, 0, 0, false);

            tex.Apply();
            float scale = 1f;
            float fwidth = (float) rect.width;
            float fheight = (float)rect.height;

            if (fwidth > fheight && fwidth > (float)baseImageWidth)
            {   //如果宽度大的话根据宽度固定大小缩放 
                scale = (float)baseImageWidth / fwidth;
            }
            else if (fwidth < fheight && fheight > (float)baseImageHeight)
            {   //如果高度高的话根据宽度固定大小缩放 
                scale = (float)baseImageHeight / fheight;
            }
            Texture2D resizeTexture = ReSetTextureSize(tex, (int)(fwidth * scale), (int)(fheight * scale));
            if (isNeedTurn)
            {
                resizeTexture = RotateTexture2D(resizeTexture);                
            }
            else
            {
                resizeTexture.Apply();
            }

            byte[] bytes = resizeTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(imgPath, bytes);
            Debug.Log(string.Format("截屏了一张照片: {0}", imgPath));
            yield return new WaitForSeconds(0.005f);
            if (null != callback)
            {
                callback(tex, imgPath);
            }
        }
        public static IEnumerator CaptureCamera(string fileName, Camera camera, int renderwidth, int renderheight, Rect rect,
         int baseImageWidth, int baseImageHeight, System.Action<Texture2D, string> callback = null)
        {

            string imgPath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            // 创建一个RenderTexture对象  
            RenderTexture rt = new RenderTexture((int)renderwidth, renderheight, 0);
            // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
            camera.targetTexture = rt;
            camera.Render();
            // 激活这个rt, 并从中中读取像素。  
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
            screenShot.Apply();

            // 重置相关参数，以使用camera继续在屏幕上显示  
            camera.targetTexture = null;
            //ps: camera2.targetTexture = null;  
            RenderTexture.active = null; // JC: added to avoid errors  
            GameObject.Destroy(rt);
            // 最后将这些纹理数据，成一个png图片文件 
            float scale = 1f;
            float fwidth = (float)renderwidth;
            float fheight = (float)renderheight;

            if (fwidth > fheight && fwidth > (float)baseImageWidth)
            {   //如果宽度大的话根据宽度固定大小缩放 
                scale = (float)baseImageWidth / fwidth;
            }
            else if (fwidth < fheight && fheight > (float)baseImageHeight)
            {   //如果高度高的话根据宽度固定大小缩放 
                scale = (float)baseImageHeight / fheight;
            }

            Texture2D resizeTexture = ReSetTextureSize(screenShot, (int)(fwidth * scale), (int)(fheight * scale));
            if (m_needTurn)
            {
                resizeTexture = RotateTexture2D(resizeTexture);
            }
            else
            {
                resizeTexture.Apply();
            }
            
            byte[] bytes = resizeTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(imgPath, bytes);
            Debug.Log(string.Format("截屏了一张照片: {0}", imgPath));

            yield return new WaitForSeconds(0.005f);
            if (null != callback)
            {
                callback(resizeTexture, imgPath);
            }
        }
        public static Texture2D ReSetTextureSize(Texture2D tex, int width, int height)
        {
            var rendTex = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            rendTex.Create();
            Graphics.SetRenderTarget(rendTex);
            GL.PushMatrix();
            GL.Clear(true, true, Color.clear);
            GL.PopMatrix();
            var mat = new Material(Shader.Find("Unlit/Transparent"));
            mat.mainTexture = tex;
            Graphics.SetRenderTarget(rendTex);
            GL.PushMatrix();
            GL.LoadOrtho();
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.TexCoord2(0, 0);
            GL.Vertex3(0, 0, 0);
            GL.TexCoord2(0, 1);
            GL.Vertex3(0, 1, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex3(1, 1, 0);
            GL.TexCoord2(1, 0);
            GL.Vertex3(1, 0, 0);
            GL.End();
            GL.PopMatrix();
            var finalTex = new Texture2D(rendTex.width, rendTex.height, TextureFormat.ARGB32, false);
            RenderTexture.active = rendTex;
            finalTex.ReadPixels(new Rect(0, 0, finalTex.width, finalTex.height), 0, 0);
            //finalTex.Apply();
            return finalTex;
        }

        /// <summary>
        /// 对纹理进行旋转
        /// </summary>
        /// <param name="verticalPic">竖屏的纹理</param>
        /// <returns>横屏的纹理</returns>
        public static Texture2D RotateTexture2D(Texture2D verticalPic)
        {
            if (null == verticalPic)
            {
                return null;
            }

            int oldWidth = verticalPic.width;
            int oldHeight = verticalPic.height;
            Color onePoint = Color.white;
            Texture2D horticalPic = new Texture2D(oldHeight, oldWidth);
            for (int i = 0; i < oldWidth - 1; i++)
            {
                for (int j = 0; j < oldHeight - 1; j++)
                {
                    onePoint = verticalPic.GetPixel(i, j);
                    if (m_isTurnLeft)
                    {
                        horticalPic.SetPixel(oldHeight - 1 - j, i, onePoint);
                    }
                    else
                    {
                        horticalPic.SetPixel(j, oldWidth - 1 - i, onePoint);
                    }
                    
                }
            }
            horticalPic.Apply();

            return horticalPic;
        }

        /// <summary>
        /// string to UTF8 byte
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string strContent)
        {
            if (null == strContent)
            {
                return null;
            }
            return System.Text.Encoding.UTF8.GetBytes(strContent);
        }

        /// <summary>
        /// StringToDictionary 目前仅支持一层级简单的
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public static Dictionary<string, string> StringToDictionary(string strContent)
        {
            if (null == strContent)
            {
                return null;
            }
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                LitJson.JsonData jsData = LitJson.JsonMapper.ToObject(strContent);
                foreach (string key in jsData)
                {
                    dict[key] = jsData[key].ToString();
                }
                return dict;
            }
            catch (System.Exception ex)
            {
                Core.Debuger.LogError(ex.Message);
            }
            return null;
        }

    }
}