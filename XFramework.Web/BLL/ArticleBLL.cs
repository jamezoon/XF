using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using XFramework.Entity;
using NLite.Data;

namespace XFramework.BLL
{
    public class ArticleBLL
    {
        /// <summary>
        /// Elinq数据库配置
        /// </summary>
        static DbConfiguration dbConfiguration = null;

        static ArticleBLL()
        {
            dbConfiguration = DbConfigManager.Instance.AddClass<CategoryEntity>().AddClass<ArticleEntity>().GetDbConfiguration();
        }

        public static PageData<ArticleEntity> GetList(int categoryID, int pageIndex, int pageSize)
        {
            PageData<ArticleEntity> rtnRst = new PageData<ArticleEntity>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            using (IDbContext dbContext = dbConfiguration.CreateDbContext())
            {
                IDbSet<CategoryEntity> category = dbContext.Set<CategoryEntity>();

                IQueryable<ArticleEntity> q = (from c in category
                                               from a in c.Articles
                                               where a.CategoryID == categoryID
                                               orderby a.OrderID
                                               select a);
                rtnRst.RecordCount = q.Count();

                rtnRst.Data = q.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return rtnRst;
            }
        }
    }
}
