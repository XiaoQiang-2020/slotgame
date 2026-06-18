using UnityEngine.SceneManagement;

namespace App.Scene
{
    public static class SceneNavigator
    {
        public static void LoadLoginScene()
        {
            SceneManager.LoadScene(SceneNames.Login);
        }

        public static void LoadGameScene()
        {
            SceneManager.LoadScene(SceneNames.Game);
        }
    }
}
