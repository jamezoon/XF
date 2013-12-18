using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XF.Api.Lib;
using XFramework.Safe;

namespace XF.Api.Core.Authenticator
{
    /// <summary>
    /// Api方法属性
    /// 该属性要函数执行前会核实查询的CUSCODE与登录的CUSCODE是否相同
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CusCodeCheckAttribute : Attribute
    {
        public string CusCodeArgName;
        public string EncryptCusCodeArgName;
        /// <summary>
        /// 设置CUSCODE对应的参数名称
        /// </summary>
        /// <param name="argName"></param>
        public CusCodeCheckAttribute(string argName = "CusCode",string encryptArgName="EncryptCusCode")
        {
            if (string.IsNullOrEmpty(argName)|| string.IsNullOrEmpty(encryptArgName))
            {
                throw new ArgumentNullException();
            }
            CusCodeArgName = argName;
            EncryptCusCodeArgName = encryptArgName;
        }

        public bool Check(string inputCusCode,string inputEncryCusCode)
        {
            if (string.IsNullOrEmpty(inputCusCode))
            {
                return false;
            }
            if (string.IsNullOrEmpty(inputEncryCusCode))
            {
                return false;
            }

            return Aes.Encrypt(inputCusCode, inputCusCode) == inputEncryCusCode;
        }
    }
}
