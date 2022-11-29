using ADT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_CollectionModel
    {
        public int Id { get; set; }
        /// <summary>
        ///学生uid
        /// </summary>
        public int StudentUid { get; set; }
        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 记录收款金额字段
        /// </summary>
        public decimal FilAmount { get; set; }

        /// <summary>
        /// 使用余额金额
        /// </summary>
        public decimal DeductAmount { get; set; }


        /// <summary>
        /// 记录扣除余额字段
        /// </summary>
        public decimal koudeductAmount { get; set; }

        /// <summary>
        /// 额外优惠
        /// </summary>
        public decimal AddedAmount { get; set; }

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
        /// 学生姓名
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 校区
        /// </summary>
        public string CampusName { get; set; }

        /// <summary>
        /// 系统用户
        /// </summary>
        public string User_Name { get; set; }

        /// <summary>
        /// 垫付状态
        /// </summary>
        public int ArrearageStatus { get; set; }

        /// <summary>
        /// 统计还款金额
        /// </summary>
        public decimal RepaymentTotal { get; set; }


        /// <summary>
        /// 详情列表
        /// </summary>
        public List<C_Collection_Detail> ListCollectionDetail { get; set; }
    }
}
