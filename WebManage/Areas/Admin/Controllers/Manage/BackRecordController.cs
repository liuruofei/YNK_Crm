using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
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
    public class BackRecordController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public BackRecordController(ICurrencyService currencyService, IC_ContracService contrac)
        {
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "L-153";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("L-153", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 退款记录列表
        /// </summary>
        /// <param name="title"></param>
        /// <param name="endTime"></param>
        /// <param name="startTime"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("L-153", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, DateTime? endTime = null, DateTime? startTime = null, int page = 0,int limit = 0)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<BackAmountRecordModel> pageModel = new PageList<BackAmountRecordModel>();
            var list = _currencyService.DbAccess().Queryable<C_BackAmountRecord, C_Contrac_User, sys_user>((bkr, u,sy) => new Object[] { JoinType.Inner, bkr.StudentUid == u.StudentUid, JoinType.Left, bkr.CreateUid == sy.User_ID }).WhereIF(!string.IsNullOrEmpty(title), (bkr, u) => u.Student_Name.Contains(title))
                .Where((bkr, u) => u.CampusId == Convert.ToInt32(campusId))
                .WhereIF(startTime.HasValue, (bkr, u) =>bkr.BackDate>=startTime).WhereIF(endTime.HasValue, (bkr, u) =>bkr.BackDate <= endTime)
                .OrderBy((bkr, u) =>bkr.CreateTime, OrderByType.Desc)
                .Select<BackAmountRecordModel>((bkr, u, sy) => new BackAmountRecordModel
                {
                    Id=bkr.Id,
                    Student_Name=u.Student_Name,
                    StudentUid=bkr.StudentUid,
                    BackAmount=bkr.BackAmount,
                    BackDate=bkr.BackDate.ToString("yyyy-MM-dd"),
                    CreateName=sy.User_Name

                }).ToPageList(page,limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

        /// <summary>
        /// 退款汇总
        /// </summary>
        /// <param name="title"></param>
        /// <param name="endTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public IActionResult TotalAmount(string title, DateTime? endTime = null, DateTime? startTime = null)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 0, msg = "获取失败" };
            DateTime now = DateTime.Now;
            if (!startTime.HasValue)
            {
                startTime = new DateTime(now.Year, now.Month, 1);
            }
            if (!endTime.HasValue)
            {
                endTime = startTime.Value.AddMonths(1).AddDays(-1);
            }
            var result = _currencyService.DbAccess().Queryable<C_BackAmountRecord, C_Contrac_User>((bkr, u) => new Object[] { JoinType.Inner, bkr.StudentUid == u.StudentUid}).WhereIF(!string.IsNullOrEmpty(title), (bkr, u) => u.Student_Name.Contains(title))
                .Where((bkr, u) => u.CampusId == Convert.ToInt32(campusId))
                .WhereIF(startTime.HasValue, (bkr, u) => bkr.BackDate>= startTime.Value).WhereIF(endTime.HasValue, (bkr, u) => bkr.BackDate <= endTime.Value).Sum((bkr, u) => bkr.BackAmount);
            rsg.code = 200;
            rsg.msg = "获取成功";
            rsg.data = result;
            return Json(rsg);
        }
    }
}
