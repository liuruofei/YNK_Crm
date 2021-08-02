using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models
{
    public class MenuList
    {
        public string name { get; set; }
        public string icon { get; set; }
        public string url { get; set; }
        public List<MenuList> subMenus { get; set; }
    }
}
