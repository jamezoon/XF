using System;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;

using FastReflectionLib;
using XF.Api.Core.Authenticator;

using XF.Api.Lib;

namespace XF.Api.Core
{
    /// <summary>
    /// 将http远程调用转为本地调用
    /// </summary>
    public class HttpAdapter
    {
        public object Invoker(string serviceKey, string serviceMethod, string serviceArgs)
        {
            //取出服务实体信息
            object service = ApiService.Instance.GetService(serviceKey);

            //获取服务类型和服务方法信息
            Type serviceType = service.GetType();

            //不支持服务方法的重载
            MethodInfo method = serviceType.GetMethod(serviceMethod);

            if (method == null) throw new XFApiException(string.Format("方法名未找到，请求方法名称：{0}。", serviceMethod));

            var attrArray = method.GetCustomAttributes(typeof(CusCodeCheckAttribute), false);

            CusCodeCheckAttribute cusCodeCheckAttr = null;

            if (attrArray.Length > 0)
            {
                cusCodeCheckAttr = attrArray[0] as CusCodeCheckAttribute;
            }

            object[] args = null;

            if (cusCodeCheckAttr != null)
            {
                var cuscode = string.Empty;
                var encryptCuscode = string.Empty;

                args = DeserializeArguments(method.GetParameters(), serviceArgs, cusCodeCheckAttr, out cuscode, out encryptCuscode);

                if (!cusCodeCheckAttr.Check(cuscode, encryptCuscode)) throw new XFApiException(string.Format("客户编号不匹配，请求方法名称：{0}。", serviceMethod));
            }
            else
            {
                //把传进来的值变成参数值数组
                args = DeserializeArguments(method.GetParameters(), serviceArgs);
            }

            //动态调用方法
            return method.FastInvoke(service, args);
        }

        object[] DeserializeArguments(ParameterInfo[] methodArgs, string input)
        {
            string s1, s2;
            return DeserializeArguments(methodArgs, input, null, out s1, out s2);
        }

        object[] DeserializeArguments(ParameterInfo[] methodArgs, string input, CusCodeCheckAttribute cuscodeAttr, out string cuscode, out string encryptCuscode)
        {
            cuscode = string.Empty;
            encryptCuscode = string.Empty;

            if (methodArgs == null)
            {
                return null;
            }

            if (methodArgs.Length == 0)
            {
                return null;
            }

            var inputDic = Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary<string, object>>(input);

            if (inputDic == null)
            {
                return null;
            }

            #region 给cuscode赋值

            if (cuscodeAttr != null)
            {
                foreach (var inputItem in inputDic)
                {
                    if (cuscodeAttr.CusCodeArgName.ToLower() == inputItem.Key.ToLower())
                    {
                        cuscode = inputItem.Value as string;
                    }

                    if (cuscodeAttr.EncryptCusCodeArgName.ToLower() == inputItem.Key.ToLower())
                    {
                        encryptCuscode = inputItem.Value as string;
                    }
                }
            }

            #endregion

            var args = new object[methodArgs.Length];

            #region 方法参数赋值

            var i = 0;

            foreach (var arg in methodArgs)
            {
                var setValue = false;

                foreach (var inputItem in inputDic)
                {
                    if (arg.Name.Length == inputItem.Key.Length && arg.Name.ToLower() == inputItem.Key.ToLower())
                    {
                        string str = Newtonsoft.Json.JsonConvert.SerializeObject(inputItem.Value);

                        args[i] = Newtonsoft.Json.JsonConvert.DeserializeObject(str, arg.ParameterType);

                        setValue = true;

                        break;
                    }
                }

                //如果在传进来的参数中未找到值，则使用默认值
                if (!setValue)
                {
                    args[i] = arg.DefaultValue;
                }

                i++;
            }

            #endregion

            return args;
        }
    }
}