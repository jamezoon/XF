using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace XFramework.Log
{
    /// <summary>
    /// XFramework获取设置的配置文件
    /// 如：
    /// <XFrameworkLog type="XFramework.Log.Log4NetLog,XFramework" appname="XFramework.Web"/>
    /// </summary>
    public class LogHandler : ConfigurationSection
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LogHandler()
        {
        }

        public LogHandler(string type)
            : this(type, "未指定")
        {

        }

        public LogHandler(string type, string appname)
        {
            Type = type;
            AppName = appname;
        }

        [ConfigurationProperty("type", DefaultValue = "XFramework.Log.Log4NetLog,XFramework", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\")]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }

            set
            {
                this["type"] = value;
            }
        }

        [ConfigurationProperty("appname", DefaultValue = "未指定", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\")]
        public string AppName
        {
            get
            {
                return (string)this["appname"];
            }

            set
            {
                this["appname"] = value;
            }
        }
    }
}
