using ADT.Models;
using ADT.Models.Enum;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models.Res;
using Rotativa.AspNetCore;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class StudentPlanController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_CourseWorkService _courseWork;
        public StudentPlanController(ICurrencyService currencyService, IC_CourseWorkService courseWork)
        {
            _currencyService = currencyService;
            _courseWork = courseWork;
        }
        protected override void Init()
        {
            this.MenuID = "H-151";
        }

        [UsersRoleAuthFilter("H-151", FunctionEnum.Have)]
        public IActionResult Index(int studentUid)
        {
            C_Contrac_User u = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(it => it.StudentUid == studentUid).First();
            ViewBag.studentUid = studentUid;
            ViewBag.studentName = u.Student_Name;
            return View();
        }

        /// <summary>
        /// 查询单个学生计划
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("H-151", FunctionEnum.Have)]
        public IActionResult QueryStudentPlan(int studentUid, int isAdd, DateTime? endtime = null, DateTime? starttime = null)
        {
            DateTime dateWeekFirstDay = GetFirstDayOfWeek(DateTime.Now);
            DateTime dateWeekLastDay = new DateTime();
            //如果点击前一周
            if (isAdd == 1)
            {
                DateTime preDay = starttime.Value.AddDays(-1);
                dateWeekFirstDay = GetFirstDayOfWeek(preDay);
            }
            //如果点击后一周
            else if (isAdd == 2)
            {
                DateTime nextDay = endtime.Value.AddDays(1);
                dateWeekFirstDay = GetFirstDayOfWeek(nextDay);
            }
            if (starttime.HasValue && endtime.HasValue && isAdd == 0)
            {
                dateWeekFirstDay = starttime.Value;
                dateWeekLastDay = endtime.Value;
            }

            string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            List<StudentWorkPlanModel> tdlistTime = new List<StudentWorkPlanModel>();
            for (var i = 0; i < 7; i++)
            {
                StudentWorkPlanModel timeModel = new StudentWorkPlanModel();
                timeModel.WorkDate = dateWeekFirstDay.AddDays(i);
                string week = Day[Convert.ToInt32(timeModel.WorkDate.DayOfWeek.ToString("d"))].ToString();
                timeModel.WorkDateName = week;
                if (i == 6)
                {
                    dateWeekLastDay = dateWeekFirstDay.AddDays(i);
                }
              ; tdlistTime.Add(timeModel);
            }
            List<sys_user> listTa = _currencyService.DbAccess().Queryable<sys_userrole, sys_user, sys_role>((ur, u, r) => new object[] { JoinType.Left, ur.UserRole_UserID == u.User_ID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where((ur, u, r) => r.Role_Name == "督学").Select<sys_user>((ur, u, r) => u).ToList();
            //查询学生排课
            //排除退班的班级
            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
            List<int> classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((ch, cl) => new object[] { JoinType.Left, ch.ClassId == cl.ClassId }).Where(ch => ch.StudentUid == studentUid && ch.ClassId > 0&&ch.Contrac_Child_Status != contracStatus).Select(ch => ch.ClassId).ToList();
            List<CourseWorkModel> listCourseWork = _currencyService.DbAccess().Queryable<C_Course_Work, sys_user, C_Room>((cour, ta, room) => new object[] { JoinType.Left, cour.TeacherUid == ta.User_ID, JoinType.Left, cour.RoomId == room.Id })
                .Where(cour => (cour.StudentUid == studentUid || classIds.Contains(cour.ClasssId)) && cour.StudyMode != 3 && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00") >= DateTime.Parse(dateWeekFirstDay.ToString("yyyy-MM-dd") + " 00:00") && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00") <= DateTime.Parse(dateWeekLastDay.ToString("yyyy-MM-dd") + " 00:00"))
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
                    CourseWork = cour.CourseWork
                }).ToList();
            //查询学生任务计划
            List<C_Student_Work_Plan> listPlan = _currencyService.DbAccess().Queryable<C_Student_Work_Plan>().Where(it => it.StudentUid == studentUid && DateTime.Parse(it.WorkDate.ToString("yyyy-MM-dd") + " 00:00") >= DateTime.Parse(dateWeekFirstDay.ToString("yyyy-MM-dd") + " 00:00") && DateTime.Parse(it.WorkDate.ToString("yyyy-MM-dd") + " 00:00") <= DateTime.Parse(dateWeekLastDay.ToString("yyyy-MM-dd") + " 00:00")).ToList();
            //查询学生抽词情况
            C_Contrac_User u = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(it => it.StudentUid == studentUid).First();
            List<MyTaskModel> listTask = new List<MyTaskModel>();
            if (!string.IsNullOrEmpty(u.Student_Account))
            {
                dynamic chouciUser = _currencyService.DbAccess().Queryable("ynk_chouci.dbo.[view_user]", "u").Where("u.User_AccountName='" + u.Student_Account + "' and u.User_Password='" + u.Student_Pwd + "'").First();
                if (chouciUser != null) {
                    listTask = _currencyService.DbAccess().Queryable(@"(select task.Task_Mode,task.Task_Name,mytask.Words_Count,mytask.PlanTime,isRightCount=(select count(*) from ynk_chouci.dbo.[view_my_task_word] taskword where taskword.Uid=mytask.Uid and taskword.TaskId=mytask.TaskId and taskword.Answer_IsRight=1),
                SecondAll=(select sum(taskword.Answer_Second) from ynk_chouci.dbo.[view_my_task_word] taskword where taskword.Uid=mytask.Uid and taskword.TaskId=mytask.TaskId) from ynk_chouci.dbo.[view_my_task] mytask 
                left join ynk_chouci.dbo.[view_task] task on  mytask.TaskId=task.TaskId where mytask.[Uid]=@uid and mytask.Finish_Status=2)", "orgin").AddParameters(new { uid = chouciUser.Uid }).Select<MyTaskModel>().ToList();
                }
            }
            tdlistTime.ForEach(it =>
            {
                //课程
                if (listCourseWork != null && listCourseWork.Count > 0)
                {
                    var filterCosurWork = listCourseWork.FindAll(iv => iv.AT_Date.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd")).ToList();
                    if (filterCosurWork != null && filterCosurWork.Count > 0)
                    {
                        filterCosurWork.ForEach(iv =>
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
                            var thanendTime6= DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 21:50");
                            var thanStartTime7 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 21:00");
                            var thanendTime7 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 23:00");
                            if (((startTimestr >= thanStartTime1 && startTimestr < thanendTime1) && (endTimestr > thanStartTime1 && endTimestr <= thanendTime1)) || ((endTimestr > thanStartTime1 && thanStartTime1 >= startTimestr) && (endTimestr <= thanendTime1 && thanendTime1 > startTimestr)))
                            {
                                it.Eight_Ten_OlockTitle+="  "+ iv.Work_Title + (iv.StudyMode!=5&&iv.StudyMode!=6?" 教师:" + iv.TeacherName:"") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                                it.Eight_Ten_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime2 && startTimestr < thanendTime2) && (endTimestr > thanStartTime2 && endTimestr <= thanendTime2)) || ((endTimestr > thanStartTime2 && thanStartTime2 >= startTimestr) && (endTimestr <= thanendTime2 && thanendTime2 > startTimestr))||((startTimestr >= thanStartTime2 && startTimestr < thanendTime2) &&endTimestr<= thanStartTime3))
                            {
                                it.Ten_Twelve_OlockTitle+= "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                it.Ten_Twelve_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime3 && startTimestr < thanendTime3) && (endTimestr > thanStartTime3 && endTimestr <= thanendTime3)) || ((endTimestr > thanStartTime3 && thanStartTime3 >= startTimestr) && (endTimestr <= thanendTime3 && thanendTime3 > startTimestr)))
                            {
                                it.Thirteen_Fifteen_OlockTitle+= "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                it.Thirteen_Fifteen_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime4 && startTimestr < thanendTime4) && (endTimestr > thanStartTime4 && endTimestr <= thanendTime4)) || ((endTimestr > thanStartTime4 && thanStartTime4 >= startTimestr) && (endTimestr <= thanendTime4 && thanendTime4 > startTimestr)))
                            {
                                it.Fifteen_Seventeen_OlockTitle+= "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                it.Fifteen_Seventeen_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime5 && startTimestr < thanendTime5) && (endTimestr > thanStartTime5 && endTimestr <= thanendTime5)) || ((endTimestr > thanStartTime5 && thanStartTime5 >= startTimestr) && (endTimestr <= thanendTime5 && thanendTime5 > startTimestr)))
                            {
                                it.Seventeen_Nineteen_OlockTitle+= "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                it.Seventeen_Nineteen_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime6 && startTimestr < thanendTime6) && (endTimestr > thanStartTime6 && endTimestr <= thanendTime6)) || ((endTimestr > thanStartTime6 && thanStartTime6 >= startTimestr) && (endTimestr <= thanendTime6 && thanendTime6 > startTimestr)))
                            {
                                it.Nineteen_TwentyOne_OlockTitle += "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                it.Nineteen_TwentyOne_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime7 && startTimestr < thanendTime7) && (endTimestr > thanStartTime7&& endTimestr <= thanendTime7)) || ((endTimestr > thanStartTime7 && thanStartTime7>= startTimestr) && (endTimestr <= thanendTime7 && thanendTime7> startTimestr)))
                            {
                                it.TwentyOne_TwentyTree_OlockTitle += "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                it.TwentyOne_TwentyTree_StudyMode = iv.StudyMode;
                            }
                            if (!string.IsNullOrEmpty(iv.Comment) && iv.Comment != it.CourseComent&& iv.StudyMode != 5 && iv.StudyMode != 6)
                                it.CourseComent += "<div style=\"height:20px;border-top:solid 3px\"></div>" +iv.Comment;
                            if ((iv.StudyMode == 5 || iv.StudyMode== 6) && !string.IsNullOrEmpty(iv.Comment)) {
                                it.OtherComent += "<div style=\"height:20px;border-top:solid 3px\"></div>" + iv.Comment;
                            }
                            if (!string.IsNullOrEmpty(iv.CourseWork) && iv.CourseWork != it.CourseWorkCotent)
                                it.CourseWorkCotent +=iv.CourseWork+ "<div style=\"height:20px;border-top:solid 3px\"></div>";

                        });
                    }

                }
                //任务计划
                if (listPlan != null && listPlan.Count > 0)
                {
                    var filterlistPlan = listPlan.FindAll(iv => iv.WorkDate.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd")).ToList();
                    if (filterlistPlan != null && filterlistPlan.Count > 0)
                    {
                        filterlistPlan.ForEach(iv =>
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
                                if (string.IsNullOrEmpty(it.Eight_Ten_OlockTitle))
                                {
                                    it.Eight_Ten_OlockTitle += iv.WorkTitle;
                                }
                                else {
                                    it.Eight_Ten_Reversion = true;
                                }
                                it.Eight_Ten_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime2 && endTimestr <= thanendTime2)
                            {
                                if (string.IsNullOrEmpty(it.Ten_Twelve_OlockTitle))
                                {
                                    it.Ten_Twelve_OlockTitle += iv.WorkTitle;
                                }
                                else {
                                    it.Ten_Twelve_Reversion = true;
                                }
                                it.Ten_Twelve_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime3 && endTimestr <= thanendTime3)
                            {
                                if (string.IsNullOrEmpty(it.Thirteen_Fifteen_OlockTitle))
                                {
                                    it.Thirteen_Fifteen_OlockTitle += iv.WorkTitle;
                                }
                                else {
                                    it.Thirteen_Fifteen_Reversion = true;
                                }
                                it.Thirteen_Fifteen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime4 && endTimestr <= thanendTime4)
                            {
                                if (string.IsNullOrEmpty(it.Fifteen_Seventeen_OlockTitle))
                                {
                                    it.Fifteen_Seventeen_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    it.Fifteen_Seventeen_Reversion = true;
                                }
                                it.Fifteen_Seventeen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime5 && endTimestr <= thanendTime5)
                            {
                                if (string.IsNullOrEmpty(it.Seventeen_Nineteen_OlockTitle))
                                {
                                    it.Seventeen_Nineteen_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    it.Seventeen_Nineteen_Reversion = true;
                                }
                                it.Seventeen_Nineteen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime6 && endTimestr <= thanendTime6)
                            {
                                if (string.IsNullOrEmpty(it.Nineteen_TwentyOne_OlockTitle))
                                {
                                    it.Nineteen_TwentyOne_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    it.Nineteen_TwentyOne_Reversion = true;
                                }
                                it.Nineteen_TwentyOne_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime7 && endTimestr <= thanendTime7)
                            {
                                if (string.IsNullOrEmpty(it.TwentyOne_TwentyTree_OlockTitle))
                                {
                                    it.TwentyOne_TwentyTree_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    it.TwentyOne_TwentyTree_Reversion = true;
                                }
                                it.TwentyOne_TwentyTree_Id = iv.Id;
                            }
                            if (!string.IsNullOrEmpty(iv.SummaryComent) || !string.IsNullOrEmpty(iv.CourseComent) || !string.IsNullOrEmpty(iv.ChouciComent) || !string.IsNullOrEmpty(iv.HomeWorkComent) || !string.IsNullOrEmpty(iv.OtherComent) || !string.IsNullOrEmpty(iv.OutSchoolTime) || !string.IsNullOrEmpty(iv.InSchoolTime))
                            {
                                it.Id = iv.Id;
                            }
                            if (!string.IsNullOrEmpty(iv.ChouciComent) && iv.ChouciComent != it.ChouciComent)
                                it.ChouciComent += iv.ChouciComent;
                            if (!string.IsNullOrEmpty(iv.HomeWorkComent))
                                it.HomeWorkComent += iv.HomeWorkComent;
                            if (!string.IsNullOrEmpty(iv.CourseComent) && iv.CourseComent != it.CourseComent)
                                it.CourseComent +=iv.CourseComent;
                            if (!string.IsNullOrEmpty(iv.SummaryComent))
                                it.SummaryComent += iv.SummaryComent;
                            if (!string.IsNullOrEmpty(iv.OtherComent))
                                it.OtherComent += iv.OtherComent;
                            if (!string.IsNullOrEmpty(iv.InSchoolTime))
                                it.InSchoolTime= iv.InSchoolTime;
                            if (!string.IsNullOrEmpty(iv.OutSchoolTime))
                                it.OutSchoolTime = iv.OutSchoolTime;
                            if (!string.IsNullOrEmpty(iv.TaUid))
                            {
                                var hasTa = listTa.Find(cn => cn.User_ID == iv.TaUid);
                                if (hasTa != null) {
                                    it.TaUseName = hasTa.User_Name;
                                }
                            }
                        });
                    }
                }
                //抽词
                if (listTask != null && listTask.Count > 0)
                {
                    var filterlistTask = listTask.FindAll(iv => iv.PlanTime.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd")).ToList();
                    if (filterlistTask != null && filterlistTask.Count > 0)
                    {
                        filterlistTask.ForEach(iv =>
                        {
                            if (iv.Task_Mode == 1 || iv.Task_Mode == 2)
                            {
                                var taskComent = iv.Task_Name + ",平均反应时间" + iv.AvgSpell + ",正确率" + iv.Accuracy + ",正确数量" + iv.isRightCount;
                                if (it.ChouciComent != taskComent)
                                    it.ChouciComent += "<div style=\"height:20px;border-top:solid 3px\"></div>"+taskComent;
                            }
                            else
                            {
                                var taskComent = iv.Task_Name + ",拼写单词-单词数量" + iv.Words_Count + ",正确率" + iv.Accuracy + ",正确数量" + iv.isRightCount;
                                if (it.ChouciComent != taskComent)
                                    it.ChouciComent += "<div style=\"height:20px;border-top:solid 3px\"></div>"+taskComent;
                            }
                        });
                    }
                }
            });

            return Json(tdlistTime);
        }



        [UsersRoleAuthFilter("H-151", FunctionEnum.Add)]
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

        [UsersRoleAuthFilter("H-151", FunctionEnum.Edit)]
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


        [UsersRoleAuthFilter("H-151", FunctionEnum.Add)]
        public IActionResult AddComment(string dataStr, int studentUid)
        {
            ViewBag.ID = 0;
            ViewBag.StudentUid = studentUid;
            if (!string.IsNullOrEmpty(dataStr))
                dataStr = DateTime.Parse(dataStr).ToString("yyyy-MM-dd");
            ViewBag.DataStr = dataStr;
            return View();
        }

        [UsersRoleAuthFilter("H-151", FunctionEnum.Edit)]
        public IActionResult EditComment(int ID)
        {
            var model = _currencyService.DbAccess().Queryable<C_Student_Work_Plan>().Where(f => f.Id == ID).First();
            ViewBag.ID = ID;
            ViewBag.StudentUid = model.StudentUid;
            ViewBag.DataStr = model.WorkDate.ToString("yyyy-MM-dd");
            return View("AddComment");
        }



        [UsersRoleAuthFilter("H-151", "Add,Edit")]
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
                list =new { };
            }
            return Json(list);
        }

        public IActionResult deleteCommend(int Id) {
            ResResult rsg = new ResResult() { code = 0, msg = "删除失败" };
            if (Id>0)
            {
                var result = _currencyService.DbAccess().Deleteable<C_Student_Work_Plan>().Where(v=>v.Id==Id).ExecuteCommand();
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


        /// <summary>
        /// 导出计划（excel）
        /// </summary>
        /// <param name="studentUid"></param>
        /// <param name="endtime"></param>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public virtual IActionResult ExportPlan(int studentUid, int isAdd, DateTime? endtime = null, DateTime? starttime = null)
        {
            DateTime dateWeekFirstDay = GetFirstDayOfWeek(DateTime.Now);
            DateTime dateWeekLastDay = new DateTime();
            //如果点击前一周
            if (starttime != null)
            {
                dateWeekFirstDay = starttime.Value;
            }
            //如果点击后一周
            if (endtime != null)
            {
                dateWeekLastDay = endtime.Value;
            }
            string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            List<StudentWorkPlanModel> tdlistTime = new List<StudentWorkPlanModel>();
            if (endtime.HasValue && starttime.HasValue)
            {
                TimeSpan span = Convert.ToDateTime(endtime.Value.ToString("yyyy-MM-dd")) - Convert.ToDateTime(starttime.Value.ToString("yyyy-MM-dd"));
                for (var i = 0; i < span.Days+1; i++) {
                    StudentWorkPlanModel timeModel = new StudentWorkPlanModel();
                    timeModel.WorkDate = dateWeekFirstDay.AddDays(i);
                    string week = Day[Convert.ToInt32(timeModel.WorkDate.DayOfWeek.ToString("d"))].ToString();
                    timeModel.WorkDateName = week;
                    tdlistTime.Add(timeModel);
                }
            }
            else {
                for (var i = 0; i < 7; i++)
                {
                    StudentWorkPlanModel timeModel = new StudentWorkPlanModel();
                    timeModel.WorkDate = dateWeekFirstDay.AddDays(i);
                    string week = Day[Convert.ToInt32(timeModel.WorkDate.DayOfWeek.ToString("d"))].ToString();
                    timeModel.WorkDateName = week;
                    if (i == 6)
                    {
                        dateWeekLastDay = dateWeekFirstDay.AddDays(i);
                    }
                    tdlistTime.Add(timeModel);
                }
            }
            List<sys_user> listTa = _currencyService.DbAccess().Queryable<sys_userrole, sys_user, sys_role>((ur, u, r) => new object[] { JoinType.Left, ur.UserRole_UserID == u.User_ID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where((ur, u, r) => r.Role_Name == "督学").Select<sys_user>((ur, u, r) => u).ToList();
            //查询学生排课
            //排除退班的班级
            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
            List<int> classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((ch, cl) => new object[] { JoinType.Left, ch.ClassId == cl.ClassId }).Where(ch => ch.StudentUid == studentUid && ch.ClassId > 0 && ch.Contrac_Child_Status != contracStatus).Select(ch => ch.ClassId).ToList();
            List<CourseWorkModel> listCourseWork = _currencyService.DbAccess().Queryable<C_Course_Work, sys_user, C_Room>((cour, ta, room) => new object[] { JoinType.Left, cour.TeacherUid == ta.User_ID, JoinType.Left, cour.RoomId == room.Id })
                .Where(cour => (cour.StudentUid == studentUid || classIds.Contains(cour.ClasssId)) && cour.StudyMode != 3 && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00") >= DateTime.Parse(dateWeekFirstDay.ToString("yyyy-MM-dd") + " 00:00") && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00") <= DateTime.Parse(dateWeekLastDay.ToString("yyyy-MM-dd") + " 00:00"))
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
                    CourseWork = cour.CourseWork
                }).ToList();
            //查询学生任务计划
            List<C_Student_Work_Plan> listPlan = _currencyService.DbAccess().Queryable<C_Student_Work_Plan>().Where(it => it.StudentUid == studentUid && DateTime.Parse(it.WorkDate.ToString("yyyy-MM-dd") + " 00:00") >= DateTime.Parse(dateWeekFirstDay.ToString("yyyy-MM-dd") + " 00:00") && DateTime.Parse(it.WorkDate.ToString("yyyy-MM-dd") + " 00:00") <= DateTime.Parse(dateWeekLastDay.ToString("yyyy-MM-dd") + " 00:00")).ToList();
            //查询学生抽词情况
            C_Contrac_User u = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(it => it.StudentUid == studentUid).First();
            List<MyTaskModel> listTask = new List<MyTaskModel>();
            if (!string.IsNullOrEmpty(u.Student_Account))
            {
                dynamic chouciUser = _currencyService.DbAccess().Queryable("ynk_chouci.dbo.[view_user]", "u").Where("u.User_AccountName='" + u.Student_Account + "' and u.User_Password='" + u.Student_Pwd + "'").First();
                if (chouciUser != null) {
                    listTask = _currencyService.DbAccess().Queryable(@"(select task.Task_Mode,task.Task_Name,mytask.Words_Count,mytask.PlanTime,isRightCount=(select count(*) from ynk_chouci.dbo.[view_my_task_word] taskword where taskword.Uid=mytask.Uid and taskword.TaskId=mytask.TaskId and taskword.Answer_IsRight=1),
                SecondAll=(select sum(taskword.Answer_Second) from ynk_chouci.dbo.[view_my_task_word] taskword where taskword.Uid=mytask.Uid and taskword.TaskId=mytask.TaskId) from ynk_chouci.dbo.[view_my_task] mytask 
                left join ynk_chouci.dbo.[view_task] task on  mytask.TaskId=task.TaskId where mytask.[Uid]=@uid and mytask.Finish_Status=2)", "orgin").AddParameters(new { uid = chouciUser.Uid }).Select<MyTaskModel>().ToList();
                }
            }
            tdlistTime.ForEach(it =>
            {
                //课程
                if (listCourseWork != null && listCourseWork.Count > 0)
                {
                    var filterCosurWork = listCourseWork.FindAll(iv => iv.AT_Date.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd")).ToList();
                    if (filterCosurWork != null && filterCosurWork.Count > 0)
                    {
                        filterCosurWork.ForEach(iv =>
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
                            var thanendTime7 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 23:50");
                            if (((startTimestr >= thanStartTime1 && startTimestr < thanendTime1)&&(endTimestr >thanStartTime1 && endTimestr <= thanendTime1)) || ((endTimestr> thanStartTime1 && thanStartTime1>=startTimestr) &&(endTimestr<= thanendTime1 && thanendTime1>startTimestr)))
                            {
                                it.Eight_Ten_OlockTitle+= iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                            }
                           else  if (((startTimestr >= thanStartTime2 && startTimestr < thanendTime2) && (endTimestr > thanStartTime2 && endTimestr <= thanendTime2)) || ((endTimestr > thanStartTime2 && thanStartTime2 >= startTimestr) && (endTimestr <= thanendTime2 && thanendTime2 > startTimestr)) || ((startTimestr >= thanStartTime2 && startTimestr < thanendTime2) && endTimestr <= thanStartTime3))
                            {
                                it.Ten_Twelve_OlockTitle+= iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                            }
                           else if (((startTimestr >= thanStartTime3 && startTimestr < thanendTime3) && (endTimestr > thanStartTime3 && endTimestr <= thanendTime3)) || ((endTimestr > thanStartTime3 && thanStartTime3 >= startTimestr) && (endTimestr <= thanendTime3 && thanendTime3 > startTimestr)))
                            {
                                it.Thirteen_Fifteen_OlockTitle+= iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                            }
                           else if (((startTimestr >= thanStartTime4 && startTimestr < thanendTime4) && (endTimestr > thanStartTime4 && endTimestr <= thanendTime4)) || ((endTimestr > thanStartTime4 && thanStartTime4 >= startTimestr) && (endTimestr <= thanendTime4 && thanendTime4 > startTimestr)))
                            {
                                it.Fifteen_Seventeen_OlockTitle+= iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                            }
                            else if (((startTimestr >= thanStartTime5 && startTimestr < thanendTime5) && (endTimestr > thanStartTime5&& endTimestr <= thanendTime5)) || ((endTimestr > thanStartTime5 && thanStartTime5 >= startTimestr) && (endTimestr <= thanendTime5 && thanendTime5> startTimestr)))
                            {
                                it.Seventeen_Nineteen_OlockTitle+= iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                            }
                            else if (((startTimestr >= thanStartTime6 && startTimestr < thanendTime6) && (endTimestr > thanStartTime6 && endTimestr <= thanendTime6)) || ((endTimestr > thanStartTime6 && thanStartTime6 >= startTimestr) && (endTimestr <= thanendTime6&& thanendTime6> startTimestr)))
                            {
                                it.Nineteen_TwentyOne_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                            }
                            else if (((startTimestr >= thanStartTime7 && startTimestr < thanendTime7) && (endTimestr > thanStartTime7 && endTimestr <= thanendTime7)) || ((endTimestr > thanStartTime7 && thanStartTime7 >= startTimestr) && (endTimestr <= thanendTime7 && thanendTime7 > startTimestr)))
                            {
                                it.TwentyOne_TwentyTree_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                            }
                            if (!string.IsNullOrEmpty(iv.Comment) && iv.Comment != it.CourseComent && iv.StudyMode != 5 && iv.StudyMode != 6)
                            {
                                it.CourseComent += iv.Comment;
                            }
                            if (!string.IsNullOrEmpty(iv.CourseWork) && iv.CourseWork != it.CourseWorkCotent)
                            {
                                it.CourseWorkCotent += iv.CourseWork;
                            }
                            if ((iv.StudyMode == 5 || iv.StudyMode == 6) && !string.IsNullOrEmpty(iv.Comment))
                            {
                                it.OtherComent += iv.Comment;
                            }

                        });
                    }

                }
                //任务计划
                if (listPlan != null && listPlan.Count > 0)
                {
                    var filterlistPlan = listPlan.FindAll(iv => iv.WorkDate.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd")).ToList();
                    if (filterlistPlan != null && filterlistPlan.Count > 0)
                    {
                        filterlistPlan.ForEach(iv =>
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
                            var thanendTime7= DateTime.Parse(iv.WorkDate.ToString("yyyy-MM-dd") + " 23:00");
                            if (startTimestr >= thanStartTime1 && endTimestr <= thanendTime1)
                            {
                                if (string.IsNullOrEmpty(it.Eight_Ten_OlockTitle)) {
                                    it.Eight_Ten_OlockTitle += iv.WorkTitle;
                                }
                                it.Eight_Ten_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime2 && endTimestr <= thanendTime2)
                            {
                                if (string.IsNullOrEmpty(it.Ten_Twelve_OlockTitle)) {
                                    it.Ten_Twelve_OlockTitle += iv.WorkTitle;
                                }
                                it.Ten_Twelve_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime3 && endTimestr <= thanendTime3)
                            {
                                if (string.IsNullOrEmpty(it.Thirteen_Fifteen_OlockTitle))
                                {
                                    it.Thirteen_Fifteen_OlockTitle += iv.WorkTitle;
                                }
                                it.Thirteen_Fifteen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime4 && endTimestr <= thanendTime4)
                            {
                                if (string.IsNullOrEmpty(it.Fifteen_Seventeen_OlockTitle))
                                {
                                    it.Fifteen_Seventeen_OlockTitle += iv.WorkTitle;
                                }
                                it.Fifteen_Seventeen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime5 && endTimestr <= thanendTime5)
                            {
                                if (string.IsNullOrEmpty(it.Seventeen_Nineteen_OlockTitle))
                                {
                                    it.Seventeen_Nineteen_OlockTitle += iv.WorkTitle;
                                }
                                it.Seventeen_Nineteen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime6 && endTimestr <= thanendTime6)
                            {
                                if (string.IsNullOrEmpty(it.Nineteen_TwentyOne_OlockTitle))
                                {
                                    it.Nineteen_TwentyOne_OlockTitle += iv.WorkTitle;
                                }
                                it.Nineteen_TwentyOne_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime7 && endTimestr <= thanendTime7)
                            {
                                if (string.IsNullOrEmpty(it.TwentyOne_TwentyTree_OlockTitle))
                                {
                                    it.TwentyOne_TwentyTree_OlockTitle += iv.WorkTitle;
                                }
                                it.TwentyOne_TwentyTree_Id = iv.Id;
                            }
                            if (!string.IsNullOrEmpty(iv.SummaryComent) || !string.IsNullOrEmpty(iv.CourseComent) || !string.IsNullOrEmpty(iv.ChouciComent) || !string.IsNullOrEmpty(iv.HomeWorkComent) || !string.IsNullOrEmpty(iv.OtherComent) || !string.IsNullOrEmpty(iv.InSchoolTime) || !string.IsNullOrEmpty(iv.OutSchoolTime))
                            {
                                it.Id = iv.Id;
                            }
                            if (!string.IsNullOrEmpty(iv.ChouciComent)&&iv.ChouciComent!=it.ChouciComent)
                                it.ChouciComent += iv.ChouciComent;
                            if (!string.IsNullOrEmpty(iv.HomeWorkComent))
                                it.HomeWorkComent += iv.HomeWorkComent;
                            if (!string.IsNullOrEmpty(iv.CourseComent)&&iv.CourseComent!=it.CourseComent)
                                it.CourseComent += iv.CourseComent;
                            if (!string.IsNullOrEmpty(iv.SummaryComent))
                                it.SummaryComent += iv.SummaryComent;
                            if (!string.IsNullOrEmpty(iv.OtherComent))
                                it.OtherComent += iv.OtherComent;
                            if (!string.IsNullOrEmpty(iv.InSchoolTime))
                                it.InSchoolTime += iv.InSchoolTime;
                            if (!string.IsNullOrEmpty(iv.OutSchoolTime))
                                it.OutSchoolTime += iv.OutSchoolTime;
                            if (!string.IsNullOrEmpty(iv.TaUid))
                            {
                                var hasTa = listTa.Find(cn => cn.User_ID == iv.TaUid);
                                if (hasTa != null)
                                {
                                    it.TaUseName = hasTa.User_Name;
                                }
                            }
                        });
                    }
                }
                //抽词
                if (listTask != null && listTask.Count > 0)
                {
                    var filterlistTask = listTask.FindAll(iv => iv.PlanTime.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd")).ToList();
                    if (filterlistTask != null && filterlistTask.Count > 0)
                    {
                        filterlistTask.ForEach(iv =>
                        {
                            if (iv.Task_Mode == 1 || iv.Task_Mode == 2)
                            {
                                var taskComent= iv.Task_Name + ",平均反应时间" + iv.AvgSpell + ",正确率" + iv.Accuracy + ",正确数量" + iv.isRightCount;
                                if (it.ChouciComent != taskComent)
                                    it.ChouciComent += taskComent;
                            }
                            else
                            {
                                var taskComent = iv.Task_Name + ",拼写单词-单词数量" + iv.Words_Count + ",正确率" + iv.Accuracy + ",正确数量" + iv.isRightCount;
                                if (it.ChouciComent != taskComent)
                                    it.ChouciComent += taskComent;
                            }
                        });
                    }
                }
            });
            //导出代码
            var workbook = new HSSFWorkbook();
            //标题列样式
            var headFont = workbook.CreateFont();
            headFont.IsBold = true;
            var headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            headStyle.VerticalAlignment = VerticalAlignment.Top;
            headStyle.BorderBottom = BorderStyle.Thin;
            headStyle.BorderLeft = BorderStyle.Thin;
            headStyle.BorderRight = BorderStyle.Thin;
            headStyle.BorderTop = BorderStyle.Thin;
            headStyle.WrapText = true;//自动换行
            headStyle.SetFont(headFont);
            var sheet = workbook.CreateSheet(u.Student_Name + "任务计划");
            sheet.DefaultColumnWidth = 25;
            var yurow = tdlistTime.Count % 7;
            var groupRow = (tdlistTime.Count - yurow) / 7;
            if (yurow > 0) {
                groupRow = groupRow + 1;
            }
            for (var j = 0; j < groupRow; j++) {
                var list = tdlistTime.Skip(j*7).Take(7).ToList();
                for (var y = 0; y < 20; y++) {
                    var yRow = (j* 20)+y;
                    var row = sheet.CreateRow(yRow);
                    if (y == 0) {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("日期");
                        cell.CellStyle = headStyle;
                        for (var x =0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x+1);
                            cell.SetCellValue(list[x].WorkDate.ToString("yyyy-MM-dd"));
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 1) {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("星期");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].WorkDateName);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y ==2)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("8:00-10:00");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].Eight_Ten_OlockTitle);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y ==3)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("10:00-12:00");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].Ten_Twelve_OlockTitle);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 4)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("13:00-15:00");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].Thirteen_Fifteen_OlockTitle);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 5)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("15:00-17:00");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].Fifteen_Seventeen_OlockTitle);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 6)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("17:00-19:00");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].Seventeen_Nineteen_OlockTitle);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 7)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("19:00-21:00");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].Nineteen_TwentyOne_OlockTitle);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 8)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("21:00-23:00");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].TwentyOne_TwentyTree_OlockTitle);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 9)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("单词");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].ChouciComent);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 10)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("课程点评");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].CourseComent);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 11)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("布置作业");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].CourseWorkCotent);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 12)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("作业完成情况");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].HomeWorkComent);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 13)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("其它");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].OtherComent);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 14)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("责任助教");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].TaUseName);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 15)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("每日总结");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].SummaryComent);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 16)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("到校时间");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].InSchoolTime);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 17)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("离校时间");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].OutSchoolTime);
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 18)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue("");
                            cell.CellStyle = headStyle;
                        }
                    }

                }
            }
            #region 不换行日期代码已注释
            //循环行
            //for (var y = 0; y < 19; y++)
            //{
            //    if (y == 0)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("日期");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.WorkDate.ToString("yyyy-MM-dd"));
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 1)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("星期");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.WorkDateName);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 2)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("8:00-10:00");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.Eight_Ten_OlockTitle);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 3)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("10:00-12:00");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.Ten_Twelve_OlockTitle);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 4)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("13:00-15:00");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.Thirteen_Fifteen_OlockTitle);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 5)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("15:00-17:00");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.Fifteen_Seventeen_OlockTitle);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 6)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("17:00-19:00");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.Seventeen_Nineteen_OlockTitle);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 7)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("19:00-21:00");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.Nineteen_TwentyOne_OlockTitle);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 8)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("21:00-23:00");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.TwentyOne_TwentyTree_OlockTitle);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 9)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("单词");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.ChouciComent);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 10)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("课程点评");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.CourseComent);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 11)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("布置作业");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.CourseWorkCotent);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 12)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("作业完成情况");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.HomeWorkComent);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 13)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("其它");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.OtherComent);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 14)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("责任助教");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.TaUseName);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 15)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("每日总结");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.SummaryComent);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 16)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("到校时间");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.InSchoolTime);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //    if (y == 17)
            //    {
            //        var row = sheet.CreateRow(y);
            //        var cell = row.CreateCell(0);
            //        cell.SetCellValue("离校时间");
            //        cell.CellStyle = headStyle;
            //        var x = 1;
            //        tdlistTime.ForEach(it =>
            //        {
            //            cell = row.CreateCell(x);
            //            cell.SetCellValue(it.OutSchoolTime);
            //            cell.CellStyle = headStyle;
            //            x++;
            //        });
            //    }
            //}
            #endregion

            //保存
            byte[] streamArr = null;
            //保存
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                streamArr = ms.ToArray();
            }
            return File(streamArr, "application/vnd.ms-excel", u.Student_Name + "任务计划" + DateTime.Now.ToString("yyyyMMddHHmmssss") + ".xls");
        }

        public IActionResult ExportPdf(int studentUid, int isAdd, DateTime? endtime = null, DateTime? starttime = null) {
            //导出pdf
            DateTime dateWeekFirstDay = GetFirstDayOfWeek(DateTime.Now);
            DateTime dateWeekLastDay = new DateTime();
            //如果点击前一周
            if (starttime != null)
            {
                dateWeekFirstDay = starttime.Value;
            }
            //如果点击后一周
            if (endtime != null)
            {
                dateWeekLastDay = endtime.Value;
            }
            string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            List<StudentWorkPlanModel> tdlistTime = new List<StudentWorkPlanModel>();
            if (endtime.HasValue && starttime.HasValue)
            {
                TimeSpan span = Convert.ToDateTime(endtime.Value.ToString("yyyy-MM-dd")) - Convert.ToDateTime(starttime.Value.ToString("yyyy-MM-dd"));
                for (var i = 0; i < span.Days + 1; i++)
                {
                    StudentWorkPlanModel timeModel = new StudentWorkPlanModel();
                    timeModel.WorkDate = dateWeekFirstDay.AddDays(i);
                    string week = Day[Convert.ToInt32(timeModel.WorkDate.DayOfWeek.ToString("d"))].ToString();
                    timeModel.WorkDateName = week;
                    tdlistTime.Add(timeModel);
                }
            }
            else
            {
                for (var i = 0; i < 7; i++)
                {
                    StudentWorkPlanModel timeModel = new StudentWorkPlanModel();
                    timeModel.WorkDate = dateWeekFirstDay.AddDays(i);
                    string week = Day[Convert.ToInt32(timeModel.WorkDate.DayOfWeek.ToString("d"))].ToString();
                    timeModel.WorkDateName = week;
                    if (i == 6)
                    {
                        dateWeekLastDay = dateWeekFirstDay.AddDays(i);
                    }
                    tdlistTime.Add(timeModel);
                }
            }
            List<sys_user> listTa = _currencyService.DbAccess().Queryable<sys_userrole, sys_user, sys_role>((ur, u, r) => new object[] { JoinType.Left, ur.UserRole_UserID == u.User_ID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where((ur, u, r) => r.Role_Name == "督学").Select<sys_user>((ur, u, r) => u).ToList();
            //查询学生排课
            //排除退班的班级
            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
            List<int> classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((ch, cl) => new object[] { JoinType.Left, ch.ClassId == cl.ClassId }).Where(ch => ch.StudentUid == studentUid && ch.ClassId > 0 && ch.Contrac_Child_Status != contracStatus).Select(ch => ch.ClassId).ToList();
            List<CourseWorkModel> listCourseWork = _currencyService.DbAccess().Queryable<C_Course_Work, sys_user, C_Room>((cour, ta, room) => new object[] { JoinType.Left, cour.TeacherUid == ta.User_ID, JoinType.Left, cour.RoomId == room.Id })
                .Where(cour => (cour.StudentUid == studentUid || classIds.Contains(cour.ClasssId)) && cour.StudyMode != 3 && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00") >= DateTime.Parse(dateWeekFirstDay.ToString("yyyy-MM-dd") + " 00:00") && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00") <= DateTime.Parse(dateWeekLastDay.ToString("yyyy-MM-dd") + " 00:00"))
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
                    CourseWork = cour.CourseWork
                }).ToList();
            //查询学生任务计划
            List<C_Student_Work_Plan> listPlan = _currencyService.DbAccess().Queryable<C_Student_Work_Plan>().Where(it => it.StudentUid == studentUid && DateTime.Parse(it.WorkDate.ToString("yyyy-MM-dd") + " 00:00") >= DateTime.Parse(dateWeekFirstDay.ToString("yyyy-MM-dd") + " 00:00") && DateTime.Parse(it.WorkDate.ToString("yyyy-MM-dd") + " 00:00") <= DateTime.Parse(dateWeekLastDay.ToString("yyyy-MM-dd") + " 00:00")).ToList();
            //查询学生抽词情况
            C_Contrac_User u = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(it => it.StudentUid == studentUid).First();
            List<MyTaskModel> listTask = new List<MyTaskModel>();
            if (!string.IsNullOrEmpty(u.Student_Account))
            {
                dynamic chouciUser = _currencyService.DbAccess().Queryable("ynk_chouci.dbo.[view_user]", "u").Where("u.User_AccountName='" + u.Student_Account + "' and u.User_Password='" + u.Student_Pwd + "'").First();
                if (chouciUser != null)
                {
                    listTask = _currencyService.DbAccess().Queryable(@"(select task.Task_Mode,task.Task_Name,mytask.Words_Count,mytask.PlanTime,isRightCount=(select count(*) from ynk_chouci.dbo.[view_my_task_word] taskword where taskword.Uid=mytask.Uid and taskword.TaskId=mytask.TaskId and taskword.Answer_IsRight=1),
                SecondAll=(select sum(taskword.Answer_Second) from ynk_chouci.dbo.[view_my_task_word] taskword where taskword.Uid=mytask.Uid and taskword.TaskId=mytask.TaskId) from ynk_chouci.dbo.[view_my_task] mytask 
                left join ynk_chouci.dbo.[view_task] task on  mytask.TaskId=task.TaskId where mytask.[Uid]=@uid and mytask.Finish_Status=2)", "orgin").AddParameters(new { uid = chouciUser.Uid }).Select<MyTaskModel>().ToList();
                }
            }
            tdlistTime.ForEach(it =>
            {
                //课程
                if (listCourseWork != null && listCourseWork.Count > 0)
                {
                    var filterCosurWork = listCourseWork.FindAll(iv => iv.AT_Date.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd")).ToList();
                    if (filterCosurWork != null && filterCosurWork.Count > 0)
                    {
                        filterCosurWork.ForEach(iv =>
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
                            var thanendTime7 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 23:50");
                            if (((startTimestr >= thanStartTime1 && startTimestr < thanendTime1) && (endTimestr > thanStartTime1 && endTimestr <= thanendTime1)) || ((endTimestr > thanStartTime1 && thanStartTime1 >= startTimestr) && (endTimestr <= thanendTime1 && thanendTime1 > startTimestr)))
                            {
                                it.Eight_Ten_OlockTitle += iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                                it.Eight_Ten_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime2 && startTimestr < thanendTime2) && (endTimestr > thanStartTime2 && endTimestr <= thanendTime2)) || ((endTimestr > thanStartTime2 && thanStartTime2 >= startTimestr) && (endTimestr <= thanendTime2 && thanendTime2 > startTimestr)) || ((startTimestr >= thanStartTime2 && startTimestr < thanendTime2) && endTimestr <= thanStartTime3))
                            {
                                it.Ten_Twelve_OlockTitle += iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                                it.Ten_Twelve_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime3 && startTimestr < thanendTime3) && (endTimestr > thanStartTime3 && endTimestr <= thanendTime3)) || ((endTimestr > thanStartTime3 && thanStartTime3 >= startTimestr) && (endTimestr <= thanendTime3 && thanendTime3 > startTimestr)))
                            {
                                it.Thirteen_Fifteen_OlockTitle += iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                                it.Thirteen_Fifteen_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime4 && startTimestr < thanendTime4) && (endTimestr > thanStartTime4 && endTimestr <= thanendTime4)) || ((endTimestr > thanStartTime4 && thanStartTime4 >= startTimestr) && (endTimestr <= thanendTime4 && thanendTime4 > startTimestr)))
                            {
                                it.Fifteen_Seventeen_OlockTitle += iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                                it.Fifteen_Seventeen_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime5 && startTimestr < thanendTime5) && (endTimestr > thanStartTime5 && endTimestr <= thanendTime5)) || ((endTimestr > thanStartTime5 && thanStartTime5 >= startTimestr) && (endTimestr <= thanendTime5 && thanendTime5 > startTimestr)))
                            {
                                it.Seventeen_Nineteen_OlockTitle += iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                                it.Seventeen_Nineteen_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime6 && startTimestr < thanendTime6) && (endTimestr > thanStartTime6 && endTimestr <= thanendTime6)) || ((endTimestr > thanStartTime6 && thanStartTime6 >= startTimestr) && (endTimestr <= thanendTime6 && thanendTime6 > startTimestr)))
                            {
                                it.Nineteen_TwentyOne_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                it.Nineteen_TwentyOne_StudyMode = iv.StudyMode;
                            }
                            else if (((startTimestr >= thanStartTime7 && startTimestr < thanendTime7) && (endTimestr > thanStartTime7 && endTimestr <= thanendTime7)) || ((endTimestr > thanStartTime7 && thanStartTime7 >= startTimestr) && (endTimestr <= thanendTime7 && thanendTime7 > startTimestr)))
                            {
                                it.TwentyOne_TwentyTree_OlockTitle += "  " + iv.Work_Title + " 教师:" + iv.TeacherName + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                it.TwentyOne_TwentyTree_StudyMode = iv.StudyMode;
                            }
                            if (!string.IsNullOrEmpty(iv.Comment) && iv.Comment != it.CourseComent && iv.StudyMode != 5 && iv.StudyMode != 6)
                            {
                                it.CourseComent += "<div style=\"height:20px;border-top:solid 3px\"></div>" + iv.Comment;
                            }
                            if ((iv.StudyMode == 5 || iv.StudyMode == 6) && !string.IsNullOrEmpty(iv.Comment))
                            {
                                it.OtherComent += "<div style=\"height:20px;border-top:solid 3px\"></div>" + iv.Comment;
                            }
                            if (!string.IsNullOrEmpty(iv.CourseWork) && iv.CourseWork != it.CourseWorkCotent)
                                it.CourseWorkCotent += iv.CourseWork + "<div style=\"height:20px;border-top:solid 3px\"></div>";

                        });
                    }

                }
                //任务计划
                if (listPlan != null && listPlan.Count > 0)
                {
                    var filterlistPlan = listPlan.FindAll(iv => iv.WorkDate.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd")).ToList();
                    if (filterlistPlan != null && filterlistPlan.Count > 0)
                    {
                        filterlistPlan.ForEach(iv =>
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
                                if (string.IsNullOrEmpty(it.Eight_Ten_OlockTitle))
                                {
                                    it.Eight_Ten_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    it.Eight_Ten_Reversion = true;
                                }
                                it.Eight_Ten_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime2 && endTimestr <= thanendTime2)
                            {
                                if (string.IsNullOrEmpty(it.Ten_Twelve_OlockTitle))
                                {
                                    it.Ten_Twelve_OlockTitle += iv.WorkTitle;
                                }
                                else { 
                                   it.Ten_Twelve_Reversion = true;
                                }
                                it.Ten_Twelve_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime3 && endTimestr <= thanendTime3)
                            {
                                if (string.IsNullOrEmpty(it.Thirteen_Fifteen_OlockTitle))
                                {
                                    it.Thirteen_Fifteen_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    it.Thirteen_Fifteen_Reversion = true;
                                }
                                it.Thirteen_Fifteen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime4 && endTimestr <= thanendTime4)
                            {
                                if (string.IsNullOrEmpty(it.Fifteen_Seventeen_OlockTitle))
                                {
                                    it.Fifteen_Seventeen_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    it.Fifteen_Seventeen_Reversion = true;
                                }
                                it.Fifteen_Seventeen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime5 && endTimestr <= thanendTime5)
                            {
                                if (string.IsNullOrEmpty(it.Seventeen_Nineteen_OlockTitle))
                                {
                                    it.Seventeen_Nineteen_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    it.Seventeen_Nineteen_Reversion = true;
                                }
                                it.Seventeen_Nineteen_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime6 && endTimestr <= thanendTime6)
                            {
                                if (string.IsNullOrEmpty(it.Nineteen_TwentyOne_OlockTitle))
                                {
                                    it.Nineteen_TwentyOne_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    it.Nineteen_TwentyOne_Reversion = true;
                                }
                                it.Nineteen_TwentyOne_Id = iv.Id;
                            }
                            else if (startTimestr >= thanStartTime7 && endTimestr <= thanendTime7)
                            {
                                if (string.IsNullOrEmpty(it.TwentyOne_TwentyTree_OlockTitle))
                                {
                                    it.TwentyOne_TwentyTree_OlockTitle += iv.WorkTitle;
                                }
                                else
                                {
                                    it.TwentyOne_TwentyTree_Reversion = true;
                                }
                                it.TwentyOne_TwentyTree_Id = iv.Id;
                            }
                            if (!string.IsNullOrEmpty(iv.SummaryComent) || !string.IsNullOrEmpty(iv.CourseComent) || !string.IsNullOrEmpty(iv.ChouciComent) || !string.IsNullOrEmpty(iv.HomeWorkComent) || !string.IsNullOrEmpty(iv.OtherComent) || !string.IsNullOrEmpty(iv.InSchoolTime) || !string.IsNullOrEmpty(iv.OutSchoolTime))
                            {
                                it.Id = iv.Id;
                            }
                            if (!string.IsNullOrEmpty(iv.ChouciComent) && iv.ChouciComent != it.ChouciComent)
                                it.ChouciComent += iv.ChouciComent;
                            if (!string.IsNullOrEmpty(iv.HomeWorkComent))
                                it.HomeWorkComent += iv.HomeWorkComent;
                            if (!string.IsNullOrEmpty(iv.CourseComent) && iv.CourseComent != it.CourseComent)
                                it.CourseComent += iv.CourseComent;
                            if (!string.IsNullOrEmpty(iv.SummaryComent))
                                it.SummaryComent += iv.SummaryComent;
                            if (!string.IsNullOrEmpty(iv.OtherComent))
                                it.OtherComent += iv.OtherComent;
                            if (!string.IsNullOrEmpty(iv.InSchoolTime))
                                it.InSchoolTime += iv.InSchoolTime;
                            if (!string.IsNullOrEmpty(iv.OutSchoolTime))
                                it.OutSchoolTime += iv.OutSchoolTime;
                            if (!string.IsNullOrEmpty(iv.TaUid))
                            {
                                var hasTa = listTa.Find(cn => cn.User_ID == iv.TaUid);
                                if (hasTa != null)
                                {
                                    it.TaUseName = hasTa.User_Name;
                                }
                            }
                        });
                    }
                }
                //抽词
                if (listTask != null && listTask.Count > 0)
                {
                    var filterlistTask = listTask.FindAll(iv => iv.PlanTime.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd")).ToList();
                    if (filterlistTask != null && filterlistTask.Count > 0)
                    {
                        filterlistTask.ForEach(iv =>
                        {
                            if (iv.Task_Mode == 1 || iv.Task_Mode == 2)
                            {
                                var taskComent = iv.Task_Name + ",平均反应时间" + iv.AvgSpell + ",正确率" + iv.Accuracy + ",正确数量" + iv.isRightCount;
                                if (it.ChouciComent != taskComent)
                                    it.ChouciComent += taskComent;
                            }
                            else
                            {
                                var taskComent = iv.Task_Name + ",拼写单词-单词数量" + iv.Words_Count + ",正确率" + iv.Accuracy + ",正确数量" + iv.isRightCount;
                                if (it.ChouciComent != taskComent)
                                    it.ChouciComent += taskComent;
                            }
                        });
                    }
                }
            });
            string studentName = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(c => c.StudentUid == studentUid).First().Student_Name;
            Dictionary<string, List<StudentWorkPlanModel>> list = new Dictionary<string, List<StudentWorkPlanModel>>();
            list.Add(studentName,tdlistTime);
            return new ViewAsPdf("ExportPdf", list) {
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        public DateTime GetFirstDayOfWeek(DateTime dt)
        {
            dt = dt == null ? DateTime.Now : dt;
            int daydiff = (int)dt.DayOfWeek - 1 < 0 ? 6 : (int)dt.DayOfWeek - 1;//如果是0结果小于0表示周日 那最后要减6天:其他天数在dayOfWeek上减1，表示回到周一
            DateTime result = dt.AddDays(-daydiff);
            return result;
        }

    }
}
