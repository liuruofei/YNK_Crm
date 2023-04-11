﻿using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Models.Res;

namespace WebManage.Controllers
{
    public class WxTemplateChildController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private ICurrencyService _currencyService;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(WxTemplateChildController));
        public WxTemplateChildController(ICurrencyService currencyService, IHostingEnvironment hostingEnvironment) {
            _currencyService = currencyService;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Zuoye() {
            return View();
        }

        public IActionResult SummaryDetail() {
            return View();
        }


        [HttpPost]
        public IActionResult GetWorkModel(int wkId) {
            ResResult rg = new ResResult() { code = 0, msg = "获取失败" };
            try
            {
                C_Course_Work work = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(v => v.Id == wkId).First();
                if (work != null)
                {
                    WxHomeWorkModel homeModel = new WxHomeWorkModel();
                    homeModel.Comment = work.Comment;
                    homeModel.Work_Title = work.Work_Title;
                    homeModel.HomeWorkComent = work.CourseWork;
                    rg.msg = "获取成功";
                    rg.data = homeModel;
                    rg.code = 200;
                }
            }
            catch (Exception er) {
                log.Info(er.Message);
            }
            return Json(rg);
        }
        [HttpPost]
        public IActionResult GetSummaryInfo(int Id) {
            ResResult rg = new ResResult() { code = 0, msg = "获取失败" };
            try
            {
                C_Summary sum = _currencyService.DbAccess().Queryable<C_Summary>().Where(v => v.Id ==Id).First();
                if (sum != null)
                {
                    rg.msg = "获取成功";
                    rg.data = sum;
                    rg.code = 200;
                }
            }
            catch (Exception er)
            {
                log.Info(er.Message);
            }
            return Json(rg);
        }
    }
}
