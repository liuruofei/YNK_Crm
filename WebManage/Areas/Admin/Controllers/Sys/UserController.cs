using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADT.Service.IService;
using Microsoft.AspNetCore.Mvc;
using ADT.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using ADT.Common;
using System.Linq.Expressions;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using ADT.Models.ResModel;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers.Sys
{
    [Authorize]
    public class UserController : BaseController
    {
        private ICurrencyService _currencyService;
        private ISys_UserService _UserService;
        private Iview_sys_user_roleService _view_sys_user_roleService;
        private ISys_RoleService _sys_RoleService;
        private ISys_UserRoleService _sys_UserRoleService;
        public UserController(ISys_UserService sys_UserService, Iview_sys_user_roleService view_sys_user_roleService, ISys_RoleService sys_RoleService, ISys_UserRoleService sys_UserRoleService, ICurrencyService currencyService)
        {
            _UserService = sys_UserService;
            _view_sys_user_roleService = view_sys_user_roleService;
            _sys_RoleService = sys_RoleService;
            _sys_UserRoleService = sys_UserRoleService;
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "Z-100";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("Z-100", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("Z-100", FunctionEnum.Have)]
        public IActionResult GetDataSource(string searchKey, string searchValue, int page = 1, int limit = 10)
        {
            Expression<Func<view_sys_user_role, bool>> exp = p => 1 == 1;
            if (searchKey == "loginname" && !string.IsNullOrEmpty(searchValue))
                exp = exp.And(p => p.User_LoginName.Contains(searchValue));
            else if (searchKey == "username" && !string.IsNullOrEmpty(searchValue))
                exp = exp.And(p => p.User_Name.Contains(searchValue));
            var list = _view_sys_user_roleService.GetPageList(exp, page, limit, "User_CreateTime asc");
            return Json(list);
        }
        [UsersRoleAuthFilter("Z-100", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = "";
            ViewBag.RoleList = _sys_RoleService.FindListByClause(null, "Role_CreateTime asc");
            return View();
        }

        [UsersRoleAuthFilter("Z-100", FunctionEnum.Edit)]
        public IActionResult Edit(string ID)
        {
            ViewBag.ID = ID;
            ViewBag.RoleList = _sys_RoleService.FindListByClause(null, "Role_CreateTime asc");
            return View("Add");
        }

        [UsersRoleAuthFilter("Z-100", "Add,Edit")]
        public IActionResult Find(string ID)
        {
            dynamic list;
            if (!string.IsNullOrEmpty(ID))
            {
                list = _view_sys_user_roleService.FindByClause(p => p.User_ID == ID);
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
        [UsersRoleAuthFilter("Z-100", FunctionEnum.Add)]
        public IActionResult AddSave(view_sys_user_role vmodel)
        {
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.User_ID))
                {
                    if (string.IsNullOrEmpty(vmodel.User_Name))
                        return Json(new { code = 100, msg = "请输入用户名" });
                    if (string.IsNullOrEmpty(vmodel.User_LoginName))
                        return Json(new { code = 100, msg = "请输入登录名" });
                    if (string.IsNullOrEmpty(vmodel.Role_ID))
                        return Json(new { code = 100, msg = "请选择角色名称" });
                    vmodel.User_ID = Guid.NewGuid().ToString();
                    vmodel.User_Pwd = Tools.MD5Encryption(vmodel.User_Pwd);
                    vmodel.User_CreateTime = DateTime.Now;
                    _UserService.InsertUsers(vmodel);
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
        [UsersRoleAuthFilter("Z-100", FunctionEnum.Edit)]
        public IActionResult EditSave(view_sys_user_role vmodel)
        {
            if (!string.IsNullOrEmpty(vmodel.User_ID))
            {
                if (string.IsNullOrEmpty(vmodel.User_Name))
                    return Json(new { code = 100, msg = "请输入用户名" });
                if (string.IsNullOrEmpty(vmodel.User_LoginName))
                    return Json(new { code = 100, msg = "请输入登录名" });
                if (string.IsNullOrEmpty(vmodel.Role_ID))
                    return Json(new { code = 100, msg = "请选择角色名称" });
                var model = _view_sys_user_roleService.FindByClause(p => p.User_ID == vmodel.User_ID);
                if (model != null)
                {
                    if (!string.IsNullOrEmpty(vmodel.User_Pwd)&&vmodel.User_Pwd!=model.User_Pwd)
                        vmodel.User_Pwd = Tools.MD5Encryption(vmodel.User_Pwd);
                    else
                        vmodel.User_Pwd = model.User_Pwd;
                    _UserService.UpdateUsers(vmodel);
                    if (!string.IsNullOrEmpty(vmodel.Role_ID)) {
                        var userRole = _currencyService.DbAccess().Queryable<sys_userrole>().Where(c => c.UserRole_ID==model.UserRole_ID).First();
                        userRole.UserRole_RoleID = vmodel.Role_ID;
                        _sys_UserRoleService.Update(userRole);
                    }
                }
                else
                {
                    return Json(new { code = 100, msg = "没有该用户" });
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
        [UsersRoleAuthFilter("Z-100", FunctionEnum.Delete)]
        public IActionResult Delete(string id)
        {
            var idList = JsonConvert.DeserializeObject<List<string>>(id);
            foreach (var item in idList)
            {
                var model = _UserService.FindByClause(p => p.User_ID == item);
                if (model == null)
                    return Json(new { code = 100, msg = "该用户不存在" });
                if (model.User_IsDelete == 2)
                    return Json(new { code = 100, msg = "该用户不能删除" });
                _UserService.Delete(p => p.User_ID == model.User_ID);
                _sys_UserRoleService.Delete(p => p.UserRole_UserID == model.User_ID);
            }
            return Json(new { status = 1 });
        }

        /// <summary>
        /// 获取校区列表
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryCampus()
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<C_Campus>().Where(n => n.Status < 1).ToList();
            return Json(rsg);
        }
    }
}
