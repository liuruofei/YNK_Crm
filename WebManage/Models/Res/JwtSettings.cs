using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class JwtSettings : IJwtSettings
    {
        /// <summary>
        /// token是谁颁发的
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// token可以给哪些客户端使用
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 加密的key
        /// </summary>
        public string SecretKey { get; set; }
    }

    public interface IJwtSettings
    {
        /// <summary>
        /// token是谁颁发的
        /// </summary>
        string Issuer { get; set; }
        /// <summary>
        /// token可以给哪些客户端使用
        /// </summary>
        string Audience { get; set; }
        /// <summary>
        /// 加密的key
        /// </summary>
        string SecretKey { get; set; }
    }

}
