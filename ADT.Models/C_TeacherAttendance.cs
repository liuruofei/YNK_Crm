using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_TeacherAttendance
    {
        public int Id { get; set; }
        public int WorkId { get; set; }
        public string TeacherId { get; set; }
        public DateTime WorkDate { get; set; }
        public DateTime InSchoolTime { get; set; }
        public float LateMinute { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
