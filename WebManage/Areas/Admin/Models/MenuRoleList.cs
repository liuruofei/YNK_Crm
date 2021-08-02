using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Areas.Admin.Models
{
    public class MenuRoleList
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<MenuList> children { get; set; }
        public CheckArr checkArr { get; set; }
    }
    public class CheckArr
    {
        public string type { get; set; }
        public string @checked { get; set; }
    }
}
