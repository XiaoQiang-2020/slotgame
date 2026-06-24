using UnityEngine;
using UnityEngine.UI;

public class InitUIMessageBox : MonoBehaviour {
	
	[SerializeField]Text content;
	[SerializeField]Button button;
	System.Action callback = null;
	// Use this for initialization
	void Start () {
		button.onClick.AddListener (onclickbutton);
	}


	void onclickbutton () {
		if (callback != null) {
			callback ();
		}
		gameObject.SetActive (false);
	}

	public void Show(string content, System.Action action=null)
	{
		gameObject.SetActive (true);
		this.content.text = content;
		callback = action;
	}
}

