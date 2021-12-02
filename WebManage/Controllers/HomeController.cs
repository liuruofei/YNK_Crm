using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ADT.Models;
using ADT.Service.IService;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebManage.Models.Req;
using WebManage.Models.Res;

namespace WebManage.Controllers
{
    public class HomeController : Controller
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public HomeController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            _currencyService = currencyService;
            redisConfig = _redisConfig.Value;
        }

        public IActionResult Index()
        {
            return Redirect("Admin/Login");
        }

        public IActionResult Detail(int id,string type)
        {
            ViewBag.Id =id;
            ViewBag.type = type;
            return View();
        }


        /// <summary>
        /// 课程页面
        /// </summary>
        /// <returns></returns>
        public IActionResult CourseList() {
            return View();
        }

        /// <summary>
        /// 案例页面
        /// </summary>
        /// <returns></returns>
        public IActionResult CaseList() {
            return View();
        }

        /// <summary>
        /// 雅思培训
        /// </summary>
        /// <returns></returns>
        public IActionResult YasiList()
        {
            return View();
        }

        

        public IActionResult About() {
            return View();
        }
    }
}
