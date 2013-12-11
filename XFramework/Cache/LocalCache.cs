using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFramework.Cache
{
    /// <summary>
    /// 采用惰性过期机制的本机缓存
    /// </summary>
    public sealed class LocalCache
    {
        #region 类构造函数

        public LocalCache(bool _enabled, int _minute = 10)
        {
            enabled = _enabled;
            minute = _minute;
        }

        public LocalCache(int _minute = 10)
        {
            enabled = true;
            minute = _minute;
        }

        #endregion

        /// <summary>
        /// 是否启用
        /// </summary>
        private bool enabled = true;

        /// <summary>
        /// 缓存时间，默认10分钟
        /// </summary>
        private int minute = 10;

        /// <summary>
        /// 实例化一个字典对象到本机内存中
        /// </summary>
        private Dictionary<string, CacheObject> cacheDicts = new Dictionary<string, CacheObject>();

        /// <summary>
        /// 用来单线程访问缓存字典对象
        /// </summary>
        private readonly object lockobj = new object();

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="cacheKey">缓存的Key值</param>
        /// <returns>缓存数据</returns>
        public object Get(string cacheKey)
        {
            if (!enabled) return null;

            CacheObject data = null;

            if (cacheDicts.ContainsKey(cacheKey)) data = cacheDicts[cacheKey];

            if (data == null) return null;

            TimeSpan ts = DateTime.Now - data.CacheTime;

            //默认缓存10分钟
            if (System.Math.Abs(ts.TotalMinutes) > minute) return null;

            return data == null ? null : data.CacheData;
        }

        /// <summary>
        /// 获取缓存数据实体
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="cacheKey">缓存的Key值</param>
        /// <returns>缓存数据实体</returns>
        public T Get<T>(string cacheKey)
        {
            object obj = Get(cacheKey);

            if (obj == null) return default(T);

            return (T)obj;
        }

        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="cacheKey">缓存的Key值</param>
        /// <param name="data">缓存数据</param>
        /// <returns>是否设置成功</returns>
        public bool Set(string cacheKey, object data)
        {
            if (!enabled) return false;

            if (string.IsNullOrWhiteSpace(cacheKey)) return false;

            if (data != null)
            {
                CacheObject c = new CacheObject { CacheData = data, CacheTime = DateTime.Now };
                lock (lockobj)
                {
                    cacheDicts[cacheKey] = c;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 删除单个缓存
        /// </summary>
        /// <param name="cacheKey">缓存的Key值</param>
        /// <returns>是否删除成功</returns>
        public bool Remove(string cacheKey)
        {
            if (!enabled) return false;

            lock (lockobj)
            {
                return cacheDicts.Remove(cacheKey);
            }
        }

        /// <summary>
        /// 清除全部缓存
        /// </summary>
        /// <returns>是否清除成功</returns>
        public bool Clear()
        {
            if (!enabled) return false;

            lock (lockobj)
            {
                cacheDicts.Clear();

                cacheDicts = new Dictionary<string, CacheObject>();

                return true;
            }
        }
    }

    /// <summary>
    /// 缓存的对象
    /// </summary>
    public sealed class CacheObject
    {
        /// <summary>
        /// 缓存中的数据
        /// </summary>
        public object CacheData { get; set; }

        /// <summary>
        /// 缓存时间
        /// </summary>
        public DateTime CacheTime { get; set; }
    }
}
