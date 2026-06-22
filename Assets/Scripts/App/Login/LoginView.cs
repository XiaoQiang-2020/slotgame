using System.Linq;
using App.UI;
using UnityEngine;
using UnityEngine.UI;

namespace App.Login
{
    public class LoginView : UIWindow
    {
        public const string LoginWindowId = "Login";

        [SerializeField]
        private InputField _playerNameInput;

        [SerializeField]
        private Button _loginButton;

        [SerializeField]
        private Text _messageText;

        public override string WindowId => LoginWindowId;
        public override UILayer Layer => UILayer.Screen;

        protected override void Awake()
        {
            InitReferences();
            base.Awake();
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

        public string PlayerName
        {
            get
            {
                InitReferences();
                return _playerNameInput != null ? _playerNameInput.text : string.Empty;
            }
        }

        public Button LoginButton
        {
            get
            {
                InitReferences();
                return _loginButton;
            }
        }

        public void SetMessage(string message)
        {
            InitReferences();

            if (_messageText != null)
                _messageText.text = message;
        }

        public void SetLoginEnabled(bool enabled)
        {
            InitReferences();

            if (_loginButton != null)
                _loginButton.interactable = enabled;
        }
    }
}
