using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    public class sys_menulist : sys_menu
    {
        public string Parent_Name { get; set; }
        public string ShowName {
            get
            {
                return Menu_IsShow == 2 ? "隐藏" : "显示";
            }
        }
    }
}
