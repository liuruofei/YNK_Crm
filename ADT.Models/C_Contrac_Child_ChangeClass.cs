using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   /// <summary>
   /// 转班申请
   /// </summary>
   public class C_Contrac_Child_ChangeClass
    {
        public int Id { get; set; }
        /// <summary>
        /// 子合同
        /// </summary>
        public string Contra_ChildNo { get; set; }
        /// <summary>
        /// 目标班级
        /// </summary>
        public int ChangeClassId { get; set; }
        /// <summary>
        /// 原班级
        /// </summary>
        public int OldClassId { get; set; }
        /// <summary>
        /// 学生uid
        /// </summary>
        public int  StudentUid { get; set; }
        /// <summary>
        /// 学习状态
        /// </summary>
        public string StudyStatus { get; set; }
        /// <summary>
        /// 学习方式
        /// </summary>
        public int StudyMode { get; set; }
        /// <summary>
        /// 周期
        /// </summary>
        public int Cycle { get; set; }
        /// <summary>
        /// 课时
        /// </summary>
        public float Class_Course_Time { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal ContraRate { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public decimal Original_Amount { get; set; }
        /// <summary>
        /// 售后价
        /// </summary>
        public decimal Saler_Amount { get; set; }
        /// <summary>
        /// 优惠价格
        /// </summary>
        public decimal Discount_Amount { get; set; }
        /// <summary>
        /// 是否优惠联报
        /// </summary>
        public int IsPreferential { get; set; }
        /// <summary>
        /// 合同属性
        /// </summary>
        public int Contra_Property { get; set; }


        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime{get;set;}
        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateUid{get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
