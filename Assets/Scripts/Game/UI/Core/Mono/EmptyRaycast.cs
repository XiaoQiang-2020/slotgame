/***************************************************
 * 文件名：EmptyRaycast.cs
 * 描  述：UI中经常用一张空的Image组件来接收点击相应，
           当前这个组件能实现这个功能，并且不产生overdraw。
 * 时  间：2017-04-14
 * 作  者：李海波
 * 修  改：
 ***************************************************/

using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    public class EmptyRaycast : MaskableGraphic
    {
        protected EmptyRaycast()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}