/***************************************************
 * 文件名： Gradient.cs
 * 描  述：
 * 时  间： 2017-05-25 17:45:03
 * 作  者： 李智海
 * 修  改： UI 2种颜色渐变特效组件
 ***************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect
{
    [SerializeField]
    private Color32
        topColor = Color.white;
    [SerializeField]
    private Color32
        bottomColor = Color.black;



    public override void ModifyMesh(VertexHelper vh)
    {
        UIVertex vert = new UIVertex();
        int count = vh.currentVertCount;
        //Debug.Log("vertexList.Cout = " + count);
        if (count > 0)
        {
            List<UIVertex> vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);
            float bottomY = vertexList[0].position.y;
            float topY = vertexList[0].position.y;

            for (int i = 0; i < count; i++)
            {
                float y = vertexList[i].position.y;
                if (y > topY)
                {
                    topY = y;
                }
                else if (y < bottomY)
                {
                    bottomY = y;
                }
            }
            float uiElementHeight = topY - bottomY;
            for (int i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                vert.color = Color32.Lerp(bottomColor, topColor, (vert.position.y - bottomY) / uiElementHeight);
                vh.SetUIVertex(vert, i);
            }
        }
    }

}
