using System;
using System.Reflection;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.Net;
using System.Linq;
using System.Management;
using XFramework.Util;

namespace XFramework.Log
{
    /// <summary>
    /// XFramework日志操作
    /// </summary>
    public class LogUtil
    {
        /// <summary>
        /// 写日志的接口
        /// </summary>
        private static ILog logInstance;

        /// <summary>
        /// 记录日志的程序名称
        /// </summary>
        private static string appName = string.Empty;

        /// <summary>
        /// 记录日志的进程名称
        /// </summary>
        private static string processName = string.Empty;

        /// <summary>
        /// 记录日志的服务器IP地址
        /// </summary>
        private static string serverIP = string.Empty;

        /// <summary>
        /// 初始化LogUtil操作
        /// </summary>
        static LogUtil()
        {
            try
            {
                //从配置文件web.config中获取程序设置是使用哪种方式记录日志，Log4Net或者MongoDB
                var config = System.Configuration.ConfigurationManager.GetSection("XFrameworkLog");

                if (config != null)
                {
                    LogHandler logConfig = config as LogHandler;

                    if (logConfig != null && !string.IsNullOrWhiteSpace(logConfig.Type))
                    {
                        appName = logConfig.AppName;

                        Type logType = Type.GetType(logConfig.Type);

                        if (logType != null)
                            logInstance = Activator.CreateInstance(logType) as ILog;
                    }
                }
            }
            catch { }
            finally
            {
                if (logInstance == null)
                    logInstance = new Log4NetLog();
            }

            //获取当前活动的进程
            Process process = Process.GetCurrentProcess();

            processName = GetProcessUserName(process.Id);

            //获取主机IP解析地址
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());

            if (ipHost != null && ipHost.AddressList.Length > 1)
            {
                IPAddress ipAddr = ipHost.AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                if (ipAddr != null)
                    serverIP = ipAddr.ToString();
            }
        }

        /// <summary>
        /// 获取程序运行的进程名称
        /// </summary>
        /// <param name="pid">进程号</param>
        /// <returns>程序运行的进程名称</returns>
        private static string GetProcessUserName(int pid)
        {
            string username = "SYSTEM";

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from win32_process where processid=" + pid);

                foreach (ManagementObject disk in searcher.Get())
                {
                    ManagementBaseObject inPar = null;
                    ManagementBaseObject outPar = null;

                    inPar = disk.GetMethodParameters("GetOwner");

                    outPar = disk.InvokeMethod("GetOwner", inPar, null);

                    username = outPar["User"].ToString();

                    break;
                }
            }
            catch { }

            return username;
        }

        /// <summary>
        /// 获取程序异常信息，拼接成字符串保存在日志信息中
        /// </summary>
        /// <param name="ex">程序异常信息</param>
        /// <returns>程序异常信息</returns>
        private static string GetExceptionMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Message:");
            sb.Append(ex.Message);
            sb.Append(Environment.NewLine);
            sb.Append("TargetSite:");
            sb.Append(ex.TargetSite);
            sb.Append(Environment.NewLine);
            sb.Append("Data:");

            foreach (var key in ex.Data.Keys)
                sb.Append(key + "|" + ex.Data[key] + ";");

            sb.Append(Environment.NewLine);
            sb.Append("StackTrace:");
            sb.Append(ex.StackTrace);
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

        /// <summary>
        /// 启动log4net组件（配置位于web.config中）
        /// </summary>
        public static void StartLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// 记录程序异常日志
        /// </summary>
        /// <param name="title">日志标题</param>
        /// <param name="ex">程序异常信息</param>
        /// <param name="level">日志级别，默认错误信息，LogLevel.Error</param>
        public static void Log(string title, Exception ex, LogLevel level = LogLevel.Error)
        {
            if (string.IsNullOrWhiteSpace(title))
                title = ex.Message;

            string logMsg = GetExceptionMessage(ex);

            Log(title, logMsg, level);
        }

        /// <summary>
        /// 记录程序异常日志
        /// </summary>
        /// <param name="title">日志标题</param>
        /// <param name="logMsg">日志内容</param>
        /// <param name="level">日志级别，默认错误信息，LogLevel.Error</param>
        public static void Log(string title, string logMsg, LogLevel level = LogLevel.Error)
        {
            var logEntity = new LogEntity()
            {
                AppName = appName,
                LogMsg = logMsg,
                LogTime = DateTime.Now,
                ProcessName = processName,
                ServerIp = serverIP,
                Title = title,
                Level = level
            };

            if (HttpContext.Current != null)
            {
                try
                {
                    logEntity.ClientIp = Misc.IPAddr;
                    logEntity.FullClientIp = Misc.GetIPAddr(true);
                    logEntity.UrlReferrer = StringUtils.ToString(HttpContext.Current.Request.UrlReferrer);
                    logEntity.RawUrl = HttpContext.Current.Request.RawUrl;
                }
                catch { }
            }

            logInstance.Log(logEntity);
        }
    }
}
