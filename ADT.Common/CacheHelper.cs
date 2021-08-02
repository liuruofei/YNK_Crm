using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Common
{
    public class CacheHelper
    {
        /// <summary>
        /// 初始化静态Memory缓存
        /// </summary>
        static MemoryCache client = new MemoryCache(new MemoryCacheOptions());
        /// <summary>
        /// 添加一个数据
        /// </summary>
        /// <typeparam name="T">添加值类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="value">缓存值</param>
        /// <param name="exdt">过期时间</param>
        public static void Set<T>(string key, T value, DateTimeOffset? exdt)
        {
            if (exdt.HasValue)
                client.Set(key, value, exdt.Value);
            else
                client.Set(key, value);
        }
        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        public static object Get<TResult>(string key)
        {
            return client.Get<TResult>(key);
        }
    }
}
