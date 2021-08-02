using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ADT.Models;
using ADT.Common;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers.Sys
{
    [Authorize]
    public class RoleController : BaseController
    {
        private ISys_RoleService _sys_RoleService;
        private ISys_MenuService _sys_MenuService;
        private ISys_RoleMenuFunctionService _sys_RoleMenuFunctionService;
        public RoleController(ISys_RoleService sys_RoleService, ISys_MenuService sys_MenuService, ISys_RoleMenuFunctionService sys_RoleMenuFunctionService)
        {
            _sys_RoleService = sys_RoleService;
            _sys_MenuService = sys_MenuService;
            _sys_RoleMenuFunctionService = sys_RoleMenuFunctionService;
        }
        protected override void Init()
        {
            this.MenuID = "Z-110";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("Z-110", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("Z-110", FunctionEnum.Have)]
        public IActionResult GetDataSource(int page = 1, int limit = 10)
        {
            var list = _sys_RoleService.GetPageList(null, page, limit, "Role_Num asc,Role_CreateTime desc");
            return Json(list);
        }
        [UsersRoleAuthFilter("Z-110", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = "";
            return View();
        }

        [UsersRoleAuthFilter("Z-110", FunctionEnum.Edit)]
        public IActionResult Edit(string ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("Z-110", "Add,Edit")]
        public IActionResult Find(string ID)
        {
            dynamic list;
            if (!string.IsNullOrEmpty(ID))
            {
                list = _sys_RoleService.FindByClause(p => p.Role_ID == ID);
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("Z-110", FunctionEnum.Add)]
        public IActionResult AddSave(sys_role vmodel)
        {
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.Role_ID))
                {
                    if (string.IsNullOrEmpty(vmodel.Role_Name))
                        return Json(new { code = 100, msg = "请输入角色名" });
                    vmodel.Role_ID = Guid.NewGuid().ToString();
                    vmodel.Role_CreateTime = DateTime.Now;
                    vmodel.Role_IsDelete = 1;
                    _sys_RoleService.Insert(vmodel);
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
        [UsersRoleAuthFilter("Z-110", FunctionEnum.Edit)]
        public IActionResult EditSave(sys_role vmodel)
        {
            if (!string.IsNullOrEmpty(vmodel.Role_ID))
            {
                if (string.IsNullOrEmpty(vmodel.Role_Name))
                    return Json(new { code = 100, msg = "请输入角色名" });
                var model = _sys_RoleService.FindByClause(p => p.Role_ID == vmodel.Role_ID);
                if (model != null)
                {
                    _sys_RoleService.UpdateWhere(vmodel, p => new { p.Role_Num, p.Role_Name, p.Role_Remark }, p => p.Role_ID == vmodel.Role_ID);
                }
                else
                {
                    return Json(new { code = 100, msg = "没有该角色" });
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
        [UsersRoleAuthFilter("Z-110", FunctionEnum.Delete)]
        public IActionResult Delete(string id)
        {
            var list = JsonConvert.DeserializeObject<List<string>>(id);
            foreach (var item in list)
            {
                var model = _sys_RoleService.FindByClause(p => p.Role_ID == item);
                if (model == null)
                    return Json(new { code = 100, msg = "该橘色不存在" });
                if (model.Role_IsDelete == 2)
                    return Json(new { code = 100, msg = "该角色不能删除" });
                _sys_RoleService.Delete(p => p.Role_ID == item);
            }
            return Json(new { code = 200, msg = "删除成功" });
        }

        /// <summary>
        /// 展示角色权限页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("Z-110", FunctionEnum.Edit)]
        public IActionResult ShowRoleList(string id)
        {
            ViewBag.ID = id;
            return View();
        }

        /// <summary>
        /// 获取角色菜单功能
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [UsersRoleAuthFilter("Z-110", FunctionEnum.Edit)]
        public IActionResult GetRoleMenuFunctionTree(string roleid)
        {
            //return Json(new { status = 1, value = _sys_MenuService.GetRoleMenuFunctionTree(roleid) });
            return Json(new
            {
                status = new { code = 200, message = "操作成功" },
                data = _sys_MenuService.GetRoleMenuFunctionTree(roleid)
            });
        }

        [HttpPost]
        [UsersRoleAuthFilter("Z-110", FunctionEnum.Edit)]
        public IActionResult SaveFunction(string rows, string roleid)
        {
            if (roleid.ToGuid() == Guid.Empty)
                return Json(new { code = 100, msg = "请选择角色" });
            var rowslist = JsonConvert.DeserializeObject<List<sys_menufunction>>(rows);
            _sys_RoleMenuFunctionService.Delete(p => p.RoleMenuFunction_RoleID == roleid);
            if (rowslist.Count > 0)
            {
                var list = new List<sys_rolemenufunction>();
                var mfList = rowslist.GroupBy(p => new { p.MenuFunction_MenuID, p.MenuFunction_FunctionID });
                foreach (var item in mfList)
                {
                    var model = new sys_rolemenufunction();
                    model.RoleMenuFunction_ID = Guid.NewGuid().ToString();
                    model.RoleMenuFunction_RoleID = roleid;
                    model.RoleMenuFunction_MenuID = item.Key.MenuFunction_MenuID;
                    model.RoleMenuFunction_FunctionID = item.Key.MenuFunction_FunctionID;
                    model.RoleMenuFunction_CreateTime = DateTime.Now;
                    list.Add(model);
                }
                var result = _sys_RoleMenuFunctionService.InsertBatch(list);
                if (result > 0)
                {
                    RegisterRoleMenu.AddRoleMenu();
                    return Json(new { code = 200, msg = "操作成功" });
                }
            }
            else
            {
                RegisterRoleMenu.AddRoleMenu();
                return Json(new { code = 200, msg = "操作成功" });
            }
            return Json(new { code = 100, msg = "设置失败" });
        }
    }
}
