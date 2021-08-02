using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ADT.Common;
using ADT.Models;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebManage.Models.Res;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers
{
    public class LoginController : BaseController
    {
        private JwtSettings _jwtSettings;
        private ISys_UserService _sys_UserService;
        public LoginController(IOptions<JwtSettings> _jwtSettingsAccess, ISys_UserService sys_UserService)
        {
            _jwtSettings = _jwtSettingsAccess.Value;
            _sys_UserService = sys_UserService;
        }

        protected override void Init()
        {
            this.IsExecutePowerLogic = false;
            base.Init();
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new MessageBox("请输入用户名！");
            if (string.IsNullOrEmpty(password))
                throw new MessageBox("请输入密码！");

            var _Sys_User = _sys_UserService.FindByClause(p => p.User_LoginName == username);
            if (_Sys_User == null)
                throw new MessageBox("账号不存在");
            if (_Sys_User.User_ID.ToGuid() == Guid.Empty)
                throw new MessageBox("帐户不存在");
            if (_Sys_User.User_Pwd.ToStr().Trim() != Tools.MD5Encryption(password))
                throw new MessageBox("密码错误");
            var claim = new Claim[]{
                    new Claim("ID",_Sys_User.User_ID.ToStr()),
                    new Claim("UserName",_Sys_User.User_Name)
                };

            //对称秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            //签名证书(秘钥，加密算法)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //生成token  [注意]需要nuget添加Microsoft.AspNetCore.Authentication.JwtBearer包，并引用System.IdentityModel.Tokens.Jwt命名空间
            var token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claim, DateTime.Now, DateTime.Now.AddYears(1), creds);
            string tokens = new JwtSecurityTokenHandler().WriteToken(token);
            HttpContext.Response.Cookies.Append("authtoken", tokens, new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddYears(1)
            });
            CacheHelper.Set(_Sys_User.User_ID, "1", DateTimeOffset.Now.AddMinutes(120));
            return Json(new { status = 200, msg = "成功", data = tokens });
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public IActionResult Out()
        {
            HttpContext.Response.Cookies.Delete("authtoken");
            HttpContext.Response.Cookies.Delete(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index");
        }
    }
}
