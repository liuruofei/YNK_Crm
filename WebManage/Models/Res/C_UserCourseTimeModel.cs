using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_UserCourseTimeModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 子合同编码
        /// </summary>
        public string Contra_ChildNo { get; set; }
        public int StudentUid { get; set; }
        public int SubjectId { get; set; }

        /// <summary>
        /// 科目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
        public int ClassId { get; set; }
        public float Course_Time { get; set; }
        public float Course_UseTime { get; set; }
        public float Class_Course_Time { get; set; }
        public float Class_Course_UseTime { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string Class_Name { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        ///分类名称
        /// </summary>
        public string SubjectName { get; set; }
    }
}
