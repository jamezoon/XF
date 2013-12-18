using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

using XF.Api.Lib;
using XFramework.Entity;
using XFramework.Safe;
using XFramework.Util;

namespace XF.Api.Lib
{
    public class DefaultClient
    {
        #region api相关的配置信息

        string apiKey;
        string apiUserName;
        string apiUserPwd;
        string apiUrl;

        const string AppSettingsItemName = "XFWebApiConfig";

        #endregion

        #region 构造函数

        private HttpWebUtils webUtils;

        public DefaultClient(string apikey)
            : this(apikey, null, null, null, null)
        {
        }

        public DefaultClient(string apikey, string apiurl, string user, string psw, int? timeout)
        {
            if (string.IsNullOrWhiteSpace(apikey))
            {
                throw new XFApiException("ApiKey Is Empty");
            }

            apiUrl = apiurl;
            apiKey = apikey;
            apiUserName = user;
            apiUserPwd = psw;

            if (string.IsNullOrEmpty(apiUserName)
                || string.IsNullOrWhiteSpace(apiUserPwd)
                || string.IsNullOrEmpty(apiUrl))
            {
                InitApiInfo();
            }

            webUtils = new HttpWebUtils();

            if (timeout.HasValue)
            {
                webUtils.Timeout = timeout.Value;
            }
        }

        #endregion

        #region 根据ApiKey读取其配置信息

        /// <summary>
        /// 根据ApiKey读取其配置信息
        /// </summary>
        void InitApiInfo()
        {
            //读取appsetting里的路径配置项
            string cfgFilePath = System.Configuration.ConfigurationManager.AppSettings[AppSettingsItemName];

            if (string.IsNullOrWhiteSpace(cfgFilePath))
            {
                throw new XFApiException("未能在AppSettings中找到名为[" + AppSettingsItemName + "]配置项");
            }

            //生成配置文件具体路径
            cfgFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cfgFilePath);

            //加载XML文件
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(cfgFilePath);
            }
            catch (Exception exp)
            {
                throw new XFApiException(exp.Message, exp.InnerException);
            }

            //读取对应的api节点
            XmlNode apiNode = doc.SelectSingleNode("./XFWebApiList/WebApi[@ApiKey='" + apiKey + "']");

            if (apiNode == null)
            {
                throw new XFApiException("未能在[" + cfgFilePath + "]中找到ApiKey=[" + apiKey + "]的配置节点");
            }

            this.apiUserName = ReadNodeAttr(apiNode, "UserName");
            this.apiUserPwd = ReadNodeAttr(apiNode, "Password");
            this.apiUrl = ReadNodeAttr(apiNode, "ApiURL");
        }

        private string ReadNodeAttr(XmlNode xmlNode, string attrName)
        {
            //配置节点中的用户名           
            if (xmlNode.Attributes[attrName] == null)
            {
                throw new XFApiException("未能在未能在[" + xmlNode.ToString() + "]中找到[" + attrName + "]的属性");
            }
            string result = xmlNode.Attributes[attrName].Value;

            if (string.IsNullOrWhiteSpace(result))
                throw new XFApiException(xmlNode.ToString() + "中[" + attrName + "]的属性值为空");

            return result;
        }

        #endregion

        #region 请求Api接口

        /// <summary>
        /// 发送Api请求
        /// </summary>
        /// <param name="req">得到返回值</param>
        /// <returns></returns>
        public SingleResult<string> Execute(XFRequest<SingleResult<string>> req)
        {
            return DoExecute<SingleResult<string>>(req, DateTime.Now);
        }

        /// <summary>
        /// 发送Api请求，且反序列化请求结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="req"></param>
        /// <returns></returns>
        public T ExecuteObject<T>(XFRequest<SingleResult<string>> req)
        {
            var res = Execute(req);

            return DeserializeObject<T>(res);
        }

        /// <summary>
        /// 反序列化请求结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        public T DeserializeObject<T>(SingleResult<string> res)
        {
            if (res == null) return default(T);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Newtonsoft.Json.JsonConvert.SerializeObject(res.Data));
        }

        /// <summary>
        /// 请求Api接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        T DoExecute<T>(XFRequest<T> request, DateTime timestamp) where T : SingleResult<string>
        {
            string apiMethod = request.GetApiMethod();

            if (string.IsNullOrWhiteSpace(apiMethod))
            {
                return CreateErrorResponse<T>("Api接口方法名为空");
            }

            // 添加协议级请求参数            
            string ts = timestamp.ToString(HttpToken2AuthInfoEntity.TimeStampFormat);
            string guid = Guid.NewGuid().ToString();

            IDictionary<string, string> header = new Dictionary<string, string>();

            string requestMD5 = MD5.Encrypt(string.Join("^-_-^", apiUserName, apiUserPwd, apiKey, apiMethod, ts, guid));

            header.Add(RequestAuthInfoEntity.URL_QUERYSTRING_AUTHTYPE, HttpToken2AuthInfoEntity.AUTH_TYPE_VALUE);
            header.Add(HttpToken2AuthInfoEntity.URL_QUERYSTRING_MOLAUSER, apiUserName);
            header.Add(HttpToken2AuthInfoEntity.URL_QUERYSTRING_SIGN, Aes.Encrypt(requestMD5, apiUserPwd));
            header.Add(HttpToken2AuthInfoEntity.URL_QUERYSTRING_TIMESTAMP, ts);
            header.Add(HttpToken2AuthInfoEntity.URL_QUERYSTRING_GUID, guid);

            string url = apiUrl + (apiUrl.EndsWith("/") ? "" : "/") + "invoke/" + apiKey + "/" + apiMethod;

            string responseString = "";

            try
            {
                responseString = webUtils.DoPost(url, header, request.ToJson());
            }
            catch (Exception exp)
            {
                return CreateErrorResponse<T>("DoPost：" + exp.Message);
            }

            if (string.IsNullOrEmpty(responseString))
            {
                //返回是个空字符串
                return default(T);
            }

            try
            {
                //反序列化为MolaResponse
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (Newtonsoft.Json.JsonSerializationException ex)
            {
                return CreateErrorResponse<T>("序列号异常" + ex.Message);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<T>("DeserializeObject" + ex.Message);
            }
        }

        /// <summary>
        /// 获取异常返回结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        static T CreateErrorResponse<T>(string errMsg) where T : SingleResult<string>
        {
            var rsp = Activator.CreateInstance<T>();
            rsp.Success = false;
            rsp.Message = errMsg;
            rsp.Data = string.Empty;
            return rsp;
        }

        #endregion        
    }
}
