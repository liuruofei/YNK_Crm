using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class TeacherTimeWorkModel
    {
        public int Id { get; set; }

        public DateTime WorkDate { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 截止时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 标签内容
        /// </summary>
        public string WorkTitle { get; set; }
        /// <summary>
        /// 星期名称
        /// </summary>
        public string WorkDateName { get; set; }

        /// <summary>
        /// 根据老师存储相关的可排时间
        /// </summary>
        public Dictionary<string, List<StudentWorkPlanModel>> ListTeacherTime { get; set;}

    }
}
