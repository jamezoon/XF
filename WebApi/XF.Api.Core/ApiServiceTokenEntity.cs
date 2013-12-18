using System;
using System.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;

using XFramework.Data;

namespace XF.Api.Core
{
    //=======================================
    //此文件由T4模板于2013-12-10自动生成。
    //=======================================

    /// <summary>
    /// Api接口验证
    /// </summary>	
    [Serializable()]
    public class ApiServiceTokenEntity
    {
        /// <summary>
        /// 接口的Key值
        /// </summary>		
        [DataMapping("ServiceKey", DbType.String)]
        public string ServiceKey { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>		
        [DataMapping("UserName", DbType.String)]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>		
        [DataMapping("Password", DbType.String)]
        public string Password { get; set; }

        /// <summary>
        /// 验证IP
        /// </summary>		
        [DataMapping("ValidIP", DbType.String)]
        public string ValidIP { get; set; }

        public const string IpSpilter =";";
    }
}
