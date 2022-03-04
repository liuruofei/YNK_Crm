using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrmApi.Models
{
    public class WXTicketModel
    {
        public string ticket { get; set; }
        public int expires_in { get; set; }

        public string errmsg { get; set; }

        public int errcode { get; set; }
    }
}
