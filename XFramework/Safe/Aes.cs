using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace XFramework.Safe
{
    /// <summary>
    /// AES数据加密高级标准算法
    /// </summary>
    public class Aes
    {
        /// <summary>
        /// 获取指定长度加密字符串，加密后的字符串全为大写。最多取原文的前dataLength位字符进行加密，超过会自动截断。
        /// </summary>
        /// <param name="s">字符串原文</param>
        /// <param name="dataLength">指定加密的长度，默认为32位，返回长度64位。</param>
        /// <returns>指定长度加密后的字符串</returns>
        public static string Encrypt2(string s, int dataLength = 32)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

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
            {
                bytDataIn[j] = bytDataIntemp[j];
            }

            cs.Write(bytDataIn, 0, bytDataIn.Length);
            cs.FlushFinalBlock();
            cs.Close();

            StringBuilder sb = new StringBuilder();

            byte[] rst = ms.ToArray();

            for (var i = 0; i < rst.Length; i++)
            {
                sb.Append(rst[i].ToString("X2"));
            }

            return sb.ToString();
        }

        #region 成员变量

        /// <summary>
        /// 密钥(32位,不足在后面补0)
        /// </summary>
        private const string DefaultKey = "Sometimes it lasts in love but sometimes it hurts instead";

        /// <summary>
        /// 运算模式
        /// </summary>
        private static CipherMode cipherMode = CipherMode.ECB;

        /// <summary>
        /// 填充模式
        /// </summary>
        private static PaddingMode paddingMode = PaddingMode.PKCS7;

        /// <summary>
        /// 字符串采用的编码
        /// </summary>
        private static Encoding encoding = Encoding.UTF8;

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取32byte密钥数据
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private static byte[] GetKeyArray(string password, int len = 32)
        {
            if (password == null)
            {
                password = string.Empty;
            }

            if (password.Length < len)
            {
                password = password.PadRight(len, '0');
            }
            else if (password.Length > len)
            {
                password = password.Substring(0, len);
            }

            return encoding.GetBytes(password);
        }

        /// <summary>
        /// 将字符数组转换成字符串
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        private static string ConvertByteToString(byte[] inputData)
        {
            var sb = new StringBuilder(inputData.Length * 2);

            foreach (var b in inputData)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 将字符串转换成字符数组
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private static byte[] ConvertStringToByte(string inputString)
        {
            if (inputString == null || inputString.Length < 2)
            {
                throw new ArgumentException();
            }

            var len = inputString.Length / 2;

            var result = new byte[len];

            for (var i = 0; i < len; ++i)
            {
                result[i] = Convert.ToByte(inputString.Substring(2 * i, 2), 16);
            }

            return result;
        }

        #endregion

        #region 加密

        /// <summary>
        /// 加密字节数据
        /// </summary>
        /// <param name="inputData">要加密的字节数据</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] inputData, string password)
        {
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider
            {
                Key = GetKeyArray(password),
                Mode = cipherMode,
                Padding = paddingMode
            };
            var transform = aes.CreateEncryptor();
            var data = transform.TransformFinalBlock(inputData, 0, inputData.Length);
            aes.Clear();
            return data;
        }

        /// <summary>
        /// 加密字符串(加密为16进制字符串)
        /// </summary>
        /// <param name="inputString">要加密的字符串</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static string Encrypt(string inputString, string password)
        {
            var toEncryptArray = encoding.GetBytes(inputString);
            var result = Encrypt(toEncryptArray, password);
            return ConvertByteToString(result);
        }

        /// <summary>
        /// 字符串加密(加密为16进制字符串)
        /// </summary>
        /// <param name="inputString">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptString(string inputString)
        {
            return Encrypt(inputString, DefaultKey);
        }

        #endregion

        #region 解密

        /// <summary>
        /// 解密字节数组
        /// </summary>
        /// <param name="inputData">要解密的字节数据</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] inputData, string password)
        {
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider
            {
                Key = GetKeyArray(password),
                Mode = cipherMode,
                Padding = paddingMode
            };

            var transform = aes.CreateDecryptor();

            byte[] data = null;

            try
            {
                data = transform.TransformFinalBlock(inputData, 0, inputData.Length);
            }
            catch
            {
                return null;
            }

            aes.Clear();

            return data;
        }

        /// <summary>
        /// 解密16进制的字符串为字符串
        /// </summary>
        /// <param name="inputString">要解密的字符串</param>
        /// <param name="password">密码</param>
        /// <returns>字符串</returns>
        public static string Decrypt(string inputString, string password)
        {
            byte[] toDecryptArray = ConvertStringToByte(inputString);
            var data = Decrypt(toDecryptArray, password);
            if (data == null)
            {
                return null;
            }
            string decryptString = encoding.GetString(data);
            return decryptString;
        }

        /// <summary>
        /// 解密16进制的字符串为字符串
        /// </summary>
        /// <param name="inputString">需要解密的字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptString(string inputString)
        {
            return Decrypt(inputString, DefaultKey);
        }
        #endregion
    }
}
