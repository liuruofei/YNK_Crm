using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.InputModel
{
   public class ChildBackCostInput
    {
        public string Contra_ChildNo { get; set;}


        /// <summary>
        /// 按原价退款
        /// </summary>
        public bool IsOrg { get; set; }

        /// <summary>
        /// 退款输入金额
        /// </summary>
        public decimal BackCost { get; set; }
    }
}
