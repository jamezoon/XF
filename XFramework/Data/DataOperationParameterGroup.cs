using System;
using System.Text;
using System.Collections.Generic;

namespace XFramework.Data
{
    /// <summary>
    /// XFramework数据库操作，SQL语句in参数数组
    /// </summary>
    public class DataOperationParameterGroup
    {
        /// <summary>
        /// XFramework数据库操作Command实例
        /// </summary>
        public DataCommand DataCommand { get; set; }

        /// <summary>
        /// SQL语句in参数值
        /// </summary>
        public string ParamValue { get; set; }

        /// <summary>
        /// SQL语句in参数名称
        /// </summary>
        private string m_ParamName;

        /// <summary>
        /// SQL语句in参数名称
        /// </summary>
        public string ParamName
        {
            get { return DataOperationParameter.BuildKey(DataCommand) + m_ParamName; }
            set { m_ParamName = value; }
        }

        /// <summary>
        /// SQL语句in参数数组操作构造函数
        /// </summary>
        /// <param name="paramName">SQL脚本in参数名称</param>
        /// <param name="paramValue">SQL脚本in参数数组</param>
        public DataOperationParameterGroup(string paramName, ICollection<string> paramValue)
        {
            this.ParamValue = string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (string s in paramValue)
                sb.AppendFormat("'{0}',", s);

            this.ParamValue = sb.ToString().TrimEnd(',');

            m_ParamName = paramName;
        }

        /// <summary>
        /// SQL语句in参数数组操作构造函数
        /// </summary>
        /// <param name="paramName">SQL脚本in参数名称</param>
        /// <param name="paramValue">SQL脚本in参数数组</param>
        public DataOperationParameterGroup(string paramName, IEnumerable<int> paramValue)
        {
            this.ParamValue = string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (int i in paramValue)
                sb.AppendFormat("{0},", i);

            this.ParamValue = sb.ToString().TrimEnd(',');

            m_ParamName = paramName;
        }

        /// <summary>
        /// SQL语句in参数数组操作构造函数
        /// </summary>
        /// <param name="paramName">SQL脚本in参数名称</param>
        /// <param name="paramValue">SQL脚本in参数数组</param>
        public DataOperationParameterGroup(string paramName, IEnumerable<long> paramValue)
        {
            this.ParamValue = string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (int i in paramValue)
                sb.AppendFormat("{0},", i);

            this.ParamValue = this.ParamValue.TrimEnd(',');

            m_ParamName = paramName;
        }

        /// <summary>
        /// SQL语句in参数数组操作构造函数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值，以逗号分隔不需要添加单引号</param>
        public DataOperationParameterGroup(string paramName, string paramValue)
        {
            if (!string.IsNullOrWhiteSpace(paramValue))
            {
                string[] pars = paramValue.Split(',');

                paramValue = "";

                for (int i = 0; i < pars.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(pars[i]))
                        continue;

                    paramValue += "'" + pars[i] + "',";
                }

                paramValue = paramValue.TrimEnd(',');
            }

            this.ParamValue = paramValue;
            m_ParamName = paramName;
        }
    }
}