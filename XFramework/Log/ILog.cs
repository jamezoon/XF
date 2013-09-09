namespace XFramework.Log
{
    /// <summary>
    /// XFramework记录日志接口
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logEntity"></param>
        void Log(LogEntity logEntity);
    }
}
