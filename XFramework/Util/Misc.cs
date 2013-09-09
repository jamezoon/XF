using System;
using System.Web;

namespace XFramework.Util
{
    /// <summary>
    /// XFramework 浏览器IP地址操作类
    /// </summary>
    public class Misc
    {
        /// <summary>
        /// 客户浏览器的IP地址
        /// </summary>
        public static string IPAddr
        {
            get
            {
                return GetIPAddr();
            }
        }

        /// <summary>
        /// 获取客户浏览器的IP地址。
        /// </summary>
        /// <param name="isGetFull">是否获取浏览器的全IP地址，默认false。</param>
        /// <param name="i">静态CDN加速i=1；动态CDN加速或不做加速i=0。</param>
        /// <returns>客户浏览器的IP地址</returns>
        public static string GetIPAddr(bool isGetFull = false, int i = 0)
        {
            string userHostAddress = string.Empty;

            HttpRequest request = HttpContext.Current.Request;

            if (request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null)
            {
                userHostAddress = request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                string _userHostAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (isGetFull)
                {
                    userHostAddress = _userHostAddress;
                }
                else
                {
                    string[] arrUserHostAddress = _userHostAddress.Split(',');

                    if (arrUserHostAddress.Length > i)
                    {
                        userHostAddress = arrUserHostAddress[i];
                    }
                    else
                    {
                        userHostAddress = arrUserHostAddress[arrUserHostAddress.Length - 1];
                    }
                }
            }

            if (string.IsNullOrEmpty(userHostAddress))
                userHostAddress = request.UserHostAddress;

            return userHostAddress;
        }
    }
}
