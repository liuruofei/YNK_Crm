using ADT.Models;
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
    public class CourseWorkRecordController : BaseController
    {

        private ICurrencyService _currencyService;
        public CourseWorkRecordController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "V-351";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("V-351", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("V-351", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<CourseWorkRecored> pageModel = new PageList<CourseWorkRecored>();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work_Recored, sys_user>((r, u) => new Object[] { JoinType.Left, r.CreateUid == u.User_ID }).WhereIF(!string.IsNullOrEmpty(title), (r, u) => r.Msg.Contains(title))
                .Where(r=>r.CampusId==Convert.ToInt32(campusId)).OrderBy(r =>r.CreateTime).Select<CourseWorkRecored>((r, u) => new CourseWorkRecored
            {
                CreateUid=r.CreateUid,
                Msg=r.Msg,
                CreateTime=r.CreateTime,
                CreateUserName=u.User_Name
            }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }



    }
}
