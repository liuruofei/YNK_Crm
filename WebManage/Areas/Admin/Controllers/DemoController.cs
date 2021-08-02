using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers
{
    public class DemoController : BaseController
    {
        protected override void Init()
        {
            this.IsExecutePowerLogic = false;
            base.Init();
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Find()
        {
            return Json(new { img_url = "/assets/images/head.png", contents = "123456" });
        }
    }
}
