using UnityEngine;
using UnityEngine.UI;


public class InitUILaunch : MonoBehaviour
{
    Slider slider_Progress = null;  // 加载资源进度条
    Text text_Progress = null;
    Text text_tip = null;
    GameObject progress;

    void Awake ()
    {
        slider_Progress = Game.CommonTool.Get<Slider>(gameObject, "progress/slider_progress");
        text_Progress = Game.CommonTool.Get<Text>(gameObject, "progress/text_progress");
        text_tip = Game.CommonTool.Get<Text>(gameObject, "progress/text_tip");
        progress = Game.CommonTool.Child(gameObject, "progress");

        if ( slider_Progress == null ||
            text_Progress == null ||
            text_tip == null)
        {
            Debug.LogError("UILaunch面板初始化失败");
            gameObject.SetActive(false);
            return;
        }
        //删除启动页
        GameObject splashLaunchGameObj = GameObject.Find("SplashCamera");
        if (null != splashLaunchGameObj)
        {
            GameObject.Destroy(splashLaunchGameObj);
        }
        SetProgess(0f, "正在初始化资源，不需要流量，请稍等...");
    }

    /// <summary>
    /// 设置进度
    /// </summary>
    /// <param name="process">0~1</param>
    /// <param name="tip">提示文字</param>
	public void SetProgess(float process, string tip="")
    {
        process = Mathf.Clamp01(process);
        slider_Progress.value = process;
        text_Progress.text = (process*100).ToString("F1") + "%";
        if (!string.IsNullOrEmpty(tip))
        {
            text_tip.text = tip;
        }
    }

    public void SetProgessActive(bool bActive)
    {
        progress.SetActive(bActive);
    }
}

