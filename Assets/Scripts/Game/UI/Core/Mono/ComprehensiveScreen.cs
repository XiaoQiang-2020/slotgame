/***************************************************
 * 文件名：ComprehensiveScreen.cs
 * 描  述：全面屏适配
 * 时  间：2018-02-10 16:35:28
 * 作  者：李海波
 * 修  改：
 ***************************************************/

using UnityGamObject = UnityEngine.GameObject;
using UnityVector3 = UnityEngine.Vector3;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRectTransform = UnityEngine.RectTransform;
using UnityCavasScaler = UnityEngine.UI.CanvasScaler;
using UnityEngine;

public class ComprehensiveScreen : UnityMonoBehaviour{

    #region 成员变量
    /// <summary>
    /// 缩放尺寸
    /// </summary>
    private static UnityVector3 m_adaptScale = UnityVector3.zero;

    /// <summary>
    /// 是否需要在Awake启动
    /// </summary>
    [SerializeField, HeaderAttribute("是否需要在Awake中启动?")]
    public bool needAwake = true;
    #endregion

    private void Awake()
    {
        if (!needAwake)
        {
            return;
        }

        m_adaptScale = UnityVector3.zero;
        if (UnityVector3.zero == m_adaptScale)
        {
            CalculateScale();
            if (UnityVector3.zero == m_adaptScale)
            {
                return;
            }
        }

        UnityRectTransform nodeRect = GetComponent<UnityRectTransform>();
        if (null == nodeRect)
        {
            return;
        }
        nodeRect.localScale = m_adaptScale;
    }

    private void Start()
    {
        if (needAwake)
        {
            return;
        }

        m_adaptScale = UnityVector3.zero;
        if (UnityVector3.zero == m_adaptScale)
        {
            CalculateScale();
            if (UnityVector3.zero == m_adaptScale)
            {
                return;
            }
        }

        UnityRectTransform nodeRect = GetComponent<UnityRectTransform>();
        if (null == nodeRect)
        {
            return;
        }
        nodeRect.localScale = m_adaptScale;
    }

    /// <summary>
    /// 计算缩放尺寸
    /// </summary>
    private void CalculateScale()
    {
        UnityGamObject CanvasGo = UnityGamObject.Find("Canvas");
        if (null == CanvasGo)
        {
            return;
        }
        UnityRectTransform canvasRect = CanvasGo.GetComponent<UnityRectTransform>();
        UnityCavasScaler scaler = CanvasGo.GetComponent<UnityCavasScaler>();
        if (null == canvasRect || null == scaler)
        {
            return;
        }

        // 是否以宽度为基准
        bool isMatchWidth = (0 == scaler.matchWidthOrHeight);

        float rate = 1.0f;

        if (isMatchWidth)
        {
            rate = canvasRect.sizeDelta.y / scaler.referenceResolution.y;            
        }
        else
        {
            rate = canvasRect.sizeDelta.x / scaler.referenceResolution.x;
        }

        m_adaptScale = UnityVector3.one * rate;
        m_adaptScale.z = 1;
    }
}

