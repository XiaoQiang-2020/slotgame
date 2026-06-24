using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Networking;
namespace Log
{

    public class PutLogToSvr
    {
        /// <summary>
        /// 上传的url
        /// </summary>
        //public const string PUT_LOG_URL = "http://test-resource.stevengame.com:85/action.php";
        // public const string PUT_LOG_URL = "http://192.168.7.26:18600/upload/logUpload";
        //public const string PUT_LOG_URL = "http://test.stv.com:8092/upload/logUpload";
        public const string PUT_LOG_URL = "http://client-log-upload.stevengame.com:80/upload/logUpload";
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        static byte[] Compress(string fileName)
        {
            //压缩后的MemoryStream
            MemoryStream ms = new MemoryStream();

            // 写入压缩
            using (System.IO.Compression.GZipStream compressedStream = new System.IO.Compression.GZipStream(ms, CompressionMode.Compress, true))
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                byte[] buf = new byte[1024];
                int count = 0;
                do
                {
                    count = fs.Read(buf, 0, buf.Length);
                    compressedStream.Write(buf, 0, count);
                }
                while (count > 0);

                fs.Close();
                compressedStream.Close();
            }
            return ms.ToArray();
        }
        /// <summary>
        /// 携程上传文件
        /// </summary>
        /// <param name="filePath">上传的文件路径</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static IEnumerator PutFileLog(string filePath,string userId,string url)
        {
            //string fileZip = filePath
            //FileToZip(filePath,)
            string zipName = "Log.zip";

            FileInfo info = new FileInfo(filePath);
            string zipFile = info.DirectoryName.Replace("Log", zipName);
            string error = "";
            if (!FileToZip(info.DirectoryName, zipFile, out error))
            {
                yield break;
            }

            string fileName = "";

            //byte[] bs = Compress(filePath);
            byte[] bs = File.ReadAllBytes(zipFile);

            //fileName = "Res-3D_3425-10000-" + userId+".zip";
            fileName = userId + ".zip";
            Debug.Log(fileName);
            WWWForm form = new WWWForm();

            //form.AddField("act", "uploadImg");
            //form.AddField("path", fileName);
            //form.AddBinaryData("source", bs, fileName);

            form.AddField("product_id", "hall");
            form.AddBinaryData("logfile", bs, fileName);

            UnityWebRequest www = UnityWebRequest.Post(url, form);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                LogDebug.Print("上传成功");
            }
            else
            {
                LogDebug.PrintError("上传失败" + www.error);
            }
        }
        public static bool FileToZip(string fileName, string zipName, out string error)
        {
            error = string.Empty;
            try
            {
                ZipOutputStream s = new ZipOutputStream(File.Create(zipName));
                s.SetLevel(6); // 0 - store only to 9 - means best compression
                zip(fileName, s);
                s.Finish();
                s.Close();
                return true;
            }
            catch (Exception ex)
            {

                error = ex.Message;
                Debug.LogError(error);
                return false;
            }
        }

        public static void zip(string fileName, ZipOutputStream s)
        {
            if (fileName[fileName.Length - 1] != Path.DirectorySeparatorChar)
                fileName += Path.DirectorySeparatorChar;
            Crc32 crc = new Crc32();
            string[] filenames = Directory.GetFileSystemEntries(fileName);

            foreach (string file in filenames)
            {
                if (Directory.Exists(file))
                {
                    zip(file, s);
                }
                else
                {
                    FileStream fs = File.OpenRead(file);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string tempfile = Path.GetFileName(file);
                    ZipEntry entry = new ZipEntry(tempfile);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);

                }
            }
        }

    }
}

