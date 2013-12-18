using System;
using System.IO;
using System.Web;
using System.Text;
using System.Net;
using System.Collections.Generic;

using XFramework.Log;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace XFramework.Util
{
    /// <summary>
    /// XFramework请求WEB服务器操作
    /// </summary>
    public class HttpWebUtil
    {
        /// <summary>
        /// 获取连接的url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dict"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetUrl(string url, IDictionary<string, string> dict, Encoding encoding)
        {
            if (dict == null || dict.Count == 0) return url;

            if (encoding == null) encoding = Encoding.UTF8;

            string query = GetUrlQuery(dict, encoding);

            if (url.Contains("?"))
                return url + "&" + query;
            else
                return url + "?" + query;
        }

        /// <summary>
        /// 获取提交到URL链接参数的字符串形式
        /// </summary>
        /// <param name="dict">参数数组</param>
        /// <param name="encoding">参数编码，默认utf-8</param>
        /// <returns>提交到URL链接参数的字符串形式</returns>
        public static string GetUrlQuery(IDictionary<string, string> dict, Encoding encoding)
        {
            if (dict == null || dict.Count == 0) return "";

            if (encoding == null) encoding = Encoding.UTF8;

            StringBuilder prestr = new StringBuilder();

            foreach (KeyValuePair<string, string> temp in dict)
            {
                prestr.Append(temp.Key + "=" + HttpUtility.UrlEncode(temp.Value, encoding) + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;

            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

    }
}
