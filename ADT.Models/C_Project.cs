using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    /// <summary>
    /// 科目信息
    /// </summary>
   public class C_Project
    {

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int Status { get; set; }
        public int Sort { get; set; }

        /// <summary>
        /// 考试类型id
        /// </summary>
        public int SubjectId { get; set; }

        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }

        public string Description { get; set; }
    }
}
