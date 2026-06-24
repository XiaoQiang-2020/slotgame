/***************************************************
 * 文件名：ButtonEffect.cs
 * 描  述：按钮上的一些特殊效果
 * 时  间：2018-04-11 19:09:39
 * 作  者：李红轩
 * 修  改：
 ***************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class ButtonEffect : MonoBehaviour
{
    /// <summary>
    /// 变大的效果
    /// </summary>
    public Vector3 ToBigVector = new Vector3(1.05f, 1.05f, 1.05f);
	// Use this for initialization
	void Start ()
    {
        AddEventTrigger(transform, EventTriggerType.PointerDown, OnClickDown);
        AddEventTrigger(transform, EventTriggerType.PointerUp, OnClickUp);
    }
    /// <summary>
    /// 点下去
    /// </summary>
    /// <param name="data"></param>
    public void OnClickDown(BaseEventData data)
    {
        transform.localScale = ToBigVector;
    }
    /// <summary>
    /// 抬起来
    /// </summary>
    /// <param name="data"></param>
    public void OnClickUp(BaseEventData data)
    {
        transform.localScale = Vector3.one;
    }
    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="insObject"></param>
    /// <param name="eventType"></param>
    /// <param name="myFunction"></param>
    public void AddEventTrigger(Transform insObject, EventTriggerType eventType, UnityAction<BaseEventData> myFunction)//泛型委托
    {
        EventTrigger eventTri = insObject.GetComponent<EventTrigger>();
        if (eventTri == null) { eventTri = insObject.gameObject.AddComponent<EventTrigger>(); }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(myFunction);
        eventTri.triggers.Add(entry);
    }
}