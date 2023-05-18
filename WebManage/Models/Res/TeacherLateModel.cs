using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class TeacherLateModel
    {
        public int Id { get; set; }
        public int WorkId { get; set; }
        public string TeacherId { get; set; }
        public DateTime WorkDate { get; set; }
        public DateTime InSchoolTime { get; set; }
        public float LateMinute { get; set; }

        public string TeacherName { get; set; }

        public string Work_Title { get; set; }


        public DateTime AT_Date { get; set; }

        public string StartTime { get; set; }

        public string CourseTimeFmt { get; set; }

        public string InSchoolTimeFmt { get; set; }

    }
}
