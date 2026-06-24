
using System;
using System.Collections.Generic;
using UnityEngine;

public class StartupAuthorize : MonoBehaviour
{
    private static StartupAuthorize _instance;
    private int CMD_REQUST_ID = 20000;

    private string REQUEST_CONTENT = @"1.  手机/电话权限      
<size=18><color=#666666>     保障账号安全，金豆不怕丢</color></size> 

2.  储存权限
<size=18><color=#666666>     记录欢乐时刻，减少流量消耗</color></size>";

    private string REQUEST_LAB_CANCEL = @"退出";
    private string REQUEST_LAB_OK = @"确定";
    private string REQUEST_JSON = null;
    private string STATUS_JSON = null;
    private float WAIT_MAX_TIME = 0.2f;

    private bool _isReadlyRequest = false;
    private float _readlyTime = 0;

    enum PERMISSION_STATE
    {
        GRANTED = 0,
        DENIED = -1,
        SETTING = -10000,
        CANCEL = -20000,
    }

    List<string> _authorizeList = null;

    private AuthorizeUI _uiAuthorize = null;

    public Action<bool> RequestCallback { set; get; }

    public static StartupAuthorize getInstance()
    {
        if (_instance == null)
        {
            GameObject go = Game.AppGameManager.Instance.gameObject;
            _instance = go.GetComponent<StartupAuthorize>();
            if (_instance == null)
            {
                _instance = go.AddComponent<StartupAuthorize>();
            }
        }
        return _instance;
    }

    private void Awake()
    {
        InitAuthorizeUI();
    }

    public bool Init()
    {
        return true;
    }

    public AuthorizeUI AuthorizeShowUI
    {
        get { return _uiAuthorize; }
    }

    void InitAuthorizeUI()
    {
        if (_uiAuthorize != null)
        {
            return;
        }

        string CANVAS_NAME = "Canvas";
        GameObject uiRoot = GameObject.Find(CANVAS_NAME);
        if (uiRoot == null)
        {
            return;
        }

        string NODE_NAME = "layer_authorize";
        Transform tf = uiRoot.transform.Find(NODE_NAME);
        if (tf == null)
        {
            return;
        }

        _uiAuthorize = tf.gameObject.AddComponent<AuthorizeUI>();
        _uiAuthorize.OnInit();
    }

    private void Start()
    {
        _authorizeList = new List<string>();
        _authorizeList.Add("android.permission.WRITE_EXTERNAL_STORAGE");
        _authorizeList.Add("android.permission.READ_EXTERNAL_STORAGE");
        _authorizeList.Add("android.permission.READ_PHONE_STATE");

        DoCheckStatus();
    }

    private void Update()
    {
        if (_isReadlyRequest)
        {
            _readlyTime -= Time.deltaTime;
            if (_readlyTime < 0)
            {
                _isReadlyRequest = false;
                _readlyTime = WAIT_MAX_TIME;
                DoRequest();
            }
        }
    }

    private void DoCheckStatus()
    {
        if (STATUS_JSON == null)
        {
            LitJson.JsonData data = new LitJson.JsonData();
            data["call_object"] = Game.AppGameManager.NativeCallbackObjectName;
            data["call_method"] = "AuthorizeStatusCallback";
            data["method"] = "doStatus";
            LitJson.JsonData param = new LitJson.JsonData();
            LitJson.JsonData array = new LitJson.JsonData();
            array.SetJsonType(LitJson.JsonType.Array);
            for (int i = 0; i < _authorizeList.Count; i++)
            {
                array.Add(_authorizeList[i]);
            }
            param["permisson_list"] = array;
            data["content"] = param;
            STATUS_JSON = data.ToJson();
            // Debug.LogError("StartupAuthorize:DoCheckStatus:" + STATUS_JSON);
        }

#if (!UNITY_EDITOR && UNITY_ANDROID)
        SdkBridge.getInstance().CallSdkForLua(STATUS_JSON, CMD_REQUST_ID, null);
#else
        DoWinSuccess();
#endif
    }

    private void DoRequest()
    {
        Core.Debuger.Log("AuthorizeUI:DoRequest");
        if (REQUEST_JSON == null)
        {
            LitJson.JsonData data = new LitJson.JsonData();
            data["call_object"] = Game.AppGameManager.NativeCallbackObjectName;
            data["call_method"] = "AuthorizeCallback";
            data["method"] = "doRequest";
            LitJson.JsonData param = new LitJson.JsonData();
            // param["isMust"] = true;
            // param["isSetting"] = false;
            LitJson.JsonData array = new LitJson.JsonData();
            array.SetJsonType(LitJson.JsonType.Array);
            for (int i = 0; i < _authorizeList.Count; i++)
            {
                array.Add(_authorizeList[i]);
            }

            param["permisson_list"] = array;
            data["content"] = param;
            string strJson = data.ToJson();
            Debug.LogError("StartupAuthorize:DoRequest:" + strJson);

            REQUEST_JSON = strJson;
        }
#if (!UNITY_EDITOR && UNITY_ANDROID)
        SdkBridge.getInstance().CallSdkForLua(REQUEST_JSON, CMD_REQUST_ID, null);
#else
        DoWinSuccess();
#endif
    }

    private void GotoSetting()
    {
        LitJson.JsonData data = new LitJson.JsonData();
        data["call_object"] = Game.AppGameManager.NativeCallbackObjectName;
        data["call_method"] = "AuthorizeCallback";
        data["method"] = "doSetting";

        LitJson.JsonData param = new LitJson.JsonData();
        LitJson.JsonData array = new LitJson.JsonData();
        array.SetJsonType(LitJson.JsonType.Array);
        for (int i = 0; i < _authorizeList.Count; i++)
        {
            array.Add(_authorizeList[i]);
        }
        param["permisson_list"] = array;
        data["content"] = param;
        string strJson = data.ToJson();
        Debug.LogError("StartupAuthorize:GotoSetting:" + strJson);
        ShowAuthorizeUI();
#if (!UNITY_EDITOR && UNITY_ANDROID)
        SdkBridge.getInstance().CallSdkForLua(strJson, CMD_REQUST_ID, null);
#else
#endif
    }

    private void ShowAuthorizeUI(string strContent = null)
    {
        if (_uiAuthorize == null)
        {
            return;
        }

        _uiAuthorize.SetContent(strContent == null ? REQUEST_CONTENT : strContent);
        _uiAuthorize.SetCancelCall(() =>
        {
            Application.Quit();
        }, REQUEST_LAB_CANCEL);

        _uiAuthorize.SetOKCall(() =>
        {
            _isReadlyRequest = true;
            _readlyTime = WAIT_MAX_TIME;
        }, REQUEST_LAB_OK);

        _uiAuthorize.ShowUI();
    }

    private void DoWinSuccess()
    {
        LitJson.JsonData data = new LitJson.JsonData();
        data["cmd"] = CMD_REQUST_ID;
        LitJson.JsonData param = new LitJson.JsonData();
        param = new LitJson.JsonData();
        param["code"] = 0;
        param["isEnable"] = true;
        param["method"] = "doRequest";
        data["content"] = param;
        AuthorizeCallback(data.ToJson());

        //// string test = "{\"content\":{\"code\":0,\"isEnable\":false,\"method\":\"doStatus\"}, \"cmd\":\"20000\"}";
        //// AuthorizeStatusCallback(test);
        // ShowAuthorizeUI();
    }

    private void DoRequestCallback(bool value)
    {
        if (value && _uiAuthorize)
        {
            _uiAuthorize.CloseUI();
        }
        if (RequestCallback != null)
        {
            RequestCallback(value);
        }
    }

    private void AuthorizeSettingCall(string json)
    {
    }

    private void AuthorizeStatusCallback(string json)
    {
        if (json == null)
        {
            return;
        }

        bool bResult = false;
        try
        {
            LitJson.JsonData jd = LitJson.JsonMapper.ToObject(json);
            Core.Debuger.LogError("AuthorizeCallback:" + json);
            LitJson.JsonData jcontent = LitJson.JsonMapper.ToObject(jd["content"].ToJson());
            bool.TryParse(jcontent["isEnable"].ToString(), out bResult);
        }
        catch (System.Exception ex)
        {
            Core.Debuger.LogError("AuthorizeCallback:" + ex.Message);
        }
        finally
        {
            if (bResult)
            {
                DoRequestCallback(true);
            }
            else
            {
                ShowAuthorizeUI();
            }
        }
    }

    private void AuthorizeCallback(string json)
    {
        if (json == null)
        {
            return;
        }

        bool bResult = false;
        bool isMust = false;
        string message = null;
        int code = -1;
        try
        {
            LitJson.JsonData jd = LitJson.JsonMapper.ToObject(json);
            Core.Debuger.Log("AuthorizeCallback:" + json);
            int cmd = 0;
            int.TryParse(jd["cmd"].ToString(), out cmd);
            isMust = cmd == CMD_REQUST_ID;
            LitJson.JsonData jcontent = LitJson.JsonMapper.ToObject(jd["content"].ToJson());
            Core.Debuger.Log("AuthorizeCallback:" + jcontent["code"]);
            int.TryParse(jcontent["code"].ToString(), out code);
            bResult = code == 0;
            if (jcontent.Keys.Contains("message"))
            {
                message = jcontent["message"].ToString();
            }
        }
        catch (System.Exception ex)
        {
            Core.Debuger.LogError("AuthorizeCallback:" + ex.Message);
        }
        finally
        {
            if (!isMust)
            {
                DoRequestCallback(bResult);
            }
            else
            {
                DoMustCallback(code, message);
            }
        }
    }

    private void DoMustCallback(int code, string message)
    {
        bool bResult = code == 0;
        if (bResult)
        {
            DoRequestCallback(true);
            return;
        }

        if ((int)PERMISSION_STATE.SETTING == code)
        {
            GotoSetting();
            return;
        }

        if (message == null)
        {
            ShowAuthorizeUI();
            return;
        }

        string subString = message.Replace("[", "");
        subString = subString.Replace("]", "");
        string[] arr = subString.Split(',');
        if (arr == null || arr.Length == 0)
        {
            ShowAuthorizeUI();
            return;
        }
        if (arr.Length == _authorizeList.Count)
        {
            ShowAuthorizeUI();
            return;
        }

        string strContent = null;
        if (message.IndexOf("READ_PHONE_STATE") > 0)
        {
            strContent = @"储存权限
<size=18><color=#666666> 记录欢乐时刻，减少流量消耗</color></size>";
            ShowAuthorizeUI(strContent);
            return;
        }

        strContent = @"手机/电话权限      
<size=18><color=#666666>  保障账号安全，金豆不怕丢</color></size>";
        ShowAuthorizeUI(strContent);
    }
}