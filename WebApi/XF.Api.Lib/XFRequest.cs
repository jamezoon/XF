using System.Collections.Generic;

using XFramework.Entity;

namespace XF.Api.Lib
{
    /// <summary>
    /// TOP请求接口。
    /// </summary>
    public abstract class XFRequest<T> where T : SingleResult<string>
    {
        /// <summary>
        /// 获取Api名称。
        /// </summary>
        /// <returns>Api名称</returns>
        public abstract string GetApiMethod();

        /// <summary>
        /// 返回当前实例的json
        /// </summary>
        /// <returns></returns>
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
