using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_Summary
    {
        public int Id { get; set; }

        public int StudentUid { get; set; }
        public string openIds { get; set; }
        public string UserNames { get; set; }
        public DateTime? CommunicationTime { get; set; }
        public string SummaryTitle { get; set; }
        public string Summary { get; set; }
        public int IsSend { get; set; }
        public DateTime? SendTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
