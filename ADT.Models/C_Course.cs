using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        public int Sort { get; set; }

        /// <summary>
        /// 课程描述
        /// </summary>
        public string Description { get; set; }

        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
    }
}
