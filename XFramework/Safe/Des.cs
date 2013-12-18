using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace XFramework.Safe
{
    /// <summary>
    /// DES数据加密标准算法
    /// </summary>
    public class DES
    {
        /// <summary>
        /// 默认加解密使用的密钥，必须为1个字节的长度，8位
        /// </summary>
        private const string DESKey = "$#$56575";

        /// <summary>
        /// 获取DES方式加密后的字符串。
        /// 加完密后的字符可能包含“+”，Request.QueryString接受，“+”字符会漏掉，所以不适合加完密后通过URL传输。
        /// 需要通过URL传输还需要urlecode
        /// </summary>
        /// <param name="s">要加密的字符串。</param>
        /// <param name="desKey">密钥，且必须为8位。</param>
        /// <returns>以Base64格式返回的加密字符串</returns> 
        public static string Encrypt(string s, string desKey = DESKey)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(s);

                des.Key = ASCIIEncoding.ASCII.GetBytes(desKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(desKey);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// 获取DES方式解密后的字符串
        /// </summary>
        /// <param name="s">要解密的以Base64</param>
        /// <param name="desKey">密钥，且必须为8位。</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt(string s, string desKey = DESKey)
        {
            byte[] inputByteArray = Convert.FromBase64String(s);

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(desKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(desKey);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}
