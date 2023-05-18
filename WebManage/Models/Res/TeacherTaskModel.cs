using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class TeacherTaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string SysUid { get; set; }
        public int TaskStatus { get; set; }
        public string TaskComment { get; set; }

        public string TaskRemarks { get; set; }
        public DateTime? CreatTime { get; set; }
        public string CreateUid { get; set; }

        public string User_Name { get; set; }
    }
}
