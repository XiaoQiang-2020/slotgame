/***************************************************
 * 鏂囦欢鍚嶏細 EasyThread.cs
 * 鎻? 杩帮細
 * 鏃? 闂达細 2017-04-27 09:35:41
 * 浣? 鑰咃細 鏉庢櫤娴?
 * 淇? 鏀癸細
 ***************************************************/
using System;
using System.Threading;

/// <summary>
/// 绠€鍗曠殑绾跨▼绫?
/// </summary>
public class EasyThread
{
    Thread m_Thread;
    /// <summary>
    /// 绾跨▼鏂规硶
    /// </summary>
    public Action m_CallBack;
    /// <summary>
    /// 绾跨▼id
    /// </summary>
    static int workerId = 0;

    public bool isOnce { set; get; }

    protected bool m_isCalled = false;

    /// <summary>
    /// 鏋勯€犳柟娉?
    /// </summary>
    /// <param name="cb">绾跨▼鏂规硶</param>
    /// <param name="priority">绾跨▼浼樺厛绾?/param>
    public EasyThread(Action cb, ThreadPriority priority = ThreadPriority.Normal)
    {
        m_Thread = new Thread(Run);
        m_CallBack = cb;
        m_Thread.Priority = priority;
        m_Thread.IsBackground = true;

        workerId++;
        UnityEngine.Debug.LogFormat("ThreadId is {0}", workerId);
#if UNITY_EDITOR
        //if(Game.GameControl.Instance!=null)
        {
            if(hasAddCallback==false)
            {
                hasAddCallback = true;
                Game.AppGameManager.EditorApplicationQuitHandler += OnEditorApplicationQuitHandler;
            }
            threadList.Add(m_Thread);
        }
#endif
    }

    /// <summary>
    /// 鏄惁娲荤潃
    /// </summary>
    /// <returns></returns>
    public bool IsAlive()
    {
        return m_Thread.IsAlive;
    }

    /// <summary>
    /// 鍚姩绾跨▼
    /// </summary>
    public void Start()
    {
        m_Thread.Start();
    }

    public void Stop()
    {
        m_Thread.Abort();
    }
    /// <summary>
    /// 鎵ц绾跨▼鏂规硶
    /// </summary>
    virtual protected void Run()
    {
        if (m_isCalled && this.isOnce)
        {
            Stop();
            return;
        }
        m_isCalled = true;
        if (null != m_CallBack)
        {
            m_CallBack();
        }
        
    }


#if UNITY_EDITOR
    bool hasAddCallback = false;
    System.Collections.Generic.List<Thread> threadList = new System.Collections.Generic.List<Thread>();
    /// <summary>
    /// 褰撶紪杈戝櫒鍋滄杩愯杩涳紝鍋滄帀鎵€鏈夌殑瀛愮嚎绋?
    /// </summary>
    void OnEditorApplicationQuitHandler()
    {
        foreach(var t in threadList)
        {
            t.Abort();
        }
        threadList.Clear();
    }

#endif
}


