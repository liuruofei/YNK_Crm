using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class RedisConfig: IRedisConfig
    {
        public bool IsOpenCache { get; set; }

        /// <summary>
        /// 配置连接
        /// </summary>
        public string RedisCon { get; set; }
    }
    interface IRedisConfig
    {
        public bool IsOpenCache { get; set; }

        /// <summary>
        /// 配置连接
        /// </summary>
        public string RedisCon { get; set; }


    }
}
