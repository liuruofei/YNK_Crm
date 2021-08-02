using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.Sys
{
    public class MenuRoleList
    {
        public string id { get; set; }
        public string title { get; set; }
        public string parentId { get; set; }
        public object basicData { get; set; }
        public string disabled { get; set; }
        public List<MenuRoleList> children { get; set; }
        public List<CheckArr> checkArr { get; set; }
    }
    public class CheckArr
    {
        public string type { get; set; }
        public string isChecked { get; set; }
    }
}
