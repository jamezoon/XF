using System.Web;

using XF.Api.Lib;

namespace XF.Api.Core.Authenticator
{
    internal class HttpTokenAuthenticator : HttpAuthenticator
    {
        public void Authenticate(RequestAuthInfoEntity requestAuthInfo, HttpRequestBase request)
        {
            requestAuthInfo.AuthResult = 0;
            requestAuthInfo.AuthResultMsg = "该认证模式尚未实现";
        }
    }
}
