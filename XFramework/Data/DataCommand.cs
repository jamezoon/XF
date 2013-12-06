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
    /// XFramework���ݿ����DataCommand
    /// </summary>
    public class DataCommand : IDisposable
    {
        #region DataCommand����

        /// <summary>
        /// SQL��䴫������б�
        /// </summary>
        public List<DataOperationParameter> ParameterList { get; set; }

        /// <summary>
        /// SQL���in�����б�
        /// </summary>
        public List<DataOperationParameterGroup> ParameterGroupCollection { get; set; }

        /// <summary>
        /// �Ƿ���������������
        /// </summary>
        public bool SupportTransaction { get; internal set; }

        ///<summary>
        ///���ݿ�����(ֻд)
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
        /// �����ṩ���������Դ���ʵ�ֵ�ʵ��
        /// </summary>
        private readonly DbProviderFactory providerFactory;

        /// <summary>
        /// ���ݿ�������ַ���
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// ���ݿ����Ӷ���
        /// </summary>
        private DbConnection connection;

        /// <summary>
        /// ���ݿ�ִ�е�SQL�ű�
        /// </summary>
        internal string CommandText { get; set; }

        /// <summary>
        /// ���ݿ���ṩ��������
        /// </summary>
        internal string ProviderName { get; private set; }

        /// <summary>
        /// ��ʾҪ������Դִ�е� SQL ����洢���̡�Ϊ��ʾ����ġ����ݿ����е����ṩһ�����ࡣ
        /// </summary>
        private DbCommand databaseCommand;

        /// <summary>
        /// ��ʾһ�� SQL �����һ�����ݿ����ӣ������������ System.Data.DataSet �͸�������Դ��
        /// </summary>
        private DbDataAdapter dataAdapter;

        /// <summary>
        /// ��ʾҪ������Դ��ִ�е��������ɷ��ʹ�ϵ���ݿ��
        /// </summary>
        private DbTransaction transaction;

        /// <summary>
        /// ָ����ν��������ַ�����
        /// </summary>
        private CommandType commandType;

        #endregion

        #region DataCommand���캯��

        /// <summary>
        /// DataCommand���� ����
        /// </summary>
        /// <param name="databaseName">������web.config��connectionStrings�ڵ�ĵ�keyֵ</param>
        /// <param name="commandText">���ݿ�ִ�е�SQL�ű�</param>
        /// <param name="commandType">SQL�ű��������ַ�����Ĭ����SQL�ı�����</param>
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

        #region DataCommand�������

        /// <summary>
        /// �������ݿ�����
        /// </summary>
        /// <returns>��ʾҪ������Դ��ִ�е�����</returns>
        public DbTransaction BeginTransaction()
        {
            this.SupportTransaction = true;

            BeginExecute();

            return transaction;
        }

        /// <summary>
        /// �ύ���ݿ�����
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
        /// �ع����ݿ�����
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

        #region DataCommandִ�����ݿ����

        /// <summary>
        /// ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С������������к��н������ԡ�
        /// </summary>
        /// <returns>������е�һ�еĵ�һ�С�</returns>
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
        /// �����Ӷ���ִ�� SQL ��䡣�������ݿ�Ӱ�������
        /// </summary>
        /// <returns>��Ӱ���������</returns>
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
        /// �����Ӷ���ִ�� SQL ��䡣�����ִ�н���򷵻�null��
        /// </summary>
        /// <returns>���ش����ʵ�����</returns>
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
        /// �����Ӷ���ִ�� SQL ��䡣�����ִ���޽���򷵻�һ�����б�
        /// </summary>
        /// <returns>���ش����ʵ������б�</returns>
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
        /// ��ȡ���ݿ������DataReader
        /// ʹ��using���Ӳ��ܹر�
        /// </summary>
        /// <returns>�������ݿ����DataReader����סҪ�ر�DataReader</returns>
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
        /// �����Ӷ���ִ�� SQL ��䡣����DataSet����
        /// </summary>
        /// <returns>DataSet�����</returns>
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
        /// �����Ӷ���ִ�� SQL ��䡣����DataTable����
        /// </summary>
        /// <returns>DataTable�����</returns>
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
        /// ��ȡSQL��䴫������Ĳ���ֵ
        /// </summary>
        /// <param name="paramName">SQL��䴫������Ĳ�������</param>
        /// <returns>SQL��䴫������Ĳ���ֵ</returns>
        public object GetParameterValue(string paramName)
        {
            if (databaseCommand.Parameters[DataOperationParameter.BuildKey(this) + paramName] != null)
            {
                return databaseCommand.Parameters[DataOperationParameter.BuildKey(this) + paramName].Value;
            }

            return null;
        }

        #endregion

        #region DataCommand��Դ����

        /// <summary>
        ///  ��д�����Dispose()�������������ݿ������Դ���ջ�����ع�
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region DataCommand˽�з���

        /// <summary>
        /// ��д�����ToString()�������������ݿ����ִ�е�SQL�ű�
        /// </summary>
        /// <returns>���ݿ����ִ�е�SQL�ű�</returns>
        public override string ToString()
        {
            if (databaseCommand != null)
            {
                return databaseCommand.CommandText;
            }

            return string.Empty;
        }

        /// <summary>
        /// ��д�����Dispose()�������������ݿ������Դ���ջ�����ع�
        /// </summary>
        /// <param name="isDisposing">�Ƿ���Ҫ��Դ����</param>
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
        /// ��ȡ���ݿ������ַ���
        /// </summary>
        /// <param name="databaseName">������web.config��connectionStrings�ڵ�ĵ�keyֵ</param>
        /// <param name="connectionString">���ݿ������ַ���</param>
        /// <param name="providerName">���ݿ���ṩ��������</param>
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
        /// ���ݿ������ʼִ��
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
        /// ���ݿ��������ִ��
        /// </summary>
        private void EndExecute()
        {
            if (this.SupportTransaction == false && transaction == null)
                ConnectionDispose();
        }

        /// <summary>
        /// �ͷ����ݿ�������Դ
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
        /// �������ݿ����Command
        /// </summary>
        private void BuildCommand()
        {
            databaseCommand = connection.CreateCommand();
            if (databaseCommand == null)
            {
                throw new NullReferenceException("BuildCommand����:databaseCommandΪnull");
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
        /// �������ݿ����DataAdapter
        /// </summary>
        private void BuildDataAdapter()
        {
            dataAdapter = providerFactory.CreateDataAdapter();

            if (dataAdapter == null)
                throw new NullReferenceException("BuildDataAdapter����:dataAdapterΪnull");
        }

        /// <summary>
        /// ����SQL���Ĵ������
        /// </summary>
        /// <returns></returns>
        private DbParameter BuildParameter()
        {
            DbParameter parameter = providerFactory.CreateParameter();
            if (parameter == null)
            {
                throw new NullReferenceException("BuildParameter����:parameterΪnull");
            }

            return parameter;
        }

        /// <summary>
        /// �������ݿ��������
        /// </summary>
        private void BuildConnection()
        {
            connection = providerFactory.CreateConnection();
            if (connection == null)
            {
                throw new NullReferenceException("BuildConnection����:connectionΪnull");
            }

            connection.ConnectionString = connectionString;
        }

        #endregion
    }
}
