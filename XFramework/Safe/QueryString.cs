using System;
using System.Web;

using XFramework.Util;

namespace XFramework.Safe
{
    /// <summary>
    /// XFramework自定义从URL的Query中获取值
    /// </summary>
    public class QueryString
    {
        /// <summary>
        /// 获取URLQuery中指定KEY中的System.Int16类型值。
        /// </summary>
        /// <param name="key">URLQuery中中的KEY值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>URLQuery中指定KEY中的System.Int16类型值</returns>
        public static short Int16SafeQ(string key, short defaultValue = 0)
        {
            return StringUtils.ToInt16(HttpContext.Current.Request.QueryString[key], defaultValue);
        }

        /// <summary>
        /// 获取URLQuery中指定KEY中的System.Int32类型值。
        /// </summary>
        /// <param name="key">URLQuery中中的KEY值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>URLQuery中指定KEY中的System.Int32类型值</returns>
        public static int Int32SafeQ(string key, int defaultValue = 0)
        {
            return StringUtils.ToInt32((HttpContext.Current.Request.QueryString[key]), defaultValue);
        }

        /// <summary>
        /// 获取URLQuery中指定KEY中的System.Int64类型值。
        /// </summary>
        /// <param name="key">URLQuery中中的KEY值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>URLQuery中指定KEY中的System.Int64类型值</returns>
        public static long Int64SafeQ(string key, long defaultValue = 0)
        {
            return StringUtils.ToInt64(HttpContext.Current.Request.QueryString[key], defaultValue);
        }

        /// <summary>
        /// 获取URLQuery中指定KEY中的System.Float类型值。
        /// </summary>
        /// <param name="key">URLQuery中中的KEY值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>URLQuery中指定KEY中的System.Float类型值</returns>
        public static float FloatSafeQ(string key, float defaultValue = 0)
        {
            return StringUtils.ToFloat(HttpContext.Current.Request.QueryString[key], defaultValue);
        }

        /// <summary>
        /// 获取URLQuery中指定KEY中的System.Double类型值。
        /// </summary>
        /// <param name="key">URLQuery中中的KEY值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>URLQuery中指定KEY中的System.Double类型值</returns>
        public static double DoubleSafeQ(string key, double defaultValue = 0)
        {
            return StringUtils.ToDouble(HttpContext.Current.Request.QueryString[key], defaultValue);
        }

        /// <summary>
        /// 获取URLQuery中指定KEY中的System.Decimal类型值。
        /// </summary>
        /// <param name="key">URLQuery中中的KEY值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>URLQuery中指定KEY中的System.Decimal类型值</returns>
        public static decimal DecimalSafeQ(string key, decimal defaultValue = 0)
        {
            return StringUtils.ToDecimal(HttpContext.Current.Request.QueryString[key], defaultValue);
        }

        /// <summary>
        /// 获取URLQuery中指定KEY中的System.String类型值。
        /// </summary>
        /// <param name="key">URLQuery中中的KEY值</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="length">获取字符串的长度，超过长度将自动截断。默认50。</param>
        /// <param name="format">获取字符串的格式。0-不做转换；1-小写格式；2-大写格式。</param>
        /// <returns>URLQuery中指定KEY中的System.String类型值</returns>
        public static string SafeQ(string key, string defaultValue = "", int length = 50, short format = 0)
        {
            string s = StringUtils.ToString(HttpContext.Current.Request.QueryString[key], defaultValue);

            s = s.Trim();

            if (s.Length > length)
                s = StringUtils.GetSubString(s, length);

            s = StringUtils.GetSafeString(s);

            switch (format)
            {
                case 1:
                    {
                        return s.ToLower();
                    }
                case 2:
                    {
                        return s.ToUpper();
                    }
                default:
                    {
                        return s;
                    }
            }
        }
    }
}
