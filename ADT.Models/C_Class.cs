using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    public class C_Class
    {
        public int ClassId { get; set; }
        /// <summary>
        /// 班级编号
        /// </summary>
        public string Class_No { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string Class_Name { get; set; }
        /// <summary>
        /// 校区id
        /// </summary>
        public int CampusId { get; set; }
        /// <summary>
        /// 课时
        /// </summary>
        public float Course_Time { get; set; }
        /// <summary>
        ///价格
        /// </summary>
        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime Start_Course_Date { get; set; }
        public DateTime End_Course_Date { get; set; }
        public int Count_Users { get; set; }

        /// <summary>
        /// 分类Id
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 考试类别
        /// </summary>
        public int SubjectId { get; set; }

        public int Material_Count { get; set; }
        public string Relevant { get; set; }
        public string Remarks { get; set; }
        public int Sort { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }
    }
}
