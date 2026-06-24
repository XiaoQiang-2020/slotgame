/***************************************************
 * 文件名：UIDepth.cs
 * 描  述：
 * 时  间：2018-09-14 19:05:42
 * 作  者：尼尔
 * 修  改：
 ***************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 
public class UIDepth : MonoBehaviour {
	public int order;
	public bool isUI = true;
	void Start () 
	{
		if(isUI){
			Canvas canvas = GetComponent<Canvas>();
			if( canvas == null){
				canvas = gameObject.AddComponent<Canvas>();
			}
			canvas.overrideSorting = true;
			canvas.sortingOrder = order;
		}
		else
		{
			Renderer []renders  =  GetComponentsInChildren<Renderer>();
 
			foreach(Renderer render in renders){
				render.sortingOrder = order;
			}
		}
	}
}

