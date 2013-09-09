using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using XFramework.Entity;
using NLite.Data;

namespace XFramework.Data
{
    public class BaseData<T> : IBaseDb<T>
    {
        public const string ConnectionStringName = "XFrameworkDB";

        public static DbConfiguration dbConfiguration = DbConfiguration.Configure(ConnectionStringName).AddClass<T>();

        public static BaseData<T> Instance { get { return new BaseData<T>(); } }

        public bool Add(T t)
        {
            using (IDbContext dbContext = dbConfiguration.CreateDbContext())
            {
                IDbSet<T> dbSet = dbContext.Set<T>();

                return dbSet.Insert(t) > 0;
            }
        }

        public bool Update(T t, Expression<Func<T, bool>> where)
        {
            using (IDbContext dbContext = dbConfiguration.CreateDbContext())
            {
                IDbSet<T> dbSet = dbContext.Set<T>();

                return dbSet.Update(t, where) > 0;
            }
        }

        public bool Update(object select, Expression<Func<T, bool>> where)
        {
            using (IDbContext dbContext = dbConfiguration.CreateDbContext())
            {
                IDbSet<T> dbSet = dbContext.Set<T>();

                return dbSet.Update(select, where) > 0;
            }
        }

        public bool Delete(Expression<Func<T, bool>> where)
        {
            using (IDbContext dbContext = dbConfiguration.CreateDbContext())
            {
                IDbSet<T> dbSet = dbContext.Set<T>();

                return dbSet.Delete(where) > 0;
            }
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            using (IDbContext dbContext = dbConfiguration.CreateDbContext())
            {
                IDbSet<T> dbSet = dbContext.Set<T>();

                return dbSet.First(where);
            }
        }

        public IList<T> GetAll(Expression<Func<T, bool>> where)
        {
            using (IDbContext dbContext = dbConfiguration.CreateDbContext())
            {
                IDbSet<T> dbSet = dbContext.Set<T>();

                return dbSet.Where(where).ToList();
            }
        }
    }
}
