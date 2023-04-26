using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Controllers
{
    public class CommunicateController : Controller
    {
        private ICurrencyService _currencyService;
        public CommunicateController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult child()
        {
            return View();
        }


        /// <summary>
        /// 获取沟通纪要列表
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCommunicateList(string openId, int page, int limit)
        {
            int total = 0;
            PageList<C_Summary> pageModel = new PageList<C_Summary>();
            List<C_Summary> list = new List<C_Summary>();
            var cuser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(cu => cu.OpenId.Equals(openId) || cu.Elder_OpenId.Equals(openId) || cu.Elder2_OpenId.Equals(openId)).First();
            if (cuser != null)
            {
                list = _currencyService.DbAccess().Queryable<C_Summary>().Where(it => it.openIds.Contains(openId)).OrderBy(it => it.CreateTime, OrderByType.Desc).ToPageList(page, limit, ref total);
            }
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }
        [HttpPost]
        public IActionResult GetCommunicateModel(int id) {
            ResResult rsg = new ResResult() { code = 0, msg = "获取失败" };
            C_Summary model = _currencyService.DbAccess().Queryable<C_Summary>().Where(ilv => ilv.Id == id).First();
            if (model != null)
            {
                rsg.data = model;
                rsg.code = 200;
                rsg.msg = "获取成功";
            }
            return Json(rsg);
        }
    }
}
