/***************************************************
 * 文件名：ZipUnCompress.cs
 * 描  述：
 * 时  间：2018-09-26 16:28:39
 * 作  者：武浩颀
 * 修  改：
 ***************************************************/

using System; 
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
public static class ZipUnCompress
{
    
	public delegate object UnCompressEventHandler(params object[] objs);
    public static int GetFilesCount(string zipfilepath)
    {             
        int totalFile = 0;
        ZipFile zip = new ZipFile(zipfilepath);
        foreach (ZipEntry entry in zip)
        {
            if (entry.IsFile)
            {
                totalFile++;
            }
        }                  
        return totalFile;
    }


   

    private static UnCompressEventHandler eventHandler;
    public static void SetEventListener(UnCompressEventHandler handler)
    {
        eventHandler=handler;
    }
 
    public static void startUnZip(string filePathName, string outputPathName, string password = null)
    {
        Thread resourcesLoadThread=new Thread (goThread);
        Dictionary<string,object>dic = new Dictionary<string, object>();
        dic.Add("file",filePathName);
        dic.Add("outputpath",outputPathName);
        dic.Add("password",password);
        resourcesLoadThread.Start(dic);
    }
    private  static void goThread(object obj)
    {
        Dictionary<string,object>dic = (Dictionary<string,object>)obj;
        string filePathName = (string)dic["file"];
        string outputPathName = (string)dic["outputpath"];
        string password = (string)dic["password"];
        UnzipFile(filePathName,outputPathName,password);
    }   
    /// <summary>
    /// 解压Zip包
    /// </summary>
    /// <param name="_filePathName">Zip包的文件路径名</param>
    /// <param name="_outputPath">解压输出路径</param>
    /// <param name="_password">解压密码</param>
    /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
    /// <returns></returns>
    public static bool UnzipFile(string _filePathName, string _outputPath, string _password = null)
    {
        if (string.IsNullOrEmpty(_filePathName) || string.IsNullOrEmpty(_outputPath))
        {
            return false;
        }

        try
        {
            return UnzipFile(File.OpenRead(_filePathName), _outputPath, _password);
        }
        catch (System.Exception _e)
        {
            Debug.LogError("[ZipUtility.UnzipFile]: " + _e.ToString());

            if (null != eventHandler)
                eventHandler("error");

            return false;
        }
    }



    /// <summary>
    /// 解压Zip包
    /// </summary>
    /// <param name="_inputStream">Zip包输入流</param>
    /// <param name="_outputPath">解压输出路径</param>
    /// <param name="_password">解压密码</param>
    /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
    /// <returns></returns>
    public static bool UnzipFile(Stream _inputStream, string _outputPath, string _password = null)
    {
        if ((null == _inputStream) || string.IsNullOrEmpty(_outputPath))
        {
            if (null != eventHandler)
                eventHandler("error");

            return false;
        }

        // 创建文件目录
        Game.FileUtils.fixedPath(ref _outputPath);
        if (!Directory.Exists(_outputPath))
            Directory.CreateDirectory(_outputPath);

        // 解压Zip包
        ZipEntry entry = null;
        using (ZipInputStream zipInputStream = new ZipInputStream(_inputStream))
        {
            if (!string.IsNullOrEmpty(_password))
                zipInputStream.Password = _password;

            while (null != (entry = zipInputStream.GetNextEntry()))
            {
                if (string.IsNullOrEmpty(entry.Name))
                    continue;


                string filePathName = Game.FileUtils.pathCombine(_outputPath, entry.Name);
                // 创建文件目录
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(filePathName);
                    continue;
                }

                // 写入文件
                try
                {
                    using (FileStream fileStream = File.Create(filePathName))
                    {
                        byte[] bytes = new byte[1024];
                        while (true)
                        {
                            int count = zipInputStream.Read(bytes, 0, bytes.Length);
                            if (count > 0)
                                fileStream.Write(bytes, 0, count);
                            else
                            {
								if (null != eventHandler)
                					eventHandler("onefile",entry);

               
                                break;
                            }
                        }
                    }
                }
                catch (System.Exception _e)
                {
                    Debug.LogError("[ZipUtility.UnzipFile]: " + _e.ToString());

                    if (null != eventHandler)
                		eventHandler("error");

                    return false;
                }
            }
        }

        if (null != eventHandler)
            eventHandler("finish");

        return true;
    }

}
