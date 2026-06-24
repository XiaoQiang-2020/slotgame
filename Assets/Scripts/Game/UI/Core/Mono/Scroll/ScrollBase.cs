/***************************************************
 * 文件名：ScrollBase.cs
 * 描  述：基础UI组件：滚动功能基础类。
 * 时  间：2017-04-14
 * 作  者：李海波
 * 修  改：
 ***************************************************/
namespace Game
{
    using UnityEngineGameObject = UnityEngine.GameObject;
    using UnityEngineRectTransform = UnityEngine.RectTransform;
    using UnityEngineTransform = UnityEngine.Transform;
    using UnityEngineResources = UnityEngine.Resources;
    using UnityEngineSprite = UnityEngine.Sprite;
    using UnityEngineVector2 = UnityEngine.Vector2;
    using UnityEngineVector3 = UnityEngine.Vector3;
    using UnityEngineColor = UnityEngine.Color;
    using UnityEngineUIImage = UnityEngine.UI.Image;
    using UnityEngineUIScrollbar = UnityEngine.UI.Scrollbar;
    using UnityEngineUIScrollRect = UnityEngine.UI.ScrollRect;
    using UnityEngineUIMask = UnityEngine.UI.Mask;
    using SystemListTransform =  System.Collections.Generic.List< UnityEngine.Transform>;

    public class ScrollBase : UnityEngineUIScrollRect
    {

        /// <summary>
        /// 滚动窗内容容器
        /// </summary>
        protected UnityEngineTransform m_contentContainer = null;

        /// <summary>
        /// 滚动条
        /// </summary>
        protected UnityEngineTransform m_scrollBar = null;

        /// <summary>
        /// 是否是垂直滚动方向
        /// </summary>
        protected bool m_bVerticalDirect = true;

        /// <summary>
        /// 是否必须保证最后一行完全可见
        /// </summary>
        protected bool m_bMustShowEnd = false;

        /// <summary>
        /// 是否显示滚动条
        /// </summary>
        protected bool m_bShowBar = true;

        /// <summary>
        /// 滚动容器大小是否超出根节点范围
        /// </summary>
        protected bool m_bOutRange = false;

        /// <summary>
        /// 滚动条粗细
        /// </summary>
        protected float m_scrollBarWid = 10;

        /// <summary>
        /// 子节点数组
        /// </summary>
        protected SystemListTransform m_arrChild = null;

        /// <summary>
        /// 滚动条托条
        /// </summary>
        private UnityEngineUIImage m_imageBarLine = null;

        /// <summary>
        /// 滚动条底槽
        /// </summary>
        private UnityEngineUIImage m_imageBarBack = null;

        /// <summary>
        /// 初始化根节点
        /// </summary>
        protected void InitScrollRoot()
        {
            // 白色图片
            UnityEngineUIImage img = gameObject.AddComponent<UnityEngineUIImage>();
            img.color = UnityEngineColor.white;
            img.preserveAspect = false;

            // 遮罩效果
            UnityEngineUIMask mask = gameObject.AddComponent<UnityEngineUIMask>();
            mask.showMaskGraphic = false;

            // 滚动矩形
            this.horizontal = !m_bVerticalDirect;
            this.vertical = m_bVerticalDirect;
            this.inertia = true;
            this.decelerationRate = 0.02f;
            this.scrollSensitivity = 1;
        }

        /// <summary>
        /// 初始化滚动条
        /// </summary>
        protected void InitScrollBar()
        {
            if (!m_bShowBar)
            {
                return;
            }

            /*
            string path = UIResourceFile.PREFAB_UI_SCROLL_BAR;
            UnityEngineGameObject prefab = UnityEngineResources.Load<UnityEngineGameObject>(path);
            if (null == prefab)
            {
                return;
            }
            UnityEngineGameObject scrollBarRoot = UnityEngineGameObject.Instantiate(prefab) as UnityEngineGameObject;
            if (null == scrollBarRoot)
            {
                return;
            }
            m_scrollBar = scrollBarRoot.transform;
            m_scrollBar.SetParent(transform);
            m_scrollBar.localScale = UnityEngineVector3.one;


            UnityEngineRectTransform rootRect = transform.GetComponent<UnityEngineRectTransform>();
            UnityEngineRectTransform barRect = scrollBarRoot.GetComponent<UnityEngineRectTransform>();
            UnityEngineUIScrollbar scrollBar = m_scrollBar.GetComponent<UnityEngineUIScrollbar>();
            if (null == rootRect || null == barRect || null == scrollBar)
            {
                return;
            }
            UnityEngineVector2 nowSize = barRect.sizeDelta;
            UnityEngineVector2 anchorPos = new UnityEngineVector2();

            // 设置锚点、大小、位置
            if (m_bVerticalDirect)
            {
                anchorPos.x = 1;
                anchorPos.y = 1;
                nowSize.x = m_scrollBarWid;
                nowSize.y = rootRect.sizeDelta.y;
            }
            else
            {
                anchorPos.x = 0;
                anchorPos.y = 0;
                nowSize.x = rootRect.sizeDelta.x;
                nowSize.y = m_scrollBarWid;
            }
            barRect.sizeDelta = nowSize;
            barRect.anchorMin = anchorPos;
            barRect.anchorMax = anchorPos;
            barRect.pivot = anchorPos;
            barRect.anchoredPosition = UnityEngineVector2.zero;

            // 设置为滚动条
            if (m_bVerticalDirect)
            {
                scrollBar.direction = UnityEngineUIScrollbar.Direction.BottomToTop;
                this.verticalScrollbar = scrollBar;
                this.horizontalScrollbar = null;
            }
            else
            {
                scrollBar.direction = UnityEngineUIScrollbar.Direction.LeftToRight;
                this.verticalScrollbar = null;
                this.horizontalScrollbar = scrollBar;
            }

            scrollBarRoot.SetActive(false);

            // 获取滚动条相关参数
            m_imageBarBack = m_scrollBar.GetComponent<UnityEngineUIImage>();
            UnityEngineTransform nodeLine = m_scrollBar.Find("Sliding Area/Handle");
            if (null != nodeLine)
            {
                m_imageBarLine = nodeLine.GetComponent<UnityEngineUIImage>();
            }*/
        }

        /// <summary>
        /// 设置滚动条显示
        /// </summary>
        /// <param name="sprLine">滚动托条</param>
        /// <param name="sprBack">滚动底槽</param>
        public void SetScrollBarShow(UnityEngineSprite sprLine, UnityEngineSprite sprBack)
        {
            if (null == m_imageBarLine || null == m_imageBarBack)
            {
                return;
            }

            if (sprLine)
            {
                m_imageBarLine.sprite = sprLine;
            }

            if (sprBack)
            {
                m_imageBarBack.sprite = sprBack;
            }
        }

        /// <summary>
        /// 获取子UI数量
        /// </summary>
        /// <returns></returns>
        public int GetChildUICount()
        {
            if (null == m_arrChild)
            {
                return 0;
            }
            else
            {
                return m_arrChild.Count;
            }
        }
    }
}
