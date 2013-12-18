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
    /// Api接口类库说明
    /// </summary>	
    [Serializable()]
    public class ApiServiceEntity
    {
        /// <summary>
        /// 接口的Key值
        /// </summary>		
        [DataMapping("ServiceKey", DbType.String)]
        public string ServiceKey { get; set; }

        /// <summary>
        /// 接口描述
        /// </summary>		
        [DataMapping("ServiceDesc", DbType.String)]
        public string ServiceDesc { get; set; }

        /// <summary>
        /// 接口的Interface类型
        /// </summary>		
        [DataMapping("InterfaceType", DbType.String)]
        public string InterfaceType { get; set; }

        /// <summary>
        /// 接口的类库，格式（类名的fullname,namespace名称）
        /// </summary>		
        [DataMapping("ImplementType", DbType.String)]
        public string ImplementType { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>		
        [DataMapping("IsEnable", DbType.Boolean)]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 接口分组
        /// </summary>		
        [DataMapping("ServiceGroup", DbType.String)]
        public string ServiceGroup { get; set; }
    }
}
