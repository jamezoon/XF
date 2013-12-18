using System;
using System.Web;

using XFramework.Safe;

namespace XFramework.Util
{
    /// <summary>
    /// XFramework自定义Cookie操作类
    /// </summary>
    public class CookieUtil
    {
        /// <summary>
        /// 默认保存在客户浏览器中的数据如果使用加密，加密的key值。此key值只能为8位。
        /// </summary>
        public const string DefaultCookieDESKey = "#E$R%4~2";

        /// <summary>
        /// 更新客户浏览器中的Cookie
        /// </summary>
        /// <param name="cookieName">设置Cookie的名称</param>
        /// <param name="cookieValue">设置Cookie的值</param>
        /// <param name="cookieDomain">设置Cookie的域名</param>
        /// <param name="cookieTime">设置Cookie的过期时间为当前时间的多少分钟以后</param>
        /// <param name="isEncrypt">设置的Cookie是否需要加密，默认不加密</param>
        public static void Set(string cookieName, string cookieValue, string cookieDomain = "", double cookieTime = 0, bool isEncrypt = false)
        {
            HttpCookie cookie = new HttpCookie(cookieName);

            if (isEncrypt) cookieValue = DES.Encrypt(cookieValue, DefaultCookieDESKey);

            cookieValue = HttpUtility.UrlEncode(cookieValue);

            //设置Cookie值
            cookie.Value = cookieValue;

            //设置CookieDomain
            if (!string.IsNullOrEmpty(cookieDomain))
                cookie.Domain = cookieDomain;

            //设置Cookie过期时间
            if (cookieTime > 0) cookie.Expires = DateTime.Now.AddMinutes(cookieTime);

            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        /// <summary>
        /// 获取客户浏览器中Cookie的值
        /// </summary>
        /// <param name="cookieName">获取Cookie的名称</param>
        /// <param name="isEncrypt">设置的Cookie是否是加密的，默认不加密</param>
        /// <returns>客户浏览器中Cookie的值</returns>
        public static string Get(string cookieName, bool isEncrypt = false)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];

            if (cookie == null)
                return string.Empty;

            string s = cookie.Value;

            if (string.IsNullOrEmpty(s))
                return string.Empty;

            s = HttpUtility.UrlDecode(s);

            if (isEncrypt)
                s = DES.Decrypt(s, DefaultCookieDESKey);

            return s;
        }

        /// <summary>
        /// 更新客户浏览器中的Cookie为过期
        /// </summary>
        /// <param name="cookieName">设置Cookie的名称</param>
        /// <param name="cookieDomain">设置Cookie的域名</param>
        public static void Remove(string cookieName, string cookieDomain = "")
        {
            HttpCookie cookie = new HttpCookie(cookieName);

            //设置Cookie值
            cookie.Value = "";

            //设置CookieDomain
            if (!string.IsNullOrEmpty(cookieDomain))
                cookie.Domain = cookieDomain;

            //设置Cookie过期时间
            cookie.Expires = DateTime.Now.AddYears(-1);

            HttpContext.Current.Response.Cookies.Set(cookie);
        }
    }
}
