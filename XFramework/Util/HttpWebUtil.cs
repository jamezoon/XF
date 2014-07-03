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
        public static HttpWebUtil Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }
            internal static readonly HttpWebUtil instance = new HttpWebUtil();
        }

        /// <summary>
        /// 获取连接的url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dict"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string GetUrl(string url, IDictionary<string, string> dict, Encoding encoding)
        {
            if (dict == null || dict.Count == 0) return url;

            if (encoding == null) encoding = Encoding.UTF8;

            string query = this.GetUrlQuery(dict, encoding);

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
        public string GetUrlQuery(IDictionary<string, string> dict, Encoding encoding)
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

        private int _timeout = 100000;

        /// <summary>
        /// 请求与响应的超时时间
        /// </summary>
        public int Timeout
        {
            get { return this._timeout; }
            set { this._timeout = value; }
        }

        public string DoPost(string url, IDictionary<string, string> parameters, string input)
        {
            HttpWebRequest req = GetWebRequest(url, "POST");

            req.ContentType = "application/json";

            #region build header

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();

            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;

                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                    req.Headers.Add(name, HttpUtility.UrlEncode(value, Encoding.UTF8));
            }

            #endregion

            byte[] postData = Encoding.UTF8.GetBytes(input);
            System.IO.Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

            return GetResponseString(rsp, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public string DoGet(string url, IDictionary<string, string> parameters, Encoding encoding)
        {
            url = this.GetUrl(url, parameters, encoding);

            HttpWebRequest req = GetWebRequest(url, "GET");
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            return GetResponseString(rsp, encoding);
        }

        /// <summary>
        /// 验证SSL安全模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开
            return true;
        }

        /// <summary>
        /// 获取Http请求对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private HttpWebRequest GetWebRequest(string url, string method)
        {
            HttpWebRequest req = null;
            if (url.Contains("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(url);
            }

            req.Method = method;
            req.KeepAlive = true;
            req.UserAgent = "XFApi4Net";
            req.Timeout = this._timeout;

            return req;
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        private string GetResponseString(HttpWebResponse rsp, Encoding encoding)
        {
            System.IO.Stream stream = null;
            StreamReader reader = null;

            try
            {
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }
    }
}
