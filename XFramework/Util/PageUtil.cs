using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFramework.Util
{
    /// <summary>
    /// Url格式
    /// </summary>
    public enum UrlFormat
    {
        /// <summary>
        /// 动态Url(不做任何处理)
        /// </summary>
        Dynamic1,

        /// <summary>
        /// 静态url，格式为url-pagenumber.后缀 
        /// </summary>
        Static1,

        /// <summary>
        ///  url-pagenumber-pagesize-all.html 
        /// </summary>
        Static2,

        /// <summary>
        /// 占位符格式的url,在url上自定义
        /// </summary>
        Placeholder
    }
}
