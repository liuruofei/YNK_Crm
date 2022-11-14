using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class StudentWorkPlanModel
    {
        public int Id { get; set; }
        public int StudentUid { get; set; }



        /// <summary>
        /// 学生姓名
        /// </summary>
        public string Student_Name { get; set; }

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
        /// 课程老师布置作业
        /// </summary>
        public string CourseWorkCotent { get; set; }

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
        /// 离校时间
        /// </summary>
        public string OutSchoolTime { get; set; }

        /// <summary>
        /// 到校时间
        /// </summary>
        public string InSchoolTime { get; set; }

        /// <summary>
        /// 在校时长
        /// </summary>
        public string TotalTime { get; set; }

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
        public string Eight_Ten_TeacherName { get; set; }
        public int Eight_Ten_StudyMode { get; set; }

        /// <summary>
        /// 10-12点内容
        /// </summary>
        public string Ten_Twelve_OlockTitle { get; set; }
        public int Ten_Twelve_Id { get; set; }
        public string Ten_Twelve_TeacherName { get; set; }
        public int Ten_Twelve_StudyMode { get; set; }
        /// <summary>
        /// 13-15点内容
        /// </summary>
        public string Thirteen_Fifteen_OlockTitle { get; set; }
        public int Thirteen_Fifteen_Id { get; set; }

        public string Thirteen_Fifteen_TeacherName { get; set; }

        public int Thirteen_Fifteen_StudyMode { get; set; }
        /// <summary>
        /// 15-17点内容
        /// </summary>
        public string Fifteen_Seventeen_OlockTitle { get; set; }
        public int Fifteen_Seventeen_Id { get; set; }

        public string Fifteen_Seventeen_TeacherName { get; set; }

        public int Fifteen_Seventeen_StudyMode { get; set; }
        /// <summary>
        /// 17-19点内容
        /// </summary>
        public string Seventeen_Nineteen_OlockTitle { get; set; }
        public int Seventeen_Nineteen_Id { get; set; }

        public string Seventeen_Nineteen_TeacherName { get; set; }

        public int Seventeen_Nineteen_StudyMode { get; set; }
        /// <summary>
        ///19-21点内容
        /// </summary>
        public string Nineteen_TwentyOne_OlockTitle { get; set; }
        public int Nineteen_TwentyOne_Id { get; set; }
        public string Nineteen_TwentyOne_TeacherName { get; set; }
        public int Nineteen_TwentyOne_StudyMode { get; set; }


        /// <summary>
        ///21-23点内容
        /// </summary>
        public string TwentyOne_TwentyTree_OlockTitle { get; set; }
        public int TwentyOne_TwentyTree_Id { get; set; }
        public string TwentyOne_TwentyTree_TeacherName { get; set; }
        public int TwentyOne_TwentyTree_StudyMode { get; set; }


        public bool Eight_Ten_Reversion { get; set; } = false;

        public bool Ten_Twelve_Reversion { get; set; } = false;

        public bool Thirteen_Fifteen_Reversion { get; set; } = false;

        public bool Fifteen_Seventeen_Reversion { get; set; } = false;

        public bool Seventeen_Nineteen_Reversion { get; set; } = false;

        public bool Nineteen_TwentyOne_Reversion { get; set; } = false;

        public bool TwentyOne_TwentyTree_Reversion { get; set; } = false;


        //可排列表用(是否整天休息)
        public bool isRest { get; set; } = false;
    }
}
