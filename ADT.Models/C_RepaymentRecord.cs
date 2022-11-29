using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_RepaymentRecord
    {
        public int Id { get; set; }
        public int CollgeId { get; set; }
        public decimal RepaymentAmount { get; set; }
        public string Contra_ChildNo { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
    }
}
