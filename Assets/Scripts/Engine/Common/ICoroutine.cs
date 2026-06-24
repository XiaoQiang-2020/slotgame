/***************************************************
 * 文件名：ICoroutine.cs
 * 描  述：协程调度预定义接口类。
 * 时  间：2017-04-20
 * 作  者：李海波
 * 修  改：
 ***************************************************/
namespace Core
{
    using SystemIEnumerator = System.Collections.IEnumerator;

    using UnityCoroutine = UnityEngine.Coroutine;

    /// <summary>
    /// 协程调用预定义接口，用于子类实现协程调度逻辑。
    /// </summary>
    public interface ICoroutine
    {
        /// <summary>
        /// 检测当前协程实例是否处于可用状态。
        /// </summary>
        /// <returns>若当前协程实例可用则返回true，否则返回false</returns>
        bool IsCoroutineEnable();

        /// <summary>
        /// 通过指定方法引用在当前运行行为对象上开始一个协程处理。
        /// </summary>
        /// <param name="routine">方法引用</param>
        /// <returns>若启动协程处理成功则返回对应的协程引用，否则返回null</returns>
        UnityCoroutine StartCoroutine(SystemIEnumerator routine);

        /// <summary>
        /// 通过指定方法名在当前运行行为对象上开始一个协程处理。
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <returns>若启动协程处理成功则返回对应的协程引用，否则返回null</returns>
        UnityCoroutine StartCoroutine(string methodName);

        /// <summary>
        /// 通过指定方法名及附属参数在当前运行行为对象上开始一个协程处理。
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="value">附属参数</param>
        /// <returns>若启动协程处理成功则返回对应的协程引用，否则返回null</returns>
        UnityCoroutine StartCoroutine(string methodName, object value);

        /// <summary>
        /// 停止在当前运行行为对象上的全部协程调度函数。
        /// </summary>
        void StopAllCoroutines();

        /// <summary>
        /// 通过指定协程引用停止在当前运行行为对象上的对应全部协程。
        /// </summary>
        /// <param name="routine">协程引用</param>
        void StopCoroutine(UnityCoroutine routine);

        /// <summary>
        /// 通过指定方法引用停止在当前运行行为对象上的对应全部协程。
        /// </summary>
        /// <param name="routine">引用目标</param>
        void StopCoroutine(SystemIEnumerator routine);

        /// <summary>
        /// 通过指定方法名停止在当前运行行为对象上的对应全部协程。
        /// </summary>
        /// <param name="methodName">方法名称</param>
        void StopCoroutine(string methodName);
    }
}
