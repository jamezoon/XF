using System;
using System.IO;
using System.Web;
using System.Text;
using System.Net;
using System.Collections.Generic;

using XFramework.Log;

namespace XFramework.Util
{
    /// <summary>
    /// XFramework请求WEB服务器操作
    /// </summary>
    public class HttpWebUtil
    {
        /// <summary>
        /// HttpPost请求
        /// </summary>
        /// <param name="dictArray">请求参数</param>
        /// <param name="url">请求地址</param>
        /// <param name="inputCharset">页面编码</param>
        /// <returns>返回http响应结果</returns>
        public static string SendPostInfo(Dictionary<string, string> dictArray, string url, string inputCharset)
        {
            return GetWebResponeString(dictArray, url, inputCharset, "post");
        }

        /// <summary>
        /// HttpGet请求
        /// </summary>
        /// <param name="dictArray">请求参数</param>
        /// <param name="url">请求地址</param>
        /// <param name="inputCharset">页面编码</param>
        /// <returns>返回http响应结果</returns>
        public static string SendGetInfo(Dictionary<string, string> dictArray, string url, string inputCharset)
        {
            return GetWebResponeString(dictArray, url, inputCharset, "get");
        }

        /// <summary>
        /// 获取Web服务器的响应结果
        /// </summary>
        /// <param name="dictArray">提交的Web服务器端的参数</param>
        /// <param name="url">web服务器端的URL链接地址</param>
        /// <param name="inputCharset">web页面编码</param>
        /// <param name="method">提交方式，post或get</param>
        /// <returns>Web服务器的响应果</returns>
        public static HttpWebResponse GetWebResponse(Dictionary<string, string> dictArray, string url, string inputCharset, string method)
        {
            //待请求参数数组字符串
            Encoding code = Encoding.GetEncoding(inputCharset);

            string strRequestData = GetDataPars(dictArray, code);

            try
            {
                //构造请求地址
                string strUrl = url;

                if (method == "post")
                    strUrl += "?_input_charset=" + inputCharset;
                else
                    strUrl += "?" + strRequestData;

                //设置HttpWebRequest基本信息
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                myReq.Method = method;

                if (method == "post")
                {
                    //把数组转换成流中所需字节数组类型
                    byte[] bytesRequestData = code.GetBytes(strRequestData);

                    myReq.ContentType = "application/x-www-form-urlencoded";
                    //填充POST数据
                    myReq.ContentLength = bytesRequestData.Length;

                    using (Stream requestStream = myReq.GetRequestStream())
                    {
                        requestStream.Write(bytesRequestData, 0, bytesRequestData.Length);
                    }
                }

                //发送POST数据请求服务器
                HttpWebResponse httpWResp = (HttpWebResponse)myReq.GetResponse();

                return httpWResp;
            }
            catch (Exception ex)
            {
                LogUtil.Log("XFramework底层Http请求异常", ex, LogLevel.Warn);

                return null;
            }
        }

        /// <summary>
        /// 获取Web服务器响应的字符串结果
        /// </summary>
        /// <param name="dictArray">提交的Web服务器端的参数</param>
        /// <param name="url">web服务器端的URL链接地址</param>
        /// <param name="inputCharset">web页面编码</param>
        /// <param name="method">提交方式，post或get</param>
        /// <returns>Web服务器响应的字符串结果</returns>
        private static string GetWebResponeString(Dictionary<string, string> dictArray, string url, string inputCharset, string method)
        {
            using (Stream myStream = GetWebResponse(dictArray, url, inputCharset, method).GetResponseStream())
            {
                StreamReader reader = new StreamReader(myStream);

                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 获取提交到URL链接参数的字符串形式
        /// </summary>
        /// <param name="dictArray">参数数组</param>
        /// <param name="encodeing">参数编码，默认utf-8</param>
        /// <returns>提交到URL链接参数的字符串形式</returns>
        private static string GetDataPars(Dictionary<string, string> dictArray, Encoding encodeing)
        {
            if (dictArray == null) return "";

            if (encodeing == null) encodeing = Encoding.UTF8;

            StringBuilder prestr = new StringBuilder();

            foreach (KeyValuePair<string, string> temp in dictArray)
            {
                prestr.Append(temp.Key + "=" + HttpUtility.UrlEncode(temp.Value, encodeing) + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;

            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }
    }
}
