using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_Course_Work_Recored
    {
        public int Id { get; set; }
        ///// <summary>
        ///// 子合同
        ///// </summary>
        //public string Contra_ChildNo { get; set; }
        ///// <summary>
        ///// 原日期
        ///// </summary>
        //public DateTime Old_At_Date { get; set; }
        ///// <summary>
        ///// 原开始时间
        ///// </summary>
        //public string Old_StartTime { get; set; }
        ///// <summary>
        ///// 原截止时间
        ///// </summary>
        //public string Old_EndTime { get; set; }
        ///// <summary>
        ///// 新日期
        ///// </summary>
        //public DateTime At_Date { get; set; }
        ///// <summary>
        ///// 新开始时间
        ///// </summary>
        //public string StartTime { get; set; }
        ///// <summary>
        ///// 新截止时间
        ///// </summary>
        //public string EndTime { get; set; }
        ///// <summary>
        ///// 学员uid
        ///// </summary>
        //public int StudentUid { get; set; }
        ///// <summary>
        ///// 老师Uid
        ///// </summary>
        //public int TeacherUid { get; set; }
        ///// <summary>
        ///// 学习模式
        ///// </summary>
        //public int StudyMode { get; set; }
        ///// <summary>
        ///// 科目大类
        ///// </summary>
        //public int SubjectId { get; set; }
        ///// <summary>
        ///// 科目小类
        ///// </summary>
        //public int ProjectId { get; set; }
        ///// <summary>
        ///// 小班
        ///// </summary>
        //public int ClasssId { get; set; }
        ///// <summary>
        ///// 督学
        ///// </summary>
        //public string TA_Uid { get; set; }
        ///// <summary>
        ///// 房间号
        ///// </summary>
        //public int RoomId { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string CreateUid { get; set; }

        /// <summary>
        /// 校区Id
        /// </summary>
        public int CampusId { get; set; }
    }
}
