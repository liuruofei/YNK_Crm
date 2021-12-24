using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class ContraCenterController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public ContraCenterController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "C-159";
        }
        [UsersRoleAuthFilter("C-159", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("C-159", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            PageList<C_ContraCenter> pageModel = new PageList<C_ContraCenter>();
            var list = _currencyService.DbAccess().Queryable<C_ContraCenter>().OrderBy(c => c.CreateTime).Select<C_ContraCenter>().ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("C-159", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("C-159", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("C-159", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_ContraCenter>().Where(f => f.Id == ID).First();
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
        [UsersRoleAuthFilter("C-159", "Add,Edit")]
        public IActionResult SaveInfo(C_ContraCenter vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.ContraCenter_Name))
                    return Json(new { code = 0, msg = "合同中心名称不能为空" });
                if (vmodel.Id > 0)
                {
                    C_ContraCenter caps = _currencyService.DbAccess().Queryable<C_ContraCenter>().Where(f => f.Id == vmodel.Id).First();
                    caps.ContraCenter_Name = vmodel.ContraCenter_Name;
                    caps.UpdateUid = userId;
                    var result = _currencyService.DbAccess().Updateable<C_ContraCenter>(caps).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
                else
                {
                    vmodel.CreateTime = DateTime.Now;
                    vmodel.CreateUid = userId;
                    vmodel.Status = 0;
                    var result = _currencyService.DbAccess().Insertable<C_ContraCenter>(vmodel).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "保存成功";
                    }
                }
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }


        public IActionResult Delete(int Id) {
            ResResult rsg = new ResResult() { code = 0, msg = "执行删除合同中心失败" };

            var anyContrc=_currencyService.DbAccess().Queryable<C_Contrac>().Where(v => v.ContraCenterId == Id).First();
            if (anyContrc == null)
            {
                var result = _currencyService.DbAccess().Deleteable<C_ContraCenter>().Where(k => k.Id == Id).ExecuteCommand();
                if (result > 0)
                {
                    rsg.code = 200;
                    rsg.msg = "删除成功";
                }
            }
            else {
                rsg.code = 0;
                rsg.msg = "该合同中心已被使用,无法删除";
            }
             
            return Json(rsg);
        }
    }
}
