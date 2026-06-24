using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthorizeUI : MonoBehaviour {

    private Text _labAdapt = null;
    private Text _labTitle = null;
    private Text _labContent = null;
    private Button _btnCancel = null;
    private Text _labCancel = null;
    private Button _btnOk = null;
    private Text _labOk = null;

    private Action _okCall = null;
    private Action _cancelCall = null;

    public void OnInit() {
        // 初始化成员
        InitMember();
        // 设置回调
        SetCallback();
        gameObject.SetActive(false);
    }


    void InitMember() {
        GetChild("frame/text_adapt", out _labAdapt);
        GetChild("frame/text_adapt/text_title", out _labTitle);
        GetChild("frame/text_adapt/text_detail", out _labContent);
        GetChild("frame/text_adapt/block_button/btn_left", out _btnCancel);
        GetChild("frame/text_adapt/block_button/btn_right", out _btnOk);
        GetChild("frame/text_adapt/block_button/btn_left/Text", out _labCancel);
        GetChild("frame/text_adapt/block_button/btn_right/Text", out _labOk);

        SetContent("");
    }

    void SetCallback() {
        if (null != _btnCancel) {
            _btnCancel.onClick.AddListener(OnClickCancel);
        }
        if (null != _btnOk) {
            _btnOk.onClick.AddListener(OnClickOK);
        }
    }


    public void SetTitle(string strValue) {
        if (strValue == null || null == _labTitle) {
            return;
        }
        _labTitle.text = strValue;
    }

    public void SetContent(string strValue) {
        if (strValue == null || null == _labContent)
        {
            return;
        }

        if (null != _labAdapt) {
            _labAdapt.text = strValue;
        }
        _labContent.text = strValue;
    }

    public void SetLabCancel(string strValue) {
        if (null == _labCancel || null == strValue) {
            return;
        }
        _labCancel.text = strValue;
    }

    public void SetLabOK(string strValue) {
        if (null == _labOk || null == strValue)
        {
            return;
        }
        _labOk.text = strValue;
    }

    public void SetCancelCall(Action call, string strValue) {
        _cancelCall = call;
        SetLabCancel(strValue);
    }

    public void SetOKCall(Action call, string strValue) {
        _okCall = call;
        SetLabOK(strValue);
    }

    public void ShowUI() {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        Core.Debuger.Log("AuthorizeUI:ShowUI");
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
        ClearCall();
        Core.Debuger.Log("AuthorizeUI:CloseUI");
    }


    private void OnClickOK() {
        if (null != _okCall) {
            _okCall();
        }
        Core.Debuger.Log("AuthorizeUI:OnClickOK");
        CloseUI();
    }

    private void OnClickCancel() {
        if (null != _cancelCall)
        {
            _cancelCall();
        }
        CloseUI();
        Core.Debuger.Log("AuthorizeUI:OnClickCancel");
    }

    private void ClearCall() {
        _okCall = null;
        _cancelCall = null;
    }


    /// <summary>
    /// 获取节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="strName"></param>
    /// <param name="t"></param>
    void GetChild<T>(string strName, out T t) {
        var child = transform.Find(strName);
        if (null != child)
        {
            t = child.gameObject.GetComponent<T>();
        }
        else {
            t = transform.GetComponent<T>();
        }
    }
}

