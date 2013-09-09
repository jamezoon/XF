using System;
using System.Text;
using System.Collections.Generic;

namespace XFramework.Data
{
    /// <summary>
    /// XFramework���ݿ������SQL���in��������
    /// </summary>
    public class DataOperationParameterGroup
    {
        /// <summary>
        /// XFramework���ݿ����Commandʵ��
        /// </summary>
        public DataCommand DataCommand { get; set; }

        /// <summary>
        /// SQL���in����ֵ
        /// </summary>
        public string ParamValue { get; set; }

        /// <summary>
        /// SQL���in��������
        /// </summary>
        private string m_ParamName;

        /// <summary>
        /// SQL���in��������
        /// </summary>
        public string ParamName
        {
            get { return DataOperationParameter.BuildKey(DataCommand) + m_ParamName; }
            set { m_ParamName = value; }
        }

        /// <summary>
        /// SQL���in��������������캯��
        /// </summary>
        /// <param name="paramName">SQL�ű�in��������</param>
        /// <param name="paramValue">SQL�ű�in��������</param>
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
        /// SQL���in��������������캯��
        /// </summary>
        /// <param name="paramName">SQL�ű�in��������</param>
        /// <param name="paramValue">SQL�ű�in��������</param>
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
        /// SQL���in��������������캯��
        /// </summary>
        /// <param name="paramName">SQL�ű�in��������</param>
        /// <param name="paramValue">SQL�ű�in��������</param>
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
        /// SQL���in��������������캯��
        /// </summary>
        /// <param name="paramName">��������</param>
        /// <param name="paramValue">����ֵ���Զ��ŷָ�����Ҫ��ӵ�����</param>
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