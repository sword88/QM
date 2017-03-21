using System;
using System.Web;
using System.IO;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using System.Security.Cryptography;
using QM.Core.Exception;

namespace QM.Core.Common
{
    #region 自定义类型
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// 目录
        /// </summary>
        Directory,

        /// <summary>
        /// 文件
        /// </summary>
        File,

        /// <summary>
        /// 图片
        /// </summary>
        Image,

        /// <summary>
        /// 视频
        /// </summary>
        Video,

        /// <summary>
        /// 音频
        /// </summary>
        Sound,

        /// <summary>
        /// 其他
        /// </summary>
        None
    }

    /// <summary>
    /// 压缩文件
    /// </summary>
    public enum CompressionType
    {
        /// <summary>
        /// GZip压缩格式
        /// </summary>
        GZip,

        /// <summary>
        /// BZip2压缩格式
        /// </summary>
        BZip2,

        /// <summary>
        /// Zip压缩格式
        /// </summary>
        Zip
    }

    #endregion

    public class QMFile
    {
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>创建是否成功</returns>
        [DllImport("dbgHelp", SetLastError = true)]
        private static extern bool MakeSureDirectoryPathExists(string name);

        /// <summary>
        /// 建立文件夹
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CreateDir(string name)
        {
            return MakeSureDirectoryPathExists(name);
        }

        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)    //web程序
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else    //非web程序引用
            {
                strPath = strPath.Replace("/", "//");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
                }
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }

        /// <summary>
        /// 返回文件是否存在
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否存在</returns>
        public static bool FileExists(string filename)
        {
            return File.Exists(filename);
        }


        /// <summary>
        /// 返回URL中结尾的文件名
        /// </summary>		
        public static string GetFilename(string url)
        {
            if (url == null)
            {
                return "";
            }
            string[] strs1 = url.Split(new char[] { '/' });
            return strs1[strs1.Length - 1].Split(new char[] { '?' })[0];
        }

        /// <summary>
        /// 获取指定文件的扩展名
        /// </summary>
        /// <param name="fileName">指定文件名</param>
        /// <returns>扩展名</returns>
        public static string GetFileExtName(string fileName)
        {
            if (fileName == null || fileName.Trim() == string.Empty || fileName.IndexOf('.') <= 0)
                return "";

            fileName = fileName.ToLower().Trim();

            return fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');

            return ret;
        }

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result);  //返回长度为44字节的字符串
        }


        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="inputLocation">要压缩的文件</param>
        /// <param name="outputLocation">压缩后的文件</param>
        /// <param name="compressLevel">密码</param>
        /// <param name="compressLevel">压缩等级</param>
        /// <value>默认值5，压缩等级:0-9</value>
        /// <param name="blockSize">压缩块大小</param>
        /// <value>默认值2048</value>
        public static void ZipFile(string inputLocation, string outputLocation, string password = null, int compressLevel = 5, int blockSize = 2048)
        {
            //如果文件没有找到，则报错
            if (!File.Exists(inputLocation))
            {
                throw new System.IO.FileNotFoundException(string.Format("找不到指定的压缩文件:{0}.", inputLocation));
            }

            using (FileStream zipFile = File.Create(outputLocation))
            {
                using (ZipOutputStream zipStream = new ZipOutputStream(zipFile))
                {
                    zipStream.Password = password;

                    using (FileStream streamToZip = new FileStream(inputLocation, FileMode.Open, FileAccess.Read))
                    {
                        string fileName = inputLocation.Substring(inputLocation.LastIndexOf("\\") + 1);

                        ZipEntry zipEntry = new ZipEntry(fileName);
                        zipStream.PutNextEntry(zipEntry);
                        zipStream.SetLevel(compressLevel);
                        byte[] buffer = new byte[blockSize];
                        int sizeRead = 0;

                        try
                        {
                            do
                            {
                                sizeRead = streamToZip.Read(buffer, 0, buffer.Length);
                                zipStream.Write(buffer, 0, sizeRead);
                            }
                            while (sizeRead > 0);
                        }
                        catch (QMException ex)
                        {
                            throw ex;
                        }

                        streamToZip.Close();
                    }

                    zipStream.Finish();
                    zipStream.Close();
                }

                zipFile.Close();
            }
        }

        /// <summary>
        /// 解压单个文件
        /// </summary>
        /// <param name="inputLocation">要压缩的文件</param>
        /// <param name="parentLocation">解压的路径</param>
        /// <param name="password">密码</param>
        /// <param name="overwrite">是否覆盖</param>
        public static void UnZipFile(string inputLocation, string parentLocation, string password, bool overwrite)
        {
            //父目录
            if (parentLocation == "")
                parentLocation = Directory.GetCurrentDirectory();
            if (!parentLocation.EndsWith("\\"))
                parentLocation = parentLocation + "\\";

            using (ZipInputStream inputStream = new ZipInputStream(File.OpenRead(inputLocation)))
            {
                inputStream.Password = password;
                ZipEntry zEntry;

                while ((zEntry = inputStream.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(zEntry.Name);
                    string fileName = Path.GetFileName(zEntry.Name);
                    if (directoryName.Length > 0)
                    {
                        CreateDir(parentLocation + inputLocation);
                    }
                    if (!directoryName.EndsWith("\\"))
                        directoryName += "\\";
                    if (fileName != string.Empty)
                    {
                        if ((File.Exists(parentLocation + inputLocation + fileName) && overwrite) || (!File.Exists(parentLocation + directoryName + fileName)))
                        {
                            using (FileStream streamWriter = File.Create(parentLocation + zEntry.Name))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = inputStream.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }//end while
            }
        }
    }
}
