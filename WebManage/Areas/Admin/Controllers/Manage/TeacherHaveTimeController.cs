using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class TeacherHaveTimeController : BaseController
    {
        private ICurrencyService _currencyService;
        public TeacherHaveTimeController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "V-357";
        }

        [UsersRoleAuthFilter("V-357", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }


        //查询学员，教师，或者班级的名称
        public IActionResult QueryNameAll(string title)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            string where = "";
            if (!string.IsNullOrEmpty(title))
            {
                where = " and charindex(@title,tach.User_Name)>0 ";
            }
            List<SeachUNameModel> listSerchName = _currencyService.DbAccess().Queryable(@"(
             select tach.User_Name as Name,tach.[User_ID] from Sys_User tach left join Sys_UserRole ur on tach.User_ID=ur.UserRole_UserID left join Sys_Role r on ur.UserRole_RoleID=r.Role_ID 
              where  (r.Role_Name='教师' or r.Role_Name='教学校长') and tach.User_IsDelete=2 " + where+ ")", "orgin").AddParameters(new { title = title }).Select<SeachUNameModel>().ToList();
            rsg.data = listSerchName;
            return Json(rsg);
        }


        [UsersRoleAuthFilter("V-357", FunctionEnum.Have)]
        public IActionResult QueryTeacherTime(string teacherNames,DateTime? endtime = null, DateTime? starttime = null)
        {
            string[] names = teacherNames.Split(",");
            List<TeacherTimeWorkModel> tdlistTime = new List<TeacherTimeWorkModel>();
            var teachers = _currencyService.DbAccess().Queryable<sys_user>().Where(c =>names.Contains(c.User_Name)).ToList();
            if (teacherNames != null&& names.Length>0) {
                var teacherUid = teachers.Select(n => n.User_ID).ToList();
                if (!starttime.HasValue)
                {
                    starttime = GetFirstDayOfWeek(DateTime.Now);
                    for (var i = 0; i < 7; i++)
                    {
                        if (i == 6)
                        {
                            endtime = starttime.Value.AddDays(i);
                        }
                    }
                }
                TimeSpan span = Convert.ToDateTime(endtime.Value.ToString("yyyy-MM-dd")) - Convert.ToDateTime(starttime.Value.ToString("yyyy-MM-dd"));

                string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
                for (var i = 0; i <= span.TotalDays; i++)
                {
                    TeacherTimeWorkModel timeModel = new TeacherTimeWorkModel();
                    timeModel.WorkDate = starttime.Value.AddDays(i);
                    string week = Day[Convert.ToInt32(timeModel.WorkDate.DayOfWeek.ToString("d"))].ToString();
                    timeModel.WorkDateName = week;
                    tdlistTime.Add(timeModel);
                }
                //查询教师可排课时间
                List<CourseWorkModel> listCourseWork = _currencyService.DbAccess().Queryable<C_Course_Work, sys_user, C_Room>((cour, ta, room) => new object[] { JoinType.Left, cour.TeacherUid == ta.User_ID, JoinType.Left, cour.RoomId == room.Id })
                    .Where(cour => teacherUid.Contains(cour.TeacherUid) && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00") >= DateTime.Parse(starttime.Value.ToString("yyyy-MM-dd") + " 00:00") && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00") <= DateTime.Parse(endtime.Value.ToString("yyyy-MM-dd") + " 00:00"))
                    .Select<CourseWorkModel>((cour, ta, room) => new CourseWorkModel
                    {
                        Id = cour.Id,
                        AT_Date = cour.AT_Date,
                        CourseTime = cour.CourseTime,
                        EndTime = cour.EndTime,
                        StartTime = cour.StartTime,
                        StudyMode = cour.StudyMode,
                        TeacherName = ta.User_Name,
                        TeacherUid = cour.TeacherUid,
                        Work_Title = cour.Work_Title
                    }).ToList();
                tdlistTime.ForEach(it =>
                {
                    it.ListTeacherTime = new Dictionary<string, List<StudentWorkPlanModel>>();
                    teachers.ForEach(tc =>
                    {
                        List<StudentWorkPlanModel> cutTeacherPlan = new List<StudentWorkPlanModel>();
                        //有课程
                        if (listCourseWork != null && listCourseWork.Count > 0)
                        {
                            var filterCosurWork = listCourseWork.FindAll(iv => iv.AT_Date.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd") && iv.TeacherUid == tc.User_ID).ToList();
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
                                    var thanendTime7 = DateTime.Parse(iv.AT_Date.ToString("yyyy-MM-dd") + " 23:00");
                                    StudentWorkPlanModel plan = new StudentWorkPlanModel();
                                    plan.WorkDate = it.WorkDate;
                                    plan.StartTime = iv.StartTime;
                                    plan.EndTime = iv.EndTime;

                                    //休息一整天
                                    if (startTimestr < thanendTime1 && endTimestr > thanendTime4 && iv.StudyMode == 3)
                                    {
                                        plan.isRest = true;
                                    }
                                    //休息早上8-10点
                                    if (startTimestr >= thanStartTime1 && endTimestr <= thanStartTime2 && iv.StudyMode == 3)
                                    {
                                        plan.Eight_Ten_Reversion = true;
                                    }
                                    //休息早上10-12点
                                    if (startTimestr >= thanStartTime2 && endTimestr <= thanendTime2 && iv.StudyMode == 3)
                                    {
                                        plan.Ten_Twelve_Reversion = true;
                                    }
                                    //休息早上8-12点
                                    if (startTimestr >= thanStartTime1 && startTimestr < thanStartTime2 && endTimestr > thanendTime1 && iv.StudyMode == 3)
                                    {
                                        plan.Eight_Ten_Reversion = true;
                                        plan.Ten_Twelve_Reversion = true;
                                    }
                                    //休息上午8点-下午3点
                                    else if (startTimestr <= thanStartTime2 && endTimestr > thanStartTime3 && endTimestr <= thanStartTime4 && iv.StudyMode == 3)
                                    {
                                        plan.Eight_Ten_Reversion = true;
                                        plan.Ten_Twelve_Reversion = true;
                                        plan.Thirteen_Fifteen_Reversion = true;
                                    }
                                    //休息下午3点-晚上7点
                                    else if (startTimestr >= thanStartTime4 && startTimestr < thanStartTime5 && endTimestr >= thanStartTime6 && iv.StudyMode == 3)
                                    {
                                        plan.Fifteen_Seventeen_Reversion = true;
                                        plan.Seventeen_Nineteen_Reversion = true;
                                    }
                                    //休息晚上6点-晚上8点
                                    else if (startTimestr > thanendTime4 && startTimestr < thanStartTime6 && endTimestr >= thanendTime5 && iv.StudyMode == 3)
                                    {
                                        plan.Seventeen_Nineteen_Reversion = true;
                                    }
                                    //休息下午1-5点
                                    else if (startTimestr >= thanStartTime3 && startTimestr < thanStartTime4 && endTimestr > thanendTime3 && endTimestr <= thanStartTime5 && iv.StudyMode == 3)
                                    {
                                        plan.Thirteen_Fifteen_Reversion = true;
                                        plan.Fifteen_Seventeen_Reversion = true;
                                    }
                                    //休息下午1-7点
                                    else if (startTimestr >= thanStartTime3 && startTimestr < thanStartTime4 && endTimestr >= thanendTime4 && iv.StudyMode == 3)
                                    {
                                        plan.Thirteen_Fifteen_Reversion = true;
                                        plan.Fifteen_Seventeen_Reversion = true;
                                        plan.Seventeen_Nineteen_Reversion = true;
                                    }
                                    else if (((startTimestr >= thanStartTime1 && startTimestr < thanendTime1) && (endTimestr > thanStartTime1 && endTimestr <= thanendTime1)) || ((endTimestr > thanStartTime1 && thanStartTime1 >= startTimestr) && (endTimestr <= thanendTime1 && thanendTime1 > startTimestr)))
                                    {
                                        plan.Eight_Ten_OlockTitle += "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName;
                                        plan.Eight_Ten_StudyMode = iv.StudyMode;
                                    }
                                    else if (((startTimestr >= thanStartTime2 && startTimestr < thanendTime2) && (endTimestr > thanStartTime2 && endTimestr <= thanendTime2)) || ((endTimestr > thanStartTime2 && thanStartTime2 >= startTimestr) && (endTimestr <= thanendTime2 && thanendTime2 > startTimestr)) || ((startTimestr >= thanStartTime2 && startTimestr < thanendTime2) && endTimestr <= thanStartTime3))
                                    {
                                        plan.Ten_Twelve_OlockTitle += "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                        plan.Ten_Twelve_StudyMode = iv.StudyMode;
                                    }
                                    else if (((startTimestr >= thanStartTime3 && startTimestr < thanendTime3) && (endTimestr > thanStartTime3 && endTimestr <= thanendTime3)) || ((endTimestr > thanStartTime3 && thanStartTime3 >= startTimestr) && (endTimestr <= thanendTime3 && thanendTime3 > startTimestr)))
                                    {
                                        plan.Thirteen_Fifteen_OlockTitle += "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                        plan.Thirteen_Fifteen_StudyMode = iv.StudyMode;
                                    }
                                    else if (((startTimestr >= thanStartTime4 && startTimestr < thanendTime4) && (endTimestr > thanStartTime4 && endTimestr <= thanendTime4)) || ((endTimestr > thanStartTime4 && thanStartTime4 >= startTimestr) && (endTimestr <= thanendTime4 && thanendTime4 > startTimestr)))
                                    {
                                        plan.Fifteen_Seventeen_OlockTitle += "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                        plan.Fifteen_Seventeen_StudyMode = iv.StudyMode;
                                    }
                                    else if (((startTimestr >= thanStartTime5 && startTimestr < thanendTime5) && (endTimestr > thanStartTime5 && endTimestr <= thanendTime5)) || ((endTimestr > thanStartTime5 && thanStartTime5 >= startTimestr) && (endTimestr <= thanendTime5 && thanendTime5 > startTimestr)))
                                    {
                                        plan.Seventeen_Nineteen_OlockTitle += "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                        plan.Seventeen_Nineteen_StudyMode = iv.StudyMode;
                                    }
                                    else if (((startTimestr >= thanStartTime6 && startTimestr < thanendTime6) && (endTimestr > thanStartTime6 && endTimestr <= thanendTime6)) || ((endTimestr > thanStartTime6 && thanStartTime6 >= startTimestr) && (endTimestr <= thanendTime6 && thanendTime6 > startTimestr)))
                                    {
                                        plan.Nineteen_TwentyOne_OlockTitle += "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                        plan.Nineteen_TwentyOne_StudyMode = iv.StudyMode;
                                    }
                                    //else if (((startTimestr >= thanStartTime7 && startTimestr < thanendTime7) && (endTimestr > thanStartTime7 && endTimestr <= thanendTime7)) || ((endTimestr > thanStartTime7 && thanStartTime7 >= startTimestr) && (endTimestr <= thanendTime7 && thanendTime7 > startTimestr)))
                                    //{
                                    //    plan.TwentyOne_TwentyTree_OlockTitle += "  " + iv.Work_Title + (iv.StudyMode != 5 && iv.StudyMode != 6 ? " 教师:" + iv.TeacherName : "") + " 时间:" + iv.StartTime + "-" + iv.EndTime + " 教室:" + iv.RoomName; ;
                                    //    plan.TwentyOne_TwentyTree_StudyMode = iv.StudyMode;
                                    //}
                                    cutTeacherPlan.Add(plan);
                                });
                            }
                        }
                        it.ListTeacherTime.Add(tc.User_Name, cutTeacherPlan);
                    });
                });
            }
            return Json(tdlistTime);
        }

        public DateTime GetFirstDayOfWeek(DateTime dt)
        {
            dt = dt == null ? DateTime.Now : dt;
            int daydiff = (int)dt.DayOfWeek - 1 < 0 ? 6 : (int)dt.DayOfWeek - 1;//如果是0结果小于0表示周日 那最后要减6天:其他天数在dayOfWeek上减1，表示回到周一
            DateTime result = dt.AddDays(-daydiff);
            return result;
        }

        [HttpPost]
        public virtual IActionResult ExportPlan([FromBody] TcherHaveTimeExcelModel data) {
            TcherHaveTimeExcelModel tcherGroupmodel = data;
            DateTime dateWeekFirstDay = tcherGroupmodel.starttime.Value;
            DateTime dateWeekLastDay = tcherGroupmodel.endtime.Value;
            string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            List<StudentWorkPlanModel> tdlistTime = new List<StudentWorkPlanModel>();
            TimeSpan span = Convert.ToDateTime(tcherGroupmodel.endtime.Value.ToString("yyyy-MM-dd")) - Convert.ToDateTime(tcherGroupmodel.starttime.Value.ToString("yyyy-MM-dd"));
            for (var i = 0; i < span.Days + 1; i++)
            {
                StudentWorkPlanModel timeModel = new StudentWorkPlanModel();
                timeModel.WorkDate = dateWeekFirstDay.AddDays(i);
                string week = Day[Convert.ToInt32(timeModel.WorkDate.DayOfWeek.ToString("d"))].ToString();
                timeModel.WorkDateName = week;
                tdlistTime.Add(timeModel);
            }

            tdlistTime.ForEach(it =>
            {
                if (tcherGroupmodel.timeArr != null && tcherGroupmodel.timeArr.Count > 0) {
                    //当天的数据
                    var filterHasDay = tcherGroupmodel.timeArr.Where(iv => iv.tmDay.Value.ToString("yyyy-MM-dd") == it.WorkDate.ToString("yyyy-MM-dd")).ToList();
                    if (filterHasDay != null && filterHasDay.Count > 0) {
                        //10-12点的教师
                        var tenTacher = filterHasDay.Where(iv => iv.tmRange.Equals("10-12")).ToList();
                        //13点-15的教师
                        var thirteenTeacher= filterHasDay.Where(iv => iv.tmRange.Equals("13-15")).ToList();
                        //15点-17的教师
                        var fifteenTeacher = filterHasDay.Where(iv => iv.tmRange.Equals("15-17")).ToList();
                        //17点-19的教师
                        var senveteenTeacher = filterHasDay.Where(iv => iv.tmRange.Equals("17-19")).ToList();
                        if (tenTacher != null && tenTacher.Count > 0) {
                            var nameGroup = string.Join(",", tenTacher.Select(iv => iv.tcName).ToList());
                            it.Ten_Twelve_OlockTitle = nameGroup;
                        }
                        if (thirteenTeacher != null && thirteenTeacher.Count > 0)
                        {
                            var nameGroup = string.Join(",", thirteenTeacher.Select(iv => iv.tcName).ToList());
                            it.Thirteen_Fifteen_OlockTitle = nameGroup;
                        }
                        if (fifteenTeacher != null && fifteenTeacher.Count > 0)
                        {
                            var nameGroup = string.Join(",", fifteenTeacher.Select(iv => iv.tcName).ToList());
                            it.Fifteen_Seventeen_OlockTitle = nameGroup;
                        }
                        if (senveteenTeacher != null && senveteenTeacher.Count > 0)
                        {
                            var nameGroup = string.Join(",", senveteenTeacher.Select(iv => iv.tcName).ToList());
                            it.Seventeen_Nineteen_OlockTitle = nameGroup;
                        }
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
            var sheet = workbook.CreateSheet("可排计划");
            sheet.DefaultColumnWidth = 25;
            var yurow = tdlistTime.Count % 7;
            var groupRow = (tdlistTime.Count - yurow) / 7;
            if (yurow > 0)
            {
                groupRow = groupRow + 1;
            }
            for (var j = 0; j < groupRow; j++)
            {
                var list = tdlistTime.Skip(j * 7).Take(7).ToList();
                for (var y = 0; y < 7; y++)
                {
                    var yRow = (j * 7) + y;
                    var row = sheet.CreateRow(yRow);
                    if (y == 0)
                    {
                        var cell = row.CreateCell(0);
                        cell.SetCellValue("日期");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.SetCellValue(list[x].WorkDate.ToString("yyyy-MM-dd"));
                            cell.CellStyle = headStyle;
                        }
                    }
                    if (y == 1)
                    {
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
                        cell.SetCellValue("10:00-12:00");
                        cell.CellStyle = headStyle;
                        for (var x = 0; x < list.Count; x++)
                        {
                            cell = row.CreateCell(x + 1);
                            cell.CellStyle = headStyle;
                            cell.SetCellValue(list[x].Ten_Twelve_OlockTitle);
                        }
                    }
                    if (y ==3)
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
                    if (y ==4)
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
                    if (y == 5)
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
                    if (y ==6)
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

            //保存
            byte[] streamArr = null;
            //保存
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                streamArr = ms.ToArray();
            }
            return File(streamArr, "application/vnd.ms-excel","教师可排列表" + DateTime.Now.ToString("yyyyMMddHHmmssss") + ".xls");

        }






    }
}
