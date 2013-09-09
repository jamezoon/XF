using System.Data;

namespace XFramework.Data
{
    /// <summary>
    /// XFramework数据库操作，SQL语句传入参数
    /// </summary>
    public class DataOperationParameter
    {
        /// <summary>
        /// SQL语句传入参数的数据类型
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// SQL语句传入参数的参数类型
        /// </summary>
        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// SQL语句传入参数的参数大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// SQL语句传入参数的参数值
        /// </summary>
        public object ParamValue { get; set; }

        /// <summary>
        /// XFramework数据库操作Command实例
        /// </summary>
        public DataCommand DataCommand { get; set; }

        /// <summary>
        /// SQL语句传入参数名称
        /// </summary>
        private string m_ParamName;

        /// <summary>
        /// SQL语句传入参数名称
        /// </summary>
        public string ParamName
        {
            get
            {
                return BuildKey(DataCommand) + m_ParamName;
            }

            set
            {
                m_ParamName = value;
            }
        }

        /// <summary>
        /// SQL语句传入参数操作构造函数
        /// </summary>
        public DataOperationParameter()
        {
            this.Direction = ParameterDirection.Input;
            this.Size = -1;
        }

        /// <summary>
        /// SQL语句传入参数操作构造函数
        /// </summary>
        /// <param name="paramName">SQL语句传入参数名称</param>
        /// <param name="paramType">SQL语句传入参数类型</param>
        /// <param name="paramValue">SQL语句传入参数值</param>
        public DataOperationParameter(string paramName, DbType paramType, object paramValue)
        {
            this.ParamValue = paramValue;
            m_ParamName = paramName;
            this.DbType = paramType;
            this.Direction = ParameterDirection.Input;
            this.Size = -1;

            //wsh added
            if (paramType == DbType.String)
            {
                this.ParamValue = paramValue ?? string.Empty;
            }
        }

        /// <summary>
        /// SQL语句传入参数操作构造函数
        /// </summary>
        /// <param name="paramName">SQL语句传入参数的名称</param>
        /// <param name="paramType">SQL语句传入参数的数据类型</param>
        /// <param name="paramValue">SQL语句传入参数的参数值</param>
        /// <param name="paramDirection">SQL语句传入参数的参数类型</param>
        public DataOperationParameter(string paramName, DbType paramType, object paramValue, ParameterDirection paramDirection)
        {
            this.ParamValue = paramValue;
            m_ParamName = paramName;
            this.DbType = paramType;
            this.Direction = paramDirection;
            this.Size = -1;

            //wsh added
            if (paramType == DbType.String)
                this.ParamValue = paramValue ?? string.Empty;
        }

        /// <summary>
        /// SQL语句传入参数操作构造函数
        /// </summary>
        /// <param name="paramName">SQL语句传入参数的名称</param>
        /// <param name="paramType">SQL语句传入参数的数据类型</param>
        /// <param name="size">SQL语句传入参数的参数大小</param>
        /// <param name="paramValue">SQL语句传入参数的参数值</param>
        /// <param name="paramDirection">SQL语句传入参数的参数类型</param>
        public DataOperationParameter(string paramName, DbType paramType, int size, object paramValue, ParameterDirection paramDirection)
        {
            this.ParamValue = paramValue;
            m_ParamName = paramName;
            this.DbType = paramType;
            this.Direction = paramDirection;
            Size = size;

            if (paramType == DbType.String)
                this.ParamValue = paramValue ?? string.Empty;
        }

        /// <summary>
        /// 获取传入参数在SQL脚本的标示方式，SQLServer使用(@)符号，Oracle使用(:)符号
        /// </summary>
        /// <param name="dataCommand">XFramework数据库操作Command实例</param>
        /// <returns>传入参数在SQL脚本的标示方式，SQLServer使用(@)符号，Oracle使用(:)符号</returns>
        public static string BuildKey(DataCommand dataCommand)
        {
            switch (dataCommand.ProviderName)
            {
                case "System.Data.SqlClient":
                    return "@";
                case "Oracle.DataAccess.Client":
                    return ":";
                case "System.Data.OracleClient":
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
