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
        private ICurrencyService _currencyService;
        public LoginController(IOptions<JwtSettings> _jwtSettingsAccess,ICurrencyService currencyService)
        {
            _jwtSettings = _jwtSettingsAccess.Value;
            _currencyService = currencyService;
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            return View();
        }

    }
}
