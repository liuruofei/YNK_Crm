using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
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
    public class MyStudentController : BaseController
    {
        private ICurrencyService _currencyService;
        public MyStudentController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "V-358";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("V-358", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("V-358", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            var teacher=_currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID })
               .Where((u, ur, r) => u.CampusId == Convert.ToInt32(campusId) &&u.User_ID==userId&&(r.Role_Name == "教师" || r.Role_Name == "教学校长")).Select<sys_role>().First();
            PageList<MyStudentCourseTime> pageModel = new PageList<MyStudentCourseTime>();
            StringBuilder sql = new StringBuilder("(select  ue.Student_Name,p.Course_Time,p.Course_UseTime,sub.SubjectName,prj.ProjectName from(");
            sql.Append("select wk.SubjectId,wk.ProjectId,wk.StudentUid,sum(wk.CourseTime)Course_Time,");
            sql.Append(" Course_UseTime=(select sum(CourseTime) from C_Course_Work ch where ch.SubjectId=wk.SubjectId and ch.ProjectId=wk.ProjectId  and ch.StudentUid=wk.StudentUid ");
            if (teacher != null) {
                sql.Append(" and TeacherUid=@TeacherUid");
            }
            sql.Append(" and ch.Work_Stutas=1)");
            sql.Append(" from C_Course_Work wk inner join C_Contrac_User u on wk.StudentUid=u.StudentUid ");
            if (teacher != null)
            {
                sql.Append(" where wk.TeacherUid=@TeacherUid");
            }
            sql.Append(" group by wk.SubjectId,wk.ProjectId,wk.StudentUid)p");
            sql.Append(" left join C_Subject sub on p.SubjectId=sub.SubjectId left join C_Project  prj on p.ProjectId=prj.ProjectId");
            sql.Append(" left join C_Contrac_User ue on p.StudentUid=ue.StudentUid)");
            List <MyStudentCourseTime> list = _currencyService.DbAccess().Queryable(sql.ToString(),"orginSql").WhereIF(!string.IsNullOrEmpty(title), "orginSql.Student_Name=@Student_Name").AddParameters(new { TeacherUid = userId,Student_Name =title}).Select<MyStudentCourseTime>().ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


    }
}
