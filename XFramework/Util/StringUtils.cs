using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

using System.Collections.Generic;

namespace XFramework.Util
{
    /// <summary>
    /// XFramework自定义字符串操作
    /// </summary>
    public class StringUtils
    {
        #region =================== | 验证字符串是否合法 | =====================

        /// <summary>
        /// 根据正则表达式校验字符串是否合法
        /// </summary>
        /// <param name="str">待校验的字符串</param>
        /// <param name="format">校验的类型，枚举RegexEnum</param>
        /// <returns>字符串是否合法</returns>
        public static bool CheckStringFormat(string str, RegexFormat format)
        {
            Regex regex;

            switch (format)
            {
                case RegexFormat.Nonnegative:
                    regex = new Regex(@"^\d+$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.Positive:
                    regex = new Regex(@"^[0-9]*[1-9][0-9]*$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.NegativePlusZero:
                    regex = new Regex(@"^((-\d+)|(0+))$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.Negative:
                    regex = new Regex(@"^-[0-9]*[1-9][0-9]*$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.Integer:
                    regex = new Regex(@"^-?\d+$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.PositiveFloatPlusZero:
                    regex = new Regex(@"^\d+(\.\d+)?$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.PositiveFloat:
                    regex = new Regex(@"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.NegativeFloatPlusZero:
                    regex = new Regex(@"^((-\d+(\.\d+)?)|(0+(\.0+)?))$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.NegativeFloat:
                    regex = new Regex(@"^(-(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)))$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.Float:
                    regex = new Regex(@"^(-?\d+)(\.\d+)?$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.EnglishChar:
                    regex = new Regex(@"^[A-Za-z]+$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.EnglishCharUpCase:
                    regex = new Regex(@"^[A-Z]+$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.EnglishCharLowerCase:
                    regex = new Regex(@"^[a-z]+$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.EnglishNumber:
                    regex = new Regex(@"^[A-Za-z0-9]+$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.EnglishNumberUnderline:
                    regex = new Regex(@"^\w+$", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                case RegexFormat.URL:
                    regex = new Regex(@"[a-zA-z]+://[^\s]*", RegexOptions.Compiled);
                    return regex.IsMatch(str);

                default:
                    return false;
            }
        }

        /// <summary>
        /// 确定字符串是否为电子邮件格式
        /// </summary>
        /// <param name="s">是否确定为电子邮件格式的字符串</param>
        /// <returns>字符串是否为电子邮件格式</returns>
        public static bool IsEmail(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            Regex regex = new Regex(@"^[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?$", RegexOptions.Compiled);

            return regex.IsMatch(s);
        }

        /// <summary>
        /// 确定字符串是否为手机号码格式
        /// </summary>
        /// <param name="s">是否确定为手机号码格式的字符串</param>
        /// <returns>字符串是否为手机号码格式</returns>
        public static bool IsMobile(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            Regex reg = new Regex(@"^1[3,4,5,8][0-9]{9}$", RegexOptions.Compiled);

            return reg.IsMatch(s);
        }

        /// <summary>
        /// 确定字符串是否为联系电话格式
        /// </summary>
        /// <param name="s">是否确定为联系电话格式的字符串</param>
        /// <returns>字符串是否为联系电话格式</returns>
        public static bool IsTelephone(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            Regex reg = new Regex(@"^\d{3,4}-\d{7,8}$|^\d{3,4}-\d{7,8}-\d{1,5}$", RegexOptions.Compiled);

            return reg.IsMatch(s);
        }

        /// <summary>
        /// 确定字符串是否为国外联系电话格式
        /// </summary>
        /// <param name="s">是否确定为国外联系电话格式的字符串</param>
        /// <returns>字符串是否为国外联系电话</returns>
        public static bool IsEnPhone(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            Regex regex = new Regex(@"^[\d-]{6,20}$", RegexOptions.Compiled);

            return regex.IsMatch(s);
        }

        /// <summary>
        /// 确定字符串是否为国外邮编格式
        /// </summary>
        /// <param name="s">是否确定为国外邮编格式的字符串</param>
        /// <returns>字符串是否为国外邮编</returns>
        public static bool IsEnPostCode(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            Regex regex = new Regex(@"^[\d\w-\s]{5,20}$", RegexOptions.Compiled);
            return regex.IsMatch(s);
        }

        /// <summary>
        /// 确定字符串是否为日期格式
        /// </summary>
        /// <param name="s">是否确定为日期格式的字符串</param>
        /// <returns>字符串是否为指定日期内的日期格式</returns>
        public static bool IsDateTime(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            DateTime result;

            return DateTime.TryParse(s, out result);
        }

        /// <summary>
        /// 确定是否为身份证号码格式
        /// </summary>
        /// <param name="s">是否确定为身份证格式的字符串</param>
        /// <returns>是否为身份证号码格式</returns>
        public static bool IsIDCard(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            Regex regex = new Regex(@"^[1-9]\d{5}[1-2]\d{3}((0\d)|(1[0-2]))((3[0-1])|([0-2]\d))\d{3}(x|X|\d)$", RegexOptions.Compiled);

            return regex.IsMatch(s);
        }

        /// <summary>
        /// 确定字符串中是否包含中文字符
        /// </summary>
        /// <param name="s">是否包含中文字符的字符串</param>
        /// <returns>字符串中是否包含中文字符</returns>
        public static bool IsHasChineseChar(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            Regex regex = new Regex(@"[\u4e00-\u9fa5]+", RegexOptions.Compiled);

            return regex.IsMatch(s);
        }

        #endregion

        #region =================== | 获取数据库安全字符 | =====================

        /// <summary>
        ///将指定字符串表示形式换成为与它等效的System.Boolean值
        /// </summary>
        /// <param name="value">包含要转换值的字符串串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>换成为与它等效的System.Boolean值</returns>
        public static bool ToBool(object value, bool defaultValue = false)
        {
            if (value == null) return defaultValue;
            bool output;
            bool rst = bool.TryParse(value.ToString(), out output);
            return rst ? output : defaultValue;
        }

        /// <summary>
        /// 将指定字符串形式转换成为与它等效的System.Byte值
        /// </summary>
        /// <param name="obj">包含要转换值的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换成为与它等效的System.Int16值</returns>
        public static byte ToByte(object value, byte defaultValue = 0)
        {
            if (value == null) return defaultValue;
            byte output;
            bool rst = byte.TryParse(value.ToString(), out output);
            return rst ? output : defaultValue;
        }

        /// <summary>
        /// 将指定字符串形式转换成为与它等效的System.Int16值
        /// </summary>
        /// <param name="value">包含要转换值的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换成为与它等效的System.Int16值</returns>
        public static short ToInt16(object value, short defaultValue = 0)
        {
            if (value == null) return defaultValue;
            short output;
            bool rst = short.TryParse(value.ToString(), out output);
            return rst ? output : defaultValue;
        }

        /// <summary>
        /// 将指定字符串表示形式转换成为与它等效的System.Int32值
        /// </summary>
        /// <param name="value">包含要转换值的字符串</param>
        /// <returns>转换成为与它等效的System.Int32值</returns>
        public static int ToInt32(object value, int defaultValue = 0)
        {
            if (value == null) return defaultValue;
            int output;
            bool rst = int.TryParse(value.ToString(), out output);
            return rst ? output : defaultValue;
        }

        /// <summary>
        /// 将指定字符串形式转换成为与它等效的System.Int64值
        /// </summary>
        /// <param name="value">包含要转换值的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换成为与它等效的System.Int64值</returns>
        public static long ToInt64(object value, long defaultValue = 0)
        {
            if (value == null) return defaultValue;
            long output;
            bool rst = long.TryParse(value.ToString(), out output);
            return rst ? output : defaultValue;
        }

        /// <summary>
        /// 将指定字符串形式转换成为与它等效的System.Float值
        /// </summary>
        /// <param name="value">包含要转换值的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换成为与它等效的System.Float值</returns>
        public static float ToFloat(string value, float defaultValue = 0)
        {
            if (value == null) return defaultValue;
            float output;
            bool rst = float.TryParse(value.ToString(), out output);
            return rst ? output : defaultValue;
        }

        /// <summary>
        /// 将指定字符串形式转换成为与它等效的System.Double值
        /// </summary>
        /// <param name="value">包含要转换值的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换成为与它等效的System.Double值</returns>
        public static double ToDouble(object value, double defaultValue = 0)
        {
            if (value == null) return defaultValue;
            double output;
            bool rst = double.TryParse(value.ToString(), out output);
            return rst ? output : defaultValue;
        }

        /// <summary>
        /// 将指定字符串形式转换成为与它等效的System.Decimal值
        /// </summary>
        /// <param name="value">包含要转换值的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换成为与它等效的System.Decimal值</returns>
        public static decimal ToDecimal(string value, decimal defaultValue = 0)
        {
            if (value == null) return defaultValue;
            decimal output;
            bool rst = decimal.TryParse(value.ToString(), out output);
            return rst ? output : defaultValue;
        }

        /// <summary>
        /// 将指定对象形式转换成为与它等效的System.String值
        /// </summary>
        /// <param name="obj">包含要转换值的指定对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换成为与它等效的System.String值</returns>
        public static string ToString(object obj, string defaultValue = "")
        {
            if (obj == null || obj == System.DBNull.Value) return defaultValue;
            return obj.ToString();
        }

        /// <summary>
        /// 将指定字符串形式转换成为与它等效的System.DateTime值
        /// </summary>
        /// <param name="value">包含要转换值的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换成为与它等效的System.DateTime值</returns>
        public static DateTime ToDateTime(object value, DateTime defaultValue)
        {
            if (defaultValue == null) defaultValue = DateTime.MinValue;
            if (value == null) return defaultValue;
            DateTime output;
            bool rst = DateTime.TryParse(value.ToString(), out output);
            return rst ? output : defaultValue;
        }

        /// <summary>
        /// 将指定数组形式转换为与它等效的List，去除数组中的null元素
        /// </summary>
        /// <typeparam name="T">数组中元素类型</typeparam>
        /// <param name="array">待转换的数组</param>
        /// <returns>转换为与数组等效的List</returns>
        public static IList<T> ToList<T>(T[] array)
        {
            IList<T> listT = null;
            if (array != null && array.Length > 0)
            {
                listT = new List<T>(array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    T obj = array[i];
                    if (obj == null)
                    {
                        continue;
                    }
                    listT.Add(obj);
                }
            }
            return listT;
        }

        #endregion

        #region =================== | 获取特定字符串结果 | =====================

        /// <summary>
        /// 从首字符取被截字符串的指定长度字符串，中文2个长度，英文1个长度
        /// </summary>
        /// <param name="s">被截字符串</param>
        /// <param name="length">被截的长度</param>
        /// <returns>被截断后，指定长度字符串</returns>
        public static string GetSubString(string s, int length)
        {
            //如果字符串长度小于等于被截断的长度，则无需再截断直接返回
            if (s.Length <= length)
                return s;

            char[] array = s.ToCharArray();

            StringBuilder sb = new StringBuilder();

            //保存已经截取的长度
            int nLength = 0;

            for (int i = 0; i < array.Length; i++)
            {
                //检测是否包含中文字符，如果包含中文字符则1个中文算2个长度
                if (IsHasChineseChar(array[i].ToString()))
                {
                    sb.Append(array[i]);
                    nLength += 2;
                }
                else
                {
                    sb.Append(array[i]);
                    nLength += 1;
                }

                if (nLength >= length)
                {
                    break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 使用掩码来替换字符串中间的字符
        /// </summary>
        /// <param name="s">被替换的字符串</param>
        /// <returns>使用掩码替换后的新字符串</returns>
        public static string GetStringWithMark(string s)
        {
            if (s.Length < 3)
                return s;

            //保存字符串从第几位开始，第几位结束需要替换字符串
            int start = 0, end = 0;

            if (s.Length == 4) //1**4
            {
                start = 1;
                end = 3;
            }
            else if (s.Length == 5)//1***5
            {
                start = 1;
                end = 4;
            }
            else if (s.Length == 6)//12**56
            {
                start = 2;
                end = 4;
            }
            else
            {
                start = 3;
                end = s.Length - 2;
            }

            char[] array = s.ToCharArray();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                if (i >= start && i <= end)
                {
                    sb.Append("*");
                }
                else
                {
                    sb.Append(array[i]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取去除HTML标签后的字符串内容
        /// </summary>
        /// <param name="content">要去除HTML标签的字符串</param>
        /// <returns>去除HTML标签后的字符串内容</returns>
        public static string GetStringWithHtmlTags(string content)
        {
            return Regex.Replace(content, "<[^>]+>", "", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 获取远程的html内容
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <returns>远程HTML内容</returns>
        public static string GetStringWithUrl(string url)
        {
            WebClient client = new WebClient();

            client.BaseAddress = url;
            client.Headers.Add("Accept", "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*");
            client.Headers.Add("Accept-Language", "zh-cn");
            client.Headers.Add("UA-CPU", "x86");
            client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");

            return client.DownloadString(url);
        }

        /// <summary>
        /// 获取可以传入到数据库中的安全字符串
        /// </summary>
        /// <param name="s">需要替换的字符串</param>
        /// <returns>能传入到数据库中的安全字符串</returns>
        public static string GetSafeString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            s = s.Replace("<", "&lt")
                .Replace(">", "&gt")
                .Replace("'", "‘")
                .Replace("\"", "＂")
                .Replace("--", "")
                .Replace("%", "")
                .Replace(";", "");

            Regex regex = new Regex("select|update|insert|delete|alert|javascript", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            s = regex.Replace(s, "");

            return s;
        }

        /// <summary>
        /// 获取指定长度的随机数字
        /// </summary>
        /// <param name="length">密码长度，默认6位</param>
        /// <returns>指定长度的随机数字</returns>
        public static string GetRadomNum(int length = 6)
        {
            int seed = (int)BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0);

            Random random = new Random(seed);

            int _minValue = (int)Math.Pow(10, length - 1), _maxValue = (int)Math.Pow(10, length);

            int radom = random.Next(_minValue, _maxValue);

            return radom.ToString();
        }

        /// <summary>
        ///将给定的字符串转换成全角字符串
        ///任意字符串
        ///全角字符串
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="s">待转换内容</param>
        /// <returns>转换成全角的字符串内容</returns>
        public static string ToSBC(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            // 半角转全角：
            char[] array = s.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 32)
                {
                    array[i] = (char)12288;
                    continue;
                }
                if (array[i] < 127)
                    array[i] = (char)(array[i] + 65248);
            }
            return new String(array);
        }

        /// <summary>
        ///将给定的字符串转换成半角的字符串
        ///任意字符串
        ///半角字符串
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="s">待转换的内容</param>
        /// <returns>转换成半角的字符串内容</returns>
        public static string ToDBC(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            char[] array = s.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 12288)
                {
                    array[i] = (char)32;
                    continue;
                }
                if (array[i] > 65280 && array[i] < 65375)
                    array[i] = (char)(array[i] - 65248);
            }
            return new String(array);
        }

        /// <summary>  
        /// 返回对象的Json序列化字符串 
        /// </summary>  
        /// <param name="obj">需要序列号的对象</param>  
        /// <returns>Json序列化字符串 </returns>  
        public static string ToJson(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        #endregion
    }

    /// <summary>
    /// XFramework自定义校验格式的正则枚举
    /// </summary>
    [Serializable]
    public enum RegexFormat
    {
        /// <summary>
        /// 非负整数（正整数 + 0） ^\d+$
        /// </summary>
        Nonnegative = 0,
        /// <summary>
        /// 正整数 ^[0-9]*[1-9][0-9]*$
        /// </summary>
        Positive = 1,
        /// <summary>
        /// 非正整数 ^((-\d+)|(0+))$
        /// </summary>
        NegativePlusZero = 2,
        /// <summary>
        /// 负整数 -[0-9]*[1-9][0-9]*
        /// </summary>
        Negative = 3,
        /// <summary>
        /// 整数 ^-?\d+$
        /// </summary>
        Integer = 4,
        /// <summary>
        /// 非负浮点数（正浮点数 + 0）^\d+(\.\d+)?$
        /// </summary>
        PositiveFloatPlusZero = 5,
        /// <summary>
        /// 正浮点数 ^(([0-9]+\\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\\.[0-9]+)|([0-9]*[1-9][0-9]*))$ 
        /// </summary>
        PositiveFloat = 6,
        /// <summary>
        /// 非正浮点数（负浮点数 + 0） ^((-\\d+(\\.\\d+)?)|(0+(\\.0+)?))$
        /// </summary>
        NegativeFloatPlusZero = 7,
        /// <summary>
        /// 负浮点数 ^(-(([0-9]+\\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\\.[0-9]+)|([0-9]*[1-9][0-9]*)))$ 
        /// </summary>
        NegativeFloat = 8,
        /// <summary>
        /// 浮点数 ^(-?\\d+)(\\.\\d+)?$ 
        /// </summary>
        Float = 9,
        /// <summary>
        /// 由26个英文字母组成的字符串 ^[A-Za-z0-9]+$
        /// </summary>
        EnglishChar = 10,
        /// <summary>
        /// 由26个英文字母的大写组成的字符串 ^[A-Z]+$
        /// </summary>
        EnglishCharLowerCase = 11,
        /// <summary>
        /// 由26个英文字母的小写组成的字符串 ^[a-z]+$
        /// </summary>
        EnglishCharUpCase = 12,
        /// <summary>
        /// 由数字和26个英文字母组成的字符串 ^[A-Za-z0-9]+$
        /// </summary>
        EnglishNumber = 13,
        /// <summary>
        /// 由数字、26个英文字母或者下划线组成的字符串 ^\\w+$
        /// </summary>
        EnglishNumberUnderline = 14,
        /// <summary>
        /// url ^[a-zA-z]+://(\\w+(-\\w+)*)(\\.(\\w+(-\\w+)*))*(\\?\\S*)?$
        /// </summary>
        URL = 15
    }
}
