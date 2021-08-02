using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Areas.Admin.Models
{
    /// <summary>
    /// 菜单页面左侧树形菜单
    /// </summary>
    public class MenuList
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<MenuList> children { get; set; }
    }
}
