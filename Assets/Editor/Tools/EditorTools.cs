/***************************************************
 * 文件名： EditorTools.cs
 * 描  述：
 * 时  间： 2017-04-21 16:37:03
 * 作  者： 李智海
 * 修  改：
 ***************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EditorTools
{
    [MenuItem("Tools/输出所有的音效文件（慎用）")]
    public static void GetAllAudio()
    {
        string path = Application.dataPath + "/../../AllAudio";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var allAssets = AssetDatabase.GetAllAssetPaths();
        int i = 0;
        int allCount = allAssets.Length;
        string endPath;
        string str = "Assets/";
        foreach (var assetPath in allAssets)
        {
            i++;
            if (assetPath.EndsWith(".mp3") || assetPath.EndsWith(".wma") || assetPath.EndsWith(".aiff") || assetPath.EndsWith(".ogg") || assetPath.EndsWith(".wav"))
            {
                endPath = path + "/" + assetPath;

                if (!Directory.Exists(Path.GetDirectoryName(endPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(endPath));
                }

                File.WriteAllBytes(endPath, File.ReadAllBytes(Application.dataPath + "/" + assetPath.Substring(str.Length, assetPath.Length - str.Length)));


                UpdateProgress(i, allCount, "loading...");
            }
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("提示", "完成", "确定");
    }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text) { return DrawHeader(text, text, false, false); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false, false); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!minimalistic) GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic)
        {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else
        {
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
        }

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        if (!minimalistic) GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }


    /// <summary>
    /// 读取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    static public T loadObjectFromJsonFile<T>(string path) where T : new()
    {
        if (!File.Exists(path))
            return new T();
        string str = File.ReadAllText(path);
        if (string.IsNullOrEmpty(str))
        {
            Debug.Log("Cannot find " + path);
            return new T();
        }
        T data = JsonUtility.FromJson<T>(str);
        if (data == null)
        {
            Debug.Log("Cannot read data from " + path);
        }

        return data;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="path"></param>
    static public void saveObjectToJsonFile<T>(T data, string path)
    {
        string jsonStr = JsonUtility.ToJson(data, true);
        writeFileWithCode(path, jsonStr, Encoding.UTF8);
    }

    static public bool writeFileWithCode(string filepath, string data, Encoding code)
    {
        try
        {
            string path = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (code != null)
            {
                File.WriteAllText(filepath, data, code);
            }
            else
            {
                File.WriteAllText(filepath, data);
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("writeFIle fail. " + filepath);
            throw e;
        }
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }
}