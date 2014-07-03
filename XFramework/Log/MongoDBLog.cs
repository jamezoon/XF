using System.Configuration;
using MongoDB.Driver;

namespace XFramework.Log
{
    /// <summary>
    /// MongoDB记录日志
    /// </summary>
    public class MongoDBLog : ILog
    {
        /// <summary>
        /// MongoDB数据库连接字符串
        /// </summary>
        private const string ConnectionStringKey = "XFrameworkMongoDBLog";

        /// <summary>
        /// MongoDB数据库记录日志的数据库名
        /// </summary>
        private const string DataBaseKey = "XFrameworkMongoDBLog.DataBaseName";

        /// <summary>
        /// MongoDB数据库记录Error日志表名
        /// </summary>
        private const string TableNameKey_Error = "XFrameworkMongoDBLog.TableName.Error";
        
        /// <summary>
        /// MongoDB数据库记录Debug日志表名
        /// </summary>
        private const string TableNameKey_Debug = "XFrameworkMongoDBLog.TableName.Debug";

        /// <summary>
        /// MongoDB数据库记录Warn日志表名
        /// </summary>
        private const string TableNameKey_Warn = "XFrameworkMongoDBLog.TableName.Warn";

        /// <summary>
        /// MongoDB数据库记录Fatal日志表名
        /// </summary>
        private const string TableNameKey_Fatal = "XFrameworkMongoDBLog.TableName.Fatal";

        /// <summary>
        /// MongoDB数据库记录Info日志表名
        /// </summary>
        private const string TableNameKey_Info = "XFrameworkMongoDBLog.TableName.Info";

        /// <summary>
        /// MongoDB记录日志
        /// </summary>
        /// <param name="logEntity">日志信息实体</param>
        public void Log(LogEntity logEntity)
        {
            if (logEntity == null)
                return;

            string collectionName = string.Empty;

            switch (logEntity.Level)
            {
                case LogLevel.Debug:
                    collectionName = ConfigurationManager.AppSettings[TableNameKey_Debug];
                    break;
                case LogLevel.Warn:
                    collectionName = ConfigurationManager.AppSettings[TableNameKey_Warn];
                    break;
                case LogLevel.Fatal:
                    collectionName = ConfigurationManager.AppSettings[TableNameKey_Fatal];
                    break;
                case LogLevel.Info:
                    collectionName = ConfigurationManager.AppSettings[TableNameKey_Info];
                    break;
                default:
                    collectionName = ConfigurationManager.AppSettings[TableNameKey_Error];
                    break;
            }

            //mongodb数据库连接字符串
            //mongodb://username:password@10.0.33.14:27017
            string connStr = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;

            MongoClient client = new MongoClient(connStr);

            MongoServer servers = client.GetServer();

            MongoDatabase db = servers.GetDatabase(ConfigurationManager.AppSettings[DataBaseKey]);

            MongoCollection<LogEntity> collection = db.GetCollection<LogEntity>(collectionName);

            collection.Insert(logEntity);
        }
    }
}
