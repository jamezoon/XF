using System;
using System.Web;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using XF.Api.Lib;
using XFramework;
using XFramework.Util;
using XFramework.Safe;

namespace XF.Api.Core.Authenticator
{
    public class HttpToken2Authenticator : HttpAuthenticator
    {
        const string APP_SETTING_KEY = "AuthConfigFilePath";

        public void Authenticate(RequestAuthInfoEntity requestAuthInfo, HttpRequestBase request)
        {
            HttpToken2AuthInfoEntity token2ReqAuthInfo = new HttpToken2AuthInfoEntity() { AppKey = requestAuthInfo.AppKey, AppMethed = requestAuthInfo.AppMethed };

            #region 读取需要验证的元素

            //用户名
            string userName = request.Headers[HttpToken2AuthInfoEntity.URL_QUERYSTRING_MOLAUSER];

            if (string.IsNullOrWhiteSpace(userName)) throw new XFApiException("请求Api的用户名为空。");

            token2ReqAuthInfo.UserName = HttpUtility.UrlDecode(userName);

            //签名
            string userSign = request.Headers[HttpToken2AuthInfoEntity.URL_QUERYSTRING_SIGN];

            if (string.IsNullOrWhiteSpace(userSign)) throw new XFApiException("请求Api的用户签名为空。");

            token2ReqAuthInfo.Sign = HttpUtility.UrlDecode(userSign);

            //时间戳
            string timeStamp = request.Headers[HttpToken2AuthInfoEntity.URL_QUERYSTRING_TIMESTAMP];

            if (string.IsNullOrWhiteSpace(timeStamp)) throw new XFApiException("请求Api的时间戳为空。");

            DateTime dtTimeStamp = DateTime.Now;
            if (!DateTime.TryParseExact(
                timeStamp,
                HttpToken2AuthInfoEntity.TimeStampFormat,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out dtTimeStamp))
                throw new XFApiException("请求Api的时间戳不合法。");

            ////时间判断，如果请求时间过早则拒绝该请求
            //if (dtTimeStamp < DateTime.Now.AddSeconds(0 - 60))
            //{
            //    throw new MolaApiException(RequestErrorCode.TimestampExpire, RequestErrorMessage.TimestampExpire);
            //}

            token2ReqAuthInfo.Timestamp = timeStamp;

            //GUID
            string guid = request.Headers[HttpToken2AuthInfoEntity.URL_QUERYSTRING_GUID];

            if (string.IsNullOrWhiteSpace(guid)) throw new XFApiException("请求Api的Guid为空。");

            //验证GUID存在，则认为是重复请求；
            //此判断可以根据业务再添加

            token2ReqAuthInfo.Guid = guid;

            #endregion

            #region 读取服务器中配置的认证信息

            ApiServiceTokenEntity token2ServerAuthInfo = ReadAuthConfigInfo(requestAuthInfo);

            if (token2ServerAuthInfo == null) return;

            #endregion

            #region 验证

            CheckRequestAuthInfo(token2ReqAuthInfo, token2ServerAuthInfo);

            requestAuthInfo.AuthResult = token2ReqAuthInfo.AuthResult;
            requestAuthInfo.AuthResultMsg = token2ReqAuthInfo.AuthResultMsg;

            #endregion
        }

        void CheckRequestAuthInfo(HttpToken2AuthInfoEntity authInfo, ApiServiceTokenEntity serverAuthInfo)
        {
            //检查用户名
            if (serverAuthInfo.UserName != authInfo.UserName)
            {
                authInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.FAILED;
                authInfo.AuthResultMsg = "用户名错误";
                return;
            }

            //解密请求中的签名
            string requestMd5 = Aes.Decrypt(authInfo.Sign, serverAuthInfo.Password);

            string responseMD5 = MD5.Encrypt(string.Join("^-_-^", serverAuthInfo.UserName, serverAuthInfo.Password, authInfo.AppKey, authInfo.AppMethed, authInfo.Timestamp, authInfo.Guid));

            if (requestMd5 != responseMD5)
            {
                authInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.FAILED;
                authInfo.AuthResultMsg = "签名验证失败";
                return;
            }

            string clientIp = Misc.IPAddr;

            if (serverAuthInfo.ValidIP == "*")
            {
                authInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.SUCCESS;
                return;
            }

            //检查IP
            foreach (string ip in serverAuthInfo.ValidIP.Split(';'))
            {
                if (ip == clientIp)
                {
                    authInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.SUCCESS;
                    return;
                }
            }

            authInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.FAILED;

            authInfo.AuthResultMsg = "IP地址未被授权";
        }

        /// <summary>
        /// 从配置文件中读取Api认证信息
        /// </summary>
        /// <param name="reqAuthInfo"></param>
        /// <returns></returns>
        ApiServiceTokenEntity ReadAuthConfigInfo(RequestAuthInfoEntity reqAuthInfo)
        {
            var serverAuthInfo = ApiService.Instance.GetServiceTokenEntity(reqAuthInfo.AppKey);

            if (serverAuthInfo == null)
            {
                LogUtil.Log("ReadAuthConfigInfo", "未在数据库中读取验证信息，AppKey=" + reqAuthInfo.AppKey, XFramework.Log.LogLevel.Warn);

                #region 读配置文件

                //读取appsetting节点内容
                var cfgFilePath = System.Configuration.ConfigurationManager.AppSettings[APP_SETTING_KEY];

                if (string.IsNullOrWhiteSpace(cfgFilePath))
                {
                    reqAuthInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.SERVER_CONFIG_ERROR;
                    reqAuthInfo.AuthResultMsg = "服务端未配置认证文件路径";
                    return null;
                }

                //生成配置文件具体路径
                cfgFilePath = System.Web.HttpContext.Current.Server.MapPath(
                    System.IO.Path.Combine("~", cfgFilePath));

                //加载XML文件
                var doc = new XmlDocument();

                try
                {
                    doc.Load(cfgFilePath);
                }
                catch
                {
                    reqAuthInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.SERVER_CONFIG_ERROR;
                    reqAuthInfo.AuthResultMsg = "服务端认证配置有误或该配置文件不存在";
                    return null;
                }

                //读取对应的api节点
                var apiNode = doc.SelectSingleNode("./UserAccessControls/UserAccess[@ApiKey='" + reqAuthInfo.AppKey + "']");

                if (apiNode == null)
                {
                    reqAuthInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.SERVER_CONFIG_ERROR;
                    reqAuthInfo.AuthResultMsg = "服务端认证配置中未找到该Api的配置项";
                    return null;
                }

                serverAuthInfo = new ApiServiceTokenEntity();

                serverAuthInfo.ServiceKey = reqAuthInfo.AppKey;

                //配置节点中的用户名           
                if (apiNode.Attributes["UserName"] == null)
                {
                    reqAuthInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.SERVER_CONFIG_ERROR;
                    reqAuthInfo.AuthResultMsg = "服务端认证配置中未找到该Api的用户名配置项";
                    return null;
                }
                serverAuthInfo.UserName = apiNode.Attributes["UserName"].Value;


                //配置节点中的密码          
                if (apiNode.Attributes["Password"] == null)
                {
                    reqAuthInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.SERVER_CONFIG_ERROR;
                    reqAuthInfo.AuthResultMsg = "服务端认证配置中未找到该Api的密码配置项";
                    return null;
                }
                serverAuthInfo.Password = apiNode.Attributes["Password"].Value;

                var ipParentNode = apiNode.SelectSingleNode("AccessList");
                if (ipParentNode == null)
                {
                    reqAuthInfo.AuthResult = RequestAuthInfoEntity.AuthResultCode.SERVER_CONFIG_ERROR;
                    reqAuthInfo.AuthResultMsg = "服务端认证配置中未找到该Api的IP地址配置项";
                    return null;
                }

                serverAuthInfo.ValidIP = "";

                #region ip list
                foreach (XmlNode ipnode in ipParentNode.ChildNodes)
                {
                    try
                    {
                        string ip = ipnode.Attributes["AccessIp"].Value;

                        serverAuthInfo.ValidIP = string.Join(ApiServiceTokenEntity.IpSpilter, serverAuthInfo.ValidIP, ip);
                    }
                    catch
                    {
                    }
                }
                #endregion
                #endregion
            }

            return serverAuthInfo;
        }
    }
}
