using System;

namespace XFramework.Entity
{
    /// <summary>
    /// XFramework自定义返回列表结果类
    /// </summary>
    /// <typeparam name="T">列表中的元素</typeparam>
    [Serializable]
    public class ListResult<T> : ListData<T>, IBaseResult
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
