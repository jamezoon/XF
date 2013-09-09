using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using XFramework.Entity;

namespace XFramework.Data
{
    /// <summary>
    /// XFramework数据库层需要重写方法
    /// </summary>
    public interface IBaseDb<T>
    {
        bool Add(T t);

        bool Update(T t, Expression<Func<T, bool>> where);

        bool Update(object select, Expression<Func<T, bool>> where);

        bool Delete(Expression<Func<T, bool>> where);

        T Get(Expression<Func<T, bool>> where);

        IList<T> GetAll(Expression<Func<T, bool>> where);
    }
}
