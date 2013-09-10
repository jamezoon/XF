using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using XFramework.Entity;
using NLite.Data;

namespace XFramework.Data
{
    public class CategoryData : BaseData<CategoryEntity>
    {
        /// <summary>
        /// ELinq数据库操作
        /// </summary>
        public static DbConfiguration _dbConfiguration = dbConfiguration.AddClass<ArticleEntity>();

        public static CategoryEntity Get(int categoryID)
        {
            using (IDbContext dbContext = _dbConfiguration.CreateDbContext())
            {
                IDbSet<CategoryEntity> category = dbContext.Set<CategoryEntity>();

                var r =
                    (from c in category
                     from a in c.Articles
                     select new { c, a })
                     .Where(x => x.a.CategoryID == categoryID)
                     .ToList();

                if (r == null || r.Count == 0)
                    return null;

                CategoryEntity rtn = r.First().c;

                foreach (var sub in r.Where(x => x.c == rtn))
                    rtn.Articles.Add(sub.a);

                return rtn;
            }
        }
    }
}
