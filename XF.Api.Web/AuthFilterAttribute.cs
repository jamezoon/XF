using System.Web.Mvc;

using XF.Api.Lib;
using XF.Api.Core.Authenticator;
using XFramework.Safe;

namespace Mola.Api.Web
{
    public class AuthFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region 从客户端请求中提取基本信息

            //请求的api
            string serviceKey = filterContext.ActionParameters[RequestAuthInfoEntity.URL_QUERYSTRING_APPKEY].ToString();

            if (string.IsNullOrWhiteSpace(serviceKey)) throw new XFApiException("请求Api的Key值为空。");

            //请求的方法
            string serviceMethed = filterContext.ActionParameters[RequestAuthInfoEntity.URL_QUERYSTRING_APPMETHOD].ToString();

            if (string.IsNullOrWhiteSpace(serviceMethed)) throw new XFApiException("请求Api的方法名称为空。");

            //验证方式
            string authType = filterContext.HttpContext.Request.GetValue(RequestAuthInfoEntity.URL_QUERYSTRING_AUTHTYPE);

            if (string.IsNullOrWhiteSpace(authType)) throw new XFApiException("请求Api的验证方式为空。");

            #endregion

            RequestAuthInfoEntity requestAuthInfo = new RequestAuthInfoEntity()
            {
                AppKey = serviceKey,
                AppMethed = serviceMethed,
                AuthType = authType,
                AuthResult = RequestAuthInfoEntity.AuthResultCode.FAILED,
                AuthResultMsg = string.Empty
            };

            //验证客户端信息
            HttpAuthorizationDispatcher.Authenticate(requestAuthInfo, filterContext.HttpContext.Request);

            if (requestAuthInfo.AuthResult != RequestAuthInfoEntity.AuthResultCode.SUCCESS) throw new XFApiException("请求Api认证失败");

            base.OnActionExecuting(filterContext);
        }
    }
}