

using System.Collections;
using UnityEngine;
using System;
namespace HotUpdate
{
    public class CheckNetWorkManager : MonoBehaviour
    {
        public Action<NET_STATE> ChangeNetState;
        static CheckNetWorkManager _instance;
        public static CheckNetWorkManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    GameObject obj = new GameObject("CheckNetWorkManager");
                    _instance = obj.AddComponent<CheckNetWorkManager>();
                }
                return _instance;
            }
        }
        /// <summary>
        /// 网络状态
        /// </summary>
        private NET_STATE NetworkState = NET_STATE.ReachableViaLocalAreaNetwork;
        /// <summary>
        /// 停止检测
        /// </summary>
        private bool IsOver = false;
        /// <summary>
        /// 启动网络状态监测
        /// </summary>
        public void Init()
        {
            StartCoroutine(RefreshNet());
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.A))
            {
                NetworkState = NET_STATE.NotReachable;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                NetworkState = NET_STATE.ReachableViaCarrierDataNetwork;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                NetworkState = NET_STATE.ReachableViaLocalAreaNetwork;
            }
#endif
        }
     
        /// <summary>
        /// 结束网络监测
        /// </summary>
        public void SetRefreshNetOver()
        {
            IsOver = true;
            Destroy(gameObject);
        }
        /// <summary>
        /// 刷新网络监测
        /// </summary>
        /// <returns></returns>
        public IEnumerator RefreshNet()
        {
            NET_STATE ShowNum = NET_STATE.ReachableViaLocalAreaNetwork;
            while (!IsOver)
            {
                //无网络时
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    NetworkState = NET_STATE.NotReachable;
                }

                //手机流量时
                if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                {
                    NetworkState = NET_STATE.ReachableViaCarrierDataNetwork;
                }

                //WiFi网络时
                if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    NetworkState = NET_STATE.ReachableViaLocalAreaNetwork;
                }
                // 更新网络
                if (NetworkState != ShowNum)
                {
                    //网络发生变化
                    ShowNum = NetworkState;
                    if (ChangeNetState != null)
                    {
                        ChangeNetState(NetworkState);
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }
    }

    public enum NET_STATE
    {
        NotReachable = 0,//无网络
        ReachableViaCarrierDataNetwork = 1,//4g
        ReachableViaLocalAreaNetwork = 2,//wifi
    }
}