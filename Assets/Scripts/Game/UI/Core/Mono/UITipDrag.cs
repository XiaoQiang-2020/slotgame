
namespace Game
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
  

    public class UITipDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler ,IPointerClickHandler
    {
        float startX = 0;

        float startY = 0;

        // 拖拽
        private Action<int> m_dragCall = null;

        // 拖拽
        public Action<int> DragCall
        {
            set
            {
                m_dragCall = value;
            }
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        private Action m_clickCall = null;
        public Action ClickCall
        {
            set
            {
                m_clickCall = value;
            }
        }

        RectTransform m_draggingBack;

        /// <summary>
        /// 滑动小于这个数就是点击
        /// </summary>
        private const float DRAG_LENGTH = 10;

        // 拖拽方向
        private enum DRAG_DIR
        {
            Vertical = 1,
            Horizontal = 2,
            NONE = 0,
        }

        private DRAG_DIR currDir = DRAG_DIR.NONE;

        /// <summary>
        /// 当前方向
        /// </summary>
        private Vector3 currDragDir = Vector3.zero;

        /// <summary>
        /// ui开始点
        /// </summary>
        private Vector3 startPos = Vector3.zero;

        /// <summary>
        /// 开始拖拽的点
        /// </summary>
        private Vector2 beginDragPos = Vector2.zero;

        private void Start()
        {
            startPos = transform.localPosition;
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            startX = eventData.position.x;
            startY = eventData.position.y;
            beginDragPos = eventData.position;
        }
        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            var endPos = eventData.position;
            var dir = 0;
            if (null != m_dragCall)
            {
                if (currDir == DRAG_DIR.Horizontal)
                {
                    if (endPos.x > startX)
                    {
                        dir = 1;
                    } else
                    {
                        dir = 2;
                    }
                }
                else if (currDir == DRAG_DIR.Vertical)
                {
                    dir = 3;
                }
                m_dragCall?.Invoke(dir);
            }
            startX = 0;
            startY = 0;
            currDir = DRAG_DIR.NONE;
            currDragDir = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            var endPos = eventData.position;
            if (currDir == DRAG_DIR.NONE)
            {
                float lerpX = endPos.x - startX;
                float lerpY = endPos.y - startY;
                if (Mathf.Abs(lerpX) < DRAG_LENGTH && Mathf.Abs(lerpY) < DRAG_LENGTH)
                {
                    return;
                }
                if (Mathf.Abs(lerpX) > Mathf.Abs(lerpY))
                {
                    currDir = DRAG_DIR.Horizontal;
                }
                else
                {
                    currDir = DRAG_DIR.Vertical;
                }
            }
            float dragY = endPos.y - beginDragPos.y > 0 ?endPos.y - beginDragPos.y :0;
            currDragDir = currDir == DRAG_DIR.Vertical ? new Vector3(0, dragY, 0) : new Vector3(endPos.x - beginDragPos.x, 0, 0);

            transform.localPosition = startPos + currDragDir;
        }

        /// <summary>
        /// 回调点击事件
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (currDir != DRAG_DIR.NONE )
            {
                return;
            }
            if (null != m_clickCall)
            {
                m_clickCall?.Invoke();
            }
        }
    }
}
