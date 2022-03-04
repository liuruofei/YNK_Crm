using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADT.Common;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers.Sys
{
    [Authorize]
    public class ChangePwdController : BaseController
    {
        private ISys_UserService _sys_UserService;
        public ChangePwdController(ISys_UserService sys_UserService)
        {
            _sys_UserService = sys_UserService;
        }
        protected override void Init()
        {
            this.MenuID = "Z-150";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("Z-150", FunctionEnum.Have)]
        public IActionResult Index()
        {
            var model = _sys_UserService.FindByClause(p => p.User_ID == account.UserID);
            ViewData["username"] = model.User_Name;
            return View();
        }

        [HttpPost]
        [UsersRoleAuthFilter("Z-150", FunctionEnum.Edit)]
        public IActionResult ChangePwd(string oldpwd, string newpwd, string newlypwd)
        {
            if (string.IsNullOrEmpty(oldpwd))
                return Json(new { code = 100, msg = "旧密码不能为空" });
            if (string.IsNullOrEmpty(newpwd))
                return Json(new { code = 100, msg = "新密码不能为空" });
            if (string.IsNullOrEmpty(newlypwd))
                return Json(new { code = 100, msg = "确认新密码不能为空" });
            if (!newpwd.Equals(newlypwd))
                return Json(new { code = 100, msg = "两次密码不一致" });
            var _Sys_UserM = _sys_UserService.FindByClause(p => p.User_ID == account.UserID);
            if (!_Sys_UserM.User_Pwd.Equals(Tools.MD5Encryption(oldpwd.Trim())))
                return Json(new { code = 100, msg = "旧密码不正确" });
            _Sys_UserM.User_Pwd = Tools.MD5Encryption(newpwd);
            var result = _sys_UserService.Update(_Sys_UserM);
            if (!result)
            {
                return Json(new { code = 100, msg = "修改密码失败" });
            }
            return Json(new { code = 200, msg = "修改密码成功" });
        }
    }
}
