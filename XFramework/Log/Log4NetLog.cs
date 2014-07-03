using log4net.Appender;

namespace XFramework.Log
{
    /// <summary>
    /// Log4Net记录日志
    /// </summary>
    public class Log4NetLog : ILog
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        static Log4NetLog()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// Log4Net记录日志
        /// </summary>
        /// <param name="logEntity">日志实体信息</param>
        public void Log(LogEntity logEntity)
        {
            if (logEntity == null)
                return;

            var logger = log4net.LogManager.GetLogger("index");

            switch (logEntity.Level)
            {
                case LogLevel.Debug:
                    logger.Debug(logEntity);
                    break;

                case LogLevel.Warn:
                    logger.Warn(logEntity);
                    break;

                case LogLevel.Error:
                    logger.Error(logEntity);
                    break;

                case LogLevel.Fatal:
                    logger.Fatal(logEntity);
                    break;

                case LogLevel.Info:
                    logger.Info(logEntity);
                    break;

                default:
                    logger.Error(logEntity);
                    break;
            }
        }
    }
}
