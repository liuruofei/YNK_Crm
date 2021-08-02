using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Req
{
    /// <summary>
    /// 提交线下精准请求参数实体
    /// </summary>
    public class PostOfflineForecastQuery
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }
    }
}
