using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class MockUserModel
    {
        /// <summary>
        /// 模考课程Id
        /// </summary>
        public int Id { get; set; }

        public int StudentUid { get; set; }

        public string Student_Name { get; set; }

        
        public int StudyMode { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        public int SubjectId { get; set; }
        /// <summary>
        /// 科目id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 单元Id
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 单元名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 模考日期
        /// </summary>
        public DateTime AT_Date { get; set; }
        /// <summary>
        /// 模考开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        ///模考截止时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 成绩
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// 模块等级
        /// </summary>
        public string MockLevel { get; set; }


        /// <summary>
        /// 试卷编号
        /// </summary>
        public string PaperCode { get; set; }

    }
}
