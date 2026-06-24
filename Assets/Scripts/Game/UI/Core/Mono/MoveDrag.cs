/***************************************************
 * 文件名：MoveDrag.cs
 * 描  述：滑动出牌
 * 时  间：2018-07-18 18:24:49
 * 作  者：文阳贤
 * 修  改：
 ***************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
namespace Game
{
    using UnityGameObject = UnityEngine.GameObject;
    using UnityEngineTransform = UnityEngine.Transform;
    using UnityEngineMonoBehaviour = UnityEngine.MonoBehaviour;
    using UnityEngineRequireComponent = UnityEngine.RequireComponent;
    using UnityEngineRectTransform = UnityEngine.RectTransform;
    using UnityEngineRectTransformUtility = UnityEngine.RectTransformUtility;
    using UnityEngineCanvasGroup = UnityEngine.CanvasGroup;
    using UnityEngineGameObject = UnityEngine.GameObject;
    using UnityEngineVector3 = UnityEngine.Vector3;
    using UnityEngineEventSystemsPointerEventData = UnityEngine.EventSystems.PointerEventData;
    using UnityEngineEventSystemsIBeginDragHandler = UnityEngine.EventSystems.IBeginDragHandler;
    using UnityEngineEventSystemsIDragHandler = UnityEngine.EventSystems.IDragHandler;
    using UnityEngineEventSystemsIEndDragHandler = UnityEngine.EventSystems.IEndDragHandler;
    using UnityEngineUIImage = UnityEngine.UI.Image;


    [UnityEngineRequireComponent(typeof(UnityEngineUIImage))]
    public class MoveDrag : UnityEngineMonoBehaviour
    {
        // 扑克UI之间的间隔距离
        float WIDTH_ONE_POKER_INTERVAL = 60;

        float WIDTH_ONE_POKER_INTERVAL_MAX = 75f;
        float SCREEN_WIDTH = 1280f;
        float SCREEN_HEIGHT = 720f;
        //牌型宽度
        float WIDTH_ONE_POKER_WIDTH = 141f;

        //牌型高度
        float WIDTH_ONE_POKER_HEIGHT = 186f;
        //手牌最大数量
        int MAX_HANDCARDS_NUM = 20;

        //手指开始按下的位置  
        private Vector2 m_vectorTouchFirst = Vector2.zero;

        //手指拖动的位置  
        private Vector2 m_vectorTouchSecond = Vector2.zero;

        //时间计数器    
        private float m_countTimer = 0f;

        //滑动选择的类型
         enum MOVE_SELECTTYPE_ENUM
        {
            DISCARD_SELECTED = 0,//筛选未选中的牌
            SELECTING = 1,//正在选中状态
            SELECTED = 2//最终选中状态    
        };

        //判断的时间间隔  
        public float m_offsetTime = 0.1f;

        //手牌目录下 扑克列表
        private List<RectTransform> m_listHandCards = new List<RectTransform>();

        //手牌根节点
        private UnityEngineTransform m_uiRootNode = null;

        //滑动事件lua回调
        private Action<int,int> m_dragEventCallBack = null;

        // 是否是竖屏
        private bool m_isVertical = false;

        //从鼠标按下到鼠标弹起，这个阶段中，满足条件的手牌列表
        List<RectTransform> m_listSelectedTargetedCards = new List<RectTransform>();

        // 从鼠标按下到持续拖动，这个阶段中，每帧满足条件的临时手牌列表
        List<RectTransform> m_listSelectedTempCards = new List<RectTransform>();

        //是否开启事件
        private bool m_isOpen = false;

        //手牌相机
        Camera m_uiCarmera = null;

        //是否点击
        bool m_isClick = false;

        float m_widthHandPokers = 0;
        //实际牌之间的间距
        float m_offSetValidSpace = 0;
        // Use this for initialization
        void Start()
        {
            m_uiCarmera = GameObject.Find("UICamera").GetComponent<Camera>();
            m_widthHandPokers = (MAX_HANDCARDS_NUM - 1) * WIDTH_ONE_POKER_INTERVAL;
        }
        //是否开启滑动选择功能
        public bool IsOpen
        {
            set { m_isOpen = value; }
        }

        /// <summary>
        /// 创建扑克滑动选择
        /// </summary>
        /// <param name="rootNode">根节点</param>
        /// <param name="callBack">lua函数回调</param>
        /// <returns>MoveDrag类型</returns>
        public static MoveDrag CreatMoveDrag(UnityEngineTransform rootNode, Action<int,int> callBack, bool isVertical)
        {
            if (null == rootNode || null == callBack)
            {
                return null;
            }
            MoveDrag dragScript = rootNode.GetComponent<MoveDrag>();
            if (null == dragScript)
            {
                dragScript = rootNode.gameObject.AddComponent<MoveDrag>();
                if (null == dragScript)
                {
                    return null;
                }
            }
            dragScript.m_dragEventCallBack = callBack;
            dragScript.m_isOpen = true;
            dragScript.m_uiRootNode = rootNode;
            dragScript.m_isVertical = isVertical;
            return dragScript;
        }

        /// <summary>
        /// 筛选滑动时满足条件的扑克牌
        /// </summary>
        /// <returns>满足调节的数组列表</returns>
        private void GetCardsSelect()
        {
#if UNITY_EDITOR||UNITY_STANDALONE_WIN
            m_vectorTouchSecond = Input.mousePosition; //记录开始按下的位置  
#elif UNITY_ANDROID || UNITY_IOS
         m_vectorTouchSecond = Input.GetTouch(0).position; //记录开始按下的位置  
#endif

            Vector2 slideDirection = m_vectorTouchFirst - m_vectorTouchSecond; //开始减去结束
            float xMove = slideDirection.x;
            float yMove = slideDirection.y;

            m_isClick = (slideDirection == Vector2.zero ? true : false);

            // 标准的面板宽高
            float realStandardCanvasWidth = 0;
            float realStandardCanvasHeight = 0;

            if (m_isVertical)
            {
                if (0 == yMove)
                {
                    return;
                }
                realStandardCanvasWidth = SCREEN_HEIGHT;
                realStandardCanvasHeight = SCREEN_WIDTH;
                float lossyScaleX = (float)Screen.height / realStandardCanvasHeight; 
                float lossyScaleY = (float)Screen.width / realStandardCanvasWidth;
                float offset = m_offSetValidSpace * lossyScaleX;
                Vector2 nowCardWorldPos;

                // 半张牌的宽度
                float halfCardWidth = WIDTH_ONE_POKER_WIDTH * lossyScaleX * 0.5f;

                // 半张牌的高度
                float halfCardHeight = WIDTH_ONE_POKER_HEIGHT * lossyScaleY * 0.5f;

                //处理范围内的牌，加入选牌列表
                for (int i = 0; i < m_listHandCards.Count; i++)
                {
                    if (i == m_listHandCards.Count - 1)
                    {
                        offset = WIDTH_ONE_POKER_WIDTH * lossyScaleX;
                    }

                    if (m_listHandCards[i] == null || !m_listHandCards[i].parent.gameObject.activeSelf)
                    {
                        continue;                        
                    }

                    if (m_listSelectedTempCards.Contains(m_listHandCards[i]))
                    {
                        continue;
                    }

                    // 牌的世界坐标
                    nowCardWorldPos = RectTransformUtility.WorldToScreenPoint(m_uiCarmera, m_listHandCards[i].position);

                    //牌左侧点
                    float leftPiont = nowCardWorldPos.y + halfCardWidth;

                    //牌上侧点
                    float topPoint = nowCardWorldPos.x + halfCardHeight;

                    //牌下侧点   
                    float bottomPoint = nowCardWorldPos.x - halfCardHeight;

                    // 触摸结束点在牌的高度方向上
                    if (m_vectorTouchSecond.x < bottomPoint || m_vectorTouchSecond.x > topPoint)
                    {
                        continue;
                    }

                    //向左滑动
                    if (yMove < 0)
                    {                        
                        // 滑动范围都在左侧边界区域
                        if (m_vectorTouchSecond.y < leftPiont - offset || m_vectorTouchFirst.y > leftPiont)
                        {
                            continue;                            
                        }                                                
                    }

                    //向右滑动
                    else if (yMove > 0) 
                    {                        
                        if (m_vectorTouchSecond.y > leftPiont || m_vectorTouchFirst.y < leftPiont - offset)
                        {
                            continue;
                        }                        
                    }
                    m_listSelectedTempCards.Add(m_listHandCards[i]);
                }
            }
            else
            {
                if (0 == xMove)
                {
                    return;
                }
                realStandardCanvasWidth = SCREEN_WIDTH;
                realStandardCanvasHeight = SCREEN_HEIGHT;
                float lossyScaleX = (float)Screen.width / realStandardCanvasWidth;
                float lossyScaleY = (float)Screen.height / realStandardCanvasHeight;
                float offset = m_offSetValidSpace * lossyScaleX;
                Vector2 nowCardWorldPos;

                // 半张牌的宽度
                float halfCardWidth = WIDTH_ONE_POKER_WIDTH * lossyScaleX * 0.5f;

                // 半张牌的高度
                float halfCardHeight = WIDTH_ONE_POKER_HEIGHT * lossyScaleY * 0.5f;

                //处理范围内的牌，加入选牌列表
                for (int i = 0; i < m_listHandCards.Count; i++)
                {
                    if (i == m_listHandCards.Count - 1)
                    {
                        offset = WIDTH_ONE_POKER_WIDTH * lossyScaleX;
                    }                    

                    if (m_listHandCards[i] == null || !m_listHandCards[i].parent.gameObject.activeSelf)
                    {
                        continue;
                    }

                    if (m_listSelectedTempCards.Contains(m_listHandCards[i]))
                    {
                        continue;
                    }
                    // 牌的世界坐标
                    nowCardWorldPos = RectTransformUtility.WorldToScreenPoint(m_uiCarmera, m_listHandCards[i].position);

                    //牌左侧点
                    float leftPiont = nowCardWorldPos.x - halfCardWidth;

                    //牌上侧点
                    float topPoint = nowCardWorldPos.y + halfCardHeight;

                    //牌下侧点   
                    float bottomPoint = nowCardWorldPos.y - halfCardHeight;

                    // 触摸结束点在牌的高度方向上
                    if (m_vectorTouchSecond.y < bottomPoint || m_vectorTouchSecond.y > topPoint)
                    {
                        continue;
                    }

                    //向左滑动
                    if (xMove > 0) 
                    {                        
                        // 滑动范围都在牌的左侧区域
                        if (m_vectorTouchSecond.x > leftPiont + offset || m_vectorTouchFirst.x < leftPiont)
                        {
                            continue;
                        }                        
                    }
                    else if (xMove < 0) //向右滑动
                    {
                        if (m_vectorTouchSecond.x < leftPiont || m_vectorTouchFirst.x > leftPiont + offset)
                        {
                            continue;                                
                        }                        
                    }
                    m_listSelectedTempCards.Add(m_listHandCards[i]);
                }
            }
        }

        // 函数功能：    返回手牌之间的牌距
        // 参数:         cardNum:牌的数量
        // 返回值：      牌距

        private float GetSpace(int cardNum)
        {
            float space = WIDTH_ONE_POKER_INTERVAL;
            if (cardNum > 1)
            {
                space = (float)((m_widthHandPokers - WIDTH_ONE_POKER_WIDTH) / (cardNum - 1));
            }
            else
            {
                return 0;
            }
            space = space > WIDTH_ONE_POKER_INTERVAL_MAX ? WIDTH_ONE_POKER_INTERVAL_MAX : space;
            return space;
        }

        void Update() // 滑动方法  
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetMouseButtonDown(0) == true) //判断当前手指是按下事件  
#elif UNITY_ANDROID || UNITY_IOS
            if(Input.touchCount > 0 && Input.GetTouch(0).phase==TouchPhase.Began)
#endif
            {
                if (!m_isOpen)
                {
                    return;
                }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                m_vectorTouchFirst = Input.mousePosition; //记录开始按下的位置  
#elif UNITY_ANDROID || UNITY_IOS
         m_vectorTouchFirst = Input.GetTouch(0).position; 
#endif
                m_listHandCards.Clear();
                m_listSelectedTargetedCards.Clear();
                int childCount = m_uiRootNode.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    m_listHandCards.Add(m_uiRootNode.GetChild(i).GetComponent<RectTransform>());
                }
                m_offSetValidSpace = GetSpace(childCount);
            }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetMouseButton(0) == true) //判断当前手指持续按下或滑动 
#elif UNITY_ANDROID || UNITY_IOS
           if(Input.touchCount > 0 && (Input.GetTouch(0).phase==TouchPhase.Moved))
#endif
            {
                if (!m_isOpen)
                {
                    return;
                }
                m_countTimer += Time.deltaTime;
                if (m_countTimer > m_offsetTime)
                {
                    m_countTimer = 0;
                    m_listSelectedTempCards.Clear();
                    m_isClick = false;
                    GetCardsSelect();
                    if (!m_isClick)
                    {
                        for (int i = 0; i < m_listHandCards.Count; i++)
                        {
                            if(m_listHandCards[i]==null)
                            {
                                continue;
                            }
                            if (m_listSelectedTempCards.Contains(m_listHandCards[i]))
                            {
                                if (!m_listSelectedTargetedCards.Contains(m_listHandCards[i]))
                                {
                                    m_listSelectedTargetedCards.Add(m_listHandCards[i]);
                                    m_dragEventCallBack.Invoke((int)MOVE_SELECTTYPE_ENUM.SELECTING, m_listHandCards[i].GetSiblingIndex());
                                }
                            }
                            else
                            {
                                if (m_listSelectedTargetedCards.Contains(m_listHandCards[i]))
                                {
                                       m_listSelectedTargetedCards.Remove(m_listHandCards[i]);
                                    m_dragEventCallBack.Invoke((int)MOVE_SELECTTYPE_ENUM.DISCARD_SELECTED, m_listHandCards[i].GetSiblingIndex());
                                 
                                }
                            }
                        }
                    }
                    m_isClick = false;
                }
            }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetMouseButtonUp(0))
#elif UNITY_ANDROID || UNITY_IOS
if (Input.touchCount > 0 && (Input.GetTouch(0).phase==TouchPhase.Ended || Input.GetTouch(0).phase==TouchPhase.Canceled))
#endif
            {
                if (!m_isOpen)
                {
                    return;
                }
                if (null != m_dragEventCallBack)
                {
                    m_dragEventCallBack.Invoke((int)MOVE_SELECTTYPE_ENUM.SELECTED,0);
                }
                m_listSelectedTargetedCards.Clear();
                m_listSelectedTempCards.Clear();
            }
        }
    }
}
