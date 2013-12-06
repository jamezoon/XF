using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Configuration;

using XFramework;

namespace XFramework.Data
{
    /// <summary>
    /// XFramework数据库操作DataCommand
    /// </summary>
    public class DataCommand : IDisposable
    {
        #region DataCommand属性

        /// <summary>
        /// SQL语句传入参数列表
        /// </summary>
        public List<DataOperationParameter> ParameterList { get; set; }

        /// <summary>
        /// SQL语句in参数列表
        /// </summary>
        public List<DataOperationParameterGroup> ParameterGroupCollection { get; set; }

        /// <summary>
        /// 是否启动了数据事务
        /// </summary>
        public bool SupportTransaction { get; internal set; }

        ///<summary>
        ///数据库事务(只写)
        ///</summary>
        public DbTransaction Transaction
        {
            set
            {
                this.SupportTransaction = true;
                connection = value.Connection;
                transaction = value;
            }
        }

        /// <summary>
        /// 创建提供程序对数据源类的实现的实例
        /// </summary>
        private readonly DbProviderFactory providerFactory;

        /// <summary>
        /// 数据库的链接字符串
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        private DbConnection connection;

        /// <summary>
        /// 数据库执行的SQL脚本
        /// </summary>
        internal string CommandText { get; set; }

        /// <summary>
        /// 数据库的提供程序名称
        /// </summary>
        internal string ProviderName { get; private set; }

        /// <summary>
        /// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个基类。
        /// </summary>
        private DbCommand databaseCommand;

        /// <summary>
        /// 表示一组 SQL 命令和一个数据库连接，它们用于填充 System.Data.DataSet 和更新数据源。
        /// </summary>
        private DbDataAdapter dataAdapter;

        /// <summary>
        /// 表示要在数据源上执行的事务，它由访问关系数据库的
        /// </summary>
        private DbTransaction transaction;

        /// <summary>
        /// 指定如何解释命令字符串。
        /// </summary>
        private CommandType commandType;

        #endregion

        #region DataCommand构造函数

        /// <summary>
        /// DataCommand构造 函数
        /// </summary>
        /// <param name="databaseName">配置在web.config的connectionStrings节点的的key值</param>
        /// <param name="commandText">数据库执行的SQL脚本</param>
        /// <param name="commandType">SQL脚本是命令字符串，默认是SQL文本命令</param>
        public DataCommand(string databaseName, string commandText, CommandType commandType = CommandType.Text)
        {
            ParameterList = new List<DataOperationParameter>();
            ParameterGroupCollection = new List<DataOperationParameterGroup>();

            string conString = string.Empty, providerName = string.Empty;

            GetConnectionString(databaseName, out conString, out providerName);

            DbProviderFactory fact = DbProviderFactories.GetFactory(providerName);

            this.ProviderName = providerName;
            this.providerFactory = fact;
            this.connectionString = conString;

            this.CommandText = commandText;
            this.SupportTransaction = false;
            this.commandType = commandType;
        }

        #endregion

        #region DataCommand事务操作

        /// <summary>
        /// 启动数据库事务
        /// </summary>
        /// <returns>表示要在数据源上执行的事务</returns>
        public DbTransaction BeginTransaction()
        {
            this.SupportTransaction = true;

            BeginExecute();

            return transaction;
        }

        /// <summary>
        /// 提交数据库事务
        /// </summary>
        public void CommitTransaction()
        {
            try
            {
                if (transaction != null)
                {
                    transaction.Commit();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                ConnectionDispose();
            }
        }

        /// <summary>
        /// 回滚数据库事务
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                ConnectionDispose();
            }
        }

        #endregion

        #region DataCommand执行数据库操作

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <returns>结果集中第一行的第一列。</returns>
        public object ExecuteScalar()
        {
            try
            {
                BeginExecute();
                BuildCommand();
                return this.databaseCommand.ExecuteScalar();
            }
            catch
            {
                throw;
            }
            finally
            {
                EndExecute();
            }
        }

        /// <summary>
        /// 对连接对象执行 SQL 语句。返回数据库影响的行数
        /// </summary>
        /// <returns>受影响的行数。</returns>
        public int ExecuteNonQuery()
        {
            try
            {
                BeginExecute();
                BuildCommand();
                return this.databaseCommand.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                EndExecute();
            }
        }

        /// <summary>
        /// 对连接对象执行 SQL 语句。如果无执行结果则返回null。
        /// </summary>
        /// <returns>返回传入的实体对象</returns>
        public T ExecuteEntity<T>() where T : class, new()
        {
            try
            {
                DataTable dt = ExecuteDataTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    return EntityBuilder.BuildEntity<T>(dt.Rows[0]);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 对连接对象执行 SQL 语句。如果无执行无结果则返回一个空列表。
        /// </summary>
        /// <returns>返回传入的实体对象列表</returns>
        public List<T> ExecuteEntityList<T>() where T : class, new()
        {
            try
            {
                DataTable dt = ExecuteDataTable();
                var list = new List<T>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    return EntityBuilder.BuildEntityList<T>(dt);

                }
                else
                {
                    return new List<T>();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取数据库操作的DataReader
        /// 使用using连接不能关闭
        /// </summary>
        /// <returns>返回数据库操作DataReader，记住要关闭DataReader</returns>
        public IDataReader ExecuteDataReader()
        {
            try
            {
                BeginExecute();
                BuildCommand();
                return this.databaseCommand.ExecuteReader();
            }
            catch
            {
                throw;
            }
            finally
            {
                EndExecute();
            }
        }

        /// <summary>
        /// 对连接对象执行 SQL 语句。返回DataSet数据
        /// </summary>
        /// <returns>DataSet结果集</returns>
        public DataSet ExecuteDataSet()
        {
            try
            {
                BeginExecute();
                BuildCommand();
                DataSet dataSet = new DataSet();
                BuildDataAdapter();
                dataAdapter.SelectCommand = this.databaseCommand;
                dataAdapter.Fill(dataSet);
                return dataSet;
            }
            catch
            {
                throw;
            }
            finally
            {
                EndExecute();
            }
        }

        /// <summary>
        /// 对连接对象执行 SQL 语句。返回DataTable数据
        /// </summary>
        /// <returns>DataTable结果集</returns>
        public DataTable ExecuteDataTable()
        {
            DataSet dataSet = ExecuteDataSet();
            if (dataSet == null || dataSet.Tables.Count <= 0)
            {
                return null;
            }

            return dataSet.Tables[0];
        }

        /// <summary>
        /// 获取SQL语句传入参数的参数值
        /// </summary>
        /// <param name="paramName">SQL语句传入参数的参数名称</param>
        /// <returns>SQL语句传入参数的参数值</returns>
        public object GetParameterValue(string paramName)
        {
            if (databaseCommand.Parameters[DataOperationParameter.BuildKey(this) + paramName] != null)
            {
                return databaseCommand.Parameters[DataOperationParameter.BuildKey(this) + paramName].Value;
            }

            return null;
        }

        #endregion

        #region DataCommand资源回收

        /// <summary>
        ///  重写基类的Dispose()方法，用于数据库操作资源回收或事物回滚
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region DataCommand私有方法

        /// <summary>
        /// 重写基类的ToString()方法，返回数据库操作执行的SQL脚本
        /// </summary>
        /// <returns>数据库操作执行的SQL脚本</returns>
        public override string ToString()
        {
            if (databaseCommand != null)
            {
                return databaseCommand.CommandText;
            }

            return string.Empty;
        }

        /// <summary>
        /// 重写基类的Dispose()方法，用于数据库操作资源回收或事物回滚
        /// </summary>
        /// <param name="isDisposing">是否需要资源回收</param>
        protected virtual void Dispose(bool isDisposing)
        {
            // if called in Dispose, commit transaction.
            // otherwise, let the runtime perform GC.
            if (isDisposing)
            {
                if (SupportTransaction)
                {
                    this.RollbackTransaction();
                }
                else
                {
                    this.EndExecute();
                }
            }
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="databaseName">配置在web.config的connectionStrings节点的的key值</param>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerName">数据库的提供程序名称</param>
        private static void GetConnectionString(string databaseName, out string connectionString, out string providerName)
        {
            connectionString = string.Empty;
            providerName = string.Empty;

            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[databaseName];

            if (setting != null)
            {
                connectionString = setting.ConnectionString;
                providerName = setting.ProviderName;
            }
        }

        /// <summary>
        /// 数据库操作开始执行
        /// </summary>
        private void BeginExecute()
        {
            if (this.SupportTransaction && transaction == null)
            {
                this.BuildConnection();
                try
                {
                    connection.Open();
                }
                catch
                {
                    connection = null;
                    throw;
                }

                try
                {
                    transaction = connection.BeginTransaction();
                }
                catch
                {
                    connection.Close();
                    connection.Dispose();
                    throw;
                }
            }
            else if (transaction != null)
            {
                connection = transaction.Connection;
            }
            else
            {
                this.BuildConnection();
                connection.Open();
            }
        }

        /// <summary>
        /// 数据库操作结束执行
        /// </summary>
        private void EndExecute()
        {
            if (this.SupportTransaction == false && transaction == null)
                ConnectionDispose();
        }

        /// <summary>
        /// 释放数据库连接资源
        /// </summary>
        private void ConnectionDispose()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// 构造数据库操作Command
        /// </summary>
        private void BuildCommand()
        {
            databaseCommand = connection.CreateCommand();
            if (databaseCommand == null)
            {
                throw new NullReferenceException("BuildCommand出错:databaseCommand为null");
            }

            databaseCommand.CommandType = commandType;
            databaseCommand.CommandText = CommandText;
            if (SupportTransaction && transaction != null)
            {
                databaseCommand.Transaction = transaction;
            }

            if (ParameterGroupCollection != null && ParameterGroupCollection.Count > 0)
            {
                foreach (DataOperationParameterGroup param in ParameterGroupCollection)
                {
                    int ix = databaseCommand.CommandText.IndexOf(param.ParamName);

                    while (ix >= 0)
                    {
                        databaseCommand.CommandText = databaseCommand.CommandText.Substring(0, ix)
                        + param.ParamValue
                        + databaseCommand.CommandText.Substring(ix + param.ParamName.Length);

                        ix = databaseCommand.CommandText.IndexOf(param.ParamName);
                    }
                }
            }

            if (ParameterList != null && ParameterList.Count > 0)
            {
                foreach (DataOperationParameter param in ParameterList)
                {
                    DbParameter parameter = BuildParameter();
                    parameter.ParameterName = param.ParamName;
                    parameter.Value = param.ParamValue;
                    if (param.Size > 0)
                    {
                        parameter.Size = param.Size;
                    }

                    parameter.DbType = param.DbType;
                    parameter.Direction = param.Direction;
                    databaseCommand.Parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// 构造数据库操作DataAdapter
        /// </summary>
        private void BuildDataAdapter()
        {
            dataAdapter = providerFactory.CreateDataAdapter();

            if (dataAdapter == null)
                throw new NullReferenceException("BuildDataAdapter出错:dataAdapter为null");
        }

        /// <summary>
        /// 构造SQL语句的传入参数
        /// </summary>
        /// <returns></returns>
        private DbParameter BuildParameter()
        {
            DbParameter parameter = providerFactory.CreateParameter();
            if (parameter == null)
            {
                throw new NullReferenceException("BuildParameter出错:parameter为null");
            }

            return parameter;
        }

        /// <summary>
        /// 构造数据库操作连接
        /// </summary>
        private void BuildConnection()
        {
            connection = providerFactory.CreateConnection();
            if (connection == null)
            {
                throw new NullReferenceException("BuildConnection出错:connection为null");
            }

            connection.ConnectionString = connectionString;
        }

        #endregion
    }
}
