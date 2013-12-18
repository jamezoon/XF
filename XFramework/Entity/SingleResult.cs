using System;
using System.Runtime.Serialization;

namespace XFramework.Entity
{
    /// <summary>
    /// XFramework自定义返回结果类
    /// </summary>
    /// <typeparam name="T">列表中的元素</typeparam>
    [Serializable]
    public class SingleResult<T> : IBaseResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回值说明
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 记录集
        /// </summary>
        public T Data { get; set; }
    }
}
