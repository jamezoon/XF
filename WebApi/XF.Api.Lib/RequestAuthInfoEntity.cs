using System;

namespace XF.Api.Lib
{
    [Serializable]
    public class RequestAuthInfoEntity
    {
        /// <summary>
        /// 认证结果代码类
        /// </summary>
        public class AuthResultCode
        {
            public const int SUCCESS = 1;
            public const int FAILED = 0;
            public const int SERVER_CONFIG_ERROR = 2;
        }

        public const string URL_QUERYSTRING_DATA = "data";

        public string AuthType { get; set; }

        public const string URL_QUERYSTRING_AUTHTYPE = "authtype";

        public string AppKey { get; set; }

        public const string URL_QUERYSTRING_APPKEY = "servicekey";

        public string AppMethed { get; set; }

        public const string URL_QUERYSTRING_APPMETHOD = "servicemethed";

        /// <summary>
        /// 认证结果（0失败，1成功）
        /// </summary>
        public int AuthResult { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string AuthResultMsg { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
