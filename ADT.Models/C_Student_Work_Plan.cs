using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    public class C_Student_Work_Plan
    {
        public int Id { get; set; }
        public int StudentUid { get; set; }
        public DateTime WorkDate { get; set; }
        /// <summary>
        /// 抽词完成情况
        /// </summary>
        public string ChouciComent { get; set; }
        /// <summary>
        /// 课程任务
        /// </summary>
        public string CourseComent { get; set; }
        /// <summary>
        /// 家庭作业任务
        /// </summary>
        public string HomeWorkComent { get; set; }
        /// <summary>
        /// 其它任务
        /// </summary>
        public string OtherComent { get; set; }
        /// <summary>
        /// 每日总结
        /// </summary>
        public string SummaryComent { get; set; }

        /// <summary>
        /// 助教
        /// </summary>
        public string TaUid { get; set; }
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
        /// 校区Id
        /// </summary>
        public int CampusId { get; set; }


        public DateTime? CreateTime { get; set; }

        public string CreateUid { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string UpdateUid { get; set; }
    }
}
