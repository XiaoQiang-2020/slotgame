namespace App.Login
{
    public class LoginModel : MVC.BaseModel
    {
        public bool IsLoggedIn { get; set; }
        public string PlayerName { get; set; }

        public LoginModel()
        {
            IsLoggedIn = false;
            PlayerName = string.Empty;
        }
    }
}
