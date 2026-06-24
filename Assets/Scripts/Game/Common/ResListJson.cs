/***************************************************
 * 文件名： ResListJson.cs
 * 描  述：
 * 时  间： 2017-04-25 14:52:29
 * 作  者： 李智海
 * 修  改：
 ***************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源配置清单的json结构
/// </summary>
[System.Serializable]
public class ResListJson
{
    public string version = string.Empty;
    public string modify_time = string.Empty;
    public List<ResListGameItem> games = new List<ResListGameItem>();
}

[System.Serializable]
public class ResListGameItem
{
    public string name = string.Empty;
    public List<ResItem> res_list = new List<ResItem>();

}

[System.Serializable]
public class ResItem
{
    public string path = string.Empty;    // 相对根目录
    public string md5 = string.Empty;
    public long size = 0;
}

