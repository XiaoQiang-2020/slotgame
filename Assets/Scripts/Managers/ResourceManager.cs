/***************************************************
 * 文件名：ResourceManager.cs
 * 描  述：资源加载与管理类
 *          分为异加载和同步加载两种
 ***************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UObject = UnityEngine.Object;
using GameCoroutine = Core.Coroutine;
using UnityEngine.Networking;
namespace Game
{
    public class AssetBundleInfo
    {
        public AssetBundle m_AssetBundle;
        public int m_ReferencedCount;
        public event Action unload;

        private bool _isReplaceShader;

        public AssetBundleInfo(AssetBundle assetBundle)
        {
            m_AssetBundle = assetBundle;
            m_ReferencedCount = 0;
            //assets = new Dictionary<string, UObject>();
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void OnUnload(bool isThorough)
        {
            m_AssetBundle.Unload(isThorough);
            if (unload != null)
            {
                unload();
            }
        }

        public T LoadAsset<T>(string assetName) where T : UObject
        {
            //if(assets.ContainsKey(assetName))
            //{
            //    return (T)assets[assetName];
            //}
            T t = m_AssetBundle.LoadAsset<T>(assetName);

            //Debug.Log("LoadAsset " + assetName + t.GetHashCode());
            //#if UNITY_EDITOR
            if (t is GameObject && !_isReplaceShader) //因为现在是单个独立资源 所以判断一次isReplaceShader即可
            {
                _isReplaceShader = true;
                ReplaceMaterialsByGameObject(t as GameObject);
            }
            //#endif
            //assets.Add(assetName,t);
            return t;
        }

        //#if UNITY_EDITOR
        public static Dictionary<string, Shader> DicUsedShader = new Dictionary<string, Shader>();

        private void ReplaceMaterialsByGameObject(GameObject go)
        {
            Renderer mr = go.GetComponent<Renderer>();

            if (mr != null && mr.sharedMaterials != null)
            {
                for (int i = 0; i < mr.sharedMaterials.Length; i++)
                {
                    ReplaceMaterial(mr.sharedMaterials[i]);
                }
            }

            Renderer[] mrs = go.GetComponentsInChildren<Renderer>();

            for (int i = 0; i < mrs.Length; i++)
            {
                if (mrs[i].sharedMaterials == null)
                {
                    continue;
                }

                for (int j = 0; j < mrs[i].sharedMaterials.Length; j++)
                {
                    ReplaceMaterial(mrs[i].sharedMaterials[j]);
                }
            }

        }

        /// <summary>
        /// 解决Mat 的 shader不支持的问题
        /// </summary>
        /// <param name="mat"></param>
        private void ReplaceMaterial(Material mat)
        {
            if (mat == null)
            {
                return;
            }

            var shaderName = mat.shader.name;
            Shader newShader = null;
            if (DicUsedShader.ContainsKey(shaderName))
            {
                newShader = DicUsedShader[shaderName];
            }
            else
            {
                newShader = Shader.Find(shaderName);//优化 应该存起来
                DicUsedShader.Add(shaderName, newShader);
            }

            if (newShader != null)
            {
                mat.shader = newShader;
            }
            else
            {
                Debug.LogWarning("unable to refresh shader: " + shaderName + " in material " + mat.name);
            }
        }
        //#endif

    }

    public class ResourceManager : Core.Singleton<ResourceManager>
    {
        // string m_Assetbundle_BaseLocalPath = "";
        string[] m_AllManifest = null;
        AssetBundleManifest m_AssetBundleManifest = null;
        /// <summary>
        /// 依赖关系
        /// </summary>
        Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
        /// <summary>
        /// 已经被加载的ab
        /// </summary>
        Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
        /// <summary>
        /// 加载请求列表, 用于异步加载时
        /// </summary>
        Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();

        class LoadAssetRequest
        {
            public Type assetType;
            public string assetName;
            public Action<UObject> sharpFunc;
        }

        void Reset()
        {
            // m_Assetbundle_BaseLocalPath = "";
            m_AllManifest = null;
            m_AssetBundleManifest = null;
            m_Dependencies.Clear();
        }

        /// <summary>
        /// 清理全部资源
        /// </summary>
        public void ClearAll()
        {
            List<String> keys = new List<string>(m_LoadedAssetBundles.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                if (null != m_LoadedAssetBundles[keys[i]].m_AssetBundle)
                {
                    m_LoadedAssetBundles[keys[i]].OnUnload(false);
                }
            }
            m_LoadedAssetBundles.Clear();
            m_Dependencies.Clear();
        }
        public void ClearAllGameBundles()
        {
            List<String> keys = new List<string>(m_LoadedAssetBundles.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Contains("min_res"))
                {
                    continue;
                }
                if (null != m_LoadedAssetBundles[keys[i]].m_AssetBundle)
                {
                    m_LoadedAssetBundles[keys[i]].OnUnload(false);
                    m_LoadedAssetBundles.Remove(keys[i]);
                }
            }

        }
        #region 异步加载方式
        /// <summary>
        /// 1、Load AssetBundleManifest.
        /// </summary>
        /// <param name="initOK"></param>
        public void Initialize_Async(Action initOK)
        {
            Reset();
            //string localURL = GetAssetBundleLocalURL(Game.GlobalVar.RES_ASSETBUNDLE_ROOT + "/" + Game.GlobalVar.RES_ASSETBUNDLE_ROOT);
            LoadAsset_Async<AssetBundleManifest>(Game.GlobalVar.RES_ASSETBUNDLE_ROOT, "AssetBundleManifest", delegate (UObject obj)
            {
                //if (obj != null)
                //{
                //    m_AssetBundleManifest = obj as AssetBundleManifest;
                //    m_AllManifest = m_AssetBundleManifest.GetAllAssetBundles();
                //    //m_AssetBundleManifest.GetAllDependencies
                //}
                //if (initOK != null) initOK();

                bool flag = false;
                if (Game.GlobalVar.IS_RES_MODE_DEBUG)
                {
#if UNITY_EDITOR
                if (initOK != null) initOK();
#else
                    flag = true;
#endif
                }
                else
                {
                    flag = true;
                }

                if (flag)
                {
                    if (obj != null)
                    {
                        m_AssetBundleManifest = obj as AssetBundleManifest;
                        m_AllManifest = m_AssetBundleManifest.GetAllAssetBundles();
                        //m_AssetBundleManifest.GetAllDependencies
                    }
                    if (initOK != null) initOK();
                }
            });
        }

        private IEnumerator AsyncLoadByMainfest()
        {
            if (m_AllManifest != null)
            {
                for (int i = 0; i < m_AllManifest.Length; i++)
                {
                    string abName = m_AllManifest[i];
                    AssetBundleInfo bundleInfo = GetLoadedAssetBundleSync(abName);

                    if (bundleInfo == null)
                    {
                        yield return GameCoroutine.Instance().StartCoroutine(OnLoadAssetBundleByMainfest(abName, typeof(UObject)));
                        //Debug.Log("OnLoadAsset<UObject> " + abName);
                    }
                }
            }
        }
        public string GetAssetBundleFilePath(string assetbundelName)
        {
            string localPath = Game.FileUtils.getInstance().getAssetBundleFilePath(assetbundelName);
            if (System.IO.File.Exists(localPath))
            {
                return localPath;
            }
            else
            {
                return null;
            }

        }

        IEnumerator OnLoadAssetBundleByMainfest(string abName, Type type)
        {
            string localPath = Game.GlobalVar.RES_ASSETBUNDLE_ROOT + "/" + abName;
            localPath = Game.FileUtils.getInstance().getFullPath(localPath);
            string url = localPath;
            if (string.IsNullOrEmpty(url))
            {
                //Debug.Log("Dont load " + abName + " File no found"+ url);
                yield break;
            }

            AssetBundleCreateRequest request = null;
            if (type == typeof(AssetBundleManifest))
            {
                try
                {
                    request = AssetBundle.LoadFromFileAsync(url);
                }
                catch
                {
                    request = null;
                }
                yield return request;
            }
            else
            {
                // 获取这个资源的所有依赖
                string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
                if (dependencies.Length > 0)
                {
                    if (m_Dependencies.ContainsKey(abName) == false)
                    {
                        m_Dependencies.Add(abName, dependencies);

                        for (int i = 0; i < dependencies.Length; i++)
                        {
                            string depName = dependencies[i];
                            AssetBundleInfo bundleInfo = null;
                            if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo))
                            {
                                bundleInfo.m_ReferencedCount++;
                            }
                            else if (!m_LoadRequests.ContainsKey(depName))
                            {
                                yield return GameCoroutine.Instance().StartCoroutine(OnLoadAssetBundleByMainfest(depName, type));
                            }
                        }
                    }
                }
                AssetBundleInfo bundleInfo2 = null;
                if (m_LoadedAssetBundles.TryGetValue(abName, out bundleInfo2))
                {
                    bundleInfo2.m_ReferencedCount++;
                }
                else
                {
                    try
                    {
                        request = AssetBundle.LoadFromFileAsync(url);
                    }
                    catch
                    {
                        request = null;
                    }
                    //download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(abName), 0);
                    yield return request;
                }
            }

            if (request != null)
            {
                AssetBundle assetObj = request.assetBundle;
                if (assetObj != null)
                {
                    if (m_LoadedAssetBundles.ContainsKey(abName) == false)
                    {
                        m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
                        //AssetBundleRequest abr = null;
                        //try
                        //{
                        //    abr = assetObj.LoadAllAssetsAsync();
                        //}
                        //catch
                        //{
                        //    abr = null;
                        //}
                        //yield return abr;
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(assetObj);
                    }
                }
            }
        }

        /// <summary>
        /// 加载一个预置体
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="func"></param>
        public void LoadPrefab_Async(string abName, string assetName, Action<UObject> func)
        {
            LoadAsset_Async<GameObject>(abName, assetName, func);
        }


        /// <summary>
        /// 加载一个预置体
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="func"></param>
        public void LoadAsset_Async(string abName, string assetName, Action<UObject> func)
        {
            LoadAsset_Async<UObject>(abName, assetName, func);
        }


        /// <summary>
        /// 加载assetbundle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">assetbundle文件夹下的相对路径</param>
        /// <param name="assetName"></param>
        /// <param name="action"></param>
        /// <param name="func"></param>
        void LoadAsset_Async<T>(string abName, string assetName, Action<UObject> action = null) where T : UObject
        {
            if (Game.GlobalVar.IS_RES_MODE_DEBUG)
            {
#if UNITY_EDITOR
                LoadAsset_From_ProjectDir_Async<T>(abName, assetName, action);
#else
                LoadAsset_From_FinalDir_Async<T>(abName, assetName, action);
#endif
            }
            else
            {
                LoadAsset_From_FinalDir_Async<T>(abName, assetName, action);
            }
        }

        void LoadAsset_From_FinalDir_Async<T>(string abName, string assetName, Action<UObject> action = null) where T : UObject
        {
            Debug.LogError("0000000000000000000");
            abName = CorrectionBundleNameForNew(abName);
            abName = GetRealAssetPath(abName);

            // 构建加载请求
            LoadAssetRequest request = new LoadAssetRequest();
            request.assetType = typeof(T);
            request.assetName = assetName;
            // request.luaFunc = func;
            request.sharpFunc = action;

            List<LoadAssetRequest> requests = null;
            if (!m_LoadRequests.TryGetValue(abName, out requests)) // 判断加载请求是否已经存在
            {
                requests = new List<LoadAssetRequest>();
                requests.Add(request);
                m_LoadRequests.Add(abName, requests);
                GameCoroutine.Instance().StartCoroutine(OnLoadAsset<T>(abName));
            }
            else
            {
                // 加载请求
                requests.Add(request);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 在调试的模式下，真接从工程内部，加载还未打成ab的原生资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="action"></param>
        /// <param name="func"></param>
        void LoadAsset_From_ProjectDir_Async<T>(string abName, string assetName, Action<UObject> action = null) where T : UObject
        {
            if (typeof(T) == typeof(AssetBundleManifest))
            {
                if (action != null)
                {
                    action(null);
                    action = null;
                }
                return;
            }

            // 获取资源的Assets的相关目录，形如：Assets/xx/xx.xx
            string assetPath = "Assets/" + Game.GlobalVar.RES_ROOT + "/" + Game.GlobalVar.RES_ASSETBUNDLE_ROOT + "/" + abName;

            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            
            if (asset == null)
            {
                Debug.LogError("OnLoadAsset--->>>" + assetPath);
            }

            if (action != null)
            {
                action(asset);
                action = null;
            }
        }
#endif
        /// <summary>
        /// 加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <returns></returns>
        IEnumerator OnLoadAsset<T>(string abName) where T : UObject
        {
            // 是否被加载过
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);

            // 没有被加载过
            if (bundleInfo == null)
            {
                yield return GameCoroutine.Instance().StartCoroutine(OnLoadAssetBundle(abName, typeof(T)));

                bundleInfo = GetLoadedAssetBundle(abName);
                if (bundleInfo == null)
                {
                    m_LoadRequests.Remove(abName);
                    Debug.Log("OnLoadAsset--->>>" + abName);
                    yield break;
                }
            }

            // 被加载过，响应回调
            List<LoadAssetRequest> list = null;
            if (!m_LoadRequests.TryGetValue(abName, out list))
            {
                m_LoadRequests.Remove(abName);
                yield break;
            }
            for (int i = 0; i < list.Count; i++)
            {
                string assetName = list[i].assetName;
                AssetBundle ab = bundleInfo.m_AssetBundle;
                Debug.Log("Async" + assetName + list[i].assetType.ToString());
                AssetBundleRequest request = ab.LoadAssetAsync(assetName, list[i].assetType);
                yield return request;


                if (list[i].sharpFunc != null)
                {
                    list[i].sharpFunc(request.asset);
                    list[i].sharpFunc = null;
                }
                bundleInfo.m_ReferencedCount++;
            }
            m_LoadRequests.Remove(abName);
        }

        // IEnumerator OnLoadAssetBundle(string abName, Type type)
        // {
        //     string url = GetAssetBundleLocalURL(abName);
        //     Debug.LogError("aaaaaaaaaaaaaa");
        //     UnityWebRequest download = null;
        //     if (type == typeof(AssetBundleManifest))
        //     {
        //         download = UnityWebRequest.Get(url);
        //         yield return download.SendWebRequest();
        //     }
        //     else
        //     {
        //         // 获取这个资源的所有依赖
        //         string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
        //         if (dependencies.Length > 0)
        //         {
        //             Debug.LogError("bbbbbbbbbbbbbbbbbbbb");
        //             if (m_Dependencies.ContainsKey(abName) == false)
        //             {
        //                 m_Dependencies.Add(abName, dependencies);

        //                 for (int i = 0; i < dependencies.Length; i++)
        //                 {
        //                     string depName = dependencies[i];
        //                     Debug.LogError(depName);  
        //                     AssetBundleInfo bundleInfo = null;
        //                     if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo))
        //                     {
        //                         bundleInfo.m_ReferencedCount++;
        //                     }
        //                     else if (!m_LoadRequests.ContainsKey(depName))
        //                     {
        //                         yield return GameCoroutine.Instance().StartCoroutine(OnLoadAssetBundle(depName, type));
        //                     }
        //                 }
        //             }
        //         }
        //         AssetBundleInfo bundleInfo2 = null;
        //         if (m_LoadedAssetBundles.TryGetValue(abName, out bundleInfo2))
        //         {
        //             bundleInfo2.m_ReferencedCount++;
        //         }
        //         else
        //         {
        //             download = UnityWebRequest.Get(url);
        //             yield return download.SendWebRequest();
        //         }
        //     }

        //     if (download != null)
        //     {
        //         AssetBundle assetObj = download.assetBundle;
        //         if (assetObj != null)
        //         {
        //             if (m_LoadedAssetBundles.ContainsKey(abName) == false)
        //             {
        //                 m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
        //             }
        //         }
        //     }
        // }

        IEnumerator OnLoadAssetBundle(string abName, Type type)
        {
            string url = GetAssetBundleLocalURL(abName);

            // 用于存储最终获取到的 AssetBundle 对象
            AssetBundle loadedBundle = null;
            UnityWebRequest request = null;

            // 1. 处理 AssetBundleManifest 特殊类型
            if (type == typeof(AssetBundleManifest))
            {
                // Manifest 通常也是 AssetBundle 格式，建议统一使用 GetAssetBundle 以便利用缓存机制
                // 如果确定是纯文本或其他格式，才用 Get()，但 Manifest 必须是 Bundle
                request = UnityWebRequestAssetBundle.GetAssetBundle(url);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    // 【关键修复】使用 GetContent 获取 Manifest
                    loadedBundle = DownloadHandlerAssetBundle.GetContent(request);

                    // 如果这是主 Manifest，可能需要赋值给全局变量 m_AssetBundleManifest
                    // m_AssetBundleManifest = loadedBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                }
                else
                {
                    Debug.LogError($"加载 Manifest 失败: {abName}, Error: {request.error}");
                }
            }
            else
            {
                // 2. 处理普通 AssetBundle
                // 获取依赖
                if (m_AssetBundleManifest != null)
                {
                    string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);

                    if (dependencies.Length > 0)
                    {
                        if (!m_Dependencies.ContainsKey(abName))
                        {
                            m_Dependencies.Add(abName, dependencies);

                            for (int i = 0; i < dependencies.Length; i++)
                            {
                                string depName = dependencies[i];

                                // 检查是否已加载
                                AssetBundleInfo bundleInfo = null;
                                if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo))
                                {
                                    // 已加载，增加引用计数
                                    bundleInfo.m_ReferencedCount++;
                                }
                                else if (!m_LoadRequests.ContainsKey(depName)) // 建议使用 HashSet 检查正在请求中的包
                                {
                                    // 递归加载依赖
                                    // 注意：这里递归调用可能会造成深层堆栈，确保 m_LoadRequests 能正确防止死循环
                                    m_LoadRequests.Add(depName,new List<LoadAssetRequest>()); // 标记为正在请求
                                    yield return GameCoroutine.Instance().StartCoroutine(OnLoadAssetBundle(depName, type));
                                    // 注意：递归返回后，依赖应该已经加载到 m_LoadedAssetBundles 中了
                                }
                            }
                        }
                    }
                }

                // 3. 加载当前包本身
                AssetBundleInfo bundleInfo2 = null;
                if (m_LoadedAssetBundles.TryGetValue(abName, out bundleInfo2))
                {
                    // 如果在加载依赖的过程中，当前包已经被其他路径加载了（虽然少见，但在复杂依赖图中可能发生）
                    bundleInfo2.m_ReferencedCount++;
                }
                else
                {
                    // 【关键修复】使用 UnityWebRequestAssetBundle.GetAssetBundle
                    request = UnityWebRequestAssetBundle.GetAssetBundle(url);
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        // 【关键修复】使用 GetContent 获取 AssetBundle
                        loadedBundle = DownloadHandlerAssetBundle.GetContent(request);
                    }
                    else
                    {
                        Debug.LogError($"加载 AssetBundle 失败: {abName}, Error: {request.error}");
                    }
                }
            }

            // 4. 统一处理结果并缓存
            if (loadedBundle != null)
            {
                if (!m_LoadedAssetBundles.ContainsKey(abName))
                {
                    m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(loadedBundle));
                    // 初始引用计数设为 1，因为当前调用者持有它
                    m_LoadedAssetBundles[abName].m_ReferencedCount = 1;
                }
                else
                {
                    // 极端情况：如果在递归依赖加载中，当前包已经被加入字典，但 loadedBundle 不为空
                    // 这通常意味着逻辑上有重复加载，应卸载当前的 loadedBundle 以避免内存泄漏
                    loadedBundle.Unload(true);
                    Debug.LogWarning($"检测到重复加载 Bundle: {abName}，已卸载冗余副本。");
                }
            }

            // 5. 【重要】释放 UnityWebRequest 资源
            if (request != null)
            {
                request.Dispose();
            }

            // 从正在请求集合中移除（如果之前添加了的话）
            if (m_LoadRequests.ContainsKey(abName))
            {
                m_LoadRequests.Remove(abName);
            }
        }
        #endregion

        #region 同步加载方式


        /// <summary>
        /// 增加同步方案下 的异步后台加载优化
        /// </summary>
        private Coroutine cAsyncLoadByMainfest;

        public void StopAsyncLoadByMainfest()
        {
            if (cAsyncLoadByMainfest != null)
            {
                GameCoroutine.Instance().StopCoroutine(cAsyncLoadByMainfest);
                cAsyncLoadByMainfest = null;
            }
        }

        public void StartAsyncLoadByMainfest()
        {
            StopAsyncLoadByMainfest();
            cAsyncLoadByMainfest = GameCoroutine.Instance().StartCoroutine(AsyncLoadByMainfest()); //开启协程异步加载资源 减少资源加载时间
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialize_Sync()
        {
            Reset();
            if (Game.GlobalVar.IS_RES_MODE_DEBUG)
            {
#if UNITY_EDITOR
                return true;
#endif
            }

            AssetBundleManifest abm = LoadAsset_Sync<AssetBundleManifest>(Game.GlobalVar.RES_ASSETBUNDLE_ROOT, "AssetBundleManifest");
            if (abm)
            {
                m_AssetBundleManifest = abm;
                m_AllManifest = m_AssetBundleManifest.GetAllAssetBundles();

                // T-Bag:异步加载有两个问题：不应该全部加载，热更新后没有重新加载新资源。先屏蔽掉。
                // StartAsyncLoadByMainfest(); //开启协程异步加载资源 减少资源加载时间
                return true;
            }
            else
            {
                return false;
            }
        }

        public GameObject LoadPrefab_Sync(string abName, string assetName)
        {
            UObject t = LoadAsset_Sync<UObject>(abName, assetName);
            GameObject go = t as GameObject;
            if (go == null)
            {
                Core.Debuger.LogError("资源不是GameObject类型 " + abName);
            }
            return go;
        }

        public Material LoadMaterial_Sync(string abName, string assetName)
        {
            UObject t = LoadAsset_Sync<UObject>(abName, assetName);
            Material asset = t as Material;
            if (asset == null)
            {
                Core.Debuger.LogError("资源不是Material类型 " + abName);
            }
            return asset;
        }

        public AudioClip LoadAudioClip_Sync(string abName, string assetName)
        {
            UObject t = LoadAsset_Sync<UObject>(abName, assetName);
            AudioClip asset = t as AudioClip;
            if (asset == null)
            {
                Core.Debuger.LogError("资源不是AudioClip类型 " + abName);
            }
            return asset;
        }

        public Sprite LoadSprite_Sync(string abName, string assetName)
        {
            GameObject asset = LoadAsset_Sync<GameObject>(abName, assetName);
            if (asset == null)
            {
                Core.Debuger.LogError("资源不是Sprite类型 " + abName);
            }
            SpriteRenderer img = asset.GetComponent<SpriteRenderer>();
            if (null == img)
            {
                return null;
            }
            return img.sprite;
        }

        public Texture2D LoadTexture_Sync(string abName, string assetName)
        {
            UObject t = LoadAsset_Sync<UObject>(abName, assetName);
            Texture2D asset = t as Texture2D;
            return asset;
        }

        public T LoadAsset_Sync<T>(string abName, string assetName) where T : UObject
        {
            if (assetName != "AssetBundleManifest" && !abName.Contains(assetName))
            {
                Core.Debuger.LogError("abName 和 assetName不匹配. " + abName + " - " + assetName);
                return null;
            }

            T asset = null;
            if (Game.GlobalVar.IS_RES_MODE_DEBUG)
            {
#if UNITY_EDITOR
                asset = LoadAsset_From_ProjectDir_Sync<T>(abName, assetName);
#else
                asset = LoadAsset_From_FinalDir_Sync<T>(abName, assetName);
#endif
            }
            else
            {
                asset = LoadAsset_From_FinalDir_Sync<T>(abName, assetName);
            }
            return asset;
        }

        T LoadAsset_From_FinalDir_Sync<T>(string abName, string assetName) where T : UObject
        {
            abName = CorrectionBundleNameForNew(abName);
            abName = GetRealAssetPath(abName);
            // 是否被加载过
            AssetBundleInfo bundleInfo = GetLoadedAssetBundleSync(abName);

            // 没有被加载过
            if (bundleInfo == null)
            {
                bundleInfo = LoadAssetBundle_Sync(abName, typeof(T));
                if (bundleInfo == null)
                {
                    Debug.LogError("OnLoadAsset--->>>" + abName);
                    return null;
                }
            }

            // 被加载过，响应回调
            return bundleInfo.LoadAsset<T>(assetName);
        }
#if UNITY_EDITOR
        public T LoadAsset_From_ProjectDir_Sync<T>(string abName, string assetName) where T : UObject
        {
            // 获取资源的Assets的相关目录，形如：Assets/xx/xx.xx
            string assetPath = "Assets/" + Game.GlobalVar.RES_ROOT + "/" + Game.GlobalVar.RES_ASSETBUNDLE_ROOT + "/" + abName;
            //T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;

            if (asset == null)
            {
                Debug.LogError("OnLoadAsset--->>>" + assetPath);
            }
            
            return asset;
        }


#endif
        //public class IOBuff
        //{
        //    public int md5;
        //    public byte[] bytes;

        //    public IOBuff(int md5,byte[] bytes)
        //    {
        //        this.md5 = md5;
        //        this.bytes = bytes;
        //    }
        //}

        //public static Dictionary<string, IOBuff> dicIOBuff = new Dictionary<string, IOBuff>();
        AssetBundleInfo LoadAssetBundle_Sync(string abName, Type type)
        {
            byte[] bytes = null;
            //IOBuff iobuff = null;
            //dicIOBuff.TryGetValue(abName, out iobuff);
            //if (iobuff != null)
            //{
            //    bytes = iobuff.bytes;
            //}

            AssetBundle bundle = null;
            AssetBundleInfo bundleInfo = null;
            if (type == typeof(AssetBundleManifest))
            {
                if (bytes == null)
                {
                    string localPath = Game.GlobalVar.RES_ASSETBUNDLE_ROOT + "/" + abName;
                    localPath = Game.FileUtils.getInstance().getFullPath(localPath);
                    bytes = Game.FileUtils.getBytes(localPath);
                    //dicIOBuff.Add(abName, new IOBuff(0, bytes));
                }
                bundle = AssetBundle.LoadFromMemory(bytes);
                if (bundle != null)
                {
                    bundleInfo = new AssetBundleInfo(bundle);
                    m_LoadedAssetBundles.Add(abName, bundleInfo);
                }
            }
            else
            {
                LoadDependencies_Sync(abName, type);

                if (m_LoadedAssetBundles.TryGetValue(abName, out bundleInfo))
                {
                    bundleInfo.m_ReferencedCount++;
                }
                else
                {
                    if (bytes == null)
                    {
                        string localPath = Game.GlobalVar.RES_ASSETBUNDLE_ROOT + "/" + abName;
                        localPath = Game.FileUtils.getInstance().getFullPath(localPath);
                        bytes = Game.FileUtils.getBytes(localPath);
                        //dicIOBuff.Add(abName, new IOBuff(0, bytes));
                    }
                    //Debug.Log(bytes.Length);
                    //Debug.Log(string.Format("abName:{0}", abName));
                    bundle = AssetBundle.LoadFromMemory(bytes);
                    if (bundle != null)
                    {
                        bundleInfo = new AssetBundleInfo(bundle);
                        m_LoadedAssetBundles.Add(abName, bundleInfo);
                    }
                }
            }

            return bundleInfo;
        }

        /// <summary>
        /// 载入依赖
        /// </summary>
        /// <param name="name"></param>
        void LoadDependencies_Sync(string abName, Type type)
        {
            if (m_AssetBundleManifest == null)
            {
                Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
            }

            // 获取这个资源的所有依赖
            string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
            if (dependencies.Length == 0) return;


            if (m_Dependencies.ContainsKey(abName)) return;//依赖项是否已经加载  //by pj 初步判断此处是为了阻塞重复引用关系的对象
            m_Dependencies.Add(abName, dependencies);

            for (int i = 0; i < dependencies.Length; i++)
            {
                string depName = dependencies[i];
                AssetBundleInfo bundleInfo = null;
                if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo))
                {
                    bundleInfo.m_ReferencedCount++;
                }
                else
                {
                    LoadAssetBundle_Sync(depName, type);
                }
            }

        }

        #endregion
        AssetBundleInfo GetLoadedAssetBundleSync(string abName)
        {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(abName, out bundle);
            if (bundle == null) return null;

            //by pj 同步完全不需要 
            // No dependencies are recorded, only the bundle itself is required.
            //string[] dependencies = null;
            //if (!m_Dependencies.TryGetValue(abName, out dependencies))
            //    return bundle;

            //// Make sure all dependencies are loaded
            //// 只要有一个依赖没有被加载这个abName就不算被加载过
            //foreach (var dependency in dependencies)
            //{
            //    AssetBundleInfo dependentBundle;
            //    m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
            //    if (dependentBundle == null) return null;
            //}
            return bundle;
        }

        AssetBundleInfo GetLoadedAssetBundle(string abName)
        {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(abName, out bundle);
            if (bundle == null) return null;

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            // 只要有一个依赖没有被加载这个abName就不算被加载过
            foreach (var dependency in dependencies)
            {
                AssetBundleInfo dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null) return null;
            }
            return bundle;
        }

        string GetRealAssetPath(string abName)
        {
            if (abName.Equals(Game.GlobalVar.RES_ASSETBUNDLE_ROOT))
            {
                return abName;
            }
            abName = abName.ToLower();  // 注：assetbundle中的资源路径都是小写的，unity就这样，不是我定的
            for (int i = 0; i < m_AllManifest.Length; i++)
            {
                string path = m_AllManifest[i];
                if (path.Equals(abName))
                {
                    return m_AllManifest[i];
                }
            }
            Debug.LogError("GetRealAssetPath Error:>>" + abName);
            return null;
        }


        public string GetAssetBundleLocalURL(string assetbundelName)
        {
            string localPath = Game.GlobalVar.RES_ASSETBUNDLE_ROOT + "/" + assetbundelName;
            string localURL = Game.FileUtils.getInstance().getFullPathForWww(localPath);
            return localURL;
        }

        /// <summary>
        /// 此函数交给外部卸载专用，自己调整是否需要彻底清除AB
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        public void UnloadAssetBundle(string abName, bool isThorough = false)
        {
            abName = CorrectionBundleNameForNew(abName);
            abName = GetRealAssetPath(abName);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
            UnloadAssetBundleInternal(abName, isThorough);
            UnloadDependencies(abName, isThorough);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
        }

        /// <summary>
        /// 清除一个资源对应的依赖资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        void UnloadDependencies(string abName, bool isThorough)
        {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency, isThorough);
            }
            m_Dependencies.Remove(abName);
        }

        /// <summary>
        /// 清除资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        void UnloadAssetBundleInternal(string abName, bool isThorough)
        {
            AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
            if (bundle == null) return;


            if (--bundle.m_ReferencedCount <= 0)
            {
                if (m_LoadRequests.ContainsKey(abName))
                {
                    // 如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
                    // 让它等待下一次再回收
                    return;
                }
                //bundle.m_AssetBundle.Unload(isThorough);
                bundle.OnUnload(isThorough);
                m_LoadedAssetBundles.Remove(abName);
                Debug.Log(abName + " has been unloaded successfully");
            }
        }

        /// <summary>
        /// 为新的打包方式，校正包名
        /// 如：games/suzhou_majiang/prefabs/ui/layer_main.prefab --> games/suzhou_majiang/prefabs/ui.ab
        /// </summary>
        /// <param name="oldBundleName"></param>
        string CorrectionBundleNameForNew(string oldBundleName)
        {
            if (oldBundleName.Equals(Game.GlobalVar.RES_ASSETBUNDLE_ROOT))
            {
                return oldBundleName;
            }


            string newbundleName = oldBundleName;
            int index = oldBundleName.LastIndexOf("/");
            if (index > 0)
            {
                newbundleName = oldBundleName.Substring(0, index);
                newbundleName += ".ab";
            }
            //Debug.Log(string.Format("oldBundleName:{0} newbundleName:{1}", oldBundleName, newbundleName));
            return newbundleName;

        }

    }
}