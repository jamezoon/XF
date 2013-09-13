using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using NLite.Data;

namespace XFramework.Data
{
    /// <summary>
    /// ELinq数据库配置信息
    /// </summary>
    public class DbConfigManager
    {
        /// <summary>
        /// ELinq数据库配置操作
        /// </summary>
        DbConfiguration dbConfiguration = null;

        private static DbConfigManager lbs;

        private static object syncLock = new object();

        private DbConfigManager()
        {
            dbConfiguration = DbConfiguration.Configure("XFrameworkDB");
        }

        public static DbConfigManager Instance
        {
            get
            {
                if (lbs == null)
                {
                    lock (syncLock)
                    {
                        if (lbs == null)
                        {
                            lbs = new DbConfigManager();
                        }
                    }
                }

                return lbs;
            }
        }

        /// <summary>
        /// 注册ELinq实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DbConfigManager AddClass<T>()
        {
            if (!dbConfiguration.HasClass<T>())
                dbConfiguration.AddClass<T>();

            return this;
        }

        /// <summary>
        /// 获取ELinq数据库操作
        /// </summary>
        /// <returns></returns>
        public DbConfiguration GetDbConfiguration()
        {
            return dbConfiguration;
        }
    }
}
