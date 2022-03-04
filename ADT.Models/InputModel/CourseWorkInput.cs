using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.InputModel
{
   public class CourseWorkInput
    {

        public int Id { get; set; }
        /// <summary>
        /// 子合同编码
        /// </summary>
        public string Contra_ChildNo { get; set; }
        /// <summary>
        /// 教师uid
        /// </summary>
        public string TeacherUid { get; set; }
        /// <summary>
        /// 上课开始日期
        /// </summary>
        public DateTime AT_Date { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 截止时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 教室Id
        /// </summary>
        public int RoomId { get; set; }
        /// <summary>
        /// 学生uid
        /// </summary>
        public int StudentUid { get; set; }
        /// <summary>
        /// 督学Uid
        /// </summary>
        public string TA_Uid { get; set; }
        /// <summary>
        /// 课时时间
        /// </summary>
        public float CourseTime { get; set; }
        /// <summary>
        /// 分类ID
        /// </summary>
        public int SubjectId { get; set; }
        /// <summary>
        /// 科目id
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// 小班Id
        /// </summary>
        public int ClasssId { get; set; }
        /// <summary>
        /// 学习模式
        /// </summary>
        public int StudyMode { get; set; }

        /// <summary>
        /// 课程状态
        /// </summary>
        public int Work_Stutas { get; set; }

        /// <summary>
        /// 排课标题
        /// </summary>
        public string Work_Title { get; set; }

        /// <summary>
        /// 时段Id
        /// </summary>
        public int RangTimeId { get; set; }

        /// <summary>
        /// 校区Id
        /// </summary>
        public int CampusId { get; set; }

        /// <summary>
        /// 试听用户
        /// </summary>
        public string ListeningName { get; set; }

        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }

        /// <summary>
        /// 评语
        /// </summary>
        public string Comment { get; set; }


        /// <summary>
        /// 时间组
        /// </summary>
        public string[] WorkDateGroup { get; set; }


        /// <summary>
        /// 微信公众号token
        /// </summary>
        public string wxaccessToken { get; set; }

        /// <summary>
        /// 模板Id
        /// </summary>
        public string templateId { get; set; }
    }
}
