using System;
using System.Text;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

using Yahoo.Yui.Compressor;
using XFramework.Util;
using XFramework.Safe;
using XFramework.Log;

namespace XFramework
{
    /// <summary>
    /// XFramework压缩Js、Css操作
    /// </summary>
    public class Compress
    {
        /// <summary>
        /// 文件压缩类的构造函数
        /// </summary>
        /// <param name="isJsFile">是否是js文件压缩</param>
        /// <param name="directory">文件存放的目录位置</param>
        public Compress(bool isJsFile, string directory)
        {
            IsJsFile = isJsFile;
            DirectoryPath = directory;
        }

        /// <summary>
        /// 是否是js文件压缩
        /// </summary>
        public bool IsJsFile { get; set; }

        /// <summary>
        /// 文件存放在WEB服务器的相对目录路径
        /// 前后不需要带斜杆(\)，如js\20131206
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// 响应压缩结果到客户端
        /// </summary>
        public void ResponseResultToClient()
        {
            //获取文件名，以逗号分隔。
            string p = QueryString.SafeQ("p");
            //文件编码格式，默认utf-8
            string inputCharset = QueryString.SafeQ("c");

            if (string.IsNullOrWhiteSpace(p))
            {
                HttpContext.Current.Response.End();
                return;
            }

            string[] files = p.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (files.Length == 0) return;

            StringBuilder sb = new StringBuilder();

            foreach (string item in files)
            {
                string compressContent = this.GetCompressContent(item, inputCharset);

                if (!string.IsNullOrEmpty(compressContent))
                    sb.Append(compressContent).Append("\r\n");
            }

            if (sb.Length == 0)
            {
                HttpContext.Current.Response.End();
                return;
            }

            if (IsJsFile)
            {
                HttpContext.Current.Response.ContentType = "text/css";
            }
            else
            {
                HttpContext.Current.Response.ContentType = "application/x-javascript";
            }

            string acceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

            if (!string.IsNullOrEmpty(acceptEncoding))
            {
                acceptEncoding = acceptEncoding.ToUpper();

                //如果头部里有包含"GZIP”,"DEFLATE",表示客户浏览器支持GZIP,DEFLATE压缩
                if (acceptEncoding.Contains("GZIP"))
                {
                    //向输出流头部添加压缩信息
                    HttpContext.Current.Response.AppendHeader("Content-encoding", "gzip");
                    HttpContext.Current.Response.Filter = new GZipStream(HttpContext.Current.Response.Filter, CompressionMode.Compress);
                }
                else if (acceptEncoding.Contains("DEFLATE"))
                {
                    //向输出流头部添加压缩信息
                    HttpContext.Current.Response.AppendHeader("Content-encoding", "deflate");
                    HttpContext.Current.Response.Filter = new DeflateStream(HttpContext.Current.Response.Filter, CompressionMode.Compress);
                }
            }

            if (sb.Length > 0)
            {
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
                HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(31536000));
                HttpContext.Current.Response.Cache.SetLastModified(DateTime.Now);
            }

            HttpContext.Current.Response.Write(sb.ToString());
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 获取文件压缩内容
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="inputCharset">web服务器编码格式</param>
        /// <returns>文件压缩内容</returns>
        private string GetCompressContent(string fileName, string inputCharset)
        {
            Encoding encoding = Encoding.UTF8;

            if (!string.IsNullOrWhiteSpace(inputCharset))
            {
                try
                {
                    encoding = Encoding.GetEncoding(inputCharset);
                }
                catch (Exception ex)
                {
                    LogUtil.Log("XFramework文件压缩编码转换异常", ex, LogLevel.Warn);
                }
            }

            string rtnRst = string.Empty;

            try
            {
                if (!string.IsNullOrWhiteSpace(DirectoryPath)) DirectoryPath = "\\" + DirectoryPath + "\\";

                string filePath = HttpContext.Current.Server.MapPath(DirectoryPath + fileName);

                if (!File.Exists(filePath)) return null;

                rtnRst = File.ReadAllText(filePath, encoding);

                if (IsJsFile)
                {
                    if (filePath.EndsWith(".no.js") && !filePath.EndsWith(".min.js")) return rtnRst;

                    var compressor = new JavaScriptCompressor { Encoding = encoding };

                    rtnRst = compressor.Compress(rtnRst);
                }
                else
                {
                    if (filePath.EndsWith(".no.css") && !filePath.EndsWith(".min.css")) return rtnRst;

                    var compressor = new CssCompressor();

                    rtnRst = compressor.Compress(rtnRst);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Log("XFramework文件压缩异常", ex, LogLevel.Warn);
            }

            return rtnRst;
        }
    }
}