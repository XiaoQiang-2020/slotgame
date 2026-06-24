using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitUIController : Game.MonoSingleton<InitUIController>
{

    public InitUILaunch uiLaunch;
	public InitUIMessageBox uiMessagebox;

	public void DestoryIt()
	{
		GameObject.Destroy (this.gameObject);
	}
}