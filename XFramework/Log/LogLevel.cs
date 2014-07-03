using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFramework.Log
{
    /// <summary>
    /// 日志的级别枚举
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 一般调试信息，正式上线后的程序不建议使用该等级
        /// </summary>
        Debug = 1,

        /// <summary>
        /// 警告信息，不影响程序正常运行
        /// </summary>
        Warn = 2,

        /// <summary>
        /// 错误信息，该错误影响范围仅限本功能
        /// </summary>
        Error = 3,

        /// <summary>
        /// 严重错误信息，整个程序已不能使用
        /// </summary>
        Fatal = 4,

        /// <summary>
        /// 严重错误信息，整个程序已不能使用
        /// </summary>
        Info = 5
    }
}
