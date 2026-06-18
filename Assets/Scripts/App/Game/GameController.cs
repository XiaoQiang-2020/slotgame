using UnityEngine;

namespace App.Game
{
    public class GameController : MVC.BaseController
    {
        [SerializeField]
        private GameView _gameView;

        private GameModel _model;

        protected override void OnInit()
        {
            base.OnInit();
            _model = new GameModel();

            if (_gameView == null)
                _gameView = GetComponentInChildren<GameView>(true);

            _model.IsInitialized = true;
            _model.PlayerName = "Player";

            RefreshView();
        }

        private void RefreshView()
        {
            if (_gameView == null)
                return;

            string status = _model.IsInitialized
                ? $"Game started. Welcome {_model.PlayerName}."
                : "Initializing game...";

            _gameView.SetStatus(status);
        }

        public void SetPlayerName(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
                return;

            _model.PlayerName = playerName;
            RefreshView();
        }
    }
}
