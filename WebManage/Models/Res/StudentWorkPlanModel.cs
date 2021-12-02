﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class StudentWorkPlanModel
    {
        public int Id { get; set; }
        public int StudentUid { get; set; }
        public DateTime WorkDate { get; set; }
        /// <summary>
        /// 星期名称
        /// </summary>
        public string WorkDateName { get; set; }
        /// <summary>
        /// 抽词完成情况
        /// </summary>
        public string ChouciComent { get; set; }
        /// <summary>
        /// 课程评论
        /// </summary>
        public string CourseComent { get; set; }
        /// <summary>
        /// 家庭作业评论
        /// </summary>
        public string HomeWorkComent { get; set; }
        /// <summary>
        /// 其它评论
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
        /// 助教名称
        /// </summary>
        public string TaUseName { get; set; }

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
        /// 8-10点内容
        /// </summary>
        public string Eight_Ten_OlockTitle { get; set; }
        public int Eight_Ten_Id { get; set; }
        /// <summary>
        /// 10-12点内容
        /// </summary>
        public string Ten_Twelve_OlockTitle { get; set; }
        public int Ten_Twelve_Id { get; set; }

        /// <summary>
        /// 13-15点内容
        /// </summary>
        public string Thirteen_Fifteen_OlockTitle { get; set; }
        public int Thirteen_Fifteen_Id { get; set; }
        /// <summary>
        /// 15-17点内容
        /// </summary>
        public string Fifteen_Seventeen_OlockTitle { get; set; }
        public int Fifteen_Seventeen_Id { get; set; }
        /// <summary>
        /// 17-19点内容
        /// </summary>
        public string Seventeen_Nineteen_OlockTitle { get; set; }
        public int Seventeen_Nineteen_Id { get; set; }
    }
}
