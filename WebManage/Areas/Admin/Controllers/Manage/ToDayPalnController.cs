using ADT.Models;
using ADT.Models.Enum;
using ADT.Models.ResModel;
using ADT.Service.IService;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
    public class ToDayPalnController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_CourseWorkService _courseWork;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(ToDayPalnController));
        public ToDayPalnController(ICurrencyService currencyService, IC_CourseWorkService courseWork)
        {
            _currencyService = currencyService;
            _courseWork = courseWork;
        }
        protected override void Init()
        {
            this.MenuID = "V-354";
        }

        [UsersRoleAuthFilter("V-354", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询所有学生计划
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("V-354", FunctionEnum.Have)]
        public IActionResult QueryStudentPlan(string studentName, int status, DateTime? atTime = null)
        {
            if (!atTime.HasValue)
            {
                atTime = DateTime.Now;
            }
            string[] dayNames = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string weekName = dayNames[Convert.ToInt32(atTime.Value.DayOfWeek.ToString("d"))].ToString();
            List<StudentWorkPlanModel> studentPlan = new List<StudentWorkPlanModel>();
            List<StudentWorkPlanModel> studentPlanHas = new List<StudentWorkPlanModel>();
            List<sys_user> listTa = _currencyService.DbAccess().Queryable<sys_userrole, sys_user, sys_role>((ur, u, r) => new object[] { JoinType.Left, ur.UserRole_UserID == u.User_ID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where((ur, u, r) => r.Role_Name == "督学"|| r.Role_Name == "督学校长").Select<sys_user>((ur, u, r) => u).ToList();
            //排除退班的班级
            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
            List<int> studentUids = new List<int>();
            //所有今日有学生任务计划
            List<C_Student_Work_Plan> listPlans = _currencyService.DbAccess().Queryable<C_Student_Work_Plan>().Where(it => DateTime.Parse(it.WorkDate.ToString("yyyy-MM-dd") + " 00:00") == DateTime.Parse(atTime.Value.ToString("yyyy-MM-dd") + " 00:00")).ToList();
            if (listPlans != null && listPlans.Count > 0) {
                listPlans.ForEach(itm =>
                {
                    studentUids.Add(itm.StudentUid);
                });
            }
           //有课程学生或班级
           List<CourseWorkModel> listCourseWork = _currencyService.DbAccess().Queryable<C_Course_Work, sys_user, C_Room>((cour, ta, room) => new object[] { JoinType.Left, cour.TeacherUid == ta.User_ID, JoinType.Left, cour.RoomId == room.Id })
           .Where(cour => cour.StudyMode != 3 && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00") == DateTime.Parse(atTime.Value.ToString("yyyy-MM-dd") + " 00:00"))
           .Select<CourseWorkModel>((cour, ta, room) => new CourseWorkModel
                        {
                            Id = cour.Id,
                            AT_Date = cour.AT_Date,
                            ClasssId = cour.ClasssId,
                            Comment = cour.Comment,
                            Contra_ChildNo = cour.Contra_ChildNo,
                            CourseTime = cour.CourseTime,
                            CreateTime = cour.CreateTime,
                            CreateUid = cour.CreateUid,
                            EndTime = cour.EndTime,
                            ProjectId = cour.ProjectId,
                            RangTimeId = cour.RangTimeId,
                            RoomId = cour.RoomId,
                            StartTime = cour.StartTime,
                            StudentUid = cour.StudentUid,
                            StudyMode = cour.StudyMode,
                            SubjectId = cour.SubjectId,
                            TA_Uid = cour.TA_Uid,
                            TeacherName = ta.User_Name,
                            TeacherUid = cour.TeacherUid,
                            Work_Stutas = cour.Work_Stutas,
                            Work_Title = cour.Work_Title,
                            Status = cour.Status,
                            UpdateTime = cour.UpdateTime,
                            UpdateUid = cour.UpdateUid,
                            RoomName = room.RoomName,
                            CourseWork=cour.CourseWork
                        }).ToList();
            var inLineClassIds = listCourseWork.Where(c => c.ClasssId > 0 && c.StudyMode == 2).Select(c => c.ClasssId).ToList();
            //查询班级课程未退班的学生
            List<C_Contrac_Child> classStudent = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((ch, cl) => new object[] { JoinType.Left, ch.ClassId == cl.ClassId }).Where(ch => ch.StudyMode==2&&inLineClassIds.Contains(ch.ClassId)&& ch.Contrac_Child_Status != contracStatus).ToList();
            if (classStudent != null && classStudent.Count > 0) {
                classStudent.ForEach(itm =>
                {
                    studentUids.Add(itm.StudentUid);
                });
            }
            //查询普通课程学生
            List<int> wkStudent = listCourseWork.Where(c => c.StudentUid > 0).Select(c => c.StudentUid).ToList();
            if (wkStudent != null && wkStudent.Count > 0)
            {
                wkStudent.ForEach(itm =>
                {
                    studentUids.Add(itm);
                });
            }
            //过滤重复的学生
            HashSet<int> hax = new HashSet<int>(studentUids);
            studentUids.Clear();
            studentUids.AddRange(hax);
            //获取过滤后的学生信息
            studentPlan = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(cu=> studentUids.Contains(cu.StudentUid)).Select(cu=>new StudentWorkPlanModel { 
             StudentUid=cu.StudentUid,Student_Name=cu.Student_Name,WorkDateName= weekName,WorkDate=atTime.Value
            }).WhereIF(!string.IsNullOrEmpty(studentName),cu=>cu.Student_Name.Contains(studentName)).ToList();
            if (studentPlan != null && studentPlan.Count > 0) {
                foreach (var stu in studentPlan)
                {
                    //当前学生任务计划
                    var plans = listPlans.Where(con => con.StudentUid == stu.StudentUid).ToList();
                    //当前学生课程
                    List<CourseWorkModel> studentCourseWork = new List<CourseWorkModel>();
                    if (classStudent != null && classStudent.Count > 0) {
                        //判断当前学生是否在班课里面
                        var inClass = classStudent.Where(c => c.StudentUid == stu.StudentUid).Select(c => c.ClassId).ToList();
                        // 如果在的话把班课放进去
                        if (inClass != null && inClass.Count > 0)
                        {
                            var classCourseWork = listCourseWork.Where(c => inClass.Contains(c.ClasssId)).ToList();
                            if (classCourseWork != null && classCourseWork.Count > 0) {
                                classCourseWork.ForEach(c =>
                                {
                                    studentCourseWork.Add(c);
                                });
                            }
                        }
                    }
                    //查询当前学生班课以外的课程
                    var oneOfone = listCourseWork.Where(c => c.StudentUid == stu.StudentUid).ToList();
                    if (oneOfone != null && oneOfone.Count > 0) {
                        oneOfone.ForEach(c =>
                        {
                            studentCourseWork.Add(c);
                        });
                    }
                    //循环课程
                    if (studentCourseWork != null && studentCourseWork.Count > 0)
                    {
                        studentCourseWork.ForEach(iv =>
                        {
                            var startTimestr = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " " + iv.StartTime);
                            var endTimestr = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " " + iv.EndTime);
                            var thanStartTime1 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 8:00");
                            var thanendTime1 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 10:50");
                            var thanStartTime2 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 10:00");
                            var thanendTime2 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 12:50");
                            var thanStartTime3 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 13:00");
                            var thanendTime3 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 15:50");
                            var thanStartTime4 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 15:00");
                            var thanendTime4 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 17:50");
                            var thanStartTime5 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 17:00");
                            var thanendTime5 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 19:50");
                            var thanStartTime6 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 19:00");
                            var thanendTime6 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 21:50");
                            var thanStartTime7 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 21:00");
                            var thanendTime7= DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 23:00");
                            if (((startTimestr >= thanStartTime1 && startTimestr < thanendTime1) && (endTimestr > thanStartTime1 && endTimestr <= thanendTime1)) || ((endTimestr > thanStartTime1 && thanStartTime1 >= startTimestr) && (endTimestr <= thanendTime1 && thanendTime1 > startTimestr)))
                            {
                                stu.Eight_Ten_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                                stu.Eight_Ten_TeacherName = iv.TeacherName;
                                stu.Eight_Ten_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime2 && startTimestr < thanendTime2) && (endTimestr > thanStartTime2 && endTimestr <= thanendTime2)) || ((endTimestr > thanStartTime2 && thanStartTime2 >= startTimestr) && (endTimestr <= thanendTime2 && thanendTime2 > startTimestr)) || ((startTimestr >= thanStartTime2 && startTimestr < thanendTime2) && endTimestr <= thanStartTime3))
                            {
                                stu.Ten_Twelve_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                stu.Ten_Twelve_TeacherName = iv.TeacherName;
                                stu.Ten_Twelve_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime3 && startTimestr < thanendTime3) && (endTimestr > thanStartTime3 && endTimestr <= thanendTime3)) || ((endTimestr > thanStartTime3 && thanStartTime3 >= startTimestr) && (endTimestr <= thanendTime3 && thanendTime3 > startTimestr)))
                            {
                                stu.Thirteen_Fifteen_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                stu.Thirteen_Fifteen_TeacherName = iv.TeacherName;
                                stu.Thirteen_Fifteen_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime4 && startTimestr < thanendTime4) && (endTimestr > thanStartTime4 && endTimestr <= thanendTime4)) || ((endTimestr > thanStartTime4 && thanStartTime4 >= startTimestr) && (endTimestr <= thanendTime4 && thanendTime4 > startTimestr)))
                            {
                                stu.Fifteen_Seventeen_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                stu.Fifteen_Seventeen_TeacherName = iv.TeacherName;
                                stu.Fifteen_Seventeen_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime5 && startTimestr < thanendTime5) && (endTimestr > thanStartTime5 && endTimestr <= thanendTime5)) || ((endTimestr > thanStartTime5 && thanStartTime5 >= startTimestr) && (endTimestr <= thanendTime5 && thanendTime5 > startTimestr)))
                            {
                                stu.Seventeen_Nineteen_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                stu.Seventeen_Nineteen_TeacherName = iv.TeacherName;
                                stu.Seventeen_Nineteen_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime6 && startTimestr < thanendTime6) && (endTimestr > thanStartTime6 && endTimestr <= thanendTime6)) || ((endTimestr > thanStartTime6 && thanStartTime6 >= startTimestr) && (endTimestr <= thanendTime6 && thanendTime6 > startTimestr)))
                            {
                                stu.Nineteen_TwentyOne_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                stu.Nineteen_TwentyOne_TeacherName = iv.TeacherName;
                                stu.Nineteen_TwentyOne_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime7 && startTimestr < thanendTime7) && (endTimestr > thanStartTime7&& endTimestr <= thanendTime7)) || ((endTimestr > thanStartTime7 && thanStartTime7 >= startTimestr) && (endTimestr <= thanendTime7 && thanendTime7 > startTimestr)))
                            {
                                stu.TwentyOne_TwentyTree_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                stu.TwentyOne_TwentyTree_TeacherName = iv.TeacherName;
                                stu.TwentyOne_TwentyTree_StudyMode = iv.StudyMode;
                            }
                            if (!string.IsNullOrEmpty(iv.Comment) && iv.Comment != stu.CourseComent)
                                stu.CourseComent += iv.Comment;
                            if (!string.IsNullOrEmpty(iv.CourseWork) && iv.CourseWork != stu.CourseWorkCotent)
                                stu.CourseWorkCotent += iv.CourseWork + "<div style=\"height:15px;border-top:solid 3px\"></div>";
                        });
                    }
                    //循环计划
                    if (plans != null && plans.Count > 0)
                    {
                        plans.ForEach(iv =>
                        {
                            var startTimestr = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " " + iv.StartTime);
                            var endTimestr = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " " + iv.EndTime);
                            var thanStartTime1 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 8:00");
                            var thanendTime1 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 10:00");
                            var thanStartTime2 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 10:00");
                            var thanendTime2 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 12:00");
                            var thanStartTime3 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 13:00");
                            var thanendTime3 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 15:00");
                            var thanStartTime4 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 15:00");
                            var thanendTime4 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 17:00");
                            var thanStartTime5 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 17:00");
                            var thanendTime5 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 19:00");
                            var thanStartTime6 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 19:00");
                            var thanendTime6 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 21:00");
                            var thanStartTime7 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 21:00");
                            var thanendTime7 = DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 23:00");
                            if (startTimestr >= thanStartTime1 && endTimestr <= thanendTime1)
                            {
                                if (string.IsNullOrEmpty(stu.Eight_Ten_OlockTitle))
                                {
                                    stu.Eight_Ten_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    stu.Eight_Ten_Reversion = true;
                                }
                                stu.Eight_Ten_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime2 && endTimestr <= thanendTime2)
                            {
                                if (string.IsNullOrEmpty(stu.Ten_Twelve_OlockTitle))
                                {
                                    stu.Ten_Twelve_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    stu.Ten_Twelve_Reversion = true;
                                }
                                stu.Ten_Twelve_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime3 && endTimestr <= thanendTime3)
                            {
                                if (string.IsNullOrEmpty(stu.Thirteen_Fifteen_OlockTitle))
                                {
                                    stu.Thirteen_Fifteen_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    stu.Thirteen_Fifteen_Reversion = true;
                                }
                                stu.Thirteen_Fifteen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime4 && endTimestr <= thanendTime4)
                            {
                                if (string.IsNullOrEmpty(stu.Fifteen_Seventeen_OlockTitle))
                                {
                                    stu.Fifteen_Seventeen_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    stu.Fifteen_Seventeen_Reversion = true;
                                }
                                stu.Fifteen_Seventeen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime5 && endTimestr <= thanendTime5)
                            {
                                if (string.IsNullOrEmpty(stu.Seventeen_Nineteen_OlockTitle))
                                {
                                    stu.Seventeen_Nineteen_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    stu.Seventeen_Nineteen_Reversion = true;
                                }
                                stu.Seventeen_Nineteen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime6 && endTimestr <= thanendTime6)
                            {
                                if (string.IsNullOrEmpty(stu.Nineteen_TwentyOne_OlockTitle))
                                {
                                    stu.Nineteen_TwentyOne_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    stu.Nineteen_TwentyOne_Reversion = true;
                                }
                                stu.Nineteen_TwentyOne_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime7 && endTimestr <= thanendTime7)
                            {
                                if (string.IsNullOrEmpty(stu.TwentyOne_TwentyTree_OlockTitle))
                                {
                                    stu.TwentyOne_TwentyTree_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    stu.TwentyOne_TwentyTree_Reversion = true;
                                }
                                stu.TwentyOne_TwentyTree_Id = iv.Id;
                            }
                            if (!string.IsNullOrEmpty(iv.SummaryComent) || !string.IsNullOrEmpty(iv.CourseComent) || !string.IsNullOrEmpty(iv.ChouciComent) || !string.IsNullOrEmpty(iv.HomeWorkComent) || !string.IsNullOrEmpty(iv.OtherComent) || !string.IsNullOrEmpty(iv.OutSchoolTime) || !string.IsNullOrEmpty(iv.InSchoolTime))
                            {
                                stu.Id = iv.Id;
                            }
                            if (!string.IsNullOrEmpty(iv.ChouciComent) && iv.ChouciComent != stu.ChouciComent)
                                stu.ChouciComent += iv.ChouciComent;
                            if (!string.IsNullOrEmpty(iv.HomeWorkComent))
                                stu.HomeWorkComent += iv.HomeWorkComent;
                            if (!string.IsNullOrEmpty(iv.CourseComent) && iv.CourseComent != stu.CourseComent)
                                stu.CourseComent += iv.CourseComent;
                            if (!string.IsNullOrEmpty(iv.SummaryComent))
                                stu.SummaryComent += iv.SummaryComent;
                            if (!string.IsNullOrEmpty(iv.OtherComent))
                                stu.OtherComent += iv.OtherComent;
                            if (!string.IsNullOrEmpty(iv.InSchoolTime))
                                stu.InSchoolTime = iv.InSchoolTime;
                            if (!string.IsNullOrEmpty(iv.OutSchoolTime))
                                stu.OutSchoolTime = iv.OutSchoolTime;
                            if (!string.IsNullOrEmpty(iv.OutSchoolTime))
                                stu.TotalTime = iv.TotalTime;
                            if (!string.IsNullOrEmpty(iv.TaUid))
                            {
                                var itemTa = listTa.Find(cn => cn.User_ID == iv.TaUid);
                                if (itemTa != null) {
                                    stu.TaUseName = listTa.Find(cn => cn.User_ID == iv.TaUid).User_Name;
                                }
                            }
                        });
                    }
                    if ((studentCourseWork != null && studentCourseWork.Count > 0) || (plans != null && plans.Count > 0))
                    {
                        studentPlanHas.Add(stu);
                    }
                }
                if (status == 1) {
                    return Json(studentPlanHas);
                }
            }
            return Json(studentPlan);
        }


        [UsersRoleAuthFilter("V-354", FunctionEnum.Add)]
        public IActionResult Add(string dataStr, string startTime, string endTime, int studentUid)
        {
            ViewBag.ID = 0;
            ViewBag.StudentUid = studentUid;
            if (!string.IsNullOrEmpty(dataStr))
                dataStr = DateTime.Parse(dataStr).ToString("yyyy-MM-dd");
            ViewBag.DataStr = dataStr;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            return View();
        }

        [UsersRoleAuthFilter("V-354", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            var model = _currencyService.DbAccess().Queryable<C_Student_Work_Plan>().Where(f => f.Id == ID).First();
            ViewBag.ID = ID;
            ViewBag.StudentUid = model.StudentUid;
            ViewBag.DataStr = model.WorkDate.ToString("yyyy-MM-dd");
            ViewBag.StartTime = model.StartTime;
            ViewBag.EndTime = model.EndTime;
            return View("Add");
        }


        [UsersRoleAuthFilter("V-354", FunctionEnum.Add)]
        public IActionResult AddComment(string dataStr, int studentUid)
        {
            ViewBag.ID = 0;
            ViewBag.StudentUid = studentUid;
            if (!string.IsNullOrEmpty(dataStr))
                dataStr = DateTime.Parse(dataStr).ToString("yyyy-MM-dd");
            ViewBag.DataStr = dataStr;
            return View();
        }

        [UsersRoleAuthFilter("V-354", FunctionEnum.Edit)]
        public IActionResult EditComment(int ID)
        {
            var model = _currencyService.DbAccess().Queryable<C_Student_Work_Plan>().Where(f => f.Id == ID).First();
            ViewBag.ID = ID;
            ViewBag.StudentUid = model.StudentUid;
            ViewBag.DataStr = model.WorkDate.ToString("yyyy-MM-dd");
            return View("AddComment");
        }



        [UsersRoleAuthFilter("V-354", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                C_Student_Work_Plan vmodel = _currencyService.DbAccess().Queryable<C_Student_Work_Plan>().Where(f => f.Id == ID).First();
                list = vmodel;
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        public IActionResult deleteCommend(int Id)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "删除失败" };
            if (Id > 0)
            {
                var result = _currencyService.DbAccess().Deleteable<C_Student_Work_Plan>().Where(v => v.Id == Id).ExecuteCommand();
                if (result > 0)
                {
                    rsg.code = 200;
                    rsg.msg = "删除任务成功";
                }
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveInfo(C_Student_Work_Plan vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
                if (string.IsNullOrEmpty(vmodel.WorkTitle))
                    return Json(new { code = 0, msg = "任务计划不能为空" });
                if (vmodel.Id > 0)
                {
                    C_Student_Work_Plan plan = _currencyService.DbAccess().Queryable<C_Student_Work_Plan>().Where(f => f.Id == vmodel.Id).First();
                    plan.WorkDate = vmodel.WorkDate;
                    plan.WorkTitle = vmodel.WorkTitle;
                    plan.StartTime = vmodel.StartTime;
                    plan.EndTime = vmodel.EndTime;
                    plan.UpdateTime = DateTime.Now;
                    plan.UpdateUid = userId;
                    var result = _currencyService.DbAccess().Updateable<C_Student_Work_Plan>(plan).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
                else
                {
                    vmodel.CreateTime = DateTime.Now;
                    vmodel.CreateUid = userId;
                    vmodel.CampusId = Convert.ToInt32(campusId);
                    var result = _currencyService.DbAccess().Insertable<C_Student_Work_Plan>(vmodel).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "保存成功";
                    }
                }
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }
        /// <summary>
        /// 保存其它任务
        /// </summary>
        /// <returns></returns>
        public IActionResult SaveCommend(C_Student_Work_Plan vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
                var ccOrSale = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
.Where((u, ur, r) => u.User_ID == userId && (r.Role_Name == "督学校长" || r.Role_Name == "督学")).First();
                if (!string.IsNullOrEmpty(vmodel.TaUid) && string.IsNullOrEmpty(vmodel.HomeWorkComent))
                {
                    return Json(new { code = 0, msg = "选择督学前请填写作业完成情况!" });
                }
                if (vmodel.Id > 0)
                {
                    C_Student_Work_Plan plan = _currencyService.DbAccess().Queryable<C_Student_Work_Plan>().Where(f => f.Id == vmodel.Id).First();
                    plan.WorkDate = vmodel.WorkDate;
                    plan.ChouciComent = vmodel.ChouciComent;
                    plan.CourseComent = vmodel.CourseComent;
                    plan.OtherComent = vmodel.OtherComent;
                    plan.SummaryComent = vmodel.SummaryComent;
                    plan.HomeWorkComent = vmodel.HomeWorkComent;
                    plan.InSchoolTime = vmodel.InSchoolTime;
                    plan.OutSchoolTime = vmodel.OutSchoolTime;
                    plan.TaUid = vmodel.TaUid;
                    plan.UpdateTime = DateTime.Now;
                    plan.UpdateUid = userId;
                    var result = _currencyService.DbAccess().Updateable<C_Student_Work_Plan>(plan).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
                else
                {
                    vmodel.CreateTime = DateTime.Now;
                    vmodel.CreateUid = userId;
                    vmodel.CampusId = Convert.ToInt32(campusId);
                    var result = _currencyService.DbAccess().Insertable<C_Student_Work_Plan>(vmodel).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "保存成功";
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
