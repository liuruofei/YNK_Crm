using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.ResModel
{
   public class UserCourseTimeModel
   {
        /// <summary>
        /// 子合同编码
        /// </summary>
        public string Contra_ChildNo { get; set; }
        public int StudentUid { get; set; }
        public int SubjectId { get; set; }
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
        ///(课程费用)等级1单价
        /// </summary>
        public decimal Lvel1Price { get; set; }

        /// <summary>
        ///(课程费用) 等级2单价
        /// </summary>
        public decimal Lvel2Price { get; set; }

        /// <summary>
        /// (课程费用)等级3单价
        /// </summary>
        public decimal Lvel3Price { get; set; }

        /// <summary>
        ///(课程费用) 等级4单价
        /// </summary>
        public decimal Lvel4Price { get; set; }

        /// <summary>
        ///(课程费用)小班费用
        /// </summary>
        public decimal ClassPrice { get; set; }
    }
}
