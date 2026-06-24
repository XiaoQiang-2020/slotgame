/***************************************************
 * 文件名：FlowLight(2).cs
 * 描  述：
 * 时  间：2019-01-07 11:51:56
 * 作  者：尼尔
 * 修  改：
 ***************************************************/

/***************************************************
 * 文件名：Flow.cs
 * 描  述：
 * 时  间：2019-01-05 12:06:55
 * 作  者：尼尔
 * 修  改：
 ***************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowLight : MonoBehaviour {
    private Image[] imageArray;
    private int curIndex = 0;
    public float loopClip = 1;
    private float widthRate = 1;
    private float heightRate = 1;
    private float xOffsetRate = 0;
    private float yOffsetRate = 0;
    public Color color = Color.yellow;
    public float power = 0.55f;
    public float speed = 1;
    public float largeWidth = 0.003f;
    public float littleWidth = 0.0003f;
    public float length = 0.1f;
    public float skewRadio = 0.4f;//倾斜
    public float moveTime = 0;
    // float endMoveTime = 0;
   
    // Use this for initialization
    void Start () {
       
        imageArray = GetComponentsInChildren<Image>(false);
        InitMat();

        StopCoroutine("SlowLight");
        StartCoroutine("SlowLight");
        StartCoroutine( StartFlow());

    }
    IEnumerator StartFlow()
    {
       
        yield return new WaitForSeconds(1.0f / speed);
        yield return flowOne();
    }
    private IEnumerator flowOne()
    {
        // imageMat = new Material(Shader.Find("Custom/UI/Flowlight"));
        moveTime = 0;
        curIndex++;
        if (curIndex == imageArray.Length)
        {
            yield return new WaitForSeconds(loopClip);
            curIndex = 0;
            yield return StartFlow();
        }
        else
        {
            yield return StartFlow();
        }
    }
        
    public void InitMat()
    {

        for (int i = 0; i < imageArray.Length; i++)
        {

            Image imagei = imageArray[i];
            widthRate = imagei.sprite.textureRect.width * 1.0f / imagei.sprite.texture.width;
            heightRate = imagei.sprite.textureRect.height * 1.0f / imagei.sprite.texture.height;
            xOffsetRate = (imagei.sprite.textureRect.xMin) * 1.0f / imagei.sprite.texture.width;
            yOffsetRate = (imagei.sprite.textureRect.yMin) * 1.0f / imagei.sprite.texture.height;
            Material mat = new Material(Shader.Find("Custom/UI/Flowlight"));
            skewRadio = Mathf.Clamp(skewRadio, 0, 1);
            length = Mathf.Clamp(length, 0, 0.5f);
            mat.SetColor("_FlowlightColor", color);
            mat.SetFloat("_Power", power);
            mat.SetFloat("_MoveSpeed", speed);
            mat.SetFloat("_LargeWidth", largeWidth);
            mat.SetFloat("_LittleWidth", littleWidth);
            mat.SetFloat("_SkewRadio", skewRadio);
            mat.SetFloat("_Lengthlitandlar", length);
            mat.SetFloat("_MoveTime", 0);

            mat.SetFloat("_WidthRate", widthRate);
            mat.SetFloat("_HeightRate", heightRate);
            mat.SetFloat("_XOffset", xOffsetRate);
            mat.SetFloat("_YOffset", yOffsetRate);
            imageArray[i].material = mat;
        }

    }
    private void SetShader1(Material mat)
    {
        skewRadio = Mathf.Clamp(skewRadio, 0, 1);
        length = Mathf.Clamp(length, 0, 0.5f);
        mat.SetColor("_FlowlightColor", color);
        mat.SetFloat("_Power", power);
        mat.SetFloat("_MoveSpeed", speed);
        mat.SetFloat("_LargeWidth", largeWidth);
        mat.SetFloat("_LittleWidth", littleWidth);
        mat.SetFloat("_SkewRadio", skewRadio);
        mat.SetFloat("_Lengthlitandlar", length);
        mat.SetFloat("_MoveTime", 0);


    }


    private void SetShader(Material mat)
    {
        skewRadio = Mathf.Clamp(skewRadio, 0, 1);
        length = Mathf.Clamp(length, 0, 0.5f);
        mat.SetColor("_FlowlightColor", color);
        mat.SetFloat("_Power", power);
        mat.SetFloat("_MoveSpeed", speed);
        mat.SetFloat("_LargeWidth", largeWidth);
        mat.SetFloat("_LittleWidth", littleWidth);
        mat.SetFloat("_SkewRadio", skewRadio);
        mat.SetFloat("_Lengthlitandlar", length);
        mat.SetFloat("_MoveTime", moveTime);


    }
    IEnumerator SlowLight()
    {
        /* yield return new WaitForSeconds(1.0f / speed*index/2.0f);*/
        moveTime = 0;
        while (true)
        {
            
            for (int i = 0; i < imageArray.Length; i++)
            {

                if (i == curIndex)
                {
                    SetShader(imageArray[i].material);
                }
                else
                {
                    SetShader1(imageArray[i].material);
                }

            }
            moveTime += Time.deltaTime;
            // Debug.Log(moveTime + ":" + endMoveTime);
            yield return null;
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
        CancelInvoke();
        for (int i = 0; i < imageArray.Length; i++)
        {
            Image image = imageArray[i];
            image.material = null;
        }
    }
}


