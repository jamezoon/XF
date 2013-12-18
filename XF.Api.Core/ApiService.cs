using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using XF.Api.Lib;
using XFramework.Data;
using XFramework.Cache;

namespace XF.Api.Core
{
    public class ApiService
    {
        /// <summary>
        /// 获取类的单例
        /// </summary>
        public static ApiService Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }
            internal static readonly ApiService instance = new ApiService();
        }

        /// <summary>
        /// 本机缓存实例
        /// </summary>
        private LocalCache localCache = new LocalCache(true, 10);

        /// <summary>
        /// 从数据库中获取Api信息
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <returns></returns>
        private ApiServiceEntity GetServiceEntity(string serviceKey)
        {
            string cacheKey = "getserviceentity_" + serviceKey;

            ApiServiceEntity serviceEntity = localCache.Get<ApiServiceEntity>(cacheKey);

            if (serviceEntity == null)
            {
                string sql = "select * from api_service where servicekey=@servicekey";

                List<DataOperationParameter> pars = new List<DataOperationParameter>();

                pars.Add(new DataOperationParameter("servicekey", DbType.String, serviceKey));

                DataCommand cmd = DataCommandManager.GetDataCommand(
                                   "XDBConnString",
                                   sql,
                                   pars);

                serviceEntity = cmd.ExecuteEntity<ApiServiceEntity>();

                if (serviceEntity != null) localCache.Set(cacheKey, serviceEntity);
            }

            return serviceEntity;
        }

        /// <summary>
        /// 从数据库中获取验证信息
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <returns></returns>
        public ApiServiceTokenEntity GetServiceTokenEntity(string serviceKey)
        {
            string chacheKey = "getservicetokenentity_" + serviceKey;

            ApiServiceTokenEntity serviceTokenEntity = localCache.Get<ApiServiceTokenEntity>(chacheKey);

            if (serviceTokenEntity == null)
            {
                string sql = "select * from api_service_token where servicekey=@servicekey";

                List<DataOperationParameter> pars = new List<DataOperationParameter>();

                pars.Add(new DataOperationParameter("servicekey", DbType.String, serviceKey));

                DataCommand cmd = DataCommandManager.GetDataCommand(
                                   "XDBConnString",
                                   sql,
                                   pars);

                serviceTokenEntity = cmd.ExecuteEntity<ApiServiceTokenEntity>();

                if (serviceTokenEntity != null) localCache.Set(chacheKey, serviceTokenEntity);
            }

            return serviceTokenEntity;
        }

        /// <summary>
        /// 根据Api信息生成服务类型实例
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <returns></returns>
        public object GetService(string serviceKey)
        {
            ApiServiceEntity serviceEntity = GetServiceEntity(serviceKey);

            if (serviceEntity == null) throw new XFApiException(string.Format("请求Api的Key值为空，ServiceKey：{0}", serviceKey));

            if (!serviceEntity.IsEnable) throw new XFApiException(string.Format("请求的Api已禁用，ServiceKey：{0}", serviceKey));

            Type type = Type.GetType(serviceEntity.ImplementType, true, true);

            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// 清空Api缓存信息
        /// </summary>
        /// <returns>是否清除成功</returns>
        public bool ClearCache()
        {
            return localCache.Clear();
        }
    }
}
