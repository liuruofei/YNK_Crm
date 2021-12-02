using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    public class C_Range_Time
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

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
    }
}
