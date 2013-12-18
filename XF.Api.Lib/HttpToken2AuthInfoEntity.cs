using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XF.Api.Lib
{
    public class HttpToken2AuthInfoEntity : RequestAuthInfoEntity
    {
        #region 外部常量

        /// <summary>
        /// 时间戳的格式
        /// </summary>
        public const string TimeStampFormat = "yyyyMMddHHmmss";

        /// <summary>
        /// 验证类型（全大写）
        /// </summary>
        public const string AUTH_TYPE_VALUE = "TOKEN2";

        #region 请求中的参数名称

        public const string URL_QUERYSTRING_MOLAUSER = "molauser";

        public string UserName { get; set; }

        public const string URL_QUERYSTRING_SIGN = "sign";

        public string Sign { get; set; }

        public const string URL_QUERYSTRING_TIMESTAMP = "timestamp";

        public string Timestamp { get; set; }

        public const string URL_QUERYSTRING_GUID = "guid";

        public string Guid { get; set; }

        #endregion

        #endregion
    }
}
