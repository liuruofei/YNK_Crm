using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_BackAmountRecord
    {
        public int Id { get; set; }
        public int StudentUid { get; set; }
        public decimal BackAmount { get; set; }
        public DateTime BackDate { get; set; }
        public string CreateUid { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
