using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ADT.Models;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebManage.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private ISys_MenuService _sys_MenuService;
        private ISys_RoleMenuFunctionService _Sys_RoleMenuFunctionService;
        public HomeController(ISys_MenuService sys_MenuService, ISys_RoleMenuFunctionService Sys_RoleMenuFunctionService)
        {
            _sys_MenuService = sys_MenuService;
            _Sys_RoleMenuFunctionService = Sys_RoleMenuFunctionService;
        }

        protected override void Init()
        {
            this.MenuID = "Home";
            base.Init();
        }
        public IActionResult Index()
        {
            ViewBag.UserName = account.UserName;
            return View();
        }

        public IActionResult Welcome()
        {
            //System.Threading.Thread.Sleep(3 * 1000);
            return View();
        }

        /// <summary>
        /// 获取菜单信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMenu()
        {
            var menu_list = _sys_MenuService.FindListByClause(p => p.Menu_IsShow == 1, "Menu_Num asc");
            List<MenuList> lists = new List<MenuList>();
            var rolemenu = new List<sys_rolemenufunction>();
            if (!account.IsSuperManage)
                rolemenu = (List<sys_rolemenufunction>)_Sys_RoleMenuFunctionService.FindListByClause(p => p.RoleMenuFunction_RoleID == account.RoleID && p.RoleMenuFunction_FunctionID == "C9518758-B2E1-4F51-B517-5282E273889C", "");
            foreach (var item in menu_list.Where(p => string.IsNullOrEmpty(p.Menu_ParentID)))
            {
                if (!account.IsSuperManage)
                {
                    if (rolemenu.Find(p => p.RoleMenuFunction_MenuID == item.Menu_ID) == null)
                    {
                        continue;
                    }
                }
                MenuList menuList = new MenuList();
                menuList.name = item.Menu_Name;
                menuList.icon = item.Menu_Icon;
                menuList.url = item.Menu_Url;
                menuList.subMenus = BindMenu(menu_list, rolemenu, item.Menu_ID);
                lists.Add(menuList);
            }
            //lists = menu_list.Where(p => string.IsNullOrEmpty(p.Menu_ParentID)).Select(p => new MenuList { name = p.Menu_Name, icon = p.Menu_Icon, url = p.Menu_Url, subMenus = BindMenu() });
            return Json(lists);
        }

        public List<MenuList> BindMenu(IEnumerable<sys_menu> sys_Menus, List<sys_rolemenufunction> rolemenu, string parentId)
        {
            List<MenuList> lists = new List<MenuList>();
            foreach (var item in sys_Menus.Where(p => p.Menu_ParentID == parentId))
            {
                if (!account.IsSuperManage)
                {
                    if (rolemenu.Find(p => p.RoleMenuFunction_MenuID == item.Menu_ID) == null)
                    {
                        continue;
                    }
                }
                MenuList menuList = new MenuList();
                menuList.name = item.Menu_Name;
                menuList.icon = item.Menu_Icon;
                menuList.url = item.Menu_Url;
                menuList.subMenus = BindMenu(sys_Menus, rolemenu, item.Menu_ID);
                lists.Add(menuList);
            }
            return lists;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult TplTheme()
        {
            return View();
        }
    }
}
