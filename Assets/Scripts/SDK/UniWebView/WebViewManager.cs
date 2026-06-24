
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
public class WebViewManager{
    private static WebViewManager instance;
    public static WebViewManager Instance()
    {
        if (instance == null)
        {
            instance = new WebViewManager();
        }
        return instance;
    }
    private Action<UniWebView, bool, string> OnLoadCompleteAction;
    private Action<UniWebView, UniWebViewMessage> ReceivedMessageAction;
    private Action<UniWebView, string> EvalJavaScriptFinishedAction;
    public UniWebView webView;
    public WebViewManager()
    {
       
    }
    public UniWebView CreateWebView(int topValue, int leftValue, int bottomValue, int rightValue)
    {
        var webViewGameObject = GameObject.Find("WebView");
        if (webViewGameObject == null)
        {
            webViewGameObject = new GameObject("WebView");
        }

        webView = webViewGameObject.AddComponent<UniWebView>();
        webView.OnWebViewShouldClose += (webView) =>
        {
            GameObject.Destroy(webView);
            webView = null;
            return true;
        };
        webView.OnLoadComplete += OnLoadComplete;
        webView.OnReceivedMessage += OnReceivedMessage;
        webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
        webView.toolBarShow = true;
        webView.insets = new UniWebViewEdgeInsets(topValue, leftValue, bottomValue, rightValue);
        return webView;
    }

    public void ShowWebView(string url,Action<UniWebView, bool, string> LoadCompleteCb,Action<UniWebView , UniWebViewMessage > ReceivedMessageCb,Action<UniWebView, string> EvalJavaScriptFinishedCb, bool ShowAfterLoaded = false)
    {
        if (webView == null)
        {
            return;
        }
        ReceivedMessageAction = ReceivedMessageCb;
        EvalJavaScriptFinishedAction = EvalJavaScriptFinishedCb;
        if (!ShowAfterLoaded)
        {
            LoadWebView(url, LoadCompleteCb);
            webView.Show();
        }
        else
        {
            //加载完毕之后再显示
            LoadWebView(url, (UniWebView view, bool succ, string eMsg)=> {
                if (webView)
                {
                    webView.Show();
                    if (LoadCompleteCb != null)
                    {
                        LoadCompleteCb.Invoke(view, succ, eMsg);
                    }
                }
            });
        }

    }

    public void LoadWebView(string url, Action<UniWebView, bool, string> loadCompleteCb)
    {
        if(webView == null)
        {
            return;
        }
        OnLoadCompleteAction = loadCompleteCb;
        webView.url = url;
        webView.Load();
    }

    public void DestroyWebView()
    {
        if (webView)
        {
            webView.Hide();
            GameObject.Destroy(webView);
            webView = null;
        }
        
    }

    public void HideWebView()
    {
        if (webView)
        {
            webView.Hide();
        }
    }

    public void ResumeWebView()
    {
        if (webView)
        {
            webView.Show();
        }
    }

    void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
    {
        if (OnLoadCompleteAction != null)
        {
            OnLoadCompleteAction(webView, success, errorMessage);
        }
       
    }
    void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
    {
        if (ReceivedMessageAction != null)
        {
            ReceivedMessageAction(webView, message);
        }
    }
    public void RunScript(string js)
    {
        if (webView == null)
        {
            
            return;
        }

        // Execute the JavaScript. The result will be returned in OnEvalJavaScriptFinished event.
        webView.EvaluatingJavaScript(js);
    }

    void OnEvalJavaScriptFinished(UniWebView webView, string recive)
    {
        if (EvalJavaScriptFinishedAction != null)
        {
            EvalJavaScriptFinishedAction(webView, recive);
        }
    }
}

#endif