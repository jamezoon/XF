using System.Data;

namespace XFramework.Data
{
    /// <summary>
    /// XFramework���ݿ������SQL��䴫�����
    /// </summary>
    public class DataOperationParameter
    {
        /// <summary>
        /// SQL��䴫���������������
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// SQL��䴫������Ĳ�������
        /// </summary>
        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// SQL��䴫������Ĳ�����С
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// SQL��䴫������Ĳ���ֵ
        /// </summary>
        public object ParamValue { get; set; }

        /// <summary>
        /// XFramework���ݿ����Commandʵ��
        /// </summary>
        public DataCommand DataCommand { get; set; }

        /// <summary>
        /// SQL��䴫���������
        /// </summary>
        private string m_ParamName;

        /// <summary>
        /// SQL��䴫���������
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
        /// SQL��䴫������������캯��
        /// </summary>
        public DataOperationParameter()
        {
            this.Direction = ParameterDirection.Input;
            this.Size = -1;
        }

        /// <summary>
        /// SQL��䴫������������캯��
        /// </summary>
        /// <param name="paramName">SQL��䴫���������</param>
        /// <param name="paramType">SQL��䴫���������</param>
        /// <param name="paramValue">SQL��䴫�����ֵ</param>
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
        /// SQL��䴫������������캯��
        /// </summary>
        /// <param name="paramName">SQL��䴫�����������</param>
        /// <param name="paramType">SQL��䴫���������������</param>
        /// <param name="paramValue">SQL��䴫������Ĳ���ֵ</param>
        /// <param name="paramDirection">SQL��䴫������Ĳ�������</param>
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
        /// SQL��䴫������������캯��
        /// </summary>
        /// <param name="paramName">SQL��䴫�����������</param>
        /// <param name="paramType">SQL��䴫���������������</param>
        /// <param name="size">SQL��䴫������Ĳ�����С</param>
        /// <param name="paramValue">SQL��䴫������Ĳ���ֵ</param>
        /// <param name="paramDirection">SQL��䴫������Ĳ�������</param>
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
        /// ��ȡ���������SQL�ű��ı�ʾ��ʽ��SQLServerʹ��(@)���ţ�Oracleʹ��(:)����
        /// </summary>
        /// <param name="dataCommand">XFramework���ݿ����Commandʵ��</param>
        /// <returns>���������SQL�ű��ı�ʾ��ʽ��SQLServerʹ��(@)���ţ�Oracleʹ��(:)����</returns>
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
