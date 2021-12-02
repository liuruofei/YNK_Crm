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
    public class MyCourseWorkController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        private IC_CourseWorkService _courseWork;
        public MyCourseWorkController(ICurrencyService currencyService, IC_ContracService contrac, IC_CourseWorkService courseWork)
        {
            _currencyService = currencyService;
            _contrac = contrac;
            _courseWork = courseWork;
        }

        protected override void Init()
        {
            this.MenuID = "V-352";
        }

        [UsersRoleAuthFilter("V-352", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }


        [UsersRoleAuthFilter("V-352", FunctionEnum.Edit)]
        public IActionResult Add(int ID)
        {
            ViewBag.ID = ID;
            return View();
        }


        [UsersRoleAuthFilter("V-352", "Edit")]
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

        /// <summary>
        /// 保存点评
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveCourseWork(C_Course_Work vmodel)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "保存点评成功" };
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var model = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(it => it.Id == vmodel.Id).First();
            var courseTime = DateTime.Parse(model.AT_Date.ToString("yyyy-MM-dd") + " " + model.EndTime);
            if (model.TeacherUid != userId)
            {
                rsg.code = 0;
                rsg.msg = "你不属于当前课程老师,无法点评";
            }
            else if (DateTime.Now < courseTime)
            {
                rsg.code = 0;
                rsg.msg = "当前课程时间还未结束，你无法点评";
            }
            else {
                vmodel.CreateUid = userId;
                var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(it => new C_Course_Work { Comment = vmodel.Comment, Work_Stutas = 1 }).Where(it => it.Id == vmodel.Id).ExecuteCommand();
                if (result > 0)
                {
                    rsg.code = 200;
                    rsg.msg = "点评成功";
                }
            }
            return Json(rsg);
        }


        /// <summary>
        /// 获取排课计划列表
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <param name="dateTime"></param>
        /// <param name="userName"></param>
        /// <param name="studymode"></param>
        /// <returns></returns>
        public IActionResult QueryWorkSource(string startStr, string endStr, string userName, int studymode)
        {
            //获取当前登录老师的课程
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            int total = 0;
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            ResResult reg = new ResResult();
            List<CourseWorkModel> list = _currencyService.DbAccess().Queryable(@"(select wk.*,contracU.Student_Name,ta.User_Name as TAUserName,tc.User_Name as TeacherName,rm.RoomName,cl.Class_Name from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.TeacherUid=@TeacherUid and wk.AT_Date>=CAST(@startStr AS date) AND wk.AT_Date<CAST(@endStr AS date))", "orginSql")
           .AddParameters(new { startStr = startStr, endStr = endStr, TeacherUid = userId })
          .WhereIF(!string.IsNullOrEmpty(userName), "(orginSql.Student_Name=@userName or orginSql.Class_Name=@userName)").AddParameters(new { userName = userName })
            .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToList();
            reg.data = list;
            reg.code = 0;
            reg.msg = "获取成功";
            if (list != null && list.Count > 0)
            {
                reg.totalRow = new totalRow();
                //统计老师课时，点评已完成才算课时
                reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                    .Where(it => it.TeacherUid == userId&& it.StudyMode != 3 && it.Work_Stutas == 1 && it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                    .Sum(it => it.CourseTime);
            }
            return Json(reg);
        }


        public IActionResult QueryWorkSource2(string startStr, string endStr, string userName, int studymode,int page=10,int limit=10)
        {
            //获取当前登录老师的课程
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            int total = 0;
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            ResResult reg = new ResResult();
            List<CourseWorkModel> list = _currencyService.DbAccess().Queryable(@"(select wk.*,contracU.Student_Name,ta.User_Name as TAUserName,tc.User_Name as TeacherName,rm.RoomName,cl.Class_Name from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.TeacherUid=@TeacherUid and wk.AT_Date>=CAST(@startStr AS date) AND wk.AT_Date<CAST(@endStr AS date))", "orginSql")
           .AddParameters(new { startStr = startStr, endStr = endStr, TeacherUid = userId })
          .WhereIF(!string.IsNullOrEmpty(userName), "(orginSql.Student_Name=@userName or orginSql.Class_Name=@userName)").AddParameters(new { userName = userName })
            .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            reg.data = list;
            reg.code = 0;
            reg.msg = "获取成功";
            if (list != null && list.Count > 0)
            {
                reg.totalRow = new totalRow();
                //统计老师课时，点评已完成才算课时
                reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                    .Where(it => it.TeacherUid == userId && it.StudyMode != 3 && it.Work_Stutas == 1 && it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                    .Sum(it => it.CourseTime);
            }
            reg.count = total;
            return Json(reg);
        }


    }
}
