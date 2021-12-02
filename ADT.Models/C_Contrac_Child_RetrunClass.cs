using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_Contrac_Child_RetrunClass
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
