using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ADT.Models;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ADT.Common;
using Newtonsoft.Json;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers.Sys
{
    [Authorize]
    public class MenuFunctionController : BaseController
    {
        private ISys_MenuService _sys_MenuService;
        private ISys_FunctionService _sys_FunctionService;
        private ISys_MenuFunctionService _sys_MenuFunctionService;
        public MenuFunctionController(ISys_MenuService sys_MenuService, ISys_FunctionService sys_FunctionService, ISys_MenuFunctionService sys_MenuFunctionService)
        {
            _sys_MenuService = sys_MenuService;
            _sys_FunctionService = sys_FunctionService;
            _sys_MenuFunctionService = sys_MenuFunctionService;
        }
        protected override void Init()
        {
            this.MenuID = "Z-130";
        }

        // GET: /<controller>/
        [UsersRoleAuthFilter("Z-130", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("Z-130", FunctionEnum.Have)]
        public IActionResult GetDataSource(string Menu_ID, string Menu_Name, int page = 1, int limit = 10)
        {
            Expression<Func<sys_menu, bool>> exp = m => 1 == 1;
            if (!string.IsNullOrEmpty(Menu_Name))
                exp = exp.And(m => m.Menu_Name == Menu_Name);
            if (Menu_ID == null)
                exp = exp.And(m => m.Menu_ParentID == null);
            else
                exp = exp.And(m => m.Menu_ParentID == Menu_ID);
            var list = _sys_MenuService.GetMenuPageList(exp, page, limit, "m.Menu_Num asc");
            return Json(list);
        }
        /// <summary>
        /// 获取菜单和功能树
        /// </summary>
        /// <returns></returns>
        [UsersRoleAuthFilter("Z-130", FunctionEnum.Have)]
        public IActionResult GetMenuAndFunctionTree()
        {
            var list = _sys_MenuService.GetMenuAndFunctionTree();
            List<MenuList> menuList = new List<MenuList>();
            foreach (var item in list.Where(p => p.pId == null))
            {
                MenuList menu = new MenuList();
                menu.id = item.id;
                menu.title = item.name;
                BindMenuList(list, menu);
                menuList.Add(menu);
            }
            return Json(new { status = new { code = 200, message = "操作成功" }, data = new List<MenuList>() { new MenuList() { id = "", title = "管理后台", children = menuList } } });
        }

        public void BindMenuList(List<sys_menulistfunc> list, MenuList model)
        {
            var newList = list.Where(p => p.pId == model.id);
            List<MenuList> menuList = new List<MenuList>();
            foreach (var item in newList)
            {
                MenuList menu = new MenuList();
                menu.id = item.id;
                menu.title = item.name;
                BindMenuList(list, menu);
                menuList.Add(menu);
            }
            model.children = menuList;
        }
        [UsersRoleAuthFilter("Z-130", FunctionEnum.Add)]
        public IActionResult Add(string pId)
        {
            ViewBag.ID = "";
            ViewBag.PID = pId;
            var list = _sys_FunctionService.FindListByClause(null, "Function_Num asc");
            ViewData["list"] = list;
            return View();
        }

        [UsersRoleAuthFilter("Z-130", FunctionEnum.Edit)]
        public IActionResult Edit(string ID, string pId)
        {
            ViewBag.ID = ID;
            ViewBag.PID = pId;
            var list = _sys_FunctionService.FindListByClause(null, "Function_Num asc");
            ViewData["list"] = list;
            return View("Add");
        }

        [UsersRoleAuthFilter("Z-130", "Add,Edit")]
        public IActionResult Find(string ID)
        {
            dynamic list;
            if (!string.IsNullOrEmpty(ID))
            {
                var model = _sys_MenuService.FindByClause(p => p.Menu_ID == ID);
                //list = 
                var menu = _sys_MenuService.FindByClause(p => p.Menu_ID == model.Menu_ParentID);
                var Menu_Power = _sys_MenuFunctionService.FindListByClause(p => p.MenuFunction_MenuID == ID, "MenuFunction_CreateTime asc").Select(p => p.MenuFunction_FunctionID).ToList();
                var json = Tools.EntityToDictionary(new Dictionary<string, object>()
                {
                    {"model",model },
                    {"pname",menu!=null?menu.Menu_Name:""},
                    {"Menu_Power",Menu_Power }
                });
                list = json;
            }
            else
            {
                var model = new sys_menu();
                var json = Tools.EntityToDictionary(new Dictionary<string, object>()
                {
                    {"sss",model },
                    {"pname",""},
                    {"Menu_Power",new string[]{ } }
                });
                list = json;
            }
            return Json(list);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("Z-130", FunctionEnum.Add)]
        public IActionResult AddSave(sys_menu vmodel, string Function_ID)
        {
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.Menu_ID))
                {
                    if (string.IsNullOrEmpty(vmodel.Menu_Num))
                        return Json(new { code = 100, msg = "请输入菜单编号" });
                    if (string.IsNullOrEmpty(vmodel.Menu_Name))
                        return Json(new { code = 100, msg = "请输入菜单名称" });
                    var FunctionList = JsonConvert.DeserializeObject<List<string>>(Function_ID);
                    vmodel.Menu_ID = Guid.NewGuid().ToString();
                    vmodel.Menu_CreateTime = DateTime.Now;
                    _sys_MenuService.Insert(vmodel);
                    foreach (var item in FunctionList)
                    {
                        _sys_MenuFunctionService.Insert(new sys_menufunction()
                        {
                            MenuFunction_ID = Guid.NewGuid().ToString(),
                            MenuFunction_FunctionID = item,
                            MenuFunction_MenuID = vmodel.Menu_ID,
                            MenuFunction_CreateTime = DateTime.Now
                        });
                    }
                }
                else
                {
                    return Json(new { code = 100, msg = "无效参数" });
                }
            }
            else
            {
                return Json(new { code = 100, msg = "无效参数" });
            }
            return Json(new { code = 200, msg = "操作成功" });
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("Z-130", FunctionEnum.Edit)]
        public IActionResult EditSave(sys_menu vmodel, string Function_ID)
        {
            if (!string.IsNullOrEmpty(vmodel.Menu_ID))
            {
                if (string.IsNullOrEmpty(vmodel.Menu_Num))
                    return Json(new { code = 100, msg = "请输入菜单编号" });
                if (string.IsNullOrEmpty(vmodel.Menu_Name))
                    return Json(new { code = 100, msg = "请输入菜单名称" });
                var FunctionList = JsonConvert.DeserializeObject<List<string>>(Function_ID);
                _sys_MenuService.UpdateWhere(vmodel, p => new { p.Menu_Num, p.Menu_Name, p.Menu_Url, p.Menu_Icon, p.Menu_ParentID, p.Menu_IsShow }, p => p.Menu_ID == vmodel.Menu_ID);
                _sys_MenuFunctionService.Delete(p => p.MenuFunction_MenuID == vmodel.Menu_ID);
                foreach (var item in FunctionList)
                {
                    _sys_MenuFunctionService.Insert(new sys_menufunction()
                    {
                        MenuFunction_ID = Guid.NewGuid().ToString(),
                        MenuFunction_FunctionID = item,
                        MenuFunction_MenuID = vmodel.Menu_ID,
                        MenuFunction_CreateTime = DateTime.Now
                    });
                }
            }
            else
            {
                return Json(new { code = 100, msg = "参数无效" });
            }
            return Json(new { code = 200, msg = "操作成功" });
        }

        /// <summary>
        /// 根据ID集合删除信息
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("Z-130", FunctionEnum.Delete)]
        public IActionResult Delete(string id)
        {
            var list = JsonConvert.DeserializeObject<List<string>>(id);
            var result = _sys_MenuService.Delete(p => list.Contains(p.Menu_ID));
            if (result)
            {
                _sys_MenuFunctionService.Delete(p => list.Contains(p.MenuFunction_MenuID));
                return Json(new { code = 200, msg = "删除成功" });
            }
            else
                return Json(new { code = 100, msg = "删除失败" });
        }
    }
}
