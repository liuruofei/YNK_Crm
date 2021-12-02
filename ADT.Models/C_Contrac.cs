using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    public class C_Contrac
    {
        public int ContracId { get; set; }
        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContraNo { get; set; }
        /// <summary>
        /// 学生uid
        /// </summary>
        public int StudentUid { get; set; }
        /// <summary>
        /// 合同中心
        /// </summary>
        public int ContraCenterId { get; set; }
        /// <summary>
        /// 线索id
        /// </summary>
        public int ClueId { get; set; }
        /// <summary>
        /// 顾问uid
        /// </summary>
        public string CC_Uid { get; set; }
        public int Constra_Status { get; set; }
        /// <summary>
        /// 校区id
        /// </summary>
        public int CampusId { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        /// 
        public decimal Total_Amount { get; set; }

        /// <summary>
        /// 总合同付款状态
        /// </summary>
        public int Pay_Status { get; set; }

        /// <summary>
        ///已付金额
        /// </summary>
        public decimal Pay_Amount { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start_Time { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End_Time { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }
    }
}
