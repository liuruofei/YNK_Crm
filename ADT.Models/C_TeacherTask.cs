using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_TeacherTask
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string SysUid { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public int TaskStatus { get; set; }
        /// <summary>
        /// 任务内容介绍
        /// </summary>
        public string TaskComment { get; set; }
        /// <summary>
        /// 备注
        /// </summary>

        public string TaskRemarks { get; set; }
        public DateTime? CreatTime { get; set; }
        public string CreateUid { get; set; }
    }
}
