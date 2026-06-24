using Core;
using System.Collections.Generic;

namespace Game
{
    using System;
    using System.Collections;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Networking;
    using GameCoroutine = Core.Coroutine;
    
    public class ResoucesUdate : Game.MonoSingleton<ResoucesUdate>
    {
        #region 内部变量
        /// <summary>
        /// 需要下载的资源清单
        /// </summary>
        List<Downloader.DownloadUnit> needDownList = new List<Downloader.DownloadUnit>();

        List<Downloader.DownloadUnit> alreadyDownList = new List<Downloader.DownloadUnit>();

        /// <summary>
        /// 临时文件后缀名
        /// </summary>
        string tempSuffix = ".temp";

        /// <summary>
        /// 总下载完成flag
        /// </summary>
        bool isOnDownData_FinishedCallback = false;

        /// <summary>
        /// 单一下载完成flag
        /// </summary>
        bool isOnDowningData_one_callback = false;

        /// <summary>
        /// 当前下载文件大小
        /// </summary>
        long OnDowningData_current_size = 0;

        /// <summary>
        /// 总下载文件的总大小
        /// </summary>
        long OnDowningData_total_size = 0;

        /// <summary>
        /// 当前下载的单元
        /// </summary>
        Downloader.DownloadUnit OnDowningData_down_unit;

        /// <summary>
        /// 下载进度Lua回调
        /// </summary>
        Action<float,string> luaOnProcessCallback = null;

        /// <summary>
        /// 下载异常Lua回调
        /// </summary>
        Action<string,float> luaOnErrorCallback = null;

        /// <summary>
        /// 热更资源地址
        /// </summary>
        string hotFixResUrl = string.Empty;

        /// <summary>
        /// 热更版本
        /// </summary>
        string hotFixVersion = string.Empty;

        /// <summary>
        /// 热更总资源列表信息
        /// </summary>
        string newHotFixResStr = string.Empty;
        #endregion
        public bool UpdateOver = false;

        #region 内置接口
        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Network = 0;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Network = 1;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Network = 2;
            }
            //下载完成回调
            if (isOnDownData_FinishedCallback)
            {
                isOnDownData_FinishedCallback = false;
                UpdateOver = true;
                string resAllPath = Game.FileUtils.getInstance().getWritablePath() + "/" + Game.GlobalVar.RES_LIST_FILE;
               
                if (File.Exists(resAllPath))
                {
                    File.Delete(resAllPath);
                    Game.FileUtils.writeString(resAllPath, newHotFixResStr);
                }
                else
                {
                    Core.Debuger.LogError("热更完成,而旧资源总清单文件丢失,异常");
                    return;
                }

                OnResourceInited();
            }

            //下载进度Lua回调
            if (isOnDowningData_one_callback)
            {
                isOnDowningData_one_callback = false;
                OnDowingData_One_CallBack(OnDowningData_current_size, OnDowningData_total_size, OnDowningData_down_unit);
            }
            if (Update_Is_Failed)
            {
                Update_Is_Failed = false;
            }
        }

        /// <summary>
        /// 检测资源
        /// </summary>
        private void CheckResource()
        {
            if (Game.GlobalVar.IS_RES_MODE_DEBUG)
           {
                //直接使用工程内部最原始的资源，不用更新不用释放的
               OnResourceInited();
           }
            else
            {
                //从服务器更新
                GameCoroutine.Instance().StartCoroutine(OnUpdateResource());
            }
        }

 
        /// <summary>
        /// 启动更新下载
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnUpdateResource()
        {
            if (!Game.GlobalVar.IS_OPEN_RES_UPDATE)
            {
                OnResourceInited();
                yield break;
            }
            //本地存储路径
            string saveRootPath = Game.FileUtils.getInstance().getWritablePath();
            //平台下热更资源地址
            string updateRootUrl = hotFixResUrl.Trim() +  "/" + Game.GlobalVar.RES_ROOT;
            //1、下载服务器上的总清单文件
            string url = updateRootUrl + "/" + Game.GlobalVar.RES_LIST_FILE;
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                OnUpdateFailed(request.error);
                yield break;
            }

            if (!Directory.Exists(saveRootPath))
            {
                Directory.CreateDirectory(saveRootPath);
            }

            //2、解析清单文件
            //网络资源总清单对象
            //   byte[] data = request.downloadHandler.data;
            ResListJson resJsonObj_server = JsonUtility.FromJson<ResListJson>(request.downloadHandler.text);
            
            if (resJsonObj_server == null || resJsonObj_server.games == null || resJsonObj_server.games.Count == 0)
            {
                OnUpdateFailed("网络资源清单异常");
                yield break;
            }

            //本地资源总清单路径
            string localResListFile = saveRootPath + "/" + Game.GlobalVar.RES_LIST_FILE;
            //本地资源总清单对象
            ResListJson resJsonObj_local = null;
            if (File.Exists(localResListFile))
            {
                resJsonObj_local = JsonUtility.FromJson<ResListJson>(Game.FileUtils.getString(localResListFile));
            }
            //需要下载的资源清单清空
            needDownList.Clear();
            //根据本地资源类获取网络资源类最新信息
            List<ResListGameItem> serverAllGames = new List<ResListGameItem>();
            serverAllGames.AddRange(resJsonObj_server.games);

            //本地资源信息
            List<ResListGameItem> localAllGames = new List<ResListGameItem>();
            //获取热更总资源信息
            newHotFixResStr = JsonUtility.ToJson(resJsonObj_server, true);
            resJsonObj_server.games.Clear();
            if (resJsonObj_local != null)
            {
                for (int i = 0; i < serverAllGames.Count; i++)
                {
                    ResListGameItem game = serverAllGames[i];
                    ResListGameItem localGame = resJsonObj_local.games.Find(local => local.name == game.name);

                    if (localGame != null)
                    {
                        resJsonObj_server.games.Add(game);
                        localAllGames.Add(localGame);
                        resJsonObj_local.games.Remove(localGame);
                    }
                    else//如果本地没有服务器有  那就直接下载下来（这里是添加一个子游戏）
                    {
                        for (int j = 0; j < game.res_list.Count; j++)
                        {
                            ResItem resServerItem = game.res_list[j];
                            string downUrl = updateRootUrl + "/" + resServerItem.path;
                            string savePath = saveRootPath + "/" + resServerItem.path;
                            needDownList.Add(new Downloader.DownloadUnit(downUrl, savePath, resServerItem.size));
                        }
                    }
                }
                // 清除本地过时的子游戏
                for (int i = 0; i < resJsonObj_local.games.Count; i++)
                {
                    ResListGameItem res = resJsonObj_local.games[i];
                    if (!res.name.Equals("common"))
                    {
                        string abPath = saveRootPath + "/assetbundle/games/" + res.name;
                        string luaPath = saveRootPath + "/assetbundle/games/" + res.name;
                        abPath = abPath.Replace('\\', '/');
                        luaPath = luaPath.Replace('\\', '/');
                        if (Directory.Exists(abPath))
                        {
                            FileUtils.getInstance().removeDirectory(abPath);//递归的方式删除文件夹里面的所有内容
                        }
                        if (Directory.Exists(luaPath))
                        {
                            FileUtils.getInstance().removeDirectory(luaPath);
                        }
                    }
                }
               resJsonObj_local.games.Clear();
            }
            else
            {
                Debuger.LogError("进行资源热更新时丢失本地资源清单,异常");
                yield break;
            }


            //比对网络与本地所有资源列表,筛选出要更新的资源
            //网络资源列表
            List<ResItem> serverResItems = new List<ResItem>();
            //本地资源列表
            List<ResItem> localResItems = new List<ResItem>();

            //获取网络所有资源列表
            for (int i = 0; i < resJsonObj_server.games.Count; i++)
            {
                ResListGameItem game = resJsonObj_server.games[i];
                serverResItems.AddRange(game.res_list);
            }
            //获取本地所有资源列表
            for (int i = 0; i < localAllGames.Count; i++)
            {
                ResListGameItem game = localAllGames[i];
                localResItems.AddRange(game.res_list);
            }

            //获取需要下载的资源列表
            ResItem resServer = null;
            ResItem resLocal = null;
            for (int i = 0; i < serverResItems.Count; i++)
            {
                resServer = serverResItems[i];
                
                resLocal = localResItems.Find(res => res.path.Trim() == resServer.path.Trim());
              
                string downUrl = updateRootUrl + "/" + resServer.path;
                string savePath = saveRootPath + "/" + resServer.path;
                if (resLocal == null)
                {
                    needDownList.Add(new Downloader.DownloadUnit(downUrl, savePath, resServer.size));
                }
                else
                {
                    if (resServer.md5.Trim() == resLocal.md5.Trim())
                    {
                        if (!File.Exists(savePath))
                        {
                            // 这个文件加入到下载列表中，注意:这时候不要删除已经存在的md5,这时体现续传的作用了。
                            needDownList.Add(new Downloader.DownloadUnit(downUrl, savePath, resServer.size));
                        }
                        else
                        {
                            Game.FileUtils.removeFile(savePath + tempSuffix);
                        }
                    }
                    else
                    {
                        needDownList.Add(new Downloader.DownloadUnit(downUrl, savePath, resServer.size));
                        Game.FileUtils.removeFile(savePath + tempSuffix);
                    }

                    localResItems.Remove(resLocal);
                }
            }

            // 清除过时的资源
            for (int i = 0; i < localResItems.Count; i++)
            {
                ResItem res = localResItems[i];
                string savePath = saveRootPath + "/" + res.path;
                Game.FileUtils.removeFile(savePath + tempSuffix);
            }
            localResItems.Clear();

            float updateSize = 0;
            StartCoroutine(RefreshNet());
            if (needDownList.Count > 0)
            {

                for (int i = 0; i < needDownList.Count; i++)
                {
                    Downloader.DownloadUnit downUnit = needDownList[i];
                    updateSize = updateSize + downUnit.size;
                }
                // updateSize：下载文件的大小  弹出热更弹框
                // LuaManager.Instance().NeedHotUpdate(updateSize);
            }
         
         }
        public bool isOnClick = false;
        /// <summary>
        /// 开始下载
        /// </summary>
        public void StartUpdate()
        {
            // 开始下载
            if (needDownList.Count > 0)
            {
                //调整AppInfo.lua文件的位置
                //Downloader.DownloadUnit targetAppInfo = null;
                //int index = needDownList.Count;
                //Downloader.DownloadUnit du = null;
                //for (int i = 0; i < needDownList.Count; i++)
                //{
                //    du = needDownList[i];
                //    if (!du.savePath.EndsWith("AppInfo.lua"))
                //    {
                //        continue;
                //    }

                //    index = i;
                //    targetAppInfo = du;
                //}
                //if (index != needDownList.Count)
                //{
                //    needDownList[index] = needDownList[needDownList.Count - 1];
                //    needDownList[needDownList.Count - 1] = targetAppInfo;
                //}

                Downloader.DownloadUnit targetAppInfo = null;
                for (int i = 0; i < needDownList.Count; i++)
                {
                    if (needDownList[i].savePath.EndsWith("AppInfo.lua"))
                    {
                        targetAppInfo = needDownList[i];
                        needDownList[i] = needDownList[needDownList.Count - 1];
                        needDownList[needDownList.Count - 1] = targetAppInfo;
                        break;
                    }
                }
                for (int i = 0; i < needDownList.Count; i++)
                {

                    if(alreadyDownList.Contains(needDownList[i]))
                    {
                        needDownList.Remove(needDownList[i]);
                    }
                }

                long indexSize = OnDowningData_total_size - OnDowningData_current_size;
                OnDowningData_current_size = 0;
                //开始批量下载
                Downloader.BatchDownload(needDownList, (currentSize, totalSize, unit) =>
                {

                    isOnDowningData_one_callback = true;
                    Current_Down_Total_Size = totalSize;
                    if (isOnClick)
                    {
                        indexSize = OnDowningData_total_size - totalSize;
                    }
               
                    OnDowningData_current_size = indexSize+ currentSize;
                    if(OnDowningData_total_size == 0) OnDowningData_total_size = totalSize;

                    OnDowningData_down_unit = unit;


                    
                    float process = (OnDowningData_current_size / (float)OnDowningData_total_size);
                    alreadyDownList.Add(unit);


                    //下载完成
                    if (OnDowningData_current_size >= OnDowningData_total_size)
                    {
                        isOnDownData_FinishedCallback = true;
                    }

                }, (downUnit) =>
                {
                    OnUpdateFailed(downUnit.downUrl);
                });
            }
            else
            {
                OnResourceInited();
            }
        }

        /// <summary>
        /// 下载进度Lua回调
        /// </summary>
        /// <param name="curentSize">当前下载位置</param>
        /// <param name="totalSize">总下载文件长度</param>
        /// <param name="unit">下载单元</param>
        private void OnDowingData_One_CallBack(long curentSize, long totalSize, Downloader.DownloadUnit unit)
        {
            float progress = (curentSize / (float)totalSize);
            string path = unit.savePath.Substring(Application.persistentDataPath.Length);
            if (luaOnProcessCallback != null)
            {
                luaOnProcessCallback.Invoke(progress, "下载中: " + path);
            }
        }
        private bool Update_Is_Failed = false;
        private string Failed_Message = "";
        /// <summary>
        /// 资源更新失败
        /// </summary>
        /// <param name="file">失败文件路径</param>
        private void OnUpdateFailed(string file)
        {
            Failed_Message = "资源更新,更新失败 -->" + file;
            if (luaOnErrorCallback != null)
            {
                Update_Is_Failed = true;
            }
        }
     
        /// <summary>
        /// 资源初始化完成后执行
        /// </summary>
        private void OnFinished()
        {
            UnityEngine.Object.Destroy(InitUIController.Instance.gameObject);
            if (Game.GlobalVar.IS_RES_MODE_DEBUG) return;
            
            // 启动Lua主程序，登录前的下载才需要以下代码
            // LuaManager.Instance().ReInitLua();
            // if (luaOnProcessCallback != null)
            // {
            //     luaOnProcessCallback.Invoke(1, "下载完成");
            // }
        }
        #endregion

        #region 公开接口
        /// <summary>
        /// 检测并更新资源
        /// </summary>
        /// <param name="hotFixResUrl">热更地址</param>
        /// <param name="hotFixVersion">热更版本</param>
        /// <param name="funcOnProgress">下载进度Lua回调</param>
        /// <param name="funcOnError">下载异常Lua回调</param>
        public void CheckUpdate(string hotFixResUrl, string hotFixVersion, Action<float, string> funcOnProgress = null, Action<string, float> funcOnError = null)
        {
            if (string.IsNullOrEmpty(hotFixResUrl) || string.IsNullOrEmpty(hotFixVersion))
            {
                Core.Debuger.LogError("热更资源地址为空,异常");
                return;
            }
            this.hotFixResUrl = hotFixResUrl;
            this.hotFixVersion = hotFixVersion;

            luaOnProcessCallback = funcOnProgress;
            luaOnErrorCallback = funcOnError;
            //检测资源
            CheckResource();
        }

        /// <summary>
        /// 资源初始化
        /// </summary>
        public void OnResourceInited()
        {
            ResourceManager.Instance().Initialize_Async(delegate ()
            {
                OnFinished();
            });
        }

        /// <summary>
        /// 资源初始化成功
        /// </summary>
        /// <param name="func">回调函数</param>
        public void InitResourceSuccess(Action func)
        {
            ResourceManager.Instance().Initialize_Async(delegate ()
            {
                UnityEngine.Object.Destroy(InitUIController.Instance.gameObject);

                if (null != func)
                {
                    func();
                }
            });
        }
        /// <summary>
        /// 网络状态切换事件
        /// </summary>
        public static Action ChangeNetState;
        /// <summary>
        /// 网络状态记录
        /// </summary>
        public int Network = 1;
        /// <summary>
        /// 还需要下载的总量
        /// </summary>
        public long Current_Down_Total_Size;
        /// <summary>
        /// 需要热更就开启网络状态监测
        /// </summary>
        /// <returns></returns>
        public IEnumerator RefreshNet()
        {
            int ShowNum = 1;
            while (!UpdateOver)
            {
          
                //无网络时
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                  Network = 0;              
                }

                //WiFi网络时

                if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                 Network = 1;
                }

                //手机流量时
              
                if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                {
                  Network = 2;
                }

                if (Network != ShowNum)
                {
                    if (Network == 2)
                    {
                        //弹出提示4g下载框
                        if (null != ChangeNetState)
                        {
                            ChangeNetState();
                        }
                        float currentSize = OnDowningData_total_size - OnDowningData_current_size;
                        luaOnErrorCallback.Invoke("changeNet", currentSize);
                    }
                    else if (Network == 1)
                    {
                        //自动更新
                        luaOnErrorCallback.Invoke("autoUpdate", 0);
                    }
                    else
                    {
                        //弹出断网提示
                        if (null != ChangeNetState)
                        {
                            ChangeNetState();
                        }
                        float currentSize = OnDowningData_total_size - OnDowningData_current_size;
                        luaOnErrorCallback.Invoke("failed", currentSize);
                    }
                    ShowNum = Network;
                }
                yield return new WaitForSeconds(1);
            }
        }



        #endregion
    }
}