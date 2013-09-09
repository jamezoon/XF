using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFramework.Entity
{
    /// <summary>
    /// XFramework自定义返回值的接口规范
    /// </summary>
    public interface IBaseResult
    {
        /// <summary>
        /// 返回值
        /// </summary>
        int Code { get; set; }

        /// <summary>
        /// 返回值说明
        /// </summary>
        string Msg { get; set; }
    }
}
