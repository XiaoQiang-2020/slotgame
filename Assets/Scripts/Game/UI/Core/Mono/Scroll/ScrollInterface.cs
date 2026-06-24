/***************************************************
 * 文件名：ScrollInterface.cs
 * 描  述：滚动UI接口类。
 * 时  间：2017-04-14
 * 作  者：李海波
 * 修  改：
 ***************************************************/

namespace Game
{
    public interface ScrollInterface
    {

        /// <summary>
        /// 初始化滚动内容容器
        /// </summary>
        void InitScrollContainer();

        /// <summary>
        /// 在滚动窗中添加子节点
        /// </summary>
        /// <param name="oneUnit">要添加的子节点</param>
        /// <param name="needRefresh">是否需要立即刷新容器</param>
        /// <returns>是否添加成功</returns>
        bool AddChild(UnityEngine.GameObject oneUnit, bool needRefresh);

        /// <summary>
        /// 设置子节点的显示和隐藏
        /// </summary>
        /// <param name="nodeChild">列表子节点</param>
        /// <param name="bShow">是否显示</param>
        /// <returns></returns>
        bool SetChildVisible(UnityEngine.Transform nodeChild, bool bShow);

        /// <summary>
        /// 设置子节点的显示和隐藏
        /// </summary>
        /// <param name="index">子节点排列序号</param>
        /// <param name="bShow">是否显示</param>
        /// <returns></returns>
        bool SetChildVisible(int index, bool bShow);

        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="nodeChild">子节点</param>
        /// <param name="needRefresh">是否需要刷新容器</param>
        /// <returns></returns>
        bool RemoveChild(UnityEngine.Transform nodeChild, bool needRefresh);

        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="index">排列序号</param>
        /// <param name="needRefresh">是否需要刷新容器</param>
        /// <returns></returns>
        bool RemoveChild(int index, bool needRefresh);

        /// <summary>
        /// 清空滚动窗
        /// </summary>
        /// <returns></returns>
        bool RemoveAllChild();

        /// <summary>
        /// 容器大小自适应修改
        /// </summary>
        void RefreshContainerSize();
    }
}
