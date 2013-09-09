﻿using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using NLite.Data;
using XFramework.Data;

namespace XFramework.BLL
{
    /// <summary>
    /// 实现XFramework业务操作基本方法
    /// </summary>
    /// <typeparam name="T">数据库单表数据实体</typeparam>
    public class BaseBLL<T>
    {
        /// <summary>
        /// 业务操作单元实例
        /// </summary>
        public static BaseBLL<T> Instance { get { return new BaseBLL<T>(); } }

        /// <summary>
        /// 添加一条新数据
        /// </summary>
        /// <param name="t">新数据实体</param>
        /// <returns>是否添加成功</returns>
        public bool Add(T t)
        {
            return Data.BaseData<T>.Instance.Add(t);
        }

        /// <summary>
        /// 根据表主键更新一条数据
        /// </summary>
        /// <param name="t">数据实体，必须包括主键</param>
        /// <param name="where">除了主键外的其他限制条件</param>
        /// <returns>是否更新成功</returns>
        public bool Update(T t, Expression<Func<T, bool>> where)
        {
            return Data.BaseData<T>.Instance.Update(t, where);
        }

        /// <summary>
        /// 根据条件更新数据
        /// </summary>
        /// <param name="select">需要更新是数据，支持集合包括(IDictionary、IList、ICollection)</param>
        /// <param name="where">更新数据的条件，不能为null。（如果where=null，将更新全表操作属于危险动作）</param>
        /// <returns>是否更新成功</returns>
        public bool Update(object select, Expression<Func<T, bool>> where)
        {
            return Data.BaseData<T>.Instance.Update(select, where);
        }

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <param name="where">删除数据条件，不能为null。（如果where=null，将删除全表操作属于危险动作）</param>
        /// <returns>是否删除成功</returns>
        public bool Delete(Expression<Func<T, bool>> where)
        {
            return Data.BaseData<T>.Instance.Delete(where);
        }

        /// <summary>
        /// 根据条件获取一条数据实体
        /// </summary>
        /// <param name="where">获取数据条件</param>
        /// <returns>数据实体</returns>
        public T Get(Expression<Func<T, bool>> where)
        {
            return Data.BaseData<T>.Instance.Get(where);
        }

        /// <summary>
        /// 根据条件获取数据列表
        /// </summary>
        /// <param name="where">获取数据列表条件</param>
        /// <returns>数据列表</returns>
        public IList<T> GetList(Expression<Func<T, bool>> where)
        {
            return Data.BaseData<T>.Instance.GetList(where);
        }
    }
}
