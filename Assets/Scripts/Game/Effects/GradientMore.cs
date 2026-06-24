
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/GradientMore")]
public class GradientMore : BaseMeshEffect
{
    [SerializeField]
    private List<Color32> colors;

    protected override void Start()
    {
        base.Start();
        if (colors == null)
        {
            colors = new List<Color32>();
        }


    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (colors==null && colors.Count < 2) return;
        int colorNum = colors.Count;


        UIVertex vert = new UIVertex();
        int count = vh.currentVertCount;
        Debug.Log("vertexList.Cout = " + count);
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
            float uiElementHeight_fragment = uiElementHeight / (float)(colorNum-1);
            Debug.Log(string.Format("topY = {0}, bottomY = {1}, uiElementHeight = {2}, uiElementHeight_fragment = {3}",
                    topY, bottomY, uiElementHeight, uiElementHeight_fragment));
            float tempPosY = 0f;
            float tempTop = 0f;
            // float tempButtom = 0f;
            float tempRate = 0f;    // 一个点到最高点的距离 与 整个
            int tempIndex = 0;      // colors的索引
            float tempOffset = 0f;
            for (int i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                ////vert.color = Color32.Lerp(bottomColor, topColor, (vert.position.y - bottomY) / uiElementHeight);
                //offsetFromTop = topY - vert.position.y;
                //rate = offsetFromTop / uiElementHeight;
                //index = (int)(rate * (colorNum-1));
                //if (index >= colorNum-1) index = colorNum - 2;
                //tempTop = index / (float)colorNum * uiElementHeight;
                //tempButtom = (index+1) / (float)colorNum * uiElementHeight;
                //vert.color = Color32.Lerp(colors[index], colors[index+1], (offsetFromTop - tempTop) / uiElementHeight_fragment);

                Debug.Log(string.Format("vert[{0}] = {1}", i, vert.position));
                tempPosY = vert.position.y;
                tempRate = (topY - tempPosY) / uiElementHeight;
                if (tempRate >= 1f) tempRate -= 0.01f;
                tempIndex = (int)(tempRate * (colorNum - 1));
                //Debug.Log(string.Format("tempRate = {0}, tempIndex = {1}", tempRate, tempIndex));
                tempTop = topY - (tempIndex / (float)(colorNum-1) * uiElementHeight);
                tempOffset = tempTop - tempPosY;
                vert.color = Color32.Lerp(colors[tempIndex], colors[tempIndex + 1], tempOffset / uiElementHeight_fragment);
                Debug.Log(string.Format("i = {0}, tempPosY = {1}, tempRate = {2}, tempIndex = {3}, tempTop = {4}, tempOffset={5}",
                    i, tempPosY, tempRate, tempIndex, tempTop, tempOffset));

                vh.SetUIVertex(vert, i);


            }
        }
    }

}
