using UnityEngine;
using UnityEngine.UI;
using App.Scene;

namespace App.Login
{
    public class LoginController : MVC.BaseController
    {
        [SerializeField]
        private LoginView _loginView;

        private LoginModel _model;

        protected override void OnInit()
        {
            base.OnInit();
            _model = new LoginModel();

            if (_loginView == null)
                _loginView = GetComponentInChildren<LoginView>(true);

            if (_loginView != null)
            {
                _loginView.Open();
                _loginView.SetMessage("Enter your player name and press Login.");
                _loginView.SetLoginEnabled(true);
                BindViewEvents();
            }
        }

        protected override void OnDispose()
        {
            UnbindViewEvents();
            base.OnDispose();
        }

        private void BindViewEvents()
        {
            if (_loginView?.LoginButton != null)
            {
                _loginView.LoginButton.onClick.AddListener(OnLoginButtonClicked);
            }
        }

        private void UnbindViewEvents()
        {
            if (_loginView?.LoginButton != null)
            {
                _loginView.LoginButton.onClick.RemoveListener(OnLoginButtonClicked);
            }
        }

        public void OnLoginButtonClicked()
        {
            if (_loginView == null)
                return;

            string playerName = _loginView.PlayerName;
            if (string.IsNullOrWhiteSpace(playerName))
            {
                _loginView.SetMessage("Please enter a valid player name.");
                return;
            }

            _model.PlayerName = playerName;
            _model.IsLoggedIn = true;
            _loginView.SetMessage($"Welcome, {playerName}!");
            _loginView.SetLoginEnabled(false);

            // SceneNavigator.LoadGameScene();
            // GameManager.Instance.SetPlayerName(playerName);
            // SceneNavigator.LoadGameScene();
             SceneChangeToHomeEvent.SendEventMessage();
        }
    }
}
