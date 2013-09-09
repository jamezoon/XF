using System;
using NLite.Data;
using System.Linq;

namespace XFramework.Entity
{
    [Table(Name = "article")]
    public class ArticleEntity
    {
        [Id(IsDbGenerated = true)]
        public int ArticleID { get; set; }

        [Column]
        public string ArticleTitle { get; set; }

        [Column]
        public string ArticleDesc { get; set; }

        [Column]
        public int CategoryID { get; set; }

        [Column]
        public int OrderID { get; set; }

        [Column]
        public DateTime CreateTime { get; set; }

        [Column]
        public bool IsRedirect { get; set; }

        [Column]
        public string RedirectUrl { get; set; }
    }
}
