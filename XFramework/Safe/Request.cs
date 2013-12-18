using System;
using System.Web;

namespace XFramework.Safe
{
    public static class Request
    {
        /// <summary>
        /// 尝试从URL或头信息中获取特定信息
        /// </summary>
        /// <param name="request">当前请求会话</param>
        /// <param name="key">获取信息的Key值</param>
        /// <returns>Key对应的Value值</returns>
        public static string GetValue(this HttpRequestBase request, string key)
        {
            string rtnRst = request[key];

            if (string.IsNullOrWhiteSpace(rtnRst)) rtnRst = request.Headers.Get(key);

            return rtnRst;
        }
    }
}
