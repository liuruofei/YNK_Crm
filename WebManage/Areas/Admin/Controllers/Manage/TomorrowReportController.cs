using ADT.Models.ResModel;
using ADT.Service.IService;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class TomorrowReportController : BaseController
    {
        private ICurrencyService _currencyService;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(TomorrowReportController));

        public TomorrowReportController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        protected override void Init()
        {
            this.MenuID = "R-151";
        }

        [UsersRoleAuthFilter("R-151", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查询明日课程报表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IActionResult TotalTomorrowCourse(int dayStatu, DateTime? startTime=null,DateTime?endTime=null) {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult reg = new ResResult();
            string sql = "(select StudyMode,count(Id) AS Total from C_Course_Work c where Id>0";
            if (startTime.HasValue) {
                sql += " and cast(c.AT_Date as date)>=cast(@startStr as date)";
            }
            if (endTime.HasValue)
            {
                sql += " and cast(c.AT_Date as date)<=cast(@endStr as date)";
            }
            //默认明日
            if (!startTime.HasValue && !endTime.HasValue) {
                if (dayStatu > 0) {
                    sql += @" and cast(c.AT_Date as date)=cast(DATEADD(DAY,"+ (dayStatu- 1)+ ", GETDATE()) AS date)";
                }
            }
            sql += " group by c.StudyMode)";
            dynamic list = _currencyService.DbAccess().Queryable(sql, "orginSql")
            .AddParameters(new { startStr = startTime, endStr = endTime })
            .Select("*").ToList();
            reg.data = list;
            reg.code = 0;
            reg.msg = "获取成功";
            return Json(reg);
        }
    }
}
