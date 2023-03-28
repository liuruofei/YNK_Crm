using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class TeacherLateReportController : BaseController
    {
        private ICurrencyService _currencyService;
        public TeacherLateReportController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "R-156";
        }

        [UsersRoleAuthFilter("R-156", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 统计教师迟到列表
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <param name="dateTime"></param>
        /// <param name="userName"></param>
        /// <param name="subjectId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public IActionResult QueryWorkSource(string startTime, string endTime, int monthStatu, int page = 10, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<TeacherLateModel> pageModel = new PageList<TeacherLateModel>();
            StringBuilder manwhere = new StringBuilder("");
            //本月
            if (monthStatu == 1 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and convert(varchar(6),tlt.WorkDate,112)=convert(varchar(6),getdate(),112) and tlt.WorkDate<getdate()");
            }
            //上月
            if (monthStatu == 2 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and convert(varchar(6),tlt.WorkDate,112)=convert(varchar(6),dateadd(mm,-1,getdate()),112)");
            }
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and tlt.WorkDate>=CAST(@startStr AS date) AND tlt.WorkDate<CAST(@endStr AS date)");
            }
            StringBuilder sql = new StringBuilder("(select tlt.*,sysU.[User_Name] as TeacherName,wk.Work_Title,wk.AT_Date,wk.StartTime from  C_TeacherAttendance tlt left join Sys_User sysU on tlt.TeacherId=sysU.[User_ID] left join C_Course_Work wk on tlt.WorkId=wk.Id ");
            sql.Append(" where  wk.CampusId=" + campusId + manwhere.ToString()+")");
            List<TeacherLateModel> list = _currencyService.DbAccess().Queryable(sql.ToString(), "orginSql")
            .AddParameters(new { startStr = startTime, endStr = endTime })
            .Select<TeacherLateModel>("*").OrderBy("orginSql.AT_Date desc").ToPageList(page, limit,ref total);
            list.ForEach(em =>
            {
                em.CourseTimeFmt = em.AT_Date.ToString("yyyy-MM-dd")+" "+em.StartTime;
                em.InSchoolTimeFmt = em.InSchoolTime.ToString("yyyy-MM-dd HH:mm");
            });
            pageModel.data = list;
            pageModel.count = total;
            pageModel.code = 0;
            pageModel.msg = "获取成功";
            return Json(pageModel);
        }

    }
}
