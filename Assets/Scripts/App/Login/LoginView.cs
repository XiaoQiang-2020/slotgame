using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace App.Login
{
    public class LoginView : MVC.BaseView
    {
        [SerializeField]
        private InputField _playerNameInput;

        [SerializeField]
        private Button _loginButton;

        [SerializeField]
        private Text _messageText;

        private void Awake()
        {
            InitReferences();
        }

        private void InitReferences()
        {
            if (_playerNameInput == null)
                _playerNameInput = GetComponentInChildren<InputField>(true);

            if (_loginButton == null)
                _loginButton = GetComponentInChildren<Button>(true);

            if (_messageText == null)
                _messageText = GetComponentsInChildren<Text>(true)
                    .FirstOrDefault(t => t.gameObject.name == "MessageText");

            if (_messageText == null)
                _messageText = GetComponentsInChildren<Text>(true)
                    .FirstOrDefault(t => t.gameObject.name != "Text");
        }

        public string PlayerName => _playerNameInput != null ? _playerNameInput.text : string.Empty;

        public Button LoginButton => _loginButton;

        public void SetMessage(string message)
        {
            if (_messageText != null)
                _messageText.text = message;
        }

        public void SetLoginEnabled(bool enabled)
        {
            if (_loginButton != null)
                _loginButton.interactable = enabled;
        }
    }
}
