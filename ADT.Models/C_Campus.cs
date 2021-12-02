using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_Campus
    {
        /// <summary>
        /// 校区id
        /// </summary>
        public int CampusId { get; set; }
        /// <summary>
        /// 校区名称
        /// </summary>
        public string CampusName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }
    }
}
