using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ADT.Common;
using ADT.Models;
using ADT.Service.IService;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebManage;
using WebManage.Models.Res;

namespace WebManage.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, "RollingLogFileAppender");
        private JwtSettings _jwtSettings;
        private IYS_Account_UserService _account_UserService;
        private ICurrencyService _currencyService;
        public LoginController(IOptions<JwtSettings> _jwtSettingsAccess, IYS_Account_UserService account_UserService, ICurrencyService currencyService)
        {
            _jwtSettings = _jwtSettingsAccess.Value;
            _account_UserService = account_UserService;
            _currencyService = currencyService;
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                return Json(new { status = 0, msg = "请输入用户名"});
            if (string.IsNullOrEmpty(password))
                return Json(new { status = 0, msg = "请输入密码！" });
            var _Account_User = _account_UserService.FindByClause(p => p.Account_Name == username);
            if (_Account_User == null)
                return Json(new { status = 0, msg = "账号不存在！" });
            if (_Account_User.Account_Pwd.ToStr().Trim() != Tools.MD5Encryption(password))
                return Json(new { status = 0, msg = "密码错误！" });
            var claim = new Claim[]{
                    new Claim("ID",_Account_User.Account_Uid.ToStr()),
                    new Claim("UserName",_Account_User.Account_Name)
                };
            //对称秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            //签名证书(秘钥，加密算法)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //生成token  [注意]需要nuget添加Microsoft.AspNetCore.Authentication.JwtBearer包，并引用System.IdentityModel.Tokens.Jwt命名空间
            var token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claim, DateTime.Now, DateTime.Now.AddHours(2), creds);
            string tokens = new JwtSecurityTokenHandler().WriteToken(token);
            HttpContext.Response.Cookies.Append("token", tokens, new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddHours(2)
            });
            CacheHelper.Set(_Account_User.Account_Name, _Account_User.Account_Uid, DateTimeOffset.Now.AddMinutes(120));
            AccountUserModel model = new AccountUserModel();
            model.apptoken=tokens;
            model.Account_Name = _Account_User.Account_Name;
            model.Mobile = _Account_User.Mobile;
            return Json(new { status = 200, msg = "登录成功", data = model });
        }

    }
}
