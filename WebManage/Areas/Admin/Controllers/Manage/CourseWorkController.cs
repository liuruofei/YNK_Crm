using ADT.Models;
using ADT.Models.Enum;
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
    public class CourseWorkController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        private IC_CourseWorkService _courseWork;
        public CourseWorkController(ICurrencyService currencyService, IC_ContracService contrac, IC_CourseWorkService courseWork)
        {
            _currencyService = currencyService;
            _contrac = contrac;
            _courseWork = courseWork;
        }

        protected override void Init()
        {
            this.MenuID = "V-350";
        }

        [UsersRoleAuthFilter("V-350", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }



        [UsersRoleAuthFilter("V-350", FunctionEnum.Add)]
        public IActionResult Add(string dataStr, string teacherName)
        {
            ViewBag.ID = 0;
            ViewBag.teacherId = "";
            ViewBag.DataStr = dataStr;
            if (!string.IsNullOrEmpty(teacherName))
            {
                var teach = _currencyService.DbAccess().Queryable<sys_user>().Where(it => it.User_Name.Equals(teacherName)).First();
                if (teach != null)
                {
                    ViewBag.teacherId = teach.User_ID;
                }
            }
            return View();
        }

        [UsersRoleAuthFilter("V-350", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.teacherId = "";
            ViewBag.DataStr = "";
            ViewBag.ID = ID;
            return View("Add");
        }


        [UsersRoleAuthFilter("V-350", "Add,Edit")]
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
        /// 查询老师
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryTeacher()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID })
                .Where((u, ur, r) => r.Role_Name == "教师"&&u.CampusId==Convert.ToInt32(campusId)).ToList();
            return Json(rsg);
        }
        /// <summary>
        /// 查询时间段
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryRangeTime()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<C_Range_Time> list = _currencyService.DbAccess().Queryable<C_Range_Time>().Where(r=>r.CampusId==Convert.ToInt32(campusId)).ToList();
            list = list.OrderBy(it => DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + it.StartTime)).ToList();
            list.Add(new C_Range_Time() { Id = 0, TimeName = "--自定义--" });
            rsg.data = list;
            return Json(rsg);
        }
        /// <summary>
        /// 下拉框查询小班或者一对一
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryUserChild(int studyModel, string title)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            if (studyModel == 1)
            {
                var list = _currencyService.DbAccess().Queryable<C_Contrac_Child_Detail, C_Contrac_User, C_Subject, C_Project,C_Contrac_Child>((ch, cu, su, pr,c) => new object[] { JoinType.Left, ch.StudentUid == cu.StudentUid, JoinType.Left, ch.SubjectId == su.SubjectId, JoinType.Left, ch.ProjectId == pr.ProjectId, JoinType.Left, ch.Contra_ChildNo ==c.Contra_ChildNo })
                    .Where((ch, cu, su, pr, c)=>(c.Pay_Stutas==(int)ConstraChild_Pay_Stutas.PartPay|| c.Pay_Stutas == (int)ConstraChild_Pay_Stutas.PayOk)
                    && (c.Contrac_Child_Status == (int)ConstraChild_Status.Confirmationed || c.Contrac_Child_Status == (int)ConstraChild_Status.ChangeClassOk || c.Contrac_Child_Status == (int)ConstraChild_Status.ChangeOk)&&c.CampusId==Convert.ToInt32(campusId))
                    .WhereIF(!string.IsNullOrEmpty(title), (ch, cu) => cu.Student_Name.Contains(title)).Select<StudyWorkModel>((ch, cu, su, pr) => new StudyWorkModel
                {
                    Contra_ChildNo = ch.Contra_ChildNo,
                    StudentUid = ch.StudentUid,
                    Student_Name = cu.Student_Name,
                    StudyMode = 1,
                    SubjectId = ch.SubjectId,
                    ProjectId = ch.ProjectId,
                    SubjectName = su.SubjectName,
                    ProjectName = pr.ProjectName
                }).ToPageList(1, 30);
                rsg.data = list;
            }
            else
            {
                var list = _currencyService.DbAccess().Queryable<C_Class>().Where(it => SqlFunc.Subqueryable<C_Contrac_Child>().Where(s => s.ClassId == it.ClassId&&(s.Contrac_Child_Status==(int)ConstraChild_Status.Confirmationed|| s.Contrac_Child_Status == (int)ConstraChild_Status.ChangeClassOk|| s.Contrac_Child_Status == (int)ConstraChild_Status.ChangeOk)&&s.CampusId==Convert.ToInt32(campusId)).Any())
                    .WhereIF(!string.IsNullOrEmpty(title), cla => cla.Class_Name.Contains(title)).OrderBy(cla => cla.CreateTime, OrderByType.Desc).Select<StudyWorkModel>(cla =>
                   new StudyWorkModel { ClasssId = cla.ClassId, ClassName = cla.Class_Name, SubjectId = cla.SubjectId, StudyMode = 2 }).ToPageList(1, 30);
                rsg.data = list;
            }
            return Json(rsg);
        }


        /// <summary>
        /// 查询督学
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryTa()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<sys_user> list = _currencyService.DbAccess().Queryable<sys_userrole, sys_user, sys_role>((ur, u, r) => new object[] { JoinType.Left, ur.UserRole_UserID == u.User_ID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where((ur, u, r) => r.Role_Name == "督学"&&u.CampusId==Convert.ToInt32(campusId)).Select<sys_user>((ur, u, r) => u).ToList();
            list.Add(new sys_user() { User_ID = "", User_Name = "--督学--", User_CreateTime = DateTime.Now });
            list = list.OrderByDescending(it => it.User_CreateTime).ToList();
            rsg.data = list;
            return Json(rsg);
        }

        public IActionResult QueryRoom()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<C_Room> list = _currencyService.DbAccess().Queryable<C_Room>().Where(r=>r.CampusId==Convert.ToInt32(campusId)).ToList();
            list.Add(new C_Room() { Id = 0, RoomName = "--选择教室--" });
            list = list.OrderByDescending(it => it.Id).ToList();
            rsg.data = list;
            return Json(rsg);
        }

        /// <summary>
        /// 查询科目集合
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public IActionResult QueryProject(int subjectId)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<C_Project> list = _currencyService.DbAccess().Queryable<C_Project>().Where(it => it.SubjectId == subjectId).ToList();
            rsg.data = list;
            return Json(rsg);
        }

        /// <summary>
        /// 保存排课课程
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveCourseWork(C_Course_Work vmodel)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            vmodel.CreateUid = userId;
            ResResult rsg = new ResResult() { code = 200, msg = "保存排课课程成功" };
            rsg = _courseWork.SaveCourseWork(vmodel);
            return Json(rsg);
        }

        /// <summary>
        /// 拖拽排课
        /// </summary>
        /// <param name="id"></param>
        /// <param name="upAtDate"></param>
        /// <returns></returns>
        public IActionResult DropCourseWork(int id, string upAtDate)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "保存排课课程成功" };
            DateTime adDate = DateTime.Parse(upAtDate);
            rsg = _courseWork.DropCourseWork(id, adDate, userId);
            return Json(rsg);
        }


        /// <summary>
        /// 删除排课
        /// </summary>
        /// <param name="id"></param>
        /// <param name="upAtDate"></param>
        /// <returns></returns>
        public IActionResult RemoveCourseWork(int id)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "删除当前课程成功" };
            rsg = _courseWork.RemoveCourseWork(id, userId);
            return Json(rsg);
        }

        /// <summary>
        /// 复制课程
        /// </summary>
        /// <param name="workIds"></param>
        /// <param name="workDate"></param>
        /// <returns></returns>
        public IActionResult CopyCourseWork(int[] workIds, DateTime? workDate = null) {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "复制课程成功" };
            rsg = _courseWork.CopyCourseWork(workIds, userId, workDate);
            return Json(rsg);
        }




        /// <summary>
        /// 获取排课计划列表
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <param name="dateTime"></param>
        /// <param name="userName"></param>
        /// <param name="subjectId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public IActionResult QueryWorkSource(string startStr, string endStr, string userName, int subjectId, int projectId)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult reg = new ResResult();
            var teacher = _currencyService.DbAccess().Queryable<sys_user>().Where(t => t.User_Name.Equals(userName)).First();
            var student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(t => t.Student_Name.Equals(userName)).First();
            List<int> classIds = new List<int>();
            if (student != null)
            {
                classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child,C_Class>((c,cl)=>new object[] { JoinType.Left,c.ClassId==cl.ClassId}).Where(c => c.StudentUid == student.StudentUid && c.ClassId > 0).WhereIF(subjectId>0,(c,cl)=>cl.SubjectId==subjectId).Select(c => c.ClassId).ToList();
            }
            string sql = @"(select wk.*,contracU.Student_Name,ta.User_Name as TaUserName,tc.User_Name as TeacherName,rm.RoomName from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.AT_Date>=CAST(@startStr AS date) AND wk.AT_Date<CAST(@endStr AS date)";
            if (classIds != null && classIds.Count > 0)
            {
                sql += " and (wk.ClasssId in(" + string.Join(",", classIds) + ") or contracU.Student_Name=@userName) ";
            }
            else
            {
                if (!string.IsNullOrEmpty(userName))
                    sql += " and (tc.User_Name=@userName or contracU.Student_Name=@userName) ";
            }
            if (subjectId > 0) {
                sql += " and wk.SubjectId="+subjectId;
            }
            if (projectId > 0)
            {
                sql += " and wk.ProjectId=" + projectId;
            }
            sql += " and wk.CampusId="+campusId;
            sql += ")";
            dynamic list = _currencyService.DbAccess().Queryable(sql, "orginSql")
            .AddParameters(new { startStr = startStr, endStr = endStr, userName = userName })
            .Select("*").OrderBy("orginSql.CreateTime desc").ToList();
            if (list != null && list.Count > 0 && !string.IsNullOrEmpty(userName))
            {
                reg.totalRow = new totalRow();
                if (teacher != null)
                {
                    //统计老师课时，点评已完成才算课时
                    reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                        .WhereIF(subjectId > 0, it => it.SubjectId == subjectId).WhereIF(subjectId > 0, it => it.ProjectId == projectId)
                        .Where(it => it.TeacherUid == teacher.User_ID && it.StudyMode != 3 && it.Work_Stutas==1&& it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                        .Sum(it => it.CourseTime);
                }
                if (student != null)
                {
                    reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                    .WhereIF(subjectId > 0, it => it.SubjectId == subjectId).WhereIF(projectId > 0, it => it.ProjectId == projectId)
                    .Where(it => (it.StudentUid == student.StudentUid||classIds.Contains(it.ClasssId)) && it.StudyMode != 3 && it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                    .Sum(it => it.CourseTime);
                }
            }
            reg.data = list;
            reg.code = 0;
            reg.msg = "获取成功";
            return Json(reg);
        }


        public IActionResult QueryWorkSource2(string startStr, string endStr, string userName, int subjectId, int projectId, int page = 1, int limit = 10)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult reg = new ResResult();
            var teacher = _currencyService.DbAccess().Queryable<sys_user>().Where(t => t.User_Name.Equals(userName)).First();
            var student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(t => t.Student_Name.Equals(userName)).First();
            List<int> classIds = new List<int>();
            if (student != null)
            {
                classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((c, cl) => new object[] { JoinType.Left, c.ClassId == cl.ClassId }).Where(c => c.StudentUid == student.StudentUid && c.ClassId > 0).WhereIF(subjectId > 0, (c, cl) => cl.SubjectId == subjectId).Select(c => c.ClassId).ToList();
            }
            string sql = @"(select wk.*,contracU.Student_Name,ta.User_Name as TaUserName,tc.User_Name as TeacherName,rm.RoomName from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.AT_Date>=CAST(@startStr AS date) AND wk.AT_Date<CAST(@endStr AS date)";
            if (classIds != null && classIds.Count > 0)
            {
                sql += " and (wk.ClasssId in(" + string.Join(",", classIds) + ") or contracU.Student_Name=@userName) ";
            }
            else
            {
                if (!string.IsNullOrEmpty(userName))
                    sql += " and (tc.User_Name=@userName or contracU.Student_Name=@userName) ";
            }
            if (subjectId > 0)
            {
                sql += " and wk.SubjectId=" + subjectId;
            }
            if (projectId > 0)
            {
                sql += " and wk.ProjectId=" + projectId;
            }
            sql += " and wk.CampusId=" + campusId;
            sql += ")";
            int total = 0;
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            List<CourseWorkModel> list= _currencyService.DbAccess().Queryable(sql, "orginSql")
            .AddParameters(new { startStr = startStr, endStr = endStr, userName = userName })
            .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            if (list != null && list.Count > 0 && !string.IsNullOrEmpty(userName))
            {
                reg.totalRow = new totalRow();
                if (teacher != null)
                {
                    //统计老师课时，点评已完成才算课时
                    reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                        .WhereIF(subjectId > 0, it => it.SubjectId == subjectId).WhereIF(subjectId > 0, it => it.ProjectId == projectId)
                        .Where(it => it.TeacherUid == teacher.User_ID && it.StudyMode != 3 && it.Work_Stutas == 1 && it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                        .Sum(it => it.CourseTime);
                }
                if (student != null)
                {
                    reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                    .WhereIF(subjectId > 0, it => it.SubjectId == subjectId).WhereIF(projectId > 0, it => it.ProjectId == projectId)
                    .Where(it => (it.StudentUid == student.StudentUid || classIds.Contains(it.ClasssId)) && it.StudyMode != 3 && it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                    .Sum(it => it.CourseTime);
                }
            }
            reg.data = list;
            reg.code = 0;
            reg.count = total;
            reg.msg = "获取成功";
            return Json(reg);
        }




    }
}
