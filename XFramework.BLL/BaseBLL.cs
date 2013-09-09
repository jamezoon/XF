using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using NLite.Data;
using XFramework.Data;

namespace XFramework.BLL
{
    public class BaseBLL<T>
    {
        public static BaseBLL<T> Instance { get { return new BaseBLL<T>(); } }

        public bool Add(T t)
        {
            return Data.BaseData<T>.Instance.Add(t);
        }

        public bool Update(T t, Expression<Func<T, bool>> where)
        {
            return Data.BaseData<T>.Instance.Update(t, where);
        }

        public bool Update(object select, Expression<Func<T, bool>> where)
        {
            return Data.BaseData<T>.Instance.Update(select, where);
        }

        public bool Delete(Expression<Func<T, bool>> where)
        {
            return Data.BaseData<T>.Instance.Delete(where);
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return Data.BaseData<T>.Instance.Get(where);
        }

        public IList<T> GetList(Expression<Func<T, bool>> where)
        {
            return Data.BaseData<T>.Instance.GetAll(where);
        }
    }
}
