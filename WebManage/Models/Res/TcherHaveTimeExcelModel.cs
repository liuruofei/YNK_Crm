using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class TcherHaveTimeExcelModel
    {

        public DateTime? starttime { get; set; }

        public DateTime? endtime { get; set; }

        public List<TcherTimeExcelArr> timeArr { get; set;}
    }
    public class TcherTimeExcelArr {
        public string tcName { get; set; }

        public DateTime? tmDay { get; set; }

        public string tmRange { get; set; }
    }
}
