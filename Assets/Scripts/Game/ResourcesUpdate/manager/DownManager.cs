using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using Game;
using UnityEngine.Networking;
namespace HotUpdate
{
    public enum DownLoadStateEnum
    {
        DOWNLOADNG_NOTDEAL = 0,
        DOWNLOADING = 1,
        DOWNLOAD_RETRY = 2,
        DOWNLOAD_SUCCESS = 3,
        DOWNLOAD_FAIL = 4
    }
    public class DownManager : MonoBehaviour
    {
        static DownManager _instance;
        public static DownManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    GameObject obj = new GameObject("DownManager");
                    _instance = obj.AddComponent<DownManager>();
                }
                return _instance;
            }
        }
        public const int MaxRetryTimes = 3;

        public bool DownLoadComplete = true;
        /// <summary>
        /// 当前空闲的线程id
        /// </summary>
        private int CurrentFreeThreadID;
        /// <summary>
        /// 可以下载
        /// </summary>
        // private bool DownOneFile;
        /// <summary>
        /// 当前取得的进度
        /// </summary>
        private int CurrentProgress;
        /// <summary>
        /// 需要下载的文件数
        /// </summary>
        private int DownListCount;
        /// <summary>
        /// 需要下载的列表
        /// </summary>
        public Dictionary<int, DownLoadUnit> downList = new Dictionary<int, DownLoadUnit>();
        public static object lockobj = new object();
        /// <summary>
        /// 当前下载文件id
        /// </summary>
        private int currentDownNum = 1;
        /// <summary>
        /// 初始化下载列表
        /// </summary>
        /// <param name="needDownList"></param>
        private Action<int, int> currentPorgressEvent;
        private Action<float> changeNetStateEvent;
        private Action<string> completeEvent;
        public float NetNumIndex = 50.0f;
        public void Init(List<Game.Downloader.DownloadUnit> needDownList)
        {
            //GetDownLoadData(needDownList);

        }
        public void InitStr(string strData, string updateRootUrl, string saveRootPath, Action<int, int> progress, Action<float> changeNetState, Action<string> complete)
        {
            DownLoadComplete = false;
            downList.Clear();
            string jsonData = @"{""downList"":" + strData + "}";
            ResDownList resList = JsonUtility.FromJson<ResDownList>(jsonData);
            DownLoadUnit newUnit = null;
            int unitId = 0;
            for (int i = 0; i < resList.downList.Count; i++)
            {
                ResItem resServerItem = resList.downList[i];
                string downUrl = updateRootUrl + "/" + resServerItem.path;
                string savePath = saveRootPath + "/" + resServerItem.path;
                unitId = i + 1;
                newUnit = new DownLoadUnit(unitId, savePath, resServerItem.size, downUrl, resServerItem);
                downList.Add(unitId, newUnit);
            }
            currentDownNum = 1;
            CurrentProgress = 0;
            CurrentTotalSize = 0;
            DownListCount = resList.downList.Count;
            Log.LogDebug.Print("========开始的下载数量=========" + DownListCount);
            currentPorgressEvent = progress;
            changeNetStateEvent = changeNetState;
            completeEvent = complete;
            CheckNetWorkManager.Instance.Init();
            CheckNetWorkManager.Instance.ChangeNetState += ChangeNetState;
            //ThreadManager.Instance.Init();
            // HotThreadManager.Instance.StartDownloadThread(downList,progress,complete);
        }
        /// <summary>
        /// 下载资源列表
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        /// <param name="callBackError"></param>
        public void DownResList(string url, Action<string> callBack, Action<string> callBackError)
        {
            StartCoroutine(OnUpdateResource(url, callBack, callBackError));
        }
        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        /// <param name="callBackError"></param>
        /// <returns></returns>
        IEnumerator OnUpdateResource(string url, Action<string> callBack, Action<string> callBackError)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                if (null != callBackError)
                {
                    callBackError(www.error);
                }
                yield break;
            }
            else
            {
                if (null != callBack)
                {
                    callBack(www.downloadHandler.text);
                }
            }
        }
        private void Update()
        {
            // if (DownOneFile)
            // {
            //     if (CurrentProgress < DownListCount)
            //     {
            //         if (currentPorgressEvent != null) currentPorgressEvent(CurrentProgress,DownListCount);
            //     }

            //     if (CurrentProgress >= DownListCount)
            //     {
            //         Log.LogDebug.Print("========结束的时候下载数量=========" + CurrentProgress);
            //         //结束
            //         if (currentPorgressEvent != null) currentPorgressEvent(CurrentProgress,DownListCount);
            //         ThreadManager.Instance.ClearAllThread();
            //     }

            //     DownOneFile = false;
            // }
            // if(!DownLoadComplete)
            // {
            //     if (CurrentFreeThreadID > 0)
            //     {
            //         DispatchDownTask(CurrentFreeThreadID);
            //         CurrentFreeThreadID = 0;
            //     }
            // }
            //if ((int)Time.time % 3 == 0)
            //{
            //    if(isCheck)
            //    {
            //        isCheck = false;
            //        Debug.LogError(Time.time + "time:" + CurrentTotalSize / Time.time);
            //        if (CurrentTotalSize / Time.time < NetNumIndex)
            //        {
            //            WeakNetState();
            //        }
            //    }
            //}
            //else
            //{
            //    isCheck = true;
            //}
        }
        // private bool isCheck = false;
        /// <summary>
        /// 派发下载任务
        /// </summary>
        /// <param name="threadid"></param>
        public void DispatchDownTask(int threadid)
        {
            DownThread downThread = ThreadManager.Instance.GetThreadByID(threadid);

            DownLoadUnit failUnit = GetOneRetryUnit();
            if (failUnit != null)
            {
                Log.LogDebug.Print(threadid + "========重新下载文件=========" + failUnit.downUrl + "  重新下载次数：" + failUnit.retryTimes);
                downThread.m_downUnit = failUnit;
                //downThread.m_downUnit.downState = DownLoadStateEnum.DOWNLOADING;
                ThreadManager.Instance.StartThreadByID(threadid);
            }
            else
            {

                if (downList.ContainsKey(currentDownNum))
                {
                    Log.LogDebug.Print(threadid + "========当前派发任务111id=========" + currentDownNum);
                    downThread.m_downUnit = downList[currentDownNum];
                    //downThread.m_downUnit.downState = DownLoadStateEnum.DOWNLOADING;
                    ThreadManager.Instance.StartThreadByID(threadid);
                    currentDownNum++;
                }
                else
                {

                    Log.LogDebug.Print("========当前派发任务2222222222222222222222id=========" + currentDownNum);
                    if (CheckDownLoadEnd())
                    {
                        DownLoadComplete = true;
                        ResDownList listobj = new ResDownList();
                        List<DownLoadUnit> downloadFailList = GetDownloadFailList();
                        for (int i = 0; i < downloadFailList.Count; i++)
                        {
                            listobj.downList.Add(downloadFailList[i].resItem);
                        }
                        string returnStr = JsonUtility.ToJson(listobj);
                        completeEvent(returnStr);
                    }
                }


            }
        }
        public List<DownLoadUnit> GetDownloadFailList()
        {
            List<DownLoadUnit> failList = new List<DownLoadUnit>();
            foreach (KeyValuePair<int, DownLoadUnit> kp in downList)
            {
                if (kp.Value.downState == DownLoadStateEnum.DOWNLOAD_FAIL)
                {
                    failList.Add(kp.Value);
                }
            }
            return failList;
        }
        public DownLoadUnit GetOneRetryUnit()
        {

            foreach (KeyValuePair<int, DownLoadUnit> kp in downList)
            {
                if (kp.Value.downState == DownLoadStateEnum.DOWNLOAD_RETRY)
                {
                    return kp.Value;
                }
            }
            return null;
        }
        public bool CheckDownLoadEnd()
        {
            bool isEnd = true;
            foreach (KeyValuePair<int, DownLoadUnit> kp in downList)
            {
                if (kp.Value.downState != DownLoadStateEnum.DOWNLOAD_SUCCESS && kp.Value.downState != DownLoadStateEnum.DOWNLOAD_FAIL)
                {
                    Debug.LogError(kp.Value.id + "kp.Value.id =====" + kp.Value.downState);
                    isEnd = false;
                }
            }
            return isEnd;
        }
        private void OnEnable()
        {
            ThreadManager.Instance.FreeDownThread += HaveFreeThread;
        }

        /// <summary>
        /// 进度监听
        /// </summary>
        void DownProgress()
        {
            // DownOneFile = true;
            CurrentProgress++;
        }
        /// <summary>
        /// 空闲线程监听
        /// </summary>
        /// <param name="freeThreadId"></param>
        void HaveFreeThread(int freeThreadId)
        {
            CurrentFreeThreadID = freeThreadId;
        }
        public void OnDisable()
        {
            ThreadManager.Instance.FreeDownThread -= HaveFreeThread;
        }

        // /// <summary>
        // /// 获取下载数据
        // /// </summary>
        // public void GetDownLoadData(List<Game.Downloader.DownloadUnit> needDownList)
        // {
        //     DownLoadUnit newUnit = null;
        //     for (int i = 0; i < needDownList.Count; i++)
        //     {
        //         int unitId = i + 1;
        //         Game.Downloader.DownloadUnit oldUnit = needDownList[i];
        //         newUnit = new DownLoadUnit(unitId, oldUnit.savePath, oldUnit.size, oldUnit.downUrl,oldUnit);
        //         downList.Add(unitId,newUnit);
        //     }
        //     DownListCount = downList.Count;
        // }
        private float CurrentTotalSize;
        /// <summary>
        /// 检测一个文件是否真的下载完
        /// </summary>
        /// <param name="unitId">下载单元id</param>
        public void CheckDownUnitFinish(int unitId)
        {
            if (downList.ContainsKey(unitId))
            {
                DownLoadUnit unit = downList[unitId];
                if (unit.checkState == false)
                {
                    if (unit.downState == DownLoadStateEnum.DOWNLOAD_SUCCESS || unit.downState == DownLoadStateEnum.DOWNLOAD_FAIL)
                    {
                        CurrentTotalSize = CurrentTotalSize + unit.size;
                        unit.checkState = true;
                        DownProgress();
                    }
                }
            }
        }
        public void OnResourceInited()
        {
            ResourceManager.Instance().Initialize_Async(delegate ()
            {
                UnityEngine.GameObject.Find("");
                //OnFinished();
            });
        }
        private void OnFinished()
        {
            UnityEngine.Object.Destroy(InitUIController.Instance.gameObject);
            // 启动Lua主程序，登录前的下载才需要以下代码
            // LuaManager.Instance().ReInitLua();
        }

        private void ChangeNetState(NET_STATE netState)
        {
            if (netState == NET_STATE.NotReachable)
            {
                ThreadManager.Instance.MyCheckThread.SuspendCheckThread();
                if (changeNetStateEvent != null)
                {
                    changeNetStateEvent(0);
                }
            }
            else
            {
                ThreadManager.Instance.MyCheckThread.ResumeCheckThread();
                if (changeNetStateEvent != null)
                {
                    changeNetStateEvent(1);
                }
            }
        }

        public void WeakNetState()
        {
            if (changeNetStateEvent != null)
            {
                changeNetStateEvent(2);
            }
        }

        private void OnDestroy()
        {
            CheckNetWorkManager.Instance.ChangeNetState -= ChangeNetState;
            ThreadManager.Instance.ClearAllThread();
            CheckNetWorkManager.Instance.SetRefreshNetOver();
        }   /// <summary>
            /// 当前是否有网络
            /// </summary>
            /// <returns></returns>
        public bool CurrentNetState()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }
            return true;
        }
    }
    [System.Serializable]
    public class ResDownList
    {
        public List<ResItem> downList = new List<ResItem>();
    }
}