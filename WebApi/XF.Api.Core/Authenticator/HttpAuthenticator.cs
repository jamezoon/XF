using System.Web;

using XF.Api.Lib;

namespace XF.Api.Core.Authenticator
{
    /// <summary>
    /// 执行服务端身份验证
    /// </summary>
    public interface HttpAuthenticator
    {
        void Authenticate(RequestAuthInfoEntity reqAuthInfo, HttpRequestBase request);
    }
}
