using System;
using System.Data;

namespace XFramework.Data
{
    /// <summary>
    /// XFramework构造实体类Entity属性
    /// </summary>
    public class DataMappingAttribute : Attribute
    {
        /// <summary>
        /// XFramework构造实体类Entity属性构造函数
        /// </summary>
        /// <param name="columnName">Entity实体的属性名称</param>
        /// <param name="dataType">Entity实体的属性数据类型</param>
        public DataMappingAttribute(string columnName, DbType dataType)
            : this(columnName, dataType, null)
        {

        }

        /// <summary>
        /// XFramework构造实体类Entity属性构造函数
        /// </summary>
        /// <param name="columnName">Entity实体的属性名称</param>
        /// <param name="dataType">Entity实体的属性数据类型</param>
        /// <param name="calculatorType">Entity实体的属性计算类型</param>
        public DataMappingAttribute(string columnName, DbType dataType, Type calculatorType)
        {
            this.ColumnName = columnName;
            this.DbType = dataType;
            this.CaculatorType = calculatorType;
        }

        /// <summary>
        /// Entity实体的属性名称
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Entity实体的属性数据类型
        /// </summary>
        public DbType DbType { get; private set; }

        /// <summary>
        /// Entity实体的属性计算类型
        /// </summary>
        public Type CaculatorType { get; set; }
    }
}
