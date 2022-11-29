using ADT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_ContracChildModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 父合同编码
        /// </summary>
        public string ContraNo { get; set; }
        /// <summary>
        /// 子合同编码
        /// </summary>
        public string Contra_ChildNo { get; set; }
        /// <summary>
        /// 学生uid
        /// </summary>
        public int StudentUid { get; set; }
        /// <summary>
        /// 顾问uid
        /// </summary>
        public string CC_Uid { get; set; }
        /// <summary>
        /// 学习状态
        /// </summary>
        public string StudyStatus { get; set; }
        /// <summary>
        /// 周期
        /// </summary>
        public int Cycle { get; set; }
        /// <summary>
        /// 学习模式
        /// </summary>
        public int StudyMode { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public int ClassId { get; set; }
        /// <summary>
        /// 课时
        /// </summary>
        public float Course_Time { get; set; }

        /// <summary>
        /// 小班课时
        /// </summary>
        public float Class_Course_Time { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal ContraRate { get; set; }
        /// <summary>
        /// 合同原价
        /// </summary>
        public decimal Original_Amount { get; set; }
        /// <summary>
        /// 折后价格
        /// </summary>
        public decimal Saler_Amount { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal Discount_Amount { get; set; }

        /// <summary>
        /// 额外优惠
        /// </summary>
        public decimal Added_Amount { get; set; }

        /// <summary>
        /// 是否联报优惠
        /// </summary>
        public int IsPreferential { get; set; }
        /// <summary>
        /// 合同属性
        /// </summary>
        public int Contra_Property { get; set; }
        /// <summary>
        /// 已付金额
        /// </summary>
        public decimal Pay_Amount { get; set; }
        /// <summary>
        /// 签约日期
        /// </summary>
        public DateTime SignIn_Data { get; set; }
        /// <summary>
        /// 子合同状态
        /// </summary>
        public int Contrac_Child_Status { get; set; }

        public DateTime StartTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 是否欠费合同
        /// </summary>
        public int ArrearageStatus { get; set; }

        /// <summary>
        /// 赠送课时
        /// </summary>
        public float PresentTime { get; set; }

        /// <summary>
        /// 数据行状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 子合同详情
        /// </summary>
        public List<C_Contrac_Child_Detail> ListItem { get; set; }

        /// <summary>
        /// 目标班级
        /// </summary>
        public int ChangeClassId { get; set; }

        /// <summary>
        /// 班课名称
        /// </summary>
        public string ClassName { get; set; }
    }
}
