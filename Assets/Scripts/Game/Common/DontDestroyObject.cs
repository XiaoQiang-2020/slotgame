/***************************************************
 * 文件名：DontDestroyObject.cs
 * 描  述：设置对象不跳场景不释放。
 * 时  间：2017-04-13
 * 作  者：李海波
 * 修  改：
 ***************************************************/

namespace Game
{
    using UnityObject = UnityEngine.Object;
    using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

    public class DontDestroyObject : UnityMonoBehaviour
    {
        void Awake()
        {
            UnityObject.DontDestroyOnLoad(this.gameObject);
        }
    }
}

