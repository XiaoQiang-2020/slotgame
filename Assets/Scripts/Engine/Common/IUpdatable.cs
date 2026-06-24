/***************************************************
 * 文件名：IUpdatable.cs
 * 描  述：对象刷新控制器接口类。
 * 时  间：2017-04-13
 * 作  者：李海波
 * 修  改：
 ***************************************************/

namespace Core
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// 对象刷新控制器接口类，负责控制场景对象的刷新操作。
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// 对象刷新通知主时序入口。
        /// </summary>
        /// <param name="delta">片段时间帧</param>
        [ComVisible(true)]
        void Update(float delta);

        /// <summary>
        /// 对象刷新后续通知入口。
        /// </summary>
        [ComVisible(true)]
        void LateUpdate();
    }
}
