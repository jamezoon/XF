using System;

using Enyim.Caching;
using Enyim.Caching.Memcached;

using XFramework.Safe;
using XFramework.Util;
using XFramework.Log;
using System.Runtime.Serialization;

namespace XFramework.Cache
{
    /// <summary>
    /// XFramework缓存(Memcached)操作
    /// </summary>
    public class Memcached
    {
        /// <summary>
        /// Memcached对象操作实例
        /// </summary>
        private static readonly MemcachedClient MemcachedClient = new MemcachedClient();

        /// <summary>
        /// 获取Memcached缓存中的值
        /// </summary>
        /// <param name="regionName">Memcached缓存区域名称</param>
        /// <param name="cacheKey">Memcached缓存键值</param> 
        /// <returns>Memcached缓存中的值</returns>
        public static object Get(string regionName, string cacheKey)
        {
            try
            {
                cacheKey = GetMemcachedKey(regionName, cacheKey);

                return MemcachedClient.Get(cacheKey);
            }
            catch (Exception ex)
            {
                LogUtil.Log("XFramework获取缓存内容异常", ex, LogLevel.Warn);

                return null;
            }
        }

        /// <summary>
        /// 获取Memcached缓存中的实体值
        /// </summary>
        /// <typeparam name="T">Memcached缓存中的实体类型</typeparam>
        /// <param name="regionName">Memcached缓存区域名称</param>
        /// <param name="cacheKey">Memcached缓存键值</param>
        /// <returns>Memcached缓存中的实体值</returns>
        public static T Get<T>(string regionName, string cacheKey)
        {
            try
            {
                cacheKey = GetMemcachedKey(regionName, cacheKey);

                return MemcachedClient.Get<T>(cacheKey);
            }
            catch (Exception ex)
            {
                LogUtil.Log("XFramework获取缓存内容(范型)异常", ex, LogLevel.Warn);

                return default(T);
            }
        }

        /// <summary>
        /// 设置Memcached缓存中的值
        /// </summary>
        /// <param name="regionName">Memcached缓存区域名称</param>
        /// <param name="cacheKey">Memcached缓存键值</param>
        /// <param name="data">Memcached缓存中保存的内容</param>
        /// <param name="cacheTime">缓存时间 单位：秒</param>
        /// <returns>是否设置成功</returns>
        public static bool Set(string regionName, string cacheKey, object data, int cacheTime = 3600)
        {
            try
            {
                //如果设置的数据未null，则不保存到Memcached缓存中
                if (data == null)
                    return false;

                //如果设置Memcached缓存的Region名称失败，则无法保存到Memcached缓存中
                if (!SetRegion(regionName))
                    return false;

                //如果提供的缓存时间大于7天，则统一为7天时间
                if (cacheTime > TimeSpan.FromDays(7).TotalSeconds)
                    cacheTime = StringUtils.ToInt32(TimeSpan.FromDays(7).TotalSeconds);

                //获取Memcached缓存的KEY值
                cacheKey = GetMemcachedKey(regionName, cacheKey);

                //删除Memcached缓存中此Key值的原值
                MemcachedClient.Remove(cacheKey);

                return MemcachedClient.Store(StoreMode.Set, cacheKey, data, new TimeSpan(0, 0, cacheTime));
            }
            catch (Exception ex)
            {
                LogUtil.Log("XFramework设置缓存内容异常", ex, LogLevel.Warn);

                return false;
            }
        }

        /// <summary>
        /// 删除Memcached缓存
        /// </summary>
        /// <param name="regionName">Memcached缓存区域名称</param>
        /// <param name="cacheKey">Memcached缓存键值</param>
        public static bool Remove(string regionName, string cacheKey)
        {
            cacheKey = GetMemcachedKey(regionName, cacheKey);

            return MemcachedClient.Remove(cacheKey);
        }

        /// <summary>
        /// 清空Memcached缓存的区域
        /// </summary>
        /// <param name="regionName">Memcached缓存区域名称</param>
        /// <returns>是否清空成功</returns>
        public static bool ClearRegion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                throw new ArgumentNullException("Memcached缓存区域名称为空");

            object obj = MemcachedClient.Get(regionName);

            //如果Memcached缓存中获取不到region值，则说明此region下无数据，则直接返回清除成功
            if (obj == null)
                return true;

            //清空掉Memcached缓存中的region值，如果清除老的region失败，则新的region就不写入
            if (!MemcachedClient.Remove(regionName))
                return false;

            //设置一个新的region值到缓存中
            string _version = DateTime.Now.ToString("hhmmssfff");

            return MemcachedClient.Store(StoreMode.Set, regionName, _version, TimeSpan.FromDays(7));
        }

        /// <summary>
        /// 设置Memcached缓存的区域版本号
        /// </summary>
        /// <param name="regionName">Memcached缓存区域名称</param>
        /// <returns>Memcached缓存的区域版本号是否设置成功</returns>
        private static bool SetRegion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                throw new ArgumentNullException("Memcached缓存区域名称为空");

            object obj = MemcachedClient.Get(regionName);

            if (obj != null)
                return true;

            //设置一个新的region值到缓存中
            string version = DateTime.Now.ToString("hhmmssfff");

            return MemcachedClient.Store(StoreMode.Set, regionName, version, TimeSpan.FromDays(7));
        }

        /// <summary>
        /// 获取Memcached缓存的实际KEY值
        /// </summary>
        /// <param name="regionName">Memcached缓存区域名称</param>
        /// <param name="cacheKey">Memcached缓存键值</param>
        /// <returns>Memcached缓存的实际KEY值</returns>
        private static string GetMemcachedKey(string regionName, string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentNullException("Memcached缓存的CacheKey不能为空");

            //将KEY值进行MD5方式加密
            cacheKey = MD5.Encrypt(cacheKey);

            string _version = GetRegionVersion(regionName);

            cacheKey = string.Format("{0}_{1}_{2}", regionName, _version, cacheKey);

            if (cacheKey.Length > 250)
                throw new ArgumentException("键值长度不能超过250位");

            return cacheKey;
        }

        /// <summary>
        /// 获取Memcached缓存的区域的版本号
        /// </summary>
        /// <param name="regionName">Memcached缓存的区域名称</param>
        /// <returns>Memcached缓存的区域的版本号</returns>
        private static string GetRegionVersion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                throw new ArgumentNullException("Memcached缓存区域名称为空");

            object obj = MemcachedClient.Get(regionName);

            if (obj != null) return (string)obj;

            return DateTime.Now.ToString("hhmmssfff");
        }
    }
}
