using System.Web;

using XF.Api.Lib;

namespace XF.Api.Core.Authenticator
{
    /// <summary>
    /// HTTP验证调度器
    /// </summary>
    public class HttpAuthorizationDispatcher
    {
        public static void Authenticate(RequestAuthInfoEntity reqAuthInfo, HttpRequestBase request)
        {
            HttpAuthenticator auther = null;

            switch (reqAuthInfo.AuthType.ToUpper())
            {
                case "BASIC":
                    auther = new HttpBasicAuthenticator();
                    break;

                case "TOKEN":
                    auther = new HttpTokenAuthenticator();
                    break;

                case HttpToken2AuthInfoEntity.AUTH_TYPE_VALUE:
                    auther = new HttpToken2Authenticator();
                    break;
            }

            if (auther != null)
            {
                auther.Authenticate(reqAuthInfo, request);
            }
        }
    }
}
