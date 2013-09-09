using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using XFramework.Util;

namespace XFramework.Data
{
    /// <summary>
    /// XFramework数据库操作管理
    /// </summary>
    public static class DataCommandManager
    {
        /// <summary>
        /// 获取数据库操作DataCommande对象实例
        /// </summary>
        /// <param name="databaseName">配置在web.config的connectionStrings节点的的key值</param>
        /// <param name="sqlCmd">数据库执行SQL语句</param>
        /// <param name="paramList">SQL语句传入参数列表</param>
        /// <param name="groupparamList">SQL语句in参数列表</param>
        /// <param name="commandType">指定如何解释命令字符串。默认执行SQL语句。</param>
        /// <returns>数据库操作DataCommande对象实例</returns>
        public static DataCommand GetDataCommand(string databaseName, string sqlCmd = null, List<DataOperationParameter> paramList = null, List<DataOperationParameterGroup> groupparamList = null, CommandType commandType = CommandType.Text)
        {
            return GetDataOperationCommand(databaseName, sqlCmd, paramList, groupparamList, commandType);
        }

        /// <summary>
        /// 获取数据库操作DataCommande对象实例
        /// </summary>
        /// <param name="databaseName">配置在web.config的connectionStrings节点的的key值</param>
        /// <param name="sqlCmd">数据库执行SQL语句</param>
        /// <param name="paramList">SQL语句传入参数列表</param>
        /// <param name="supportTran">是否使用事务</param>
        /// <param name="commandType">指定如何解释命令字符串。默认执行SQL语句。</param>
        /// <returns>数据库操作DataCommande对象实例</returns>
        public static DataCommand GetDataCommand(string databaseName, string sqlCmd, List<DataOperationParameter> paramList, bool supportTran, CommandType commandType = CommandType.Text)
        {
            DataCommand command = GetDataOperationCommand(databaseName, sqlCmd, paramList, null, commandType);

            command.SupportTransaction = supportTran;

            return command;
        }

        /// <summary>
        /// 获取数据库操作DataCommande对象实例
        /// </summary>
        /// <param name="databaseName">配置在web.config的connectionStrings节点的的key值</param>
        /// <param name="sqlCmd">数据库执行SQL语句</param>
        /// <param name="paramList">SQL语句传入参数列表</param>
        /// <param name="groupparamList">SQL语句in参数列表</param>
        /// <param name="supportTran">是否使用事务</param>
        /// <param name="commandType">指定如何解释命令字符串。默认执行SQL语句。</param>
        /// <returns>数据库操作DataCommande对象实例</returns>
        public static DataCommand GetDataCommand(string databaseName, string sqlCmd, List<DataOperationParameter> paramList, List<DataOperationParameterGroup> groupparamList, bool supportTran, CommandType commandType = CommandType.Text)
        {
            DataCommand command = GetDataOperationCommand(databaseName, sqlCmd, paramList, groupparamList, commandType);

            command.SupportTransaction = supportTran;

            return command;
        }

        /// <summary>
        /// 获取数据库操作DataCommande对象实例
        /// </summary>
        /// <param name="databaseName">配置在web.config的connectionStrings节点的的key值</param>
        /// <param name="sqlCmd">数据库执行SQL语句</param>
        /// <param name="paramList">SQL语句传入参数列表</param>
        /// <param name="groupparamList">SQL语句in参数列表</param>
        /// <param name="trans">数据库操作事务</param>
        /// <param name="commandType">指定如何解释命令字符串。默认执行SQL语句。</param>
        /// <returns>数据库操作DataCommande对象实例</returns>
        public static DataCommand GetDataCommand(string databaseName, string sqlCmd, List<DataOperationParameter> paramList, List<DataOperationParameterGroup> groupparamList, DbTransaction trans, CommandType commandType = CommandType.Text)
        {
            DataCommand command = GetDataOperationCommand(databaseName, sqlCmd, paramList, groupparamList, commandType);

            if (trans != null)
            {
                command.Transaction = trans;
            }

            return command;
        }

        /// <summary>
        /// 获取数据库操作DataCommande对象实例
        /// </summary>
        /// <param name="databaseName">配置在web.config的connectionStrings节点的的key值</param>
        /// <param name="sqlCmd">数据库执行SQL语句</param>
        /// <param name="paramList">SQL语句传入参数列表</param>
        /// <param name="groupparamList">SQL语句in参数列表</param>
        /// <param name="commandType">指定如何解释命令字符串。默认执行SQL语句。</param>
        /// <returns>数据库操作DataCommande对象实例</returns>
        private static DataCommand GetDataOperationCommand(string databaseName, string sqlCmd, List<DataOperationParameter> paramList, List<DataOperationParameterGroup> groupparamList, CommandType commandType = CommandType.Text)
        {
            DataCommand dataCommand = new DataCommand(databaseName, sqlCmd, commandType);

            if (paramList != null)
            {
                dataCommand.ParameterList = paramList;
                foreach (var dataOperationParameter in dataCommand.ParameterList)
                {
                    dataOperationParameter.DataCommand = dataCommand;
                }
            }

            if (groupparamList != null)
            {
                dataCommand.ParameterGroupCollection = groupparamList;

                foreach (var dataOperationParameterGroup in dataCommand.ParameterGroupCollection)
                {
                    dataOperationParameterGroup.DataCommand = dataCommand;
                }
            }

            return dataCommand;
        }  
    }
}