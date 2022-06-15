using ADT.Models;
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
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class TaskFinishController : BaseController
    {
        private ICurrencyService _currencyService;
        public TaskFinishController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "V-355";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("V-355", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("V-355", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title,int? finishStatus=-1,DateTime?startDate=null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<TaskFinishModel> pageModel = new PageList<TaskFinishModel>();
            string sql1 = "select  Id,u.Student_Name,CourseWork as TaskName,FinishStatus,AT_Date as WorkDate,StartTime,EndTime, isZuoye=1 from C_Course_Work  wk left join C_Contrac_User u  on wk.StudentUid=u.StudentUid where wk.CourseWork is not null";
            string sql2 = "select Id,u.Student_Name,WorkTitle as TaskName,FinishStatus,WorkDate,StartTime,EndTime,isZuoye=0 from C_Student_Work_Plan pl left join C_Contrac_User u   on pl.StudentUid=u.StudentUid  where WorkTitle is not null";
            if (!string.IsNullOrEmpty(title)) {
                sql1 += " and charindex(@studentName,u.Student_Name)>0";
                sql2 += " and charindex(@studentName,u.Student_Name)>0";
            }
            if (finishStatus > -1) {
                sql1 += " and isnull(wk.FinishStatus,0)="+finishStatus;
                sql2 += " and isnull(pl.FinishStatus,0)="+finishStatus;
            }
            if (startDate.HasValue) {
                sql1 += " and wk.AT_Date>=CAST('" + startDate.Value +"' AS date)";
                sql2 += " and pl.WorkDate>=CAST('" + startDate.Value + "' AS date)";
            }
            if (endDate.HasValue)
            {
                sql1 += " and wk.AT_Date<=CAST('" + endDate.Value + "' AS date)";
                sql2 += " and pl.WorkDate<=CAST('" + endDate.Value + "' AS date)";
            }
            var list = _currencyService.DbAccess().Queryable(@"("+ sql1+ " union all "+sql2+")", "org").AddParameters(new { studentName = title }).Select<TaskFinishModel>().OrderBy(org=>org.WorkDate,SqlSugar.OrderByType.Desc).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

        public IActionResult changeFinishStatus(int Id,int isZuoye) {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (Id > 0)
            {
                if (isZuoye == 1)
                {
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(item => new C_Course_Work { FinishStatus = 1 })
                        .Where(item => item.Id == Id).ExecuteCommand();
                    if (result > 0)
                    {
                        reg.msg = "状态已更新完成";
                        reg.code = 200;
                    }
                }
                else {
                    var result = _currencyService.DbAccess().Updateable<C_Student_Work_Plan>().SetColumns(item => new C_Student_Work_Plan { FinishStatus = 1 })
                     .Where(item => item.Id == Id).ExecuteCommand();
                    if (result > 0)
                    {
                        reg.msg = "状态已更新完成";
                        reg.code = 200;
                    }
                }
            }
            else
            {
                reg.msg = "缺少参数";
                reg.code = 300;
            }
            return Json(reg);
        }
    }
}
