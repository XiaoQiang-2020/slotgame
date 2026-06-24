/***************************************************
 * 文件名：EventTriggerListener.cs
 * 描  述：统一将所有Unity Ui 事件从C#导出到Lua中
 * 时  间：2018-07-06 17:05:46
 * 作  者：Aven
 * 修  改：文阳贤
 ***************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
namespace Game
{
    /// <summary>
    /// UGUI中的按钮监听事件
    /// </summary>
    public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
    {
        public delegate void VoidDelegate(GameObject go, object data);
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;
        public VoidDelegate onPress;
        public VoidDelegate onDrag;
        public VoidDelegate onBeginDrag;
        public VoidDelegate onEndDrag;
        public VoidDelegate onScroll;

        private object m_Param = null;


        static public EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerListener>();
            return listener;
        }
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="insObject"></param>
        /// <param name="eventType"></param>
        /// <param name="myFunction"></param>
        static public void AddEventTrigger(GameObject insObject, EventTriggerType eventType, UnityAction<BaseEventData> myFunction)//泛型委托
        {
            if (insObject==null)
            {
                return ;
            }
            EventTriggerListener eventTrigger = Get(insObject);
            EventTriggerListener.Entry entry = new EventTriggerListener.Entry();
            entry.eventID = eventType;
            entry.callback.AddListener(myFunction);
            eventTrigger.triggers.Add(entry);
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (onClick != null) onClick(gameObject, m_Param);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            isPress = true;
            base.OnPointerDown(eventData);
            if (onDown != null) onDown(gameObject, m_Param);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (onEnter != null) onEnter(gameObject, m_Param);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (onExit != null) onExit(gameObject, m_Param);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            isPress = false;
            base.OnPointerUp(eventData);
            if (onUp != null) onUp(gameObject, m_Param);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (onSelect != null) onSelect(gameObject, m_Param);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            base.OnUpdateSelected(eventData);
            if (onUpdateSelect != null) onUpdateSelect(gameObject, m_Param);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (onBeginDrag != null) onBeginDrag(gameObject, m_Param);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            if (onDrag != null) onDrag(gameObject, m_Param);
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (onEndDrag != null) onEndDrag(gameObject, m_Param);
        }
        public override void OnScroll(PointerEventData eventData)
        {
            base.OnScroll(eventData);
            if (onScroll != null) onScroll(gameObject, m_Param);
        }

        protected bool isPress = false;
        protected void OnPress()
        {
            if (onPress != null)
                onPress(gameObject, m_Param);
        }
        void Update()
        {
            if (isPress)
                OnPress();
        }

        public void SetParam(object data)
        {
            m_Param = data;
        }
    }

}
