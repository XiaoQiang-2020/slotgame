/***************************************************
 * 文件名：ScrollGridContainer.cs
 * 描  述：高级UI组件：可滚动的网格容器。
 * 时  间：2017-04-14
 * 作  者：李海波
 * 修  改：
 ***************************************************/

namespace Game
{
    using UnityEngineGameObject = UnityEngine.GameObject;
    using UnityEngineRectTransform = UnityEngine.RectTransform;
    using UnityEngineTransform = UnityEngine.Transform;
    using UnityEngineVector2 = UnityEngine.Vector2;
    using UnityEngineVector3 = UnityEngine.Vector3;
    using UnityEngineTextAnchor = UnityEngine.TextAnchor;
    using UnityEngineRectOffset = UnityEngine.RectOffset;
    using UnityEngineUIGridLayoutGroup = UnityEngine.UI.GridLayoutGroup;
    using SystemListTransform = System.Collections.Generic.List<UnityEngine.Transform>;
    using Debuger = Core.Debuger;

    public class ScrollGridContainer : ScrollBase, ScrollInterface
    {

        /// <summary>
        /// 网格容器
        /// </summary>
        private UnityEngineUIGridLayoutGroup m_gridGroup = null;

        /// <summary>
        /// 一行（纵向滚动）的单元个数 或 一列（横向滚动）的单元个数
        /// </summary>
        private int m_numOfOneLine = 0;

        /// <summary>
        /// 子单元大小
        /// </summary>
        private UnityEngineVector2 m_sizeUnit = UnityEngineVector2.zero;

        /// <summary>
        /// 创建网格容器
        /// </summary>
        /// <param name="parent">挂载点</param>
        /// <param name="bVerDirect">滚动方向</param>
        /// <param name="showBar">是否显示滚动条</param>
        /// <param name="showEnd">刷新时是否显示行尾</param>
        /// <returns></returns>
        public static ScrollGridContainer CreateScrollGridContainer(UnityEngineTransform parent, bool bVerDirect = true, bool showBar = true, bool showEnd = false)
        {
            if (null == parent)
            {
                return null;
            }

            ScrollGridContainer container = parent.GetComponent<ScrollGridContainer>();
            if (null == container)
            {
                container = parent.gameObject.AddComponent<ScrollGridContainer>();
                if (null == container)
                {
                    return null;
                }
            }

            container.Init(bVerDirect, showBar, showEnd);
            return container;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="bVerDirect">滚动方向是否是垂直方向</param>
        /// <param name="showBar">是否显示滚动条</param>
        /// <param name="showEnd">是否必须显示最后一格末尾</param>
        private void Init(bool bVerDirect, bool showBar, bool showEnd)
        {
            m_bVerticalDirect = bVerDirect;
            m_bShowBar = showBar;
            m_bMustShowEnd = showEnd;

            InitScrollRoot();
            InitScrollContainer();
            //InitScrollBar();
        }

        /// <summary>
        /// 初始化滚动内容容器
        /// </summary>
        public void InitScrollContainer()
        {
            UnityEngineGameObject containerGo = new UnityEngineGameObject();
            containerGo.transform.SetParent(transform);

            // 设置锚点和坐标
            UnityEngineRectTransform rectTrans = containerGo.AddComponent<UnityEngineRectTransform>();
            UnityEngineVector2 pos = new UnityEngineVector2(0, 1);
            rectTrans.anchorMin = pos;
            rectTrans.anchorMax = pos;
            rectTrans.pivot = pos;
            rectTrans.anchoredPosition3D = UnityEngineVector3.zero;

            UnityEngineVector2 rootSize = transform.GetComponent<UnityEngineRectTransform>().sizeDelta;

            // 添加网格容器
            m_gridGroup = containerGo.AddComponent<UnityEngineUIGridLayoutGroup>();
            m_gridGroup.padding = new UnityEngineRectOffset(0, 0, 0, 0);
            m_gridGroup.spacing = UnityEngineVector2.zero;
            m_gridGroup.startCorner = UnityEngineUIGridLayoutGroup.Corner.UpperLeft;
            m_gridGroup.childAlignment = UnityEngineTextAnchor.UpperLeft;
            m_gridGroup.constraint = UnityEngineUIGridLayoutGroup.Constraint.Flexible;
            if (m_bVerticalDirect)
            {
                m_gridGroup.startAxis = UnityEngineUIGridLayoutGroup.Axis.Horizontal;
            }
            else
            {
                m_gridGroup.startAxis = UnityEngineUIGridLayoutGroup.Axis.Vertical;
            }

            rectTrans.sizeDelta = rootSize;
            rectTrans.localScale = UnityEngineVector3.one;

            // 设置为内容容器
            m_contentContainer = containerGo.transform;
            this.content = rectTrans;

            m_arrChild = new SystemListTransform();
        }

        /// <summary>
        /// 在滚动窗中添加子节点
        /// </summary>
        /// <param name="oneUnit">要添加的子节点</param>
        /// <param name="needRefresh">是否刷新容器</param>
        /// <returns>是否添加成功</returns>
        public bool AddChild(UnityEngineGameObject oneUnit, bool needRefresh)
        {
            if (null == oneUnit)
            {
                return false;
            }

            UnityEngineRectTransform rectUnit = oneUnit.GetComponent<UnityEngineRectTransform>();
            if (null == rectUnit)
            {
                return false;
            }

            UnityEngineRectTransform rectContainer = m_contentContainer.GetComponent<UnityEngineRectTransform>();
            UnityEngineVector2 sizeContainer = rectContainer.sizeDelta;

            // 检查单元格大小是否一致
            if (UnityEngineVector2.zero == m_sizeUnit)
            {
                m_sizeUnit = rectUnit.sizeDelta;
                m_gridGroup.cellSize = m_sizeUnit;
                if (m_bVerticalDirect)
                {
                    m_numOfOneLine = (int)(sizeContainer.x / m_sizeUnit.x);
                }
                else
                {
                    m_numOfOneLine = (int)(sizeContainer.y / m_sizeUnit.y);
                }
            }
            else
            {
                if (m_sizeUnit != rectUnit.sizeDelta)
                {
                    return false;
                }
            }

            // 添加到容器
            oneUnit.transform.SetParent(m_contentContainer);
            rectUnit.localScale = UnityEngineVector3.one;
            rectUnit.localEulerAngles = UnityEngineVector3.zero;
            oneUnit.transform.SetAsLastSibling();
            UnityEngineVector3 pos = rectUnit.anchoredPosition3D;
            pos.z = 0;
            rectUnit.anchoredPosition3D = pos;
            m_arrChild.Add(oneUnit.transform);

            // 刷新网格的容器大小
            if (needRefresh)
            {
                RefreshContainerSize();
            }
            return true;
        }

        /// <summary>
        /// 设置子节点的显示和隐藏
        /// </summary>
        /// <param name="nodeChild">列表子节点</param>
        /// <param name="bShow">是否显示</param>
        /// <returns></returns>
        public virtual bool SetChildVisible(UnityEngineTransform nodeChild, bool bShow)
        {
            if (null == nodeChild)
            {
                return false;
            }

            // 查询序号
            int index = -1;
            for (int i = 0; i < m_arrChild.Count; i++)
            {
                if (nodeChild == m_arrChild[i])
                {
                    index = i;
                    break;
                }
            }
            if (-1 == index)
            {
                return false;
            }

            return SetChildVisible(index, bShow);
        }

        /// <summary>
        /// 设置子节点的显示和隐藏
        /// </summary>
        /// <param name="index">子节点排列序号</param>
        /// <param name="bShow">是否显示</param>
        /// <returns></returns>
        public virtual bool SetChildVisible(int index, bool bShow)
        {
            if (index < 0 || index >= m_arrChild.Count || null == m_arrChild[index])
            {
                return false;
            }

            if (m_arrChild[index].gameObject.activeSelf == bShow)
            {
                return false;
            }

            m_arrChild[index].gameObject.SetActive(bShow);
            RefreshContainerSize();
            return true;
        }

        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="nodeChild">子节点</param>
        /// <param name="needRefresh">是否需要刷新容器</param>
        /// <returns></returns>
        public bool RemoveChild(UnityEngineTransform nodeChild, bool needRefresh)
        {
            // 容错检查
            if (null == nodeChild)
            {
                Debuger.LogError("ScrowWindow can't RemoveChild null childNode!");
                return false;
            }

            if (nodeChild.parent != m_contentContainer)
            {
                Debuger.LogError("ScrowWindow can't RemoveChild childNode that is not son!");
                return false;
            }

            // 删除子节点
            m_arrChild.Remove(nodeChild);
            Destroy(nodeChild.gameObject);

            // 修改容器大小
            if (needRefresh)
            {
                RefreshContainerSize();
            }

            return true;
        }

        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="index">排列序号</param>
        /// <param name="needRefresh">是否需要刷新容器</param>
        /// <returns></returns>
        public bool RemoveChild(int index, bool needRefresh)
        {
            int childNum = m_arrChild.Count;
            if (index < 0 || index >= childNum)
            {
                Debuger.LogError("RemoveChild index is wrond!");
                return false;
            }

            UnityEngineTransform child = m_arrChild[index];
            return RemoveChild(child, needRefresh);
        }

        /// <summary>
        /// 清空滚动窗
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllChild()
        {
            if (m_arrChild.Count <= 0)
            {
                return true;
            }
            for (int i = m_arrChild.Count - 1; i >= 0; i--)
            {
                RemoveChild(i, false);
            }
            RefreshContainerSize();
            return true;
        }

        /// <summary>
        /// 刷新网格的容器大小
        /// </summary>
        public void RefreshContainerSize()
        {
            if (UnityEngineVector2.zero == m_sizeUnit || m_numOfOneLine <= 0)
            {
                return;
            }

            // 统计要显示的子节点数量
            int numShowGrid = 0;
            for (int i = 0; i < m_arrChild.Count; i++)
            {
                if (null != m_arrChild[i] && m_arrChild[i].gameObject.activeSelf)
                {
                    numShowGrid++;
                }
            }

            // 挂载点大小
            UnityEngineVector2 rootSize = transform.GetComponent<UnityEngineRectTransform>().sizeDelta;

            // 滚动窗容器大小
            UnityEngineRectTransform rectContainer = m_contentContainer.GetComponent<UnityEngineRectTransform>();
            UnityEngineVector2 sizeContainer = rectContainer.sizeDelta;

            int numOfCol = numShowGrid / m_numOfOneLine;
            if (numOfCol * m_numOfOneLine < numShowGrid)
            {
                numOfCol++;
            }

            // 垂直滚动
            bool bFull = true;  // 是否大于挂载点
            if (m_bVerticalDirect)
            {
                sizeContainer.y = numOfCol * m_sizeUnit.y;
                if (sizeContainer.y <= rootSize.y)
                {
                    bFull = false;
                    sizeContainer.y = rootSize.y;
                }
            }

            // 水平滚动
            else
            {
                sizeContainer.x = numOfCol * m_sizeUnit.x;
                if (sizeContainer.x <= rootSize.x)
                {
                    bFull = false;
                    sizeContainer.x = rootSize.x;
                }
            }

            // 设置大小
            rectContainer.sizeDelta = sizeContainer;

            // 滚动条
            if (null != m_scrollBar)
            {
                m_scrollBar.gameObject.SetActive(bFull && m_bShowBar);
            }

            if (m_bVerticalDirect)
            {
                this.vertical = bFull;
            }
            else
            {
                this.horizontal = bFull;
            }


            // 是否隐藏滚动条：容器超出根节点才显示
            bool bOverRoot = false;  // 是否超出根节点大小
            if (m_bVerticalDirect)
            {
                if (sizeContainer.y > rootSize.y)
                {
                    bOverRoot = true;
                }
            }
            else
            {
                if (sizeContainer.x > rootSize.x)
                {
                    bOverRoot = true;
                }
            }
            if (null != m_scrollBar)
            {
                m_scrollBar.gameObject.SetActive(bOverRoot && m_bShowBar);
            }
        }

        /// <summary>
        /// 确保网格容器中指定子节点一定显示出来
        /// </summary>
        /// <param name="index">节点序号</param>
        /// <returns></returns>
        public bool ShowChildIndex(int index)
        {
            if (index < 0 || index >= m_arrChild.Count)
            {
                return false;
            }

            if (m_arrChild.Count <= 1)
            {
                return true;
            }

            if (null == m_contentContainer)
            {
                return false;
            }

            UnityEngineRectTransform rectRoot = transform.GetComponent<UnityEngineRectTransform>();
            UnityEngineRectTransform rectContainer = m_contentContainer.GetComponent<UnityEngineRectTransform>();
            if (null == rectRoot || null == rectContainer)
            {
                return false;
            }
            // 滚动窗内容还未铺满窗口，则所有节点都必然被展示
            bool isSmall = false;
            if (m_bVerticalDirect)
            {
                isSmall = (rectContainer.sizeDelta.y <= rectRoot.sizeDelta.y);
            }
            else
            {
                isSmall = (rectContainer.sizeDelta.x <= rectRoot.sizeDelta.x);
            }
            if (true == isSmall)
            {
                return true;
            }

            UnityEngineVector2 frontSize = UnityEngineVector2.zero;
            UnityEngineVector2 backSize = UnityEngineVector2.zero;

            if (m_bVerticalDirect)
            {
                frontSize.x = rectContainer.sizeDelta.x;
                backSize.x = frontSize.x;
            }
            else
            {
                frontSize.y = rectContainer.sizeDelta.y;
                backSize.y = frontSize.y;
            }

            // 统计从起始子节点到当前子节点占用的空间大小
            int numLineIndex = index / m_numOfOneLine;
            if (index > numLineIndex * m_numOfOneLine)
            {
                numLineIndex++;
            }
            if (m_bVerticalDirect)
            {
                frontSize.y = numLineIndex * m_sizeUnit.y;
            }
            else
            {
                frontSize.x = numLineIndex * m_sizeUnit.x;
            }

            if (m_bVerticalDirect)
            {
                backSize.y = rectContainer.sizeDelta.y - frontSize.y;
            }
            else
            {
                backSize.y = rectContainer.sizeDelta.x - frontSize.x;
            }

            // 优先置顶显示
            UnityEngineVector3 anchorPos = rectContainer.anchoredPosition3D;
            if (m_bVerticalDirect)
            {
                if (backSize.y >= rectRoot.sizeDelta.y)
                {
                    anchorPos.y = frontSize.y;
                }
                else
                {
                    anchorPos.y = rectContainer.sizeDelta.y - rectRoot.sizeDelta.y;
                }
            }
            else
            {
                if (backSize.x >= rectRoot.sizeDelta.x)
                {
                    anchorPos.x = frontSize.x;
                }
                else
                {
                    anchorPos.x = rectContainer.sizeDelta.x - rectRoot.sizeDelta.x;
                }
            }
            rectContainer.anchoredPosition3D = anchorPos;
            return true;
        }
    }
}
