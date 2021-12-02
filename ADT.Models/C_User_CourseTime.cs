using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_User_CourseTime
    {
        public int Id { get; set; }
        /// <summary>
        /// 子合同编码
        /// </summary>
        public string Contra_ChildNo { get; set; }
        public int StudentUid { get; set; }
        public int SubjectId { get; set; }

        /// <summary>
        /// 科目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
        public int ClassId { get; set; }
        public float Course_Time { get; set; }
        public float Course_UseTime { get; set; }
        public float Class_Course_Time { get; set; }
        public float Class_Course_UseTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
    }
}
