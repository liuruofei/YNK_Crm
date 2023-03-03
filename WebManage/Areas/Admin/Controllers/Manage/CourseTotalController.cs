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
    public class CourseTotalController : BaseController
    {
        private ICurrencyService _currencyService;
        public CourseTotalController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "R-152";
        }

        // GET: /<controller>/
        [UsersRoleAuthFilter("R-152", FunctionEnum.Have)]
        public IActionResult Index()
        {
            //var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            //CourseTotalModel model = _currencyService.DbAccess().Queryable(@"(select sum(Course_Time)CourseTime,sum(Course_UseTime)CourseUseTime,
            //ClassTime=(select sum(Class_Course_Time) from(select  Class_Course_Time,ClassId from C_User_CourseTime where ClassId>0 group by ClassId,Class_Course_Time)A),
            //ClassUseTime=(select sum(Class_Course_UseTime) from(select  Class_Course_UseTime,ClassId from C_User_CourseTime where ClassId>0 AND Class_Course_UseTime>0 AND Class_Course_Time>0 group by ClassId,Class_Course_UseTime)A),
            //PresentTime=(select sum(Present_Time) from C_User_PresentTime),PresentUseTime=(select sum(Present_UseTime) from C_User_PresentTime),
            //ShitingTime=(select sum(CourseTime) from C_Course_Work where StudyMode=4)
            //from C_User_CourseTime)", "orginSql")
            //.Select<CourseTotalModel>("*").First();
            return View();
        }
        public IActionResult QuereyTotalCourse(DateTime? startTime = null, DateTime? endTime = null) {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            var chaildWhere = "";
            if (startTime.HasValue) {
                chaildWhere += " and AT_Date>='" + startTime.Value.ToString("yyyy-MM-dd")+"'";
            }
            if (endTime.HasValue)
            {
                chaildWhere += " and AT_Date<='" + endTime.Value.ToString("yyyy-MM-dd")+"'";
            }
            StringBuilder str = new StringBuilder("(select CourseUseTime=(select sum(CourseTime) from C_Course_Work  where StudyMode=1 and Work_Stutas=1"+ chaildWhere + "),");
            str.Append("ClassUseTime=(select sum(CourseTime) from C_Course_Work  where StudyMode=2 and Work_Stutas=1 " + chaildWhere + "),");
            str.Append("ShitingTime=(select sum(CourseTime) from C_Course_Work  where StudyMode=4 and Work_Stutas=1" + chaildWhere + "))");
            CourseTotalModel model = _currencyService.DbAccess().Queryable(str.ToString(), "orginSql")
            .Select<CourseTotalModel>("*").First();
            return Json(model);
        }

     }
}
