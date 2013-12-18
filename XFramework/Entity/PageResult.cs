using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFramework.Entity
{
    /// <summary>
    /// XFramework自定义返回分页结果类
    /// </summary>
    /// <typeparam name="T">列表中的元素</typeparam>
    [Serializable]
    public sealed class PageResult<T> : PageData<T>, IBaseResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回值说明
        /// </summary>
        public string Message { get; set; }
    }
}
