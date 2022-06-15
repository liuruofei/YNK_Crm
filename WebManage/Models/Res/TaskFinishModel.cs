using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class TaskFinishModel
    {
        
        public int Id { get; set; }

        /// <summary>
        /// 作业或者任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 学生名称
        /// </summary>
        public string Student_Name { get; set; }

        /// <summary>
        /// 完成状态
        /// </summary>
        public int FinishStatus { get; set; }
        /// <summary>
        /// 布置日期
        /// </summary>
        public DateTime? WorkDate { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        /// <summary>
        /// 是否作业
        /// </summary>
        public int isZuoye { get; set; }
    }
}
