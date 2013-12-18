using System.Web;

using XF.Api.Lib;

namespace XF.Api.Core.Authenticator
{
    internal class HttpBasicAuthenticator : HttpAuthenticator
    {
        public void Authenticate(RequestAuthInfoEntity requestAuthInfo, HttpRequestBase request)
        {
            requestAuthInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.SUCCESS;
            requestAuthInfo.AuthResultMsg = "仅限测试期使用";
        }
    }
}
