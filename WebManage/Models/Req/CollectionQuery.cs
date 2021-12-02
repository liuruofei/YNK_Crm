using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Req
{
    public class CollectionQuery:BaseSearch
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 收款方式
        /// </summary>
        public int payMothed { get; set; }
    }
}
