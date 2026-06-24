/***************************************************
 * 文件名：ScreenAdapt.cs
 * 描  述：ui全屏拉伸。
 * 时  间：2017-04-13
 * 作  者：李海波
 * 修  改：
 ***************************************************/

using UnityEngine;

[ExecuteInEditMode]
public class ScreenAdapt : MonoBehaviour
{
    /// <summary>
    /// 是否是竖屏
    /// </summary>
    [SerializeField, HeaderAttribute("是竖屏吗？")]
    public bool  isVertical = true;
    /// <summary>
    /// 竖屏全屏尺寸
    /// </summary>
    private static Vector2 m_verticalFullSize = Vector2.zero;

    /// <summary>
    /// 横屏全屏尺寸
    /// </summary>
    private static Vector2 m_horizontalFullSize = Vector2.zero;

    // Use this for initialization
    void Awake()
    {
        if (Vector2.zero == m_verticalFullSize)
        {
            // 屏幕大小
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            // 美术制作的全屏UI大小
            int designWidth = Game.GlobalVar.STANDORD_SCREEN_WIDTH;
            int designHeight = Game.GlobalVar.STANDORD_SCREEN_HEIGHT;
            float s1 = (float)designWidth / (float)designHeight;
            float s2 = (float)screenWidth / (float)screenHeight;
            if (s1 > s2)
            {
                designWidth = Mathf.CeilToInt(designHeight * s2);
            }
            else if (s1 < s2)
            {
                designHeight = Mathf.CeilToInt(designWidth / s2);
            }
            m_verticalFullSize.x = (float)designWidth;
            m_verticalFullSize.y = (float)designHeight;
            m_horizontalFullSize.x = (float)designHeight;
            m_horizontalFullSize.y = (float)designWidth;
        }

        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            if (isVertical)
            {
                rectTransform.sizeDelta = m_verticalFullSize;
            }
            else
            {
                rectTransform.sizeDelta = m_horizontalFullSize;
            }
        }
    }
}
