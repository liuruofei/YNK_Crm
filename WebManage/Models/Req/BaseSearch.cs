using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Req
{
    public class BaseSearch
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int page { get; set; } = 1;
        /// <summary>
        /// 页
        /// </summary>
        public int limit { get; set; } = 10;
    }
}
