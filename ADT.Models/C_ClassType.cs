using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    /// <summary>
    /// 班级类型
    /// </summary>
   public class C_ClassType
    {
        public int Id { get; set; }
        /// <summary>
        /// 分类类型
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 校区Id
        /// </summary>
        public int CampusId { get; set; }

        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
    }
}
