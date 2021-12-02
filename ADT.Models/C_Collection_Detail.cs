using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    /// <summary>
    /// 收款登记详情
    /// </summary>
   public class C_Collection_Detail
    {
        public int Id { get; set; }
        /// <summary>
        /// 子合同详细id
        /// </summary>
        public int ChildDetailId { get; set; }
        /// <summary>
        /// 收据id
        /// </summary>
        public int CollectionId { get; set; }
        /// <summary>
        /// 详情金额
        /// </summary>
        public decimal Amount { get; set; }
        public string CreateUid { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUid { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
