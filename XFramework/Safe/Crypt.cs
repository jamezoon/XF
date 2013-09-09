using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace XFramework.Safe
{
    /// <summary>
    /// XFramework自定义加解密类，包括DES加解密，MD5加密，指定长度解密
    /// </summary>
    public class Crypt
    {
        /// <summary>
        /// 默认加解密使用的密钥，必须为8位长度
        /// </summary>
        public const string DefaultDESKey = "$#$56575";

        /// <summary>
        /// 获取DES方式加密后的字符串。
        /// 加完密后的字符可能包含“+”，Request.QueryString接受，“+”字符会漏掉，所以不适合加完密后通过URL传输。
        /// 需要通过URL传输还需要urlecode
        /// </summary>
        /// <param name="s">要加密的字符串。</param>
        /// <param name="desKey">密钥，且必须为8位。</param>
        /// <returns>以Base64格式返回的加密字符串</returns> 
        public static string DESEncrypt(string s, string desKey = DefaultDESKey)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(s);

                des.Key = ASCIIEncoding.ASCII.GetBytes(desKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(desKey);

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
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
        public static string DESDecrypt(string s, string desKey = DefaultDESKey)
        {
            byte[] inputByteArray = Convert.FromBase64String(s);

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(desKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(desKey);

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
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

        /// <summary>
        /// 获取字符串通过MD5加密后的字符
        /// </summary>
        /// <param name="s">需要加密的字符串</param>
        /// <returns>MD5加密后的字符串</returns>
        public static string MD5Encrypt(string s)
        {
            byte[] array = System.Text.Encoding.UTF8.GetBytes(s);

            array = new MD5CryptoServiceProvider().ComputeHash(array);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                sb.Append(array[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取指定长度加密字符串，加密后的字符串全为大写。最多取原文的前dataLength位字符进行加密，超过会自动截断。
        /// </summary>
        /// <param name="s">字符串原文</param>
        /// <param name="dataLength">指定加密的长度，默认为32位，返回长度64位。</param>
        /// <returns>指定长度加密后的字符串</returns>
        public static string Encrypt(string s, int dataLength = 32)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            Rijndael rijndael = Rijndael.Create();

            rijndael.Key = new byte[] { 99, 104, 101, 110, 120, 105, 97, 110, 103, 99, 115, 54, 56, 56, 54, 46 };
            rijndael.Mode = CipherMode.ECB;
            rijndael.Padding = PaddingMode.None;

            MemoryStream ms = new MemoryStream();

            CryptoStream cs = new CryptoStream(ms, rijndael.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] bytDataIn = new byte[dataLength];
            byte[] bytDataIntemp = Encoding.ASCII.GetBytes(s);

            int count = (bytDataIntemp.Length > bytDataIn.Length) ? bytDataIn.Length : bytDataIntemp.Length;

            for (var j = 0; j < count; j++)
                bytDataIn[j] = bytDataIntemp[j];

            cs.Write(bytDataIn, 0, bytDataIn.Length);
            cs.FlushFinalBlock();
            cs.Close();

            StringBuilder sb = new StringBuilder();

            byte[] rst = ms.ToArray();

            for (var i = 0; i < rst.Length; i++)
                sb.Append(rst[i].ToString("X2"));

            return sb.ToString();
        }
    }
}
