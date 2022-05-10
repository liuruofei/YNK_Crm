using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Models.Res;

namespace WebManage.Controllers
{
    public class WxComentController : Controller
    {
        private ICurrencyService _currencyService;
        public WxComentController(ICurrencyService currencyService)
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
        /// 获取点评列表
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetComentList(string openId, int page, int limit)
        {
            int total = 0;
            PageList<C_Course_Work> pageModel = new PageList<C_Course_Work>();
            List<C_Course_Work> list = new List<C_Course_Work>();
            var cuser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(cu => cu.OpenId.Equals(openId) || cu.Elder_OpenId.Equals(openId) || cu.Elder2_OpenId.Equals(openId)).First();
            if (cuser != null)
            {
                List<int> listClass = _currencyService.DbAccess().Queryable<C_Contrac_Child>().Where(cu => cu.StudentUid == cuser.StudentUid && cu.StudyMode == 2).Select<int>(cu => cu.ClassId).ToList();
                list = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(it => (it.StudentUid == cuser.StudentUid || listClass.Contains(it.ClasssId)) && !string.IsNullOrEmpty(it.Comment)).OrderBy(it => it.Comment_Time, OrderByType.Desc).ToPageList(page, limit, ref total);
            }
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


    }
}
