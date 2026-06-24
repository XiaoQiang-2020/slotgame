namespace Game
{
    using UnityObject = UnityEngine.GameObject;
    using UnityCanvasScaler = UnityEngine.UI.CanvasScaler;
    using UnityVector2 = UnityEngine.Vector2;
    using UnityScreen = UnityEngine.Screen;
    using UnityEngine;

    public class CanvasAdapt : UnityEngine.MonoBehaviour
    {
        public int ScreenHight = 1920;
        public int ScreenWidth = 1080;
        void Awake()
        {
            UnityObject.DontDestroyOnLoad(this.gameObject);

            //  获取或添加 UnityCanvasScaler
            UnityCanvasScaler scaler = transform.GetComponent<UnityCanvasScaler>();
            if (null == scaler)
            {
                scaler = gameObject.AddComponent<UnityCanvasScaler>();
            }
            scaler.uiScaleMode = UnityCanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new UnityVector2(Game.GlobalVar.STANDORD_SCREEN_WIDTH, Game.GlobalVar.STANDORD_SCREEN_HEIGHT);
            scaler.screenMatchMode = UnityCanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            // 根据宽高比的不同，设置按宽度缩放或者按高度缩放
            int width = UnityScreen.width;
            int height = UnityScreen.height;
            float s1 = (float)Game.GlobalVar.STANDORD_SCREEN_WIDTH / (float)Game.GlobalVar.STANDORD_SCREEN_HEIGHT;
            float s2 = (float)width / (float)height;


            // 美术制作的全屏UI大小
            int designWidth = Game.GlobalVar.STANDORD_SCREEN_WIDTH;
            int designHeight = Game.GlobalVar.STANDORD_SCREEN_HEIGHT;


            if (s2 > s1)
            {
                designHeight = Mathf.FloorToInt(designWidth / s2);
                scaler.matchWidthOrHeight = 0;
            }
            else
            {
                designWidth = Mathf.FloorToInt(designHeight * s2);
                scaler.matchWidthOrHeight = 1;
            }

            ScreenWidth = designWidth;
            ScreenHight = designHeight;

        }
    }

}