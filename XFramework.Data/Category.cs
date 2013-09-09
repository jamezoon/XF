using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using XFramework.Entity;
using NLite.Data;

namespace XFramework.Data
{
    public class Category : BaseData<CategoryEntity>
    {
        static DbConfiguration _dbConfiguration = dbConfiguration.AddClass<ArticleEntity>();

        public static IList<CategoryEntity> GetAll(Expression<Func<CategoryEntity, ArticleEntity, bool>> where)
        {
            using (IDbContext dbContext = _dbConfiguration.CreateDbContext())
            {
                IDbSet<CategoryEntity> category = dbContext.Set<CategoryEntity>();

                var r =
                    (from c in category
                     from a in c.Articles
                     select new { c, a })
                     .Where(x => where.TailCall)
                     .ToList();

                IList<CategoryEntity> rtn = new List<CategoryEntity>();

                foreach (var item in r.GroupBy(x => x.c))
                {
                    CategoryEntity entity = item.Key;

                    foreach (var sub in r.Where(x => x.c == item.Key))
                    {
                        entity.Articles.Add(sub.a);
                    }

                    rtn.Add(entity);
                }

                return rtn;
            }
        }
    }
}
