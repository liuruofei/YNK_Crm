using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    /// <summary>
    /// 子合同申请退款
    /// </summary>
    public class C_Contrac_Child_ConstBack
    {
        public int Id { get; set; }
        public string Contra_ChildNo { get; set; }
        public decimal BackCost { get; set; }
        public bool IsOrg { get; set; }
        public int StudentUid { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Msg { get; set; }
    }
}
