using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_Collection
    {
        public int Id { get; set; }
        /// <summary>
        ///学生uid
        /// </summary>
        public int StudentUid { get; set; }

        /// <summary>
        /// 学生名称
        /// </summary>
        public string StudentName { get; set; }
        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 使用余额金额
        /// </summary>
        public decimal DeductAmount { get; set; }

        /// <summary>
        /// 收款方式
        /// </summary>
        public int PayMothed { get; set; }
        /// <summary>
        /// 付款图片
        /// </summary>
        public string PayImg { get; set; }
        /// <summary>
        /// 校区
        /// </summary>
        public int CampusId { get; set; }
        /// <summary>
        /// 关联合同
        /// </summary>
        public string RelationShip_Contras { get; set; }
        /// <summary>
        /// 登记时间
        /// </summary>
        public DateTime Registration_Time { get; set; }
        /// <summary>
        /// 收款时间
        /// </summary>
        public DateTime Collection_Time { get; set; }
        /// <summary>
        /// 付款状态
        /// </summary>
        public int PayStatus { get; set; }
        /// <summary>
        ///审核者
        /// </summary>
        public string AuditUid { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateUid { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateUid { get; set; }
    }
}
