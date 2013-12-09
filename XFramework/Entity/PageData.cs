using System;

namespace XFramework.Entity
{
    /// <summary>
    /// XFramework自定义分页结果类
    /// </summary>
    /// <typeparam name="T">列表中的元素</typeparam>
    [Serializable]
    public class PageData<T> : ListData<T>
    {
        ///<summary>
        /// 总行数
        ///</summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (RecordCount != 0 && PageSize != 0)
                {
                    if (RecordCount % PageSize == 0)
                    {
                        return RecordCount / PageSize;
                    }

                    return (RecordCount / PageSize) + 1;
                }

                return 0;
            }
        }
    }
}
