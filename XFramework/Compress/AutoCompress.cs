using System;
using System.Text;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

using Yahoo.Yui.Compressor;

namespace XFramework.Compressor
{
    /// <summary>
    /// XFramework压缩Js、Css操作
    /// </summary>
    public class AutoCompress
    {
        /// <summary>
        /// 加载js和css
        /// </summary>
        public static void LoadJsAndCSS()
        {
            string j = HttpContext.Current.Request.QueryString["p"];
            if (!string.IsNullOrEmpty(j))
            {
                string[] ids = j.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (ids != null && ids.Length > 0)
                {
                    getJS(ids);
                }
            }
        }

        /// <summary>
        /// 输出JS内容
        /// </summary>
        /// <param name="jsID"></param>
        private static void getJS(string[] path)
        {
            if (path == null || path.Length == 0)
                return;

            string fileType = string.Empty;

            StringBuilder compressContent = new StringBuilder();

            for (int i = 0; i < path.Length; i++)
            {
                try
                {
                    string _filePath = HttpContext.Current.Server.MapPath(path[i]);

                    string _fileContent = CompressJsAndCssFile(_filePath, out fileType);

                    if (!string.IsNullOrEmpty(_fileContent))
                        compressContent.Append(_fileContent).Append("\r\n");

                }
                catch
                {
                }
            }

            if (fileType == "css")
            {
                HttpContext.Current.Response.ContentType = "text/css";
            }
            else if (fileType == "js")
            {
                HttpContext.Current.Response.ContentType = "application/x-javascript";
            }

            string acceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

            if (!String.IsNullOrEmpty(acceptEncoding))
            {
                string _acceptEncoding = acceptEncoding.ToUpperInvariant();

                //如果头部里有包含"GZIP”,"DEFLATE",表示客户浏览器支持GZIP,DEFLATE压缩
                if (_acceptEncoding.Contains("GZIP"))
                {
                    //向输出流头部添加压缩信息
                    HttpContext.Current.Response.AppendHeader("Content-encoding", "gzip");
                    HttpContext.Current.Response.Filter = new GZipStream(HttpContext.Current.Response.Filter, CompressionMode.Compress);
                }
                else if (_acceptEncoding.Contains("DEFLATE"))
                {
                    //向输出流头部添加压缩信息
                    HttpContext.Current.Response.AppendHeader("Content-encoding", "deflate");
                    HttpContext.Current.Response.Filter = new DeflateStream(HttpContext.Current.Response.Filter, CompressionMode.Compress);
                }
            }

            if (compressContent.Length > 0)
            {
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
                HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(31536000));
                HttpContext.Current.Response.Cache.SetLastModified(DateTime.Now);
            }

            HttpContext.Current.Response.Write(compressContent.ToString());
        }

        /// <summary>
        /// 功能：压缩单个js以及css文件
        /// 执行过程：
        /// (1)、获取单个js以及css文件
        /// </summary>
        /// <param name="filePath">JS,CSS路径</param>
        /// <param name="jsContent">将JS,CSS压缩后的内容</param>
        /// <returns></returns>
        private static string CompressJsAndCssFile(string filePath, out string fileType)
        {
            fileType = "js";

            string compressContent = string.Empty;

            //文件不存在 
            if (!File.Exists(filePath))
            {
                return compressContent;
            }

            FileInfo fileInfo = new FileInfo(filePath);

            try
            {
                //文件内容
                compressContent = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);

                string _fileExten = fileInfo.Extension.ToLower();

                string _fileFullName = fileInfo.FullName.ToLower();

                if (_fileExten == ".js")
                {
                    fileType = "js";

                    if (!_fileFullName.EndsWith(".no.js") && !_fileFullName.EndsWith(".min.js"))
                    {
                        //初始化
                        var js = new JavaScriptCompressor(compressContent, false, Encoding.UTF8, System.Globalization.CultureInfo.CurrentCulture);

                        //压缩该js
                        compressContent = js.Compress();
                    }
                }
                else if (_fileExten == ".css") //该文件为css文件
                {
                    fileType = "css";

                    //若为不需要的css则不进行压缩
                    if (!_fileFullName.EndsWith(".no.css") && !_fileFullName.EndsWith(".min.css"))
                    {
                        //压缩css
                        compressContent = CssCompressor.Compress(compressContent);
                    }
                }
            }
            catch (Exception e)
            {
            }

            return compressContent;
        }
    }
}