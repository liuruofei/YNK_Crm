using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class CommunicationModel
    {

        public int Id { get; set; }

        public int StudentUid { get; set; }
        public string[] openIds { get; set; }

        public string[] UserNames { get; set; }

        public DateTime? CommunicationTime { get; set; }
        public string SummaryTitle { get; set; }
        public string Summary { get; set; }
    }
}
