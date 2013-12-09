using System;
using System.Text;

using MongoDB.Bson;

namespace XFramework.Log
{
    /// <summary>
    /// XFramework日志实体信息
    /// </summary>
    public class LogEntity
    {
        /// <summary>
        /// 自带编号
        /// </summary>
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public ObjectId _id { get; set; }

        /// <summary>
        /// 记录日志的项目名称，配置于web.config中
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 日志标题（错误日志一般为出错的方法名）
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 详细日志信息
        /// </summary>
        public string LogMsg { get; set; }

        /// <summary>
        /// 日志记录时间
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// 进程名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        /// URL跳转来源
        /// </summary>
        public string UrlReferrer { get; set; }

        /// <summary>
        /// 原始URL地址
        /// </summary>
        public string RawUrl { get; set; }

        /// <summary>
        /// 日志各项之间的连接符
        /// </summary>
        private string spilt;

        /// <summary>
        /// 日志各项之间的连接符
        /// </summary>
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public string Spilt
        {
            get
            {
                if (string.IsNullOrEmpty(spilt))
                {
                    spilt = Environment.NewLine;
                }

                return spilt;
            }

            set
            {
                spilt = value;
            }
        }

        /// <summary>
        /// 完整的客户端IP
        /// </summary>
        public string FullClientIp { get; set; }

        /// <summary>
        /// 默认的日志级别
        /// </summary>
        LogLevel defaultLevel = LogLevel.Error;

        /// <summary>
        /// 日志级别
        /// </summary>
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public LogLevel Level
        {
            get
            {
                return defaultLevel;

            }
            set
            {
                defaultLevel = value;
            }
        }

        /// <summary>
        /// 重写ToString()，自定义格式
        /// </summary>
        /// <returns>自定义格式的字符串</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("AppName={0}", AppName); sb.Append(Spilt);
            sb.AppendFormat("LogTime={0}", LogTime); sb.Append(Spilt);
            sb.AppendFormat("Title={0}", Title); sb.Append(Spilt);
            sb.AppendFormat("Message={0}", LogMsg); sb.Append(Spilt);
            sb.AppendFormat("ProcessName={0}", ProcessName); sb.Append(Spilt);
            sb.AppendFormat("ClientIp={0}", ClientIp); sb.Append(Spilt);
            sb.AppendFormat("ClientIp2={0}", FullClientIp); sb.Append(Spilt);
            sb.AppendFormat("ServerIp={0}", ServerIp); sb.Append(Spilt);
            sb.AppendFormat("UrlReferrer={0}", UrlReferrer); sb.Append(Spilt);
            sb.AppendFormat("RawUrl={0}", RawUrl); sb.Append(Spilt);

            return sb.ToString();
        }
    }
}
