/***************************************************
 * 文件名：RollingWindow.cs
 * 描  述：基础UI组件：优化版的滚动窗。
 * 时  间：2017-04-14
 * 作  者：李海波
 * 修  改：
 ***************************************************/

namespace Game
{
    using UnityTime = UnityEngine.Time;
    using UnityMathf = UnityEngine.Mathf;
    using UnityColor = UnityEngine.Color;
    using UnityObject = UnityEngine.Object;
    using UnityVector2 = UnityEngine.Vector2;
    using UnityVector3 = UnityEngine.Vector3;
    using UnityResources = UnityEngine.Resources;
    using UnityTextAnchor = UnityEngine.TextAnchor;
    using UnityGameObject = UnityEngine.GameObject;
    using UnityTransform = UnityEngine.Transform;
    using UnityRectTransform = UnityEngine.RectTransform;
    using UnityRectOffset = UnityEngine.RectOffset;


    using UnityUIMask = UnityEngine.UI.Mask;
    using UnityUIImage = UnityEngine.UI.Image;
    using UnityUIScrollRect = UnityEngine.UI.ScrollRect;
    using UnityLayoutElement = UnityEngine.UI.LayoutElement;
    using UnityUIVerticalLayoutGroup = UnityEngine.UI.VerticalLayoutGroup;
    using UnityUIHorizontalLayoutGroup = UnityEngine.UI.HorizontalLayoutGroup;

    using Debuger = Core.Debuger;
    using System;

    public class RollingWindow : UnityUIScrollRect
    {
        #region 常量
        /// <summary>
        /// 滚动方向
        /// </summary>
        public enum ROLL_DIRECT_ENUM : int
        {
            DIRECT_VERTICAL   = 0,  // 垂直滚动
            DIRECT_HORIZONTAL = 1,  // 水平滚动
        }

        /// <summary>
        /// 子单元相对位置
        /// </summary>
        public enum ROLL_CELL_POS_ENUM : int
        {
            BEFORE = 0,    // 相对最前面
            AFTER = 1,     // 相对最后面
        }

        /// <summary>
        /// 滚动容器的名字
        /// </summary>
        private const string NAME_CONTAINER = "roll_container";

        /// <summary>
        /// 顶部垫子的名字
        /// </summary>
        private const string NAME_FIRST_PADDER = "first_padder";

        /// <summary>
        /// 底部垫子的名字
        /// </summary>
        private const string NAME_LAST_PADDER = "last_padder";

        /// <summary>
        /// 老虎机逻辑单元数量乘以3
        /// </summary>
        private const int TIMES_OF_SLOT_MOTHINE = 3;

        /// <summary>
        /// 滚动窗惯性滚动的临界速度，低于这个速度就交给Update控制滚动窗把焦点定位到最近的子单元上
        /// </summary>
        private const float MAX_VELOCITY_AUTO_MOVE = 0.01f;

        /// <summary>
        /// 系统控制滚动的停止条件
        /// </summary>
        private const float MIN_AUTO_MOVE_SPEED = 2f;

        /// <summary>
        /// 自动滚动速度基数
        /// </summary>
        private const float MAX_AUTO_SPEED_BASE = 10f;

        #endregion

        #region 成员变量
        /// <summary>
        /// 滚动窗的滚动方向
        /// </summary>
        private ROLL_DIRECT_ENUM m_rollDirect = ROLL_DIRECT_ENUM.DIRECT_VERTICAL;

        /// <summary>
        /// 子单元预制件
        /// </summary>
        private UnityGameObject m_cellPrefab = null;

        /// <summary>
        /// 逻辑上的子单元数量
        /// </summary>
        private int m_logicCellNum = 0;

        /// <summary>
        /// 循环列表逻辑上的子单元数量
        /// </summary>
        private int m_loopRealLogicNum = 0;

        /// <summary>
        /// 显示上的子单元数量
        /// </summary>
        private int m_showCellNum = 0;

        /// <summary>
        /// 是否需要首尾循环
        /// </summary>
        private bool m_isloop = false;

        /// <summary>
        /// 子单元之间的间隔距离
        /// </summary>
        private float m_spacing = 0;

        /// <summary>
        /// 滚动容器边框空格区域
        /// </summary>
        private UnityRectOffset m_padding;

        /// <summary>
        /// 窗口的ScrollRect组件
        /// </summary>
        private UnityUIScrollRect m_scrollRect = null;

        /// <summary>
        /// 窗口的RectTransform组件
        /// </summary>
        private UnityRectTransform m_rootRectTransform = null;

        /// <summary>
        /// 容器的RectTransform组件
        /// </summary>
        private UnityRectTransform m_containerRectTransform = null;

        /// <summary>
        /// 容器的HorizontalOrVerticalLayoutGroup组件
        /// </summary>
        //private UnityHorizontalOrVerticalLayoutGroup m_containerLayoutGroup;

        /// <summary>
        /// 是否完成初始化
        /// </summary>
        private bool m_isFinishInit = false;

        /// <summary>
        /// 子单元大小
        /// </summary>
        private UnityVector2 m_childSize = UnityVector2.zero;

        /// <summary>
        /// 顶部海绵垫子
        /// </summary>
        private UnityLayoutElement m_firstPadder = null;

        /// <summary>
        /// 底部海绵垫子
        /// </summary>
        private UnityLayoutElement m_lastPadder = null;

        /// <summary>
        /// 当前显示的第一个子单元在逻辑上的序号
        /// </summary>
        private int m_logicCellStartIndex = 0;

        /// <summary>
        /// 当前显示的最后一个子单元在逻辑上的序号
        /// </summary>
        private int m_logicCellEndIndex = 0;

        /// <summary>
        /// 各相邻子单元之间的间隔距离（包含子单元本身）
        /// </summary>
        private float[] m_cellSizeArray = null;

        /// <summary>
        /// 各个子单元相对于起始点的偏移距离
        /// </summary>
        private float[] m_cellOffsetArray = null;

        /// <summary>
        /// 各个子单元创建时的顺序
        /// </summary>
        private int[] m_sonOrderArray = null;

        /// <summary>
        /// 子单元实体列表
        /// </summary>
        private UnityTransform[] m_childTransfroms = null;

        /// <summary>
        /// 滚动位置
        /// </summary>
        private float m_scrollPos = 0;

        /// <summary>
        /// 循环滚动，逻辑单元分为重复的3部分，中间部分的第一个单元的序号
        /// </summary>
        private int m_loopFirstCellIndexOfMidPart = 0;

        /// <summary>
        /// 循环滚动，逻辑单元分为重复的3部分，中间部分的最后一个单元的序号
        /// </summary>
        private int m_loopLastCellIndexOfMidPart = 0;

        /// <summary>
        /// 循环滚动，中间部分上边界位置点
        /// </summary>
        private float m_loopFirstPosOfMidPart = 0;

        /// <summary>
        /// 循环滚动，中间部分下边界位置点
        /// </summary>
        private float m_loopLastPosOfMidPart = 0;

        /// <summary>
        /// 循环滚动，整个逻辑区域的上边界位置点
        /// </summary>
        private float m_loopFirstBorderPos = 0;

        /// <summary>
        /// 循环滚动，整个逻辑区域的下边界位置点
        /// </summary>
        private float m_loopLastBorderPos = 0;

        /// <summary>
        /// 子单元刷新的委托方法
        /// </summary>
        /// <param name="indexGo"></param>
        /// <param name="indexLogic"></param>
        public delegate void RefreshChildContentDelegate(int indexGo, int indexLogic);
        private RefreshChildContentDelegate m_childRefreshDelegate = null;
        private Action<int, int> m_childRefreshLuaCall = null;

        /// <summary>
        /// 委托定义：当前突出显示刷新
        /// </summary>
        /// <param name="firstIndex">第一个突出显示的子单元</param>
        /// <param name="firstRate">第一个突出显示的子单元突出显示程度</param>
        /// <param name="secondIndex">第二个突出显示的子单元</param>
        /// <param name="secondRate">第二个突出显示的子单元突出显示程度</param>
        public delegate void RollFocusDelegate(int firstIndex, int secondIndex, float firstRate);
        private RollFocusDelegate m_focusDelegate = null;

        /// <summary>
        /// 自动滚动完成后的委托事件
        /// </summary>
        /// <param name="indexGo">焦点项实体序号</param>
        public delegate void FinishAutoMove(int indexGo);
        private FinishAutoMove m_finishCallBack = null;

        /// <summary>
        /// 是否是老虎机
        /// </summary>
        private bool m_isSlotMachine = false;

        /// <summary>
        /// 是否正在自动滚动
        /// </summary>
        private bool m_isAutoMoving = false;

        /// <summary>
        /// 自动滚动的目标子单元序号
        /// </summary>
        private int m_autoScrollTargetIndex = 0;

        /// <summary>
        /// 自动滚动的目标位置
        /// </summary>
        private float m_autoScrollTargetPos = 0;

        /// <summary>
        /// 自动滚动速度
        /// </summary>
        private float m_autoScrollSpeed = 0;
        #endregion

        #region 创建方法
        /// <summary>
        /// 创建优化滚动窗
        /// </summary>
        /// <param name="rootNode">挂载点</param>
        /// <param name="sonPrefab">子单元的预制件</param>
        /// <param name="direct">滚动方向</param>
        /// <param name="numLogic">逻辑上的子单元数量</param>
        /// <param name="isloop">是否要循环滚动</param>
        /// <returns>滚动窗组件</returns>
        public static RollingWindow CreateRollingWindow(UnityTransform rootNode, UnityGameObject sonPrefab, ROLL_DIRECT_ENUM direct, int numLogic, bool isloop)
        {
            if (null == rootNode)
            {
                Debuger.LogError("创建优化滚动窗失败：挂载点为空！");
                return null;
            }

            if (numLogic <= 0)
            {
                Debuger.LogError("创建优化滚动窗失败：子单元数据量必须大于0！");
                return null;
            }

            if (null == sonPrefab)
            {
                Debuger.LogError("创建优化滚动窗失败：传入的子单元预制件为空！");
                return null;
            }

            RollingWindow rollWin = rootNode.GetComponent<RollingWindow>();
            if (null != rollWin)
            {
                return rollWin;
            }

            rollWin = rootNode.gameObject.AddComponent<RollingWindow>();
            if (null == rollWin)
            {
                return null;
            }

            // 设置滚动窗参数
            rollWin.m_rootRectTransform = rootNode.GetComponent<UnityRectTransform>();
            rollWin.m_scrollRect = rollWin;
            rollWin.m_cellPrefab = sonPrefab;
            rollWin.m_isloop = isloop;
            rollWin.m_rollDirect = direct;
            rollWin.m_padding = new UnityRectOffset(0, 0, 0, 0);

            if (!rollWin.Init(numLogic))
            {
                UnityGameObject.Destroy(rollWin);
                return null;
            }

            rollWin.m_isFinishInit = true;
            return rollWin;
        }
        #endregion

        #region 外部函数
        /// <summary>
        /// 将滚动窗设置成老虎机
        /// </summary>
        /// <param name="focusDelegate">刷新焦点子单元的委托方法</param>
        /// <returns></returns>
        public bool SetAsSlotMachine(RollFocusDelegate focusDelegate, FinishAutoMove finishAutoDelegate)
        {
            if(null == focusDelegate || null == finishAutoDelegate)
            {
                return false;
            }
            m_isSlotMachine = true;
            m_focusDelegate = focusDelegate;
            m_finishCallBack = finishAutoDelegate;

            BeganAutoMove();
            return true;
        }

        /// <summary>
        /// 设置老虎机焦点显示项
        /// </summary>
        /// <param name="indexOfLogic">逻辑编号</param>
        public void SetSlotMothineFocusIndex(int indexOfLogic)
        {
            if (indexOfLogic < 0)
            {
                Debuger.LogError("SetSlotMothineFocusIndex逻辑序号必须不小于0！");
                return;
            }

            indexOfLogic += m_loopRealLogicNum;
            UnityVector3 pos = m_containerRectTransform.anchoredPosition3D;
            UnityVector2 rootSize = m_rootRectTransform.sizeDelta;
            if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
            {
                pos.y = m_cellOffsetArray[indexOfLogic] - m_cellSizeArray[indexOfLogic] * 0.5f - rootSize.y * 0.5f;
            }
            else
            {
                pos.x = m_cellSizeArray[indexOfLogic] - m_cellSizeArray[indexOfLogic] * 0.5f - rootSize.x * 0.5f;
            }
            m_containerRectTransform.anchoredPosition3D = pos;
            RefreshChildShow(true);
        }

        /// <summary>
        /// 设置循环滚动窗的第一个显示项
        /// </summary>
        /// <param name="indexLogic">逻辑编号</param>
        public void SetLoopRollFirstIndex(int indexLogic)
        {
            if (indexLogic < 0)
            {
                Debuger.LogError("SetLoopRollFirstIndex逻辑序号必须不小于0！");
                return;
            }
            UnityVector3 pos = m_containerRectTransform.anchoredPosition3D;
            if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
            {
                pos.y = m_cellOffsetArray[indexLogic + m_loopRealLogicNum - 1];
                m_scrollPos = pos.y + 1;
            }
            else
            {
                pos.x = m_cellOffsetArray[indexLogic + m_loopRealLogicNum - 1];
                m_scrollPos = pos.x + 1;
            }
            m_containerRectTransform.anchoredPosition3D = pos;
            RefreshChildShow(true);
        }

        /// <summary>
        /// 改变逻辑单元数量
        /// </summary>
        /// <param name="logicNum">新的逻辑单元数量</param>
        /// <param name="keepOldPos">是否保留旧的显示位置</param>
        /// <returns></returns>
        public bool ChangeLogicCellNum(int logicNum, bool keepOldPos)
        {
            if (logicNum == m_logicCellNum)
            {
                return false;
            }

            if (m_isloop)
            {
                m_loopRealLogicNum = logicNum;
                m_logicCellNum = logicNum * TIMES_OF_SLOT_MOTHINE;
            }
            else
            {
                m_logicCellNum = logicNum;
            }

            if (logicNum <= 0)
            {
                m_containerRectTransform.gameObject.SetActive(false);
                return false;
            }
            m_containerRectTransform.gameObject.SetActive(true);

            if (keepOldPos)
            {
                return true;
            }

            // 初始化子单元
            if (!InitChild())
            {
                Debuger.LogError("ChangeLogicCellNum 初始化子单元失败！");
                return false;
            }
            else
            {                
                SetChildVisible();
            }

            return true;
        }

        /// <summary>
        /// 完全刷新全部列表
        /// </summary>
        public void CompleteRefresh()
        {

            for (int i = 0; i < m_showCellNum; i++)
            {

                if (m_isloop)
                {
                    if (null != m_childRefreshDelegate)
                    {
                        m_childRefreshDelegate(m_sonOrderArray[i], (m_logicCellStartIndex + i) % m_loopRealLogicNum);
                    }
                    if (null != m_childRefreshLuaCall)
                    {
                        m_childRefreshLuaCall.Invoke(m_sonOrderArray[i], (m_logicCellStartIndex + i) % m_loopRealLogicNum);
                    }
                }
                else
                {
                    m_childRefreshLuaCall.Invoke(0,0);
                    if (null != m_childRefreshDelegate)
                    {
                        m_childRefreshDelegate(m_sonOrderArray[i], m_logicCellStartIndex + i);
                    }
                    if (null != m_childRefreshLuaCall)
                    {
                        m_childRefreshLuaCall.Invoke(m_sonOrderArray[i], m_logicCellStartIndex + i);
                    }
                }
            }
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>true:初始化成功，false：初始化失败</returns>
        private bool Init(int logicNum)
        {
            // 初始化挂载点
            if (!InitRoot())
            {
                Debuger.LogError("Init 初始化滚动窗的挂载点失败！");
                return false;
            }

            // 初始化容器
            if (!InitContainer())
            {
                Debuger.LogError("Init 初始化滚动窗的容器失败！");
                return false;
            }

            if (!ChangeLogicCellNum(logicNum, false))
            {
                Debuger.LogError("Init 改变逻辑单元数量失败！");
                return false;
            }

            // 设置滚动监听
            m_scrollRect.onValueChanged.AddListener(ScrollRectValueChange);
            return true;
        }

        /// <summary>
        /// 初始化挂载点
        /// </summary>
        /// <returns></returns>
        private bool InitRoot()
        {
            // 白色图片
            UnityUIImage img = gameObject.AddComponent<UnityUIImage>();
            img.color = UnityColor.white;
            img.preserveAspect = false;

            // 遮罩效果
            UnityUIMask mask = gameObject.AddComponent<UnityUIMask>();
            mask.showMaskGraphic = false;

            // 滚动矩形
            if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
            {
                this.vertical = true;
                this.horizontal = false;
            }
            else
            {
                this.vertical = false;
                this.horizontal = true;
            }
            this.inertia = true;
            this.decelerationRate = 0.02f;
            this.scrollSensitivity = 1;
            return true;
        }

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <returns></returns>
        private bool InitContainer()
        {
            UnityGameObject containerGo = new UnityGameObject(NAME_CONTAINER, typeof(UnityRectTransform));
            containerGo.transform.SetParent(transform);

            // 设置锚点和坐标
            m_containerRectTransform = containerGo.GetComponent<UnityRectTransform>();
            m_containerRectTransform.anchorMin = UnityVector2.up;
            m_containerRectTransform.anchorMax = UnityVector2.up;
            m_containerRectTransform.pivot = UnityVector2.up;
            m_containerRectTransform.localScale = UnityVector2.one;
            m_containerRectTransform.localEulerAngles = UnityVector3.zero;
            m_containerRectTransform.anchoredPosition3D = UnityVector3.zero;
            m_containerRectTransform.sizeDelta = m_rootRectTransform.sizeDelta;

            // 添加网格容器
            if (m_rollDirect == ROLL_DIRECT_ENUM.DIRECT_VERTICAL)
            {
                UnityUIVerticalLayoutGroup layoutGroup = containerGo.AddComponent<UnityUIVerticalLayoutGroup>();
                layoutGroup.spacing = m_spacing;
                layoutGroup.padding = m_padding;
                layoutGroup.childAlignment = UnityTextAnchor.UpperLeft;
                layoutGroup.childForceExpandWidth = true;
                //layoutGroup.childForceExpandHeight = true;
            }
            else
            {
                UnityUIHorizontalLayoutGroup layoutGroup = containerGo.AddComponent<UnityUIHorizontalLayoutGroup>();
                layoutGroup.spacing = m_spacing;
                layoutGroup.padding = m_padding;
                layoutGroup.childAlignment = UnityTextAnchor.UpperLeft;
                //layoutGroup.childForceExpandWidth = true;
                layoutGroup.childForceExpandHeight = true;
            }

            // 设置为内容容器
            m_scrollRect.content = m_containerRectTransform;
            return true;
        }

        /// <summary>
        /// 初始化子单元
        /// </summary>        
        /// <returns>是否初始化成功</returns>
        private bool InitChild()
        {
            // 获取子单元大小信息
            if (UnityVector2.zero == m_childSize)
            {
                UnityGameObject firstCellGo = UnityGameObject.Instantiate(m_cellPrefab) as UnityGameObject;
                if (null == firstCellGo)
                {
                    Debuger.LogError("InitChild 实例化子单元预制件失败！");
                    return false;
                }
                if (!AddChild(m_containerRectTransform, firstCellGo.transform))
                {
                    Debuger.LogError("InitChild 添加子单元预制件失败！");
                    return false;
                }

                m_childSize = firstCellGo.GetComponent<UnityRectTransform>().sizeDelta;
                UnityGameObject.Destroy(firstCellGo);
            }

            if (!FirstCalculateChildShowNum())
            {
                Debuger.LogError("InitChild 计算可显示子单元数量失败！");
                return false;
            }

            // 初始化子单元逻辑数据
            if (m_isFinishInit)
            {
                if (m_logicCellEndIndex >= m_logicCellNum)
                {                    
                    m_logicCellEndIndex = m_logicCellNum - 1;
                    m_logicCellStartIndex = m_logicCellEndIndex - m_showCellNum;
                    if (m_logicCellStartIndex < 0)
                    {
                        m_logicCellStartIndex = 0;
                    }
                }                
            }
            else
            {
                m_logicCellStartIndex = 0;
                m_logicCellEndIndex = m_showCellNum - 1;
                m_sonOrderArray = new int[m_showCellNum];
            }
            
            for (int i = 0; i < m_showCellNum; i++)
            {
                m_sonOrderArray[i] = i;
                if (null != m_childRefreshDelegate)
                {
                    m_childRefreshDelegate(i, i);
                }
                if (null != m_childRefreshLuaCall)
                {
                    m_childRefreshLuaCall.Invoke(i, i);
                }
            }

            // 初始化垫子
            if (null == m_firstPadder && null == m_lastPadder)
            {
                // 添加顶部垫子
                UnityGameObject padderGo = new UnityGameObject(NAME_FIRST_PADDER, typeof(UnityRectTransform), typeof(UnityLayoutElement));
                padderGo.transform.SetParent(m_containerRectTransform, false);
                m_firstPadder = padderGo.GetComponent<UnityLayoutElement>();

                // 添加底部垫子
                padderGo = new UnityGameObject(NAME_LAST_PADDER, typeof(UnityRectTransform), typeof(UnityLayoutElement));
                padderGo.transform.SetParent(m_containerRectTransform, false);
                m_lastPadder = padderGo.GetComponent<UnityLayoutElement>();
            }
            m_firstPadder.transform.SetAsFirstSibling();
            m_lastPadder.transform.SetAsLastSibling();

            if (m_isloop)
            {
                // 窗口大小
                float scrollSize = (m_rollDirect == ROLL_DIRECT_ENUM.DIRECT_VERTICAL ? m_rootRectTransform.sizeDelta.y : m_rootRectTransform.sizeDelta.x);

                m_loopFirstCellIndexOfMidPart = m_loopRealLogicNum;
                m_loopLastCellIndexOfMidPart = m_loopFirstCellIndexOfMidPart + m_loopRealLogicNum - 1;

                m_loopFirstPosOfMidPart = GetScrollPosForLogicChildIndex(m_loopFirstCellIndexOfMidPart, ROLL_CELL_POS_ENUM.BEFORE) + m_spacing * 0.5f;
                m_loopLastPosOfMidPart = GetScrollPosForLogicChildIndex(m_loopLastCellIndexOfMidPart, ROLL_CELL_POS_ENUM.AFTER) - scrollSize + 0.5f;

                m_loopFirstBorderPos = m_loopFirstPosOfMidPart - scrollSize;
                m_loopLastBorderPos = m_loopLastPosOfMidPart + scrollSize;

                SetLoopRollFirstIndex(0);
            }
            else
            {
                SetPadderShow();
            }

            return true;
        }
        #endregion

        #region 内部函数
        /// <summary>
        /// 重新设置滚动容器的尺寸
        /// </summary>
        /// <param name="numLogic">逻辑子单元的数量</param>
        private void ReSetScrollContainerSize(int numLogic)
        {
            if (numLogic == m_logicCellNum)
            {
                return;
            }

            m_logicCellNum = numLogic;

            // 记录子单元尺寸数据
            CalculateLogicChildSize();

            // 记录子单元偏移数据
            CalculateLogicChildOffset();

            // 计算窗口最多可显示的子单元数量
            CalculateShowCellNum();

            // 设置容器大小
            if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
            {
                m_containerRectTransform.sizeDelta = new UnityVector2(m_rootRectTransform.sizeDelta.x, m_cellOffsetArray[m_logicCellNum - 1] + m_padding.top + m_padding.bottom);
            }
            else
            {
                m_containerRectTransform.sizeDelta = new UnityVector2(m_cellOffsetArray[m_logicCellNum - 1] + m_padding.left + m_padding.right, m_rootRectTransform.sizeDelta.y);
            }
        }

        /// <summary>
        /// 第一次计算子单元显示数量
        /// </summary>
        /// <returns>子单元数量</returns>
        private bool FirstCalculateChildShowNum()
        {
            if (m_childSize.x <= 0 || m_childSize.y <= 0)
            {
                Debuger.LogError("FirstCalculateChildShowNum 子单元根节点大小为0");
                return false;
            }

            // 设置容器尺寸
            int numLogic = m_logicCellNum;
            m_logicCellNum = 0;
            ReSetScrollContainerSize(numLogic);

            // 添加子单元
            if (null == m_childTransfroms)
            {
                m_childTransfroms = new UnityTransform[m_showCellNum];
            }

            UnityGameObject childCellGo = null;
            for (int i = 0; i < m_showCellNum; i++ )
            {
                if (null != m_childTransfroms[i])
                {
                    m_childTransfroms[i].SetAsLastSibling();
                    continue;
                }
                childCellGo = UnityGameObject.Instantiate(m_cellPrefab);
                childCellGo.name = "child_" + i;
                if (null == childCellGo)
                {
                    Debuger.LogError("FirstCalculateChildShowNum 实例化第{0}个子单元预制件失败", i);
                    return false;
                }
                if(!AddChild(m_containerRectTransform, childCellGo.transform))
                {
                    Debuger.LogError("FirstCalculateChildShowNum 添加第{0}个子单元到容器上失败", i);
                    return false;
                }

                UnityLayoutElement element = childCellGo.AddComponent<UnityLayoutElement>();
                if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
                {
                    element.minHeight = m_childSize.y;
                }
                else
                {
                    element.minWidth = m_childSize.x;
                }

                m_childTransfroms[i] = childCellGo.transform;
            }

            return true;
        }

        /// <summary>
        /// 计算逻辑子单元的大小
        /// </summary>
        private void CalculateLogicChildSize()
        {
            if (null != m_cellSizeArray)
            {
                m_cellSizeArray = null;
            }
            m_cellSizeArray = new float[m_logicCellNum];

            if (m_rollDirect == ROLL_DIRECT_ENUM.DIRECT_VERTICAL)
            {
                m_cellSizeArray[0] = m_childSize.y;
            }
            else
            {
                m_cellSizeArray[0] = m_childSize.x;
            }

            float elseChildSize = m_cellSizeArray[0] + m_spacing;
            for (int i = 1; i < m_logicCellNum; i++)
            {
                m_cellSizeArray[i] = elseChildSize;
            }
        }

        /// <summary>
        /// 计算逻辑子单元的偏移数据
        /// </summary>
        private void CalculateLogicChildOffset()
        {
            float offset = 0;
            if (null != m_cellOffsetArray)
            {
                m_cellOffsetArray = null;
            }
            m_cellOffsetArray = new float[m_logicCellNum];
            for (int i = 0; i < m_logicCellNum; i++)
            {
                offset += m_cellSizeArray[i];
                m_cellOffsetArray[i] = offset;
            }
        }

        /// <summary>
        /// 计算窗口显示子单元数量
        /// </summary>
        private void CalculateShowCellNum()
        {
            float rootSize = 0;
            float childSize = 0;
            float numCell = 0;
            if (m_rollDirect ==  ROLL_DIRECT_ENUM.DIRECT_VERTICAL)
            {
                rootSize = m_rootRectTransform.sizeDelta.y;
                childSize = m_childSize.y + m_spacing;
            }
            else
            {
                rootSize = m_rootRectTransform.sizeDelta.x;
                childSize = m_childSize.x + m_spacing;
            }
            numCell = rootSize / childSize;
            m_showCellNum = (int)numCell;
            if (m_showCellNum < numCell)
            {
                m_showCellNum++;
            }

            m_showCellNum += 2;
        }

        /// <summary>
        /// 设置垫子的显示
        /// </summary>
        private void SetPadderShow()
        {
            if (m_logicCellNum < m_showCellNum)
            {
                m_firstPadder.gameObject.SetActive(false);
                m_lastPadder.gameObject.SetActive(false);
                return;
            }

            float firstSize = m_cellOffsetArray[m_logicCellStartIndex] - m_cellSizeArray[m_logicCellStartIndex];
            float lastSize = m_cellOffsetArray[m_logicCellNum - 1] - m_cellOffsetArray[m_logicCellEndIndex];

            UnityVector2 sizeFirst = m_containerRectTransform.sizeDelta;
            UnityVector2 sizeLast = m_containerRectTransform.sizeDelta;
            if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
            {
                sizeFirst.y = firstSize;
                sizeLast.y = lastSize;
                m_firstPadder.minHeight = firstSize;
                m_lastPadder.minHeight = lastSize;
            }
            else
            {
                sizeFirst.x = firstSize;
                sizeLast.x = lastSize;
                m_firstPadder.minWidth = firstSize;
                m_lastPadder.minWidth = lastSize;
            }
            m_firstPadder.GetComponent<UnityRectTransform>().sizeDelta = sizeFirst;
            m_lastPadder.GetComponent<UnityRectTransform>().sizeDelta = sizeLast;
            m_firstPadder.gameObject.SetActive(firstSize > 0);
            m_lastPadder.gameObject.SetActive(lastSize > 0);
        }

        /// <summary>
        /// 滚动窗滚动位置变化
        /// </summary>
        /// <param name="nowPos">滚动窗当前位置在容器中所占的比例</param>
        private void ScrollRectValueChange(UnityVector2 nowPos)
        {
            // 老虎机直接在Update中刷新
            if (m_isSlotMachine)
            {
                return;
            }

            RefreshScrollValue();
        }

        /// <summary>
        /// 刷新滚动窗位置
        /// </summary>
        private void RefreshScrollValue()
        {
            if (m_rollDirect == ROLL_DIRECT_ENUM.DIRECT_VERTICAL)
            {
                m_scrollPos = m_containerRectTransform.anchoredPosition3D.y;//(1.0f - nowPos.y) * m_containerRectTransform.sizeDelta.y;
            }
            else
            {
                m_scrollPos = -m_containerRectTransform.anchoredPosition3D.x;//nowPos.x * m_containerRectTransform.sizeDelta.x;
            }

            // 循环滚动需要调整滚动位置
            if (m_isloop)
            {
                bool needChange = true;
                UnityVector2 velocity = m_scrollRect.velocity;
                UnityVector3 anchorPos = m_containerRectTransform.anchoredPosition3D;
                if (m_scrollPos < m_loopFirstBorderPos)
                {
                    m_scrollPos = m_loopLastPosOfMidPart - (m_loopFirstBorderPos - m_scrollPos);
                }
                else if (m_scrollPos > m_loopLastBorderPos)
                {
                    m_scrollPos = m_loopFirstPosOfMidPart + (m_scrollPos - m_loopLastBorderPos);
                }
                else
                {
                    needChange = false;
                }

                if (needChange)
                {
                    if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
                    {
                        anchorPos.y = m_scrollPos;
                        m_scrollRect.verticalNormalizedPosition = 1f - (m_scrollPos / m_containerRectTransform.sizeDelta.y);
                    }
                    else
                    {
                        anchorPos.x = -m_scrollPos;
                        m_scrollRect.horizontalNormalizedPosition = m_scrollPos / m_containerRectTransform.sizeDelta.x;
                    }
                    m_scrollRect.velocity = velocity;
                    m_containerRectTransform.anchoredPosition3D = anchorPos;
                }
            }

            RefreshChildShow();
        }

        /// <summary>
        /// 刷新子单元显示
        /// </summary>
        /// <param name="isForce">是否强制执行</param>
        private void RefreshChildShow(bool isForce = false)
        {
            if (m_showCellNum > m_logicCellNum)
            {
                return;
            }

            int startIndex = 0;
            int endIndex = 0;

            // 计算子单元显示区间
            CalculateCurrentActiveCellRange(out startIndex, out endIndex);

            if (!isForce && startIndex == m_logicCellStartIndex && endIndex == m_logicCellEndIndex)
            {
                return;
            }

            // 移动数量
            int changeNum = UnityMathf.Abs(m_logicCellStartIndex - startIndex);

            // 子单元实体序号
            int childGoIndex = 0;

            // 子单元实体
            UnityTransform childCell = null;

            int realLogicIndex = 0;

            int tmp = 0;

            // 是否需要部分刷新
            bool isPartRefresh = false;

            // 是否需要全部刷新
            bool isAllRefresh = false;

            // 变动子单元顺序
            if (!isForce && changeNum > 0)
            {
                if (changeNum > 1)
                {
                    isAllRefresh = true;
                }
                else
                {
                    isPartRefresh = true;
                }
            }

            // 部分更新
            if (isPartRefresh)
            {
                if (startIndex < m_logicCellStartIndex)
                {
                    for (int i = 0; i < changeNum; i++)
                    {
                        // 将最后一个子单元移动到容器顶部
                        childGoIndex = m_sonOrderArray[m_showCellNum - 1];
                        childCell = m_childTransfroms[childGoIndex];
                        childCell.SetAsFirstSibling();

                        // 刷新被移动的子单元的显示内容
                        realLogicIndex = startIndex + changeNum - 1 - i;
                        if (m_isloop)
                        {
                            realLogicIndex %= m_loopRealLogicNum;
                        }
                        if (null != m_childRefreshDelegate)
                        {
                            m_childRefreshDelegate(childGoIndex, realLogicIndex);
                        }
                        if (null != m_childRefreshLuaCall)
                        {
                            m_childRefreshLuaCall.Invoke(childGoIndex, realLogicIndex);
                        }

                        // 重新设置实体创建序号
                        tmp = m_sonOrderArray[m_showCellNum - 1];
                        for (int j = m_showCellNum - 1; j > 0; j--)
                        {
                            m_sonOrderArray[j] = m_sonOrderArray[j - 1];
                        }
                        m_sonOrderArray[0] = tmp;
                    }
                    m_firstPadder.transform.SetAsFirstSibling();
                }
                else
                {
                    for (int i = 0; i < changeNum; i++)
                    {
                        // 将第一个子单元移动到容器底部
                        childGoIndex = m_sonOrderArray[i];
                        childCell = m_childTransfroms[childGoIndex];
                        childCell.SetAsLastSibling();

                        // 刷新被移动的子单元的显示内容
                        realLogicIndex = endIndex - changeNum + 1 + i;
                        if (m_isloop)
                        {
                            realLogicIndex %= m_loopRealLogicNum;
                        }
                        if (null != m_childRefreshDelegate)
                        {
                            m_childRefreshDelegate(childGoIndex, realLogicIndex);
                        }
                        if (null != m_childRefreshLuaCall)
                        {
                            m_childRefreshLuaCall.Invoke(childGoIndex, realLogicIndex);
                        }

                        // 重新设置实体创建序号
                        tmp = m_sonOrderArray[0];
                        for (int j = 0; j < m_showCellNum - 1; j++)
                        {
                            m_sonOrderArray[j] = m_sonOrderArray[j + 1];
                        }
                        m_sonOrderArray[m_showCellNum - 1] = tmp;
                    }
                    m_lastPadder.transform.SetAsLastSibling();
                }
            }

            // 全部更新
            if (isAllRefresh)
            {
                for (int i = 0; i < m_showCellNum; i++)
                {
                    childGoIndex = m_sonOrderArray[i];

                    // 刷新被移动的子单元的显示内容
                    realLogicIndex = startIndex + i;
                    if (m_isloop)
                    {
                        realLogicIndex %= m_loopRealLogicNum;
                    }
                    if (null != m_childRefreshDelegate)
                    {
                        m_childRefreshDelegate(childGoIndex, realLogicIndex);
                    }

                    if (null != m_childRefreshLuaCall)
                    {
                        m_childRefreshLuaCall.Invoke(childGoIndex, realLogicIndex);
                    }
                }
            }

            m_logicCellStartIndex = startIndex;
            m_logicCellEndIndex = endIndex;

            // 刷新首尾两端的垫子
            SetPadderShow();
        }

        /// <summary>
        /// 设置子单元显示
        /// </summary>
        private void SetChildVisible()
        {
            for (int i = 0; i < m_childTransfroms.Length; i++)
            {
                if (!m_isloop && m_showCellNum > m_logicCellNum && i >= m_logicCellNum)
                {
                    m_childTransfroms[i].gameObject.SetActive(false);
                    //UnityGameObject.Destroy(m_childTransfroms[i].gameObject);
                }
                else
                {
                    m_childTransfroms[i].gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 计算当前滚动位置对应的显示单元
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        private void CalculateCurrentActiveCellRange(out int startIndex, out int endIndex)
        {
            startIndex = 0;
            endIndex = m_showCellNum - 1;

            startIndex = GetCellViewIndexWithPosition(m_scrollPos);
            if(startIndex < 0)
            {
                startIndex = 0;
            }

            endIndex = startIndex + m_showCellNum - 1;
            if (m_showCellNum > m_logicCellNum && endIndex <= m_logicCellNum)
            {
                endIndex = m_logicCellNum - 1;
            }
            else if(endIndex >= m_logicCellNum)
            {
                endIndex = m_logicCellNum - 1;
                startIndex = endIndex - m_showCellNum + 1;
            }
        }

        /// <summary>
        /// 根据给定滚动位置计算子单元的逻辑编号
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int GetCellViewIndexWithPosition(float pos)
        {
            return GetCellViewIndexWithPosition(pos, 0, m_logicCellNum - 1);
        }

        /// <summary>
        /// 在制定区间
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private int GetCellViewIndexWithPosition(float pos, int startIndex, int endIndex)
        {
            if (startIndex >= endIndex)
            {
                return startIndex;
            }

            int middleIndex = (startIndex + endIndex) / 2;

            // 二分查找
            if (m_cellOffsetArray[middleIndex] + (m_rollDirect == ROLL_DIRECT_ENUM.DIRECT_VERTICAL ? m_padding.top : m_padding.left) >= pos)
            {
                return GetCellViewIndexWithPosition(pos, startIndex, middleIndex);
            }
            else
            {
                return GetCellViewIndexWithPosition(pos, middleIndex + 1, endIndex);
            }
        }

        /// <summary>
        /// 获取逻辑子单元在滚动窗中的位置
        /// </summary>
        /// <param name="logicIndex">子单元序号</param>
        /// <param name="relative">相对位置</param>
        /// <returns></returns>
        private float GetScrollPosForLogicChildIndex(int logicIndex, ROLL_CELL_POS_ENUM relative)
        {
            if (m_logicCellNum <= 0)
            {
                return 0;
            }

            if (0 == logicIndex && ROLL_CELL_POS_ENUM.BEFORE == relative)
            {
                return 0;
            }

            if (logicIndex >= m_logicCellNum)
            {
                return m_cellOffsetArray[m_logicCellNum - 2];
            }

            float pos = 0;
            if (ROLL_CELL_POS_ENUM.BEFORE == relative)
            {
                pos = m_cellOffsetArray[logicIndex - 1] + m_spacing;
            }
            else
            {
                pos = m_cellOffsetArray[logicIndex];
            }
            if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
            {
                pos += m_padding.top;
            }
            else
            {
                pos += m_padding.left;
            }

            return pos;
        }
        #endregion

        #region 老虎机相关的实现
        /// <summary>
        /// 开始拖动
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (!m_isSlotMachine)
            {
                return;
            }

            m_isAutoMoving = false;
        }

        /// <summary>
        /// 结束拖动
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (!m_isSlotMachine)
            {
                return;
            }

            BeganAutoMove();
        }

        /// <summary>
        /// 开始自由滚动
        /// </summary>
        private void BeganAutoMove()
        {
            m_autoScrollTargetIndex = -1;
            m_isAutoMoving = true;
        }

        /// <summary>
        /// 每一帧刷新
        /// </summary>
        void Update()
        {
            // 只有老虎机才需要系统控制
            if (!m_isSlotMachine)
            {
                return;
            }

            RefreshScrollValue();

            // 玩家正在触控操作，则只刷新焦点显示，不控制滚动窗
            if (!m_isAutoMoving)
            {
                CalculateFocusChildIndex();
                return;
            }

            // 当前滚动窗的惯性滚动速度
            float inertiaSpeed = 0;
            if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
            {
                inertiaSpeed = m_scrollRect.velocity.y;
            }
            else
            {
                inertiaSpeed = m_scrollRect.velocity.x;
            }

            if (inertiaSpeed < 0)
            {
                inertiaSpeed = 0 - inertiaSpeed;
            }

            // 未达到临界值，继续保持惯性滚动
            if (inertiaSpeed > MAX_VELOCITY_AUTO_MOVE)
            {
                CalculateFocusChildIndex();
                return;
            }
            if (m_autoScrollTargetIndex < 0)
            {
                CalculateFocusChildIndex(true);
            }

            // 停止惯性滚动
            m_scrollRect.velocity = UnityVector2.zero;

            // 滚动容器当前位置
            UnityVector3 containerPos = m_containerRectTransform.anchoredPosition3D;

            // 滚动容器的目标位置
            UnityVector3 targetPos = containerPos;

            // 滚动容器距离目标点的距离
            float offsetDistance = 0;
            if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
            {
                offsetDistance = containerPos.y;
                targetPos.y = m_autoScrollTargetPos;
            }
            else
            {
                offsetDistance = containerPos.x;
                targetPos.x = m_autoScrollTargetPos;
            }
            offsetDistance = offsetDistance - m_autoScrollTargetPos;
            if (offsetDistance < 0)
            {
                offsetDistance = 0 - offsetDistance;
            }

            // 检查自动滚动是否完成
            if (offsetDistance < MIN_AUTO_MOVE_SPEED)
            {
                m_containerRectTransform.anchoredPosition3D = targetPos;
                int indexOrder = m_autoScrollTargetIndex - m_logicCellStartIndex;
                if (indexOrder < 0 || indexOrder > m_sonOrderArray.Length)
                {
                    Debuger.LogError("自动滚动完成，计算子单元编号错误！");
                }
                else
                {
                    m_finishCallBack(m_sonOrderArray[indexOrder]);
                }
                m_isAutoMoving = false;
                m_autoScrollTargetIndex = -1;
                return;
            }

            // 自动滚动
            UnityVector3 pos = m_containerRectTransform.anchoredPosition3D;
            if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
            {
                pos.y = UnityMathf.Lerp(m_containerRectTransform.anchoredPosition3D.y, targetPos.y, UnityTime.deltaTime * m_autoScrollSpeed);
            }
            else
            {
                pos.x = UnityMathf.Lerp(m_containerRectTransform.anchoredPosition3D.x, targetPos.x, UnityTime.deltaTime * m_autoScrollSpeed);
            }
            m_containerRectTransform.anchoredPosition3D = pos;
            CalculateFocusChildIndex();
        }

        /// <summary>
        /// 计算当前焦点显示的子单元
        /// <param name="needComfirmTarget">是否需要确定目标点
        /// </summary>
        private void CalculateFocusChildIndex(bool needComfirmTarget = false)
        {
            if (null == m_focusDelegate)
            {
                Debuger.LogError("焦点刷新的委托方法不能为空！！！");
                return;
            }

            // 窗口中心线当前的滚动位置
            float nowCenterPos = 0;

            float halfWindowSize = 0;

            if (ROLL_DIRECT_ENUM.DIRECT_VERTICAL == m_rollDirect)
            {
                halfWindowSize = m_rootRectTransform.sizeDelta.y * 0.5f;
                nowCenterPos = m_containerRectTransform.anchoredPosition3D.y + halfWindowSize;
            }
            else
            {
                halfWindowSize = m_rootRectTransform.sizeDelta.x * 0.5f;
                nowCenterPos = -m_containerRectTransform.anchoredPosition3D.x + halfWindowSize;
            }


            // 计算窗口中心线的逻辑子单元编号
            int centerLogicIndex = GetCellViewIndexWithPosition(nowCenterPos + 1);

            // 窗口中心线的逻辑子单元的中心线位置
            float realCenterPos = m_cellOffsetArray[centerLogicIndex] - m_cellSizeArray[centerLogicIndex] * 0.5f;

            // 第一个子单元偏离该子单元中心的比例
            float offRate = (nowCenterPos - realCenterPos) / m_cellSizeArray[centerLogicIndex];
            if (offRate > 1 || offRate < -1)
            {
                Debuger.LogError("CalculateFocusChildIndex 计算有误！！！！");
            }

            int firstLogicIndex = 0;
            int secondLogicIndex = 0;
            float firstRate = 0;

            if (offRate > 0)
            {
                firstLogicIndex = centerLogicIndex;
                secondLogicIndex = centerLogicIndex + 1;
                firstRate = 1 - offRate;
            }
            else
            {
                firstLogicIndex = centerLogicIndex - 1;
                secondLogicIndex = centerLogicIndex;
                firstRate = 0 - offRate;
            }

            // 刷新显示
            int indexOfSonOne = firstLogicIndex - m_logicCellStartIndex;
            int indexOfSonTwo = secondLogicIndex - m_logicCellStartIndex;
            if (indexOfSonOne > m_showCellNum)
            {
                indexOfSonOne %= m_showCellNum;
            }
            if (indexOfSonTwo > m_showCellNum)
            {
                indexOfSonTwo %= m_showCellNum;
            }

            if (indexOfSonOne < 0 || indexOfSonOne >= m_sonOrderArray.Length || indexOfSonTwo < 0 || indexOfSonTwo >= m_sonOrderArray.Length)
            {
                Debuger.LogError("计算焦点位置有误！！！");
            }
            else
            {
                m_focusDelegate(m_sonOrderArray[indexOfSonOne], m_sonOrderArray[indexOfSonTwo], firstRate);
            }

            // 记录目标点位置
            if (needComfirmTarget)
            {
                m_autoScrollTargetPos = realCenterPos - halfWindowSize;
                if (ROLL_DIRECT_ENUM.DIRECT_HORIZONTAL == m_rollDirect)
                {
                    m_autoScrollTargetPos = 0 - m_autoScrollTargetPos;
                }
                m_autoScrollTargetIndex = centerLogicIndex;
            }

            if (m_autoScrollTargetIndex >= 0)
            {
                if (offRate < 0)
                {
                    offRate = 0 - offRate;
                }
                m_autoScrollSpeed = (1 - offRate) * MAX_AUTO_SPEED_BASE;
            }
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="child">子节点</param>
        /// <returns>是否添加成功</returns>
        private bool AddChild(UnityTransform root, UnityTransform child)
        {
            if (null == root || null == child)
            {
                Core.Debuger.LogError("AddChild() root and child can not be null!");
                return false;
            }
            child.SetParent(root);
            child.localScale = UnityVector3.one;
            child.localPosition = UnityVector3.zero;
            child.localEulerAngles = UnityVector3.zero;

            // 默认显示为同级最前
            child.SetAsLastSibling();
            return true;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 子单元实体数组
        /// </summary>
        public UnityTransform[] ChildCells
        {
            get { return m_childTransfroms; }
        }

        /// <summary>
        /// 子单元排列顺序
        /// </summary>
        public int[] ChildrenShowOrder
        {
            get { return m_sonOrderArray; }
        }

        /// <summary>
        /// 当前显示内容的最新的逻辑起始序号
        /// </summary>
        public int NowLogicStart
        {
            get { return m_logicCellStartIndex; }
        }

        /// <summary>
        /// 当前窗口子单元实体数量
        /// </summary>
        public int WindownContainChildNum
        {
            get
            {
                if (m_showCellNum <= m_loopRealLogicNum)
                {
                    return m_showCellNum;
                }
                else
                {
                    return m_loopRealLogicNum;
                }
            }
        }

        /// <summary>
        /// 子单元刷新委托方法
        /// </summary>
        public RefreshChildContentDelegate RefreshChildDelegate
        {
            set
            {
                if (value == m_childRefreshDelegate)
                {
                    return;
                }
                m_childRefreshDelegate = value;
                for (int i = 0; i < m_showCellNum; i++)
                {
                    if (m_isloop)
                    {
                        m_childRefreshDelegate(m_sonOrderArray[i], (m_logicCellStartIndex + i) % m_loopRealLogicNum);
                    }
                    else
                    {
                        m_childRefreshDelegate(m_sonOrderArray[i], m_logicCellStartIndex + i);
                    }

                }
            }
        }

        public Action<int, int> ChildRefreshLuaCall
        {
            set
            {
                m_childRefreshLuaCall = value;
                for (int i = 0; i < m_showCellNum; i++)
                {
                    if (m_isloop)
                    {
                        m_childRefreshLuaCall.Invoke(m_sonOrderArray[i], (m_logicCellStartIndex + i) % m_loopRealLogicNum);
                    }
                    else
                    {
                        m_childRefreshLuaCall.Invoke(m_sonOrderArray[i], m_logicCellStartIndex + i);
                    }
                }
            }
        }
        #endregion
    }
}


