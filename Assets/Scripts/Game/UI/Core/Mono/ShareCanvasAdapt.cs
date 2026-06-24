/***************************************************
 * 文件名：CanvasAdapt.cs
 * 描  述：ugui canvas屏幕适配。
 * 时  间：2017-04-13
 * 作  者：李海波
 * 修  改：
 ***************************************************/


namespace Game
{
    using UnityObject = UnityEngine.GameObject;
    using UnityCanvasScaler = UnityEngine.UI.CanvasScaler;
    using UnityVector2 = UnityEngine.Vector2;
    using UnityScreen = UnityEngine.Screen;
    using UnityEngine;

    public class ShareCanvasAdapt : UnityEngine.MonoBehaviour
    {
        public int ScreenHight = 720;
        public int ScreenWidth = 1280;

        public bool isVerticalScreen = false;
        
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
            Vector2 fullSize= Vector2.zero;
            if (isVerticalScreen)
            {
                fullSize.x = Game.GlobalVar.STANDORD_SCREEN_WIDTH;
                fullSize.y = Game.GlobalVar.STANDORD_SCREEN_HEIGHT;
            }
            else
            {
                fullSize.x = Game.GlobalVar.STANDORD_SCREEN_HEIGHT;
                fullSize.y = Game.GlobalVar.STANDORD_SCREEN_WIDTH;
            }
            scaler.referenceResolution = fullSize;
            scaler.screenMatchMode = UnityCanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            // 根据宽高比的不同，设置按宽度缩放或者按高度缩放
            int width = UnityScreen.width;
            int height = UnityScreen.height;
            float s1 = (float)fullSize.x / (float)fullSize.y;
            float s2 = (float)width / (float)height;


            // 美术制作的全屏UI大小
            int designWidth = (int)fullSize.x;
            int designHeight = (int )fullSize.y;


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