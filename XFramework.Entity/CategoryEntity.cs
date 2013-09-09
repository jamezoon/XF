using System;
using NLite.Data;
using System.Collections.Generic;
using System.Linq;

namespace XFramework.Entity
{
    /// <summary>
    /// 文章分类信息
    /// </summary>
    [Table(Name = "category")]
    public class CategoryEntity
    {
        /// <summary>
        /// 文章分类编号
        /// </summary>
        [Id(IsDbGenerated = true)]
        public int CategoryID { get; set; }

        /// <summary>
        /// 文章分类名称
        /// </summary>
        [Column]
        public string CategoryName { get; set; }

        /// <summary>
        /// 文章分类描述
        /// </summary>
        [Column]
        public string CategoryDesc { get; set; }

        /// <summary>
        /// 文章分类父类编号
        /// </summary>
        [Column]
        public int ParentID { get; set; }

        /// <summary>
        /// 分类排序编号，由小到大
        /// </summary>
        [Column]
        public int OrderID { get; set; }

        /// <summary>
        /// 文章创建时间
        /// </summary>
        [Column]
        public DateTime CreateTime { get; set; }

        [Association(ThisKey = "CategoryID", OtherKey = "CategoryID")]
        public IList<ArticleEntity> Articles = new List<ArticleEntity>();
    }
}
