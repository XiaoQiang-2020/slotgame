namespace App.Game
{
    public class GameModel : MVC.BaseModel
    {
        public bool IsInitialized { get; set; }
        public string PlayerName { get; set; }

        public GameModel()
        {
            IsInitialized = false;
            PlayerName = string.Empty;
        }
    }
}
