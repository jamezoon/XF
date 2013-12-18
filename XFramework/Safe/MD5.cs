using System.Text;
using System.Security.Cryptography;

namespace XFramework.Safe
{
    /// <summary>
    /// MD5方式加密
    /// </summary>
    public class MD5
    {
        /// <summary>
        /// 获取字符串通过MD5加密后的字符
        /// </summary>
        /// <param name="s">需要加密的字符串</param>
        /// <returns>MD5加密后的字符串</returns>
        public static string Encrypt(string s)
        {
            byte[] array = Encoding.UTF8.GetBytes(s);

            array = new MD5CryptoServiceProvider().ComputeHash(array);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                sb.Append(array[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }
    }
}
