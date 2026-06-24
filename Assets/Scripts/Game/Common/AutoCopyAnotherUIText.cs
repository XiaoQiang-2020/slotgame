/***************************************************
 * 文件名： AutoCopyAnotherUIText.cs
 * 描  述：
 * 时  间： 2017-06-07 14:23:32
 * 作  者： 李智海
 * 修  改： 自动另一个Text的内空到自身的Text中的，即用于将这自身的文本内容与另一个的时刻保持一至
 ***************************************************/
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/AutoCopyAnotherUIText")]
[ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Text))]
public class AutoCopyAnotherUIText : MonoBehaviour
{
    public Text anotherText;
    Text selfText;

	// Use this for initialization
	void Start ()
    {
        selfText =  transform.GetComponent<Text>();
        if (anotherText != null) {
            selfText.text = anotherText.text;
        }
    }

    void Update()
    {
        if(anotherText != null)
        {
            selfText.text = anotherText.text;
        }
    }


}

