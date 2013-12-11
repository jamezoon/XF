using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XFramework.Mvc
{
    /// <summary>
    /// 重写Asp.NetMVC日期格式的返回值
    /// </summary>
    public class DateTimeResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(ContentType))
                response.ContentType = ContentType;
            else
                response.ContentType = "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();

                timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";

                string json = JsonConvert.SerializeObject(Data, Formatting.None, timeConverter);

                response.Write(json);
                response.End();
            }
        }
    }
}
