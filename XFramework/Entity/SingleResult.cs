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
        /// 返回值
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回值说明
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 记录集
        /// </summary>
        public T Data { get; set; }
    }
}
