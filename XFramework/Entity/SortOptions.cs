using System;
using System.Runtime.Serialization;

namespace Mola.Framework.Common.Entity
{
    /// <summary>
    /// XFramework自定义排序枚举
    /// </summary>
    [Serializable]
    public enum SortOptions
    {
        /// <summary>
        /// 禁用(不排序)
        /// </summary>
        Disable = 0,
        /// <summary>
        /// 降序
        /// </summary>
        Desc = 1,
        /// <summary>
        /// 升序
        /// </summary>
        Asc = 2
    }
}
