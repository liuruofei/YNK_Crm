using ADT.Models.ResModel;
using ADT.Service.IService;
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
    public class TeacherCourseReportController : BaseController
    {
        private ICurrencyService _currencyService;
        public TeacherCourseReportController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "R-150";
        }

       
        [UsersRoleAuthFilter("R-150", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取课程统计列表
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <param name="dateTime"></param>
        /// <param name="userName"></param>
        /// <param name="subjectId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public IActionResult QueryWorkSource(string startTime, string endTime, int monthStatu)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult reg = new ResResult();
            string manwhere = "",childWhere="";
            //本月
            if (monthStatu == 1&&string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere += " and convert(varchar(6),work.AT_Date,112)=convert(varchar(6),getdate(),112) and work.AT_Date<getdate()";
                childWhere += ",(select isnull(sum(chwork.CourseTime),0) from C_Course_Work chwork where chwork.TeacherUid=u.User_ID and chwork.Work_Stutas<>1 and chwork.StudyMode<>3 and chwork.StudyMode<>5 and chwork.StudyMode<>8 and chwork.StudyMode<>7 and convert(varchar(6),chwork.AT_Date,112)=convert(varchar(6),getdate(),112) and chwork.AT_Date<getdate())unvaliteTotal";
                childWhere += ",(select isnull(sum(chwork.CourseTime),0) from C_Course_Work chwork where chwork.TeacherUid=u.User_ID and chwork.Work_Stutas=1 and chwork.StudyMode<>3 and chwork.StudyMode<>5 and chwork.StudyMode<>8 and chwork.StudyMode<>7 and convert(varchar(6),chwork.AT_Date,112)=convert(varchar(6),getdate(),112) and chwork.AT_Date<getdate())valiteTotal";
                childWhere += ",(select isnull(sum(chwork.TeacherDeductTime),0) from C_Course_Work chwork where chwork.TeacherUid=u.User_ID and chwork.Work_Stutas=1 and chwork.TeacherDeductTime>0 and chwork.StudyMode<>3 and chwork.StudyMode<>5 and chwork.StudyMode<>8 and chwork.StudyMode<>7 and convert(varchar(6),chwork.AT_Date,112)=convert(varchar(6),getdate(),112) and chwork.AT_Date<getdate())deductTotal";
            }
            //上月
            if (monthStatu ==2 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere += " and convert(varchar(6),work.AT_Date,112)=convert(varchar(6),dateadd(mm,-1,getdate()),112)";
                childWhere += ",(select isnull(sum(chwork.CourseTime),0) from C_Course_Work chwork where chwork.TeacherUid=u.User_ID and chwork.Work_Stutas<>1 and chwork.StudyMode<>3 and chwork.StudyMode<>5 and chwork.StudyMode<>8 and chwork.StudyMode<>7 and convert(varchar(6),chwork.AT_Date,112)=convert(varchar(6),dateadd(mm,-1,getdate()),112))unvaliteTotal";
                childWhere += ",(select isnull(sum(chwork.CourseTime),0) from C_Course_Work chwork where chwork.TeacherUid=u.User_ID and chwork.Work_Stutas=1 and chwork.StudyMode<>3 and chwork.StudyMode<>5 and chwork.StudyMode<>8 and chwork.StudyMode<>7 and convert(varchar(6),chwork.AT_Date,112)=convert(varchar(6),dateadd(mm,-1,getdate()),112))valiteTotal";
                childWhere += ",(select isnull(sum(chwork.TeacherDeductTime),0) from C_Course_Work chwork where chwork.TeacherUid=u.User_ID and chwork.Work_Stutas=1 and chwork.TeacherDeductTime>0 and chwork.StudyMode<>3 and chwork.StudyMode<>5 and chwork.StudyMode<>8 and chwork.StudyMode<>7 and convert(varchar(6),chwork.AT_Date,112)=convert(varchar(6),dateadd(mm,-1,getdate()),112))deductTotal";
            }
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime)) {
                manwhere += " and work.AT_Date>=CAST(@startStr AS date) AND work.AT_Date<CAST(@endStr AS date)";
                childWhere += ",(select isnull(sum(chwork.CourseTime),0) from C_Course_Work chwork where chwork.TeacherUid=u.User_ID and chwork.Work_Stutas<>1 and chwork.StudyMode<>3 and chwork.StudyMode<>5 and chwork.StudyMode<>8 and chwork.StudyMode<>7 and chwork.AT_Date>=CAST(@startStr AS date) AND chwork.AT_Date<CAST(@endStr AS date))unvaliteTotal";
                childWhere += ",(select isnull(sum(chwork.CourseTime),0) from C_Course_Work chwork where chwork.TeacherUid=u.User_ID and chwork.Work_Stutas=1 and chwork.StudyMode<>3 and chwork.StudyMode<>5 and chwork.StudyMode<>8 and chwork.StudyMode<>7 and chwork.AT_Date>=CAST(@startStr AS date) AND chwork.AT_Date<CAST(@endStr AS date))valiteTotal";
                childWhere += ",(select isnull(sum(chwork.TeacherDeductTime),0) from C_Course_Work chwork where chwork.TeacherUid=u.User_ID and chwork.Work_Stutas=1 and chwork.TeacherDeductTime>0 and chwork.StudyMode<>3 and chwork.StudyMode<>5 and chwork.StudyMode<>8 and chwork.StudyMode<>7 and chwork.AT_Date>=CAST(@startStr AS date) AND chwork.AT_Date<CAST(@endStr AS date))deductTotal";
            }
            string sql = "(select u.User_ID,u.User_Name,sum(work.CourseTime)totalTime " + childWhere + " from  Sys_User u left join C_Course_Work work  on work.TeacherUid=u.User_ID"
                  + " where work.StudyMode<>3 and work.StudyMode<>5 and work.StudyMode<>8 and work.StudyMode<>7 and work.StudyMode<>6" + manwhere;
            sql += " and work.CampusId=" + campusId;
            sql += " group by u.User_ID,u.User_Name)";
            dynamic list = _currencyService.DbAccess().Queryable(sql, "orginSql")
            .AddParameters(new { startStr = startTime, endStr = endTime})
            .Select("*").ToList();
            totalRow total = _currencyService.DbAccess().Queryable("(select sum(totalTime)as totalCourseTime,sum(unvaliteTotal)as totalUnValiteTime,sum(valiteTotal)as totalValiteTime,sum(deductTotal)as totalDeductTime from " + sql+" as b)", "orginSql").AddParameters(new { startStr = startTime, endStr = endTime })
            .Select<totalRow>("*").First();
            reg.data = list;
            reg.totalRow = total;
            reg.code = 0;
            reg.msg = "获取成功";
            return Json(reg);
        }
    }
}
