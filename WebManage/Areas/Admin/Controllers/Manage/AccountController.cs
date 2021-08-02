using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ADT.Models;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ADT.Common;
using System.Data;
using SqlSugar;
using System.Drawing;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ISys_UserRoleService _sys_UserRoleService;
        private ISys_RoleService _sys_RoleService;
        private IYS_User_EnterService _ys_User_EnterService;
        private IYS_Account_UserService _ys_Account_UserService;
        public AccountController(IYS_Account_UserService ys_Account_UserService, IYS_User_EnterService ys_User_EnterService, ISys_UserRoleService sys_UserRoleService, ISys_RoleService sys_RoleService)
        {
            _ys_Account_UserService = ys_Account_UserService;
            _ys_User_EnterService = ys_User_EnterService;
            _sys_UserRoleService = sys_UserRoleService;
            _sys_RoleService = sys_RoleService;
        }
        protected override void Init()
        {
            this.MenuID = "A-101";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("A-101", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("A-101", FunctionEnum.Have)]
        public IActionResult GetDataSource(string txtphone, string txtnickname, int page = 1, int limit = 10)
        {
            var adminUid = this.User.Claims.Where(n => n.Type == "ID").FirstOrDefault().Value;
            var sysUserRole = _sys_UserRoleService.FindByClause(n => n.UserRole_UserID.Equals(adminUid));
            Expression<Func<YS_Account_User, bool>> exp = p => true;
            if (!string.IsNullOrEmpty(txtphone))
                exp = exp.And(p => p.Mobile.Contains(txtphone));
            if (!string.IsNullOrEmpty(txtnickname))
                exp = exp.And(p => p.Account_Name.Contains(txtnickname));
            exp = exp.And(p => p.Status == 0);
            if (sysUserRole != null) {
                var sysRole = _sys_RoleService.FindById(sysUserRole.UserRole_RoleID);
                if(!sysRole.Role_Name.Contains("超级管理员"))
                exp = exp.And(p => p.CreateId.Equals(sysUserRole.UserRole_UserID));
            }
            var list = _ys_Account_UserService.GetPageList(exp, page, limit, "CreateTime desc");
            return Json(list);
        }


        [UsersRoleAuthFilter("A-101", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("A-101", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            YS_Account_User model = _ys_Account_UserService.FindByClause(f => f.Account_Uid == ID && f.Status == 0);
            return View("Add");
        }

        [UsersRoleAuthFilter("A-101", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _ys_Account_UserService.FindByClause(f => f.Account_Uid == ID);
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
        [UsersRoleAuthFilter("A-101", "Add,Edit")]
        public IActionResult SaveInfo(YS_Account_User vmodel)
        {
            int code = 0;
            string msg = string.Empty;
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.Account_Name))
                    return Json(new { code = 0, msg = "用户名不能为空" });
                if (string.IsNullOrEmpty(vmodel.Account_Pwd))
                    return Json(new { code = 0, msg = "密码不能为空" });
                var info = _ys_Account_UserService.FindByClause(f => f.Account_Name == vmodel.Account_Name && f.Status == 0);
                if (vmodel.Account_Uid > 0)
                {
                    if (info == null)
                    {
                        info = _ys_Account_UserService.FindByClause(f => f.Account_Uid == vmodel.Account_Uid && f.Status == 0);
                    }
                    if (info.Account_Uid != vmodel.Account_Uid)
                    {
                        return Json(new { code = 0, msg = "该账号名称已存在，请重新输入一个名称" });
                    }
                    if (!string.IsNullOrEmpty(info.Mobile) && info.Account_Name != vmodel.Account_Name)
                    {
                        return Json(new { code = 0, msg = "该账号已被绑定，无法修改账户名称" });
                    }
                    if (!info.Account_Pwd.Equals(vmodel.Account_Pwd))
                    {
                        info.Account_Pwd = Tools.MD5Encryption(vmodel.Account_Pwd);
                        info.Pwd_Display = vmodel.Account_Pwd;
                    }
                    info.Account_Name = vmodel.Account_Name;
                    var result = _ys_Account_UserService.Update(info);
                    if (result)
                    {
                        code = 200;
                        msg = "更新成功";
                    }
                }
                else
                {
                    var adminUid = this.User.Claims.Where(n => n.Type == "ID").FirstOrDefault().Value;
                    if (info != null)
                        return Json(new { code = 0, msg = "该账号已存在，请更换其它名称" });
                    vmodel.Pwd_Display = vmodel.Account_Pwd;
                    vmodel.Account_Pwd = Tools.MD5Encryption(vmodel.Account_Pwd);
                    vmodel.CreateTime = DateTime.Now;
                    vmodel.CreateId=adminUid;
                    vmodel.IsUseed = 0;
                    vmodel.Status = 0;
                    var result = _ys_Account_UserService.Insert(vmodel);
                    if (result > 0)
                    {
                        code = 200;
                        msg = "保存成功";
                    }

                }
            }
            else
            {
                return Json(new { code = code, msg = "缺少参数" });
            }
            return Json(new { code = code, msg = msg });
        }


        /// <summary>
        /// 注销账户
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("A-101", FunctionEnum.Delete)]
        public IActionResult Delete(int accountId)
        {
            var model = _ys_Account_UserService.FindByClause(p => p.Account_Uid == accountId);
            if (model != null && !string.IsNullOrEmpty(model.Mobile))
            {
                return Json(new { code = 100, msg = "该账号已被绑定,无法注销" });
            }
            var result = _ys_Account_UserService.Delete(p => p.Account_Uid == accountId);
            if (result)
                return Json(new { code = 200, msg = "注销成功" });
            else
                return Json(new { code = 0, msg = "注销失败" });
        }

    }
}
