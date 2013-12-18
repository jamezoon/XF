using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

using XF.Api.Lib;
using XFramework.Mvc;
using XFramework.Util;
using XFramework.Entity;

namespace Mola.Api.Web
{
    public abstract class BaseController : Controller
    {
        protected override void OnException(ExceptionContext context)
        {
            if (context.Exception != null)
            {
                var serviceKey = context.RequestContext.RouteData.Values[RequestAuthInfoEntity.URL_QUERYSTRING_APPKEY];
                var serviceMethod = context.RequestContext.RouteData.Values[RequestAuthInfoEntity.URL_QUERYSTRING_APPMETHOD];

                LogUtil.Log(string.Format("请求Api异常，ServiceKey：{0}，ServiceMethod：{1}。", serviceKey, serviceMethod), context.Exception.ToString());

                //返回错误消息给客户端
                //application/json
                string contentType = ControllerContext.HttpContext.Request.ContentType;

                context.HttpContext.Response.Clear();
                context.HttpContext.Response.StatusCode = 500;
                context.ExceptionHandled = true;

                if (contentType == "application/json")
                {
                    SingleResult<string> result = new SingleResult<string>()
                    {
                        Data = string.Empty,
                        Success = false,
                        Message = context.Exception.Message
                    };

                    context.Result = Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    context.Result = Content(context.Exception.Message);
                }
            }

            base.OnException(context);
        }

        protected override JsonResult Json(object data, string contentType, Encoding encoding, JsonRequestBehavior behavior)
        {
            return new DateTimeResult { Data = data, ContentType = contentType, ContentEncoding = encoding, JsonRequestBehavior = behavior };
        }
    }
}