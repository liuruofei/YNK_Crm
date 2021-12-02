using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_RangeTimeModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 时间间隔名称
        /// </summary>
        public string TimeName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 截止时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 校区id
        /// </summary>
        public int CampusId { get; set; }

        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }

        /// <summary>
        /// 校区名称
        /// </summary>
        public string CampusName { get; set; }

    }
}
