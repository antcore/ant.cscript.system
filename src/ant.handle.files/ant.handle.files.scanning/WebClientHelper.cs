using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ant.handle.files.scanning
{
    class WebClientHelper
    {

        /// <summary>
        /// WebClient上传文件至服务器
        /// site http://www.jbxue.com
        /// </summary>
        /// <param name="localFilePath">文件名，全路径格式</param>
        /// <param name="serverFolder">服务器文件夹路径</param>
        /// <param name="reName">是否需要修改文件名，这里默认是日期格式</param>
        /// <returns></returns>
        public static bool UploadFile(string localFilePath, string serverFolder, bool reName)
        {
            string fileNameExt, newFileName, uriString;
            if (reName)
            {
                fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf(".") + 1);
                newFileName = DateTime.Now.ToString("yyMMddhhmmss") + fileNameExt;
            }
            else
            {
                newFileName = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);
            }
            if (!serverFolder.EndsWith("/") && !serverFolder.EndsWith("\\"))
            {
                serverFolder = serverFolder + "/";
            }
            uriString = serverFolder + newFileName;   //服务器保存路径
                                                      /// 创建WebClient实例
            WebClient myWebClient = new WebClient();
            myWebClient.Credentials = CredentialCache.DefaultCredentials;
            // 要上传的文件
            FileStream fs = new FileStream(newFileName, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);
            try
            {
                //使用UploadFile方法可以用下面的格式
                //myWebClient.UploadFile(uriString,"PUT",localFilePath);
                byte[] postArray = r.ReadBytes((int)fs.Length);
                Stream postStream = myWebClient.OpenWrite(uriString, "POST");
                if (postStream.CanWrite)
                {
                    postStream.Write(postArray, 0, postArray.Length);
                }
                else
                {
                    //MessageBox.Show("文件目前不可写！");
                }
                postStream.Close();
            }
            catch
            {
                //MessageBox.Show("文件上传失败，请稍候重试~");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 下载服务器文件至客户端
        /// </summary>
        /// <param name="uri">被下载的文件地址</param>
        /// <param name="savePath">另存放的目录</param>
        public static bool Download(string uri, string savePath)
        {
            string fileName;  //被下载的文件名
            if (uri.IndexOf("\\") > -1)
            {
                fileName = uri.Substring(uri.LastIndexOf("\\") + 1);
            }
            else
            {
                fileName = uri.Substring(uri.LastIndexOf("/") + 1);
            }

            if (!savePath.EndsWith("/") && !savePath.EndsWith("\\"))
            {
                savePath = savePath + "/";
            }
            savePath += fileName;   //另存为的绝对路径＋文件名
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(uri, savePath);
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
