/***************************************************
 * 文件名： UIAnimationCall.cs
 * 描  述： UI类Animation动画事件的回调脚本
 * 时  间： 2017-05-23 20:16:15
 * 作  者： 李海波
 * 修  改：
 ***************************************************/
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationCall : MonoBehaviour
{

    /// <summary>
    /// 动画阶段 0:开始，-1:结束，-1000：被销毁 大于等于1：过程中 
    /// </summary>
    private int m_aniStage = 0;

    /// <summary>
    /// UID 标识当前动画对象
    /// </summary>
    private long m_uid = 0;

    /// <summary>
    /// 扩展信息方便扩展 存储json信息
    /// </summary>
    private string m_extInfo = null;

    /// <summary>
    /// 动画组件
    /// </summary>
    Animator m_cmpAnimator = null;

    /// <summary>
    /// 动画控制器
    /// </summary>
    AnimatorOverrideController m_cmAnimatorCtr;

    /// <summary>
    /// 事件定义
    /// </summary>
    [Serializable]
    class AniEventInfo : System.Object
    {
        public int intParameter;
        public float floatParameter;
        public string stringParameter;
        public long uid;
        public string extInfo;
    }
    AniEventInfo m_eventInfo = null;

    void Awake()
    {
        // 获取动画组件
        m_cmpAnimator = GetComponent<Animator>();
        if (null != m_cmpAnimator)
        {
            m_cmAnimatorCtr = new AnimatorOverrideController(m_cmpAnimator.runtimeAnimatorController);
            m_cmpAnimator.runtimeAnimatorController = m_cmAnimatorCtr;
        }
        m_eventInfo = new AniEventInfo();

    }

    void Start()
    {
    }

    void OnDestroy()
    {
        // 如果动画播放没有结束被销毁直接标识动画结束
        if (m_aniStage >= 0)
        {
            // -1000 目前先代表标识动画被销毁状态用于区别
            m_aniStage = -1000;
            m_eventInfo.intParameter = m_aniStage;
            NotifyCallback();
        }
    }

    /// <summary>
    /// 回到到lua
    /// </summary>
    void NotifyCallback()
    {
        string jsonString = null;
        try
        {
            m_eventInfo.uid = m_uid;
            m_eventInfo.extInfo = m_extInfo;
            jsonString = JsonUtility.ToJson(m_eventInfo, true);
        }
        catch (System.Exception ex)
        {
            Core.Debuger.LogError("notifyToLua：fail", ex.Message);
        }
        // Game.LuaManager.Instance().UiAniCallBack(jsonString);
        //CallMethod("Method.UiAniEventCall", jsonString);//
        //todo 通过事件回调
    }

    /// <summary>
    /// UI动画事件回调
    /// </summary>
    /// <param name="index"></param>
	public void AniUICall(int intParameter)
    {
        m_eventInfo.intParameter = intParameter;
        m_aniStage = intParameter;
        NotifyCallback();
    }

    /// <summary>
    /// 动画控制器
    /// </summary>
    public AnimatorOverrideController OverrideController
    {
        set { m_cmAnimatorCtr = value; }
        get { return m_cmAnimatorCtr; }
    }
    public long UID { set { m_uid = value; } }
    public string ExtraInfor { set { m_extInfo = value; } }

}

