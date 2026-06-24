/***************************************************
 * 文件名：Loom.cs
 * 描  述：
 * 时  间：2018-12-15 15:07:31
 * 作  者：武浩頎
 * 修  改：
 ***************************************************/

using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;  
using System;  
using System.Threading;  
using System.Linq;  

public class LoomMain : MonoBehaviour  
{  
    public static int maxThreads = 5;  
    public static int numThreads = 0;

    private static LoomMain _current;
    public static LoomMain Current  
    {  
        get  
        {  
            Initialize();  
            return _current;  
        }  
    }  
    static bool initialized;  

    //####作为初始化方法自己调用，可在初始化场景调用一次即可
    public static void Initialize()  
    {  
        if (!initialized)  
        {  

            if(!Application.isPlaying)  
                return;  
            initialized = true;  
            GameObject g = new GameObject("LoomMain");  
            //####永不销毁
            DontDestroyOnLoad (g);
            _current = g.AddComponent<LoomMain>();  
        }  

    }  

    private List<Action> _actions = new List<Action>();  
    public struct DelayedQueueItem  
    {  
        public float time;  
        public Action action;  
    }  
    private List<DelayedQueueItem> _delayed = new  List<DelayedQueueItem>();  

    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();  

    public static void QueueOnMainThread(Action action)  
    {  
        QueueOnMainThread( action, 0f);  
    }  
    public static void QueueOnMainThread(Action action, float time)  
    {  
        if(time != 0)  
        {
            if (Current != null)
            {
                lock (Current._delayed)
                {
                    Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
        }  
        else  
        {
            if (Current != null)
            {
                lock (Current._actions)
                {
                    Current._actions.Add(action);
                }
            }
        }  
    }  

    public static Thread RunAsync(Action a)  
    {  
        Initialize();  
 
        Interlocked.Increment(ref numThreads);  
        ThreadPool.QueueUserWorkItem(RunAction, a);  
        return null;  
    }  

    private static void RunAction(object action)  
    {  
        try  
        {  
            ((Action)action)();  
        }  
        catch  
        {  
        }  
        finally  
        {  
            Interlocked.Decrement(ref numThreads);  
        }  

    }  


    void OnDisable()  
    {  
        if (_current == this)  
        {  

            _current = null;  
        }  
    }  



    // Use this for initialization  
    void Start()  
    {  

    }  

    List<Action> _currentActions = new List<Action>();  

    // Update is called once per frame  
    void Update()  
    {  
        lock (_actions)  
        {  
            _currentActions.Clear();  
            _currentActions.AddRange(_actions);  
            _actions.Clear();  
        }  
        foreach(var a in _currentActions)  
        {  
            a();  
        }  
        lock(_delayed)  
        {  
            _currentDelayed.Clear();  
            _currentDelayed.AddRange(_delayed.Where(d=>d.time <= Time.time));  
            foreach(var item in _currentDelayed)  
                _delayed.Remove(item);  
        }  
        foreach(var delayed in _currentDelayed)  
        {  
            delayed.action();  
        }  



    }  
}  


