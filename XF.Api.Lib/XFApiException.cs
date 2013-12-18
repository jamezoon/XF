using System;
using System.Web;
using System.Text;
using System.Runtime.Serialization;

namespace XF.Api.Lib
{
    /// <summary>
    /// TOP客户端异常。
    /// </summary>
    public class XFApiException : Exception
    {
        #region 构造函数

        public XFApiException()
            : base()
        {
            Init();
        }

        public XFApiException(string message)
            : base(message)
        {
            this.errorMsg = message;
            Init();
        }

        protected XFApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Init();
        }

        public XFApiException(string message, Exception innerException)
            : base(message, innerException)
        {
            Init();
        }

        #endregion

        /// <summary>
        /// 错误信息
        /// </summary>
        private string errorMsg;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg
        {
            get { return this.errorMsg; }
            set { this.ErrorMsg = value; }
        }

        /// <summary>
        /// 原始请求地址
        /// </summary>
        public string RequestRawUrl { get; set; }

        /// <summary>
        /// POST Form数据
        /// </summary>
        public StringBuilder RequestFormData { get; set; }

        /// <summary>
        /// POST 流数据
        /// </summary>
        public StringBuilder RequestStreamData { get; set; }

        /// <summary>
        /// 头部信息
        /// </summary>
        public StringBuilder RequestHeader { get; set; }

        void Init()
        {
            //Http请求会话
            HttpContext httpContext = System.Web.HttpContext.Current;

            if (httpContext != null)
            {
                //请求地址，带有QueryString
                this.RequestRawUrl = HttpUtility.UrlDecode(httpContext.Request.RawUrl);

                //Form中的数据
                this.RequestFormData = new StringBuilder();

                foreach (var key in httpContext.Request.Form.AllKeys)
                {
                    this.RequestFormData.AppendFormat("{0}:{1},", key, httpContext.Request.Form[key]);
                }

                #region 流中的数据

                this.RequestStreamData = new StringBuilder();

                if (httpContext.Request.InputStream.CanRead)
                {
                    httpContext.Request.InputStream.Position = 0;

                    // Now read s into a byte buffer.
                    var bytes = new byte[httpContext.Request.InputStream.Length];

                    var numBytesToRead = httpContext.Request.InputStream.Length;

                    int numBytesReaded = 0;

                    while (numBytesToRead > 0)
                    {
                        // Read may return anything from 0 to 1024.
                        var readCount = httpContext.Request.InputStream.Read(bytes, numBytesReaded, 1024);

                        // The end of the file is reached.
                        if (readCount == 0)
                        {
                            break;
                        }

                        numBytesReaded += readCount;
                        numBytesToRead -= readCount;
                    }

                    this.RequestStreamData.Append(httpContext.Request.ContentEncoding.GetString(bytes));
                }

                #endregion

                //头信息
                this.RequestHeader = new StringBuilder();

                foreach (var key in httpContext.Request.Headers.AllKeys)
                {
                    this.RequestHeader.AppendFormat("{0}:{1},", key, httpContext.Request.Headers[key]);
                }
            }
        }

        public override string ToString()
        {
            return "ErrorMsg:" + ErrorMsg + "," + Environment.NewLine
                + "RequestRawUrl:" + RequestRawUrl + "," + Environment.NewLine
                + "RequestFormData:" + RequestFormData + "," + Environment.NewLine
                + "RequestStreamData:" + RequestStreamData + "," + Environment.NewLine
                + "RequestHeader:" + RequestHeader + "," + Environment.NewLine
                + "Message:" + base.Message + "," + Environment.NewLine
                + "StackTrace:" + base.StackTrace;
        }
    }
}
