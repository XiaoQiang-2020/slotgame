using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ResourcesExtract
{
    public static Queue<Action> actionqueue = new Queue<Action>();

    /// <summary>
    /// 资源列表内容。
    /// </summary>
    private static string m_resListStr = string.Empty;

    /// <summary>
    /// 将 StreamingAssets 中的资源拷贝到 persistentDataPath 下的可读写目录。
    /// 这个流程用于旧 AssetBundle 资源初始化，新项目主流程不再依赖它。
    /// </summary>
    public static IEnumerator Extract()
    {
        if (Game.GlobalVar.IS_RES_MODE_DEBUG)
        {
            yield break;
        }

        // 资源输入路径。
        string inPath = Application.streamingAssetsPath + "/" + Game.GlobalVar.RES_ROOT;
        // 资源输出路径。
        string outPath = Application.persistentDataPath + "/" + Game.GlobalVar.RES_ROOT;
        // 输入资源列表文件。
        string infile = inPath + "/" + Game.GlobalVar.RES_LIST_FILE;
        // 输出资源列表文件。
        string outfile = outPath + "/" + Game.GlobalVar.RES_LIST_FILE;

        // 如果资源已经初始化完成，则不再重复拷贝。
        if (File.Exists(outfile) && Game.GlobalVar.IS_NEED_RES_ECTRACT == false)
        {
            Debuger.Log("资源已经初始化完成，无需重复拷贝。" + outfile);
            // InitUIController.Instance.uiLaunch.SetProgessActive(false);
        }
        else
        {
            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
            }
            Directory.CreateDirectory(outPath);

            // 1. 先读取资源清单文件。
            if (Application.platform == RuntimePlatform.Android)
            {
                System.Uri u = new System.Uri(infile);
                infile = u.AbsoluteUri;
                UnityWebRequest www = UnityWebRequest.Get(infile);
                yield return www.SendWebRequest();

                if (string.IsNullOrEmpty(www.error))
                {
                    m_resListStr = www.downloadHandler.text;
                    // File.WriteAllBytes(outfile, www.bytes);
                }
                else
                {
                    Debuger.Log("StreamingAssetsPath 下资源清单文件不存在或读取失败。" + infile);
                    InitUIController.Instance.uiMessagebox.Show("错误！\n资源不存在或丢失，请下载游戏并安装！", () => Application.Quit());
                    yield break;
                }
            }
            else
            {
                if (!File.Exists(infile))
                {
                    Debuger.Log("StreamingAssetsPath 下资源清单文件不存在或读取失败。" + infile);
                    InitUIController.Instance.uiMessagebox.Show("错误！\n资源不存在或丢失，请下载游戏并安装！", () => Application.Quit());
                    yield break;
                }
                // File.Copy(infile, outfile, true);
                m_resListStr = Game.FileUtils.getString(infile);
            }
            yield return new WaitForEndOfFrame();

            // 2. 解析资源清单。
            if (string.IsNullOrEmpty(m_resListStr))
            {
                Core.Debuger.LogError("资源清单文件内容为空，无法初始化资源。");
                InitUIController.Instance.uiMessagebox.Show("错误！\n资源不存在或丢失，请下载游戏并安装！", () => Application.Quit());
                yield break;
            }

            ResListJson resJsonObj = JsonUtility.FromJson<ResListJson>(m_resListStr);

            if (resJsonObj == null || resJsonObj.games == null || resJsonObj.games.Count == 0)
            {
                Debuger.Log("资源清单解析失败。" + infile);
                InitUIController.Instance.uiMessagebox.Show("错误！\n资源初始化失败，请重试或重新下载游戏！", () => Application.Quit());
                yield break;
            }

            int resCount = 0;
            for (int i = 0; i < resJsonObj.games.Count; i++)
            {
                ResListGameItem game = resJsonObj.games[i];
                if (game.res_list == null || game.res_list.Count == 0) continue;

                for (int j = 0; j < game.res_list.Count; j++)
                {
                    ResItem item = game.res_list[j];
                    if (item == null || string.IsNullOrEmpty(item.path))
                    {
                        continue;
                    }
                    resCount++;
                }
            }

            InitUIController.Instance.uiLaunch.gameObject.SetActive(true);
            int resIndex = 0;
            for (int i = 0; i < resJsonObj.games.Count; i++)
            {
                ResListGameItem game = resJsonObj.games[i];
                if (game.res_list == null || game.res_list.Count == 0) continue;
                for (int j = 0; j < game.res_list.Count; j++)
                {
                    ResItem item = game.res_list[j];
                    if (item == null || string.IsNullOrEmpty(item.path))
                    {
                        continue;
                    }
                    string file = item.path.Trim();
                    string fileSavePath = Game.FileUtils.pathCombine(outPath, file);
                    string fileDir = Path.GetDirectoryName(fileSavePath);
                    string infileUrl = Game.FileUtils.pathCombine(inPath, file);
                    if (!Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }

                    // Debug.Log("正在拷贝资源文件：" + infileUrl);

                    if (File.Exists(fileSavePath))
                    {
                        File.Delete(fileSavePath);
                    }
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        infileUrl = new System.Uri(infileUrl).AbsoluteUri;
                        UnityWebRequest request = UnityWebRequest.Get(infileUrl);
                        yield return request.SendWebRequest();

                        if (request.result == UnityWebRequest.Result.Success)
                        {
                            File.WriteAllBytes(fileSavePath, request.downloadHandler.data);
                        }
                    }
                    else
                    {
                        File.Copy(infileUrl, fileSavePath, true);
                        yield return 0;
                    }

                    resIndex++;
                    InitUIController.Instance.uiLaunch.SetProgess(resIndex / (float)resCount);
                }
            }

            InitUIController.Instance.uiLaunch.SetProgess(1);

            // 保存资源清单文件。
            if (Application.platform == RuntimePlatform.Android)
            {
                File.WriteAllText(outfile, m_resListStr);
            }
            else
            {
                File.Copy(infile, outfile, true);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    static bool unCompressEnd;
    public static bool unCompressSuccess;
    private static int ZipCount = 0;
    private static int ZipComplete = 0;

    public static IEnumerator ExtractExt()
    {
        unCompressSuccess = true;
        // 资源输入路径。
        string inPath = Application.streamingAssetsPath + "/" + Game.GlobalVar.RES_ROOT;
        // 资源输出路径。
        string outPath = Application.persistentDataPath + "/" + Game.GlobalVar.RES_ROOT;
        // 输入资源列表文件。
        string infile = inPath + "/" + Game.GlobalVar.RES_LIST_FILE;
        // 输出资源列表文件。
        string outfile = outPath + "/" + Game.GlobalVar.RES_LIST_FILE;

        // 压缩包输入输出路径。
        string infilePackage = Application.streamingAssetsPath + "/" + "resource.zip";
        string outfilePackage = Application.persistentDataPath + "/" + "resource.zip";

        // 如果资源已经初始化完成，则不再重复解压。
        if (File.Exists(outfile) && Game.GlobalVar.IS_NEED_RES_ECTRACT == false && !File.Exists(outfilePackage))
        {
            Debuger.Log("资源已经初始化完成，无需重复解压。" + outfile);
            Game.AppGameManager.hasUncompress = false;
            // InitUIController.Instance.uiLaunch.SetProgessActive(false);
        }
        else
        {
            Game.AppGameManager.hasUncompress = true;
            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
            }
            Directory.CreateDirectory(outPath);

            // 1. 先拷贝资源压缩包。
            unCompressEnd = false;
            ZipComplete = 0;
            if (Application.platform == RuntimePlatform.Android)
            {
                infilePackage = new System.Uri(infilePackage).AbsoluteUri;
                UnityWebRequest request = UnityWebRequest.Get(infilePackage);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    byte[] data = request.downloadHandler.data;
                    File.WriteAllBytes(outfilePackage, data);
                }
            }
            else
            {
                File.Copy(infilePackage, outfilePackage, true);
                yield return 0;
            }

            ZipCount = ZipUnCompress.GetFilesCount(outfilePackage);
            if (!File.Exists(outfilePackage))
            {
                Debuger.Log(outfilePackage + " 不存在");
            }

            ZipUnCompress.SetEventListener(OnZipFileCb);
            ZipUnCompress.startUnZip(outfilePackage, Application.persistentDataPath + "/", null);
            while (!unCompressEnd)
            {
                while (actionqueue.Count > 0)
                {
                    lock (actionqueue)
                    {
                        Action action = actionqueue.Dequeue();
                        action();
                    }
                }
                InitUIController.Instance.uiLaunch.SetProgess((float)ZipComplete / (float)ZipCount * 0.5f, "正在解压文件，请稍等...");
                yield return null;
            }

            if (unCompressSuccess)
            {
                File.Delete(outfilePackage);
            }
            else
            {
                InitUIController.Instance.uiLaunch.SetProgess(1.0f, "解压失败，请重启游戏再试");
                File.Delete(outfilePackage);
                if (Directory.Exists(outPath))
                {
                    Directory.Delete(outPath, true);
                }
            }
        }
        yield return null;
    }

    static object OnZipFileCb(params object[] objs)
    {
        string cbType = (string)objs[0];
        switch (cbType)
        {
            case "error":
                lock (actionqueue)
                {
                    actionqueue.Enqueue(() =>
                    {
                        unCompressSuccess = false;
                        Debuger.Log("解压失败");
                        unCompressEnd = true;
                    });
                }
                break;
            case "finish":
                lock (actionqueue)
                {
                    actionqueue.Enqueue(() =>
                    {
                        Debuger.Log("解压完成");
                        unCompressEnd = true;
                    });
                }
                break;
            case "onefile":
                lock (actionqueue)
                {
                    actionqueue.Enqueue(() =>
                    {
                        ZipComplete++;
                    });
                }
                break;
            case "zipDic":
                break;
        }
        return null;
    }
}