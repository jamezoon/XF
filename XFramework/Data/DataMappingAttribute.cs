using System;
using System.Data;

namespace XFramework.Data
{
    /// <summary>
    /// XFramework����ʵ����Entity����
    /// </summary>
    public class DataMappingAttribute : Attribute
    {
        /// <summary>
        /// XFramework����ʵ����Entity���Թ��캯��
        /// </summary>
        /// <param name="columnName">Entityʵ�����������</param>
        /// <param name="dataType">Entityʵ���������������</param>
        public DataMappingAttribute(string columnName, DbType dataType)
            : this(columnName, dataType, null)
        {

        }

        /// <summary>
        /// XFramework����ʵ����Entity���Թ��캯��
        /// </summary>
        /// <param name="columnName">Entityʵ�����������</param>
        /// <param name="dataType">Entityʵ���������������</param>
        /// <param name="calculatorType">Entityʵ������Լ�������</param>
        public DataMappingAttribute(string columnName, DbType dataType, Type calculatorType)
        {
            this.ColumnName = columnName;
            this.DbType = dataType;
            this.CaculatorType = calculatorType;
        }

        /// <summary>
        /// Entityʵ�����������
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Entityʵ���������������
        /// </summary>
        public DbType DbType { get; private set; }

        /// <summary>
        /// Entityʵ������Լ�������
        /// </summary>
        public Type CaculatorType { get; set; }
    }
}
