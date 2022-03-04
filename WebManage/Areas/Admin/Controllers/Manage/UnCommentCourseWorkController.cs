using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using log4net;
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
    public class UnCommentCourseWorkController : BaseController
    {

        private ICurrencyService _currencyService;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(UnCommentCourseWorkController));
        public UnCommentCourseWorkController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        protected override void Init()
        {
            this.MenuID = "V-353";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("V-353", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("V-353", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title,int workStutas, int studymode, DateTime? startTime=null, DateTime? endTime = null, int page = 1, int limit = 10)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            sys_user teacher = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Left, u.User_ID== ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
                .Where((u,ur,r)=>u.User_ID==userId&&r.Role_Name.Contains("教师")).First();
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Campus, sys_user>((c, ca, ta) => new Object[] { JoinType.Left, c.CampusId == ca.CampusId, JoinType.Left, c.TeacherUid == ta.User_ID })
                .Where(c => c.CampusId == Convert.ToInt32(campusId)&& c.Work_Stutas == workStutas)
                .WhereIF(studymode>0, c => c.StudyMode== studymode)
                .WhereIF(teacher!=null, c => c.TeacherUid == userId)
                .WhereIF(startTime.HasValue,c=>c.AT_Date>=startTime.Value)
                .WhereIF(endTime.HasValue, c => c.AT_Date< endTime.Value)
                .WhereIF(!string.IsNullOrEmpty(title), (c, ca) => c.Work_Title.Contains(title)).OrderByIF(workStutas>0, c => c.AT_Date,OrderByType.Desc).OrderByIF(workStutas <1, c => c.AT_Date, OrderByType.Asc).Select<CourseWorkModel>((c, ca, ta) => new CourseWorkModel
                {
                    Id = c.Id,
                    Work_Title = c.Work_Title,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    TeacherName=ta.User_Name,
                    StudyMode=c.StudyMode,
                    AT_Date=c.AT_Date,
                    Comment=c.Comment,
                    CreateTime = c.CreateTime,
                    CreateUid = c.CreateUid
                }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

        [UsersRoleAuthFilter("V-353", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(f => f.Id == ID).First();
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        [UsersRoleAuthFilter("V-353", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("V-353", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }


        /// <summary>
        /// 保存点评
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("V-353", "Add,Edit")]
        public IActionResult SaveCommend(C_Course_Work vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.Comment))
                    return Json(new { code = 0, msg = "点评内容不能为空" });
                if (vmodel.Id > 0)
                {
                    C_Course_Work work = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(f => f.Id == vmodel.Id).First();
                    if (work.IsSendComment==1) {
                        return Json(new { code = 0, msg = "点评内容已被推送，无法修改" });
                    }
                    work.Comment = vmodel.Comment;
                    work.UpdateTime = DateTime.Now;
                    work.UpdateUid = userId;
                    work.Work_Stutas = 1;
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>(work).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }

    }
}
