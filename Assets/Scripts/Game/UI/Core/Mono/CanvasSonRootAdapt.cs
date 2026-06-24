/***************************************************
 * 文件名：CanvasSonRootAdapt.cs
 * 描  述：
 * 时  间：2018-11-17 17:53:15
 * 作  者：李海波
 * 修  改：
 ***************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSonRootAdapt : MonoBehaviour {

	// Use this for initialization
	void Start () {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Game.GlobalVar.STANDORD_SCREEN_WIDTH, Game.GlobalVar.STANDORD_SCREEN_HEIGHT);
	}	
}

