using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace App.Game
{
    public class GameView : MVC.BaseView
    {
        [SerializeField]
        private Text _statusText;

        private void Awake()
        {
            if (_statusText == null)
                _statusText = GetComponentsInChildren<Text>(true)
                    .FirstOrDefault(t => t.gameObject.name == "StatusText");

            if (_statusText == null)
                _statusText = GetComponentInChildren<Text>(true);
        }

        public void SetStatus(string text)
        {
            if (_statusText != null)
                _statusText.text = text;
        }
    }
}
