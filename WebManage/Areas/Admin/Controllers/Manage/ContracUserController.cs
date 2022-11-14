using ADT.Common;
using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models.Res;
namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class ContracUserController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public ContracUserController(ICurrencyService currencyService, IC_ContracService contrac)
        {
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "H-150";
        }

        [UsersRoleAuthFilter("H-150", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("H-150", FunctionEnum.Add)]
        public IActionResult Add() {
            C_SequenceCode sequence = new C_SequenceCode();
            long sequenceNoLast = _currencyService.DbAccess().Queryable<C_SequenceCode>().Where(c => c.type == 2).Max<long>(c => c.SequenceNo);//最新学号
            if (sequenceNoLast > 0)
            {
                sequence.SequenceNo = sequenceNoLast + 1;
            }
            else
            {
                sequence.SequenceNo = 10000;
            }
            sequence.type = 2;
            sequence.Remarks = "学生编号";
            _currencyService.DbAccess().Insertable<C_SequenceCode>(sequence).ExecuteCommand();
            ViewBag.StudentNo = "CC" + sequence.SequenceNo;
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("H-150", FunctionEnum.Audit)]
        public IActionResult SettingAccount(int ID) {
            ViewBag.ID = ID;
            return View();
        }

        [UsersRoleAuthFilter("H-150", FunctionEnum.Audit)]
        public IActionResult SettingExam(int ID)
        {
            ViewBag.ID = ID;
            return View();
        }


        [UsersRoleAuthFilter("H-150", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            ViewBag.StudentNo = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(c => c.StudentUid == ID).First().Student_No;
            return View("Add");
        }

        public IActionResult AssinCC(int ID) {
            ViewBag.ID = ID;
            return View();
        }

        public IActionResult AssinCR(int ID)
        {
            ViewBag.ID = ID;
            return View();
        }


        //学生课程详情
        public IActionResult CourseDetail(int studentUid)
        {
            ViewBag.StudentUid = studentUid;
            return View();
        }

        /// <summary>
        /// 统计学生科目课时
        /// </summary>
        /// <param name="studentUid"></param>
        /// <returns></returns>
        public IActionResult QueryStudentTotal(int studentUid)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            //1对1和班课(不包含赠送课)
            //List<C_UserCourseTimeModel> list = _currencyService.DbAccess().Queryable<C_User_CourseTime, C_Project, C_Subject, C_Class>((time, pro, sub, cla) => new object[] {
            // JoinType.Left,time.ProjectId==pro.ProjectId,JoinType.Left,time.SubjectId==sub.SubjectId,JoinType.Left,time.ClassId==cla.ClassId
            //}).Where((time, pro, sub, cla) => time.StudentUid == studentUid).Select<C_UserCourseTimeModel>((time, pro, sub, cla) =>
            //    new C_UserCourseTimeModel
            //    {
            //        Id = time.Id,
            //        StudentUid = time.StudentUid,
            //        ClassId = time.ClassId,
            //        SubjectName = sub.SubjectName,
            //        ProjectName = pro.ProjectName,
            //        Class_Name = cla.Class_Name,
            //        Contra_ChildNo = time.Contra_ChildNo,
            //        Course_Time = time.Course_Time,
            //        Course_UseTime = time.Course_UseTime,
            //        IsLinsting=0,
            //        IsPresent= 0,
            //        Class_Course_Time = time.Class_Course_Time,
            //        Class_Course_UseTime = time.Class_Course_UseTime
            //    }).ToList();

            StringBuilder sql = new StringBuilder("(select  tm.Id,tm.StudentUid,tm.ClassId,sub.SubjectName,pro.ProjectName,cla.Class_Name,tm.Contra_ChildNo,tm.Course_Time,");
            sql.Append("tm.Course_UseTime,tm.Class_Course_Time,tm.Class_Course_UseTime,IsLinsting=0,IsPresent=0,");
            sql.Append("ShjiUseTime=(select isnull(sum(wk.CourseTime),0) from C_Course_Work wk where wk.Contra_ChildNo=tm.Contra_ChildNo and wk.StudentUid=tm.StudentUid and wk.SubjectId=tm.SubjectId and wk.ProjectId=tm.ProjectId and (wk.Comment is not null and wk.Comment!='')),");
            sql.Append("ShjiClassUseTime=(select isnull(sum(wk.CourseTime),0) from C_Course_Work wk where wk.ClasssId=tm.ClassId and (wk.Comment is not null and wk.Comment!='') and tm.ClassId<>0)");
            sql.Append("from C_User_CourseTime  tm left join C_Project pro on tm.ProjectId=pro.ProjectId  left join C_Subject sub on tm.SubjectId=sub.SubjectId left join C_Class cla on tm.ClassId=cla.ClassId ");
            sql.Append("  where tm.StudentUid=@StudentUid)");
            List<C_UserCourseTimeModel> list = _currencyService.DbAccess().Queryable(sql.ToString(), "orginSql").AddParameters(new { StudentUid = studentUid }).Select<C_UserCourseTimeModel>().ToList();
            //已试听课
            List<C_UserCourseTimeModel> list2 = _currencyService.DbAccess().Queryable("(select sum(wk.CourseTime)as Course_Time,sub.SubjectName,pro.ProjectName,u.StudentUid,IsLinsting=1 from C_Course_Work wk left join C_Subject sub on wk.SubjectId=sub.SubjectId" +
                " left join C_Project pro on wk.ProjectId=pro.ProjectId left join C_Contrac_User u on wk.ListeningName=u.Student_Name where wk.StudyMode=4 and wk.StudentUid=" + studentUid + " group by sub.SubjectName,pro.ProjectName,u.StudentUid)",
                "orginSql").Select<C_UserCourseTimeModel>().ToList();
            if (list2 != null && list2.Count > 0) {
                list2.ForEach(item =>
                {
                    item.IsPresent = 0;
                    list.Add(item);
                });
            }
            //赠送课程
            List<C_UserCourseTimeModel> list3 = _currencyService.DbAccess().Queryable<C_User_PresentTime>().Where(time => time.StudentUid == studentUid).Select<C_UserCourseTimeModel>(time =>
                new C_UserCourseTimeModel
                {
                    Id = time.Id,
                    StudentUid = time.StudentUid,
                    Contra_ChildNo = time.Contra_ChildNo,
                    Course_Time = time.Present_Time,
                    Course_UseTime = time.Present_UseTime,
                    IsLinsting = 0,
                    IsPresent = 1,
                    Class_Course_Time = 0,
                    Class_Course_UseTime = 0
                }).ToList();
            if (list3 != null && list3.Count > 0)
            {
                list3.ForEach(item =>
                {
                    list.Add(item);
                });
            }
            rsg.code = 200;
            rsg.msg = "获取成功";
            rsg.data = list;
            return Json(rsg);
        }
        /// <summary>
        /// 统计学生科目总课时和已排课时
        /// </summary>
        /// <param name="studentUid"></param>
        /// <returns></returns>
        public IActionResult QuerySudentProjectTotal(int studentUid) {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            StringBuilder sql = new StringBuilder("(select sum(Course_Time) as Course_Time,sum(Course_UseTime) as Course_UseTime,sub.SubjectName,pro.ProjectName");
            sql.Append(" from C_User_CourseTime  tm left join C_Project pro on tm.ProjectId=pro.ProjectId  left join C_Subject sub on tm.SubjectId=sub.SubjectId");
            sql.Append(" where tm.StudentUid=@StudentUid Group by sub.SubjectName,pro.ProjectName)");
            List<C_UserCourseTimeModel> list = _currencyService.DbAccess().Queryable(sql.ToString(), "orginSql").AddParameters(new { StudentUid = studentUid }).Select<C_UserCourseTimeModel>().ToList();
            rsg.code = 200;
            rsg.msg = "获取成功";
            rsg.data = list;
            return Json(rsg);
        }

        /// <summary>
        ///获取学员上课详情
        /// </summary>
        /// <param name="startStr">开始时间</param>
        /// <param name="endStr">结束时间</param>
        /// <param name="studentUid"></param>
        /// <param name="subjectId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public IActionResult QueryWorkDetailSource(int studentUid,string startStr, string endStr, int subjectId, int projectId,int? mockOut=0, int page = 1, int limit = 10)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult reg = new ResResult();
            List<int> classIds = new List<int>();
            classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((c, cl) => new object[] { JoinType.Left, c.ClassId == cl.ClassId }).Where(c => c.StudentUid == studentUid && c.ClassId > 0).WhereIF(subjectId > 0, (c, cl) => cl.SubjectId == subjectId).Select(c => c.ClassId).ToList();
            string sql = @"(select wk.*,contracU.Student_Name,ta.User_Name as TAUserName,tc.User_Name as TeacherName,rm.RoomName,sub.SubjectName from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id left join C_Subject sub on wk.SubjectId=sub.SubjectId where 1=1";

            if (classIds != null && classIds.Count > 0)
            {
                sql += " and (wk.ClasssId in(" + string.Join(",", classIds) + ") or wk.StudentUid=" + studentUid+")";
            }
            else {
                sql += " and wk.StudentUid=" + studentUid;
            }
            if (!string.IsNullOrEmpty(startStr)) {
                sql += " and wk.AT_Date>=CAST(@startStr AS date)";
            }
            if (!string.IsNullOrEmpty(endStr))
            {
                sql += " AND wk.AT_Date<CAST(@endStr AS date)";
            }
            if (subjectId > 0)
            {
                sql += " and wk.SubjectId=" + subjectId;
            }
            if (projectId > 0)
            {
                sql += " and wk.ProjectId=" + projectId;
            }
            if (mockOut.HasValue&& mockOut.Value>0) {
                sql += " and wk.StudyMode<>5 and wk.StudyMode<>6";
            }
            sql += " and wk.CampusId="+campusId;
            sql += ")";
            int total = 0;
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            List<CourseWorkModel> list = _currencyService.DbAccess().Queryable(sql, "orginSql")
            .AddParameters(new { startStr = startStr, endStr = endStr})
            .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            pageModel.msg = "获取成功";
            return Json(pageModel);
        }


        /// <summary>
        /// 导出明细
        /// </summary>
        /// <param name="studentUid"></param>
        /// <param name="startStr"></param>
        /// <param name="endStr"></param>
        /// <param name="subjectId"></param>
        /// <param name="projectId"></param>
        /// <param name="mockOut"></param>
        /// <returns></returns>
        public IActionResult ExportWorkDetailSource(int studentUid, string startStr, string endStr, int subjectId, int projectId, int? mockOut = 0) {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult reg = new ResResult();
            var u = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(v=>v.StudentUid==studentUid).First();
            List<int> classIds = new List<int>();
            classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((c, cl) => new object[] { JoinType.Left, c.ClassId == cl.ClassId }).Where(c => c.StudentUid == studentUid && c.ClassId > 0).WhereIF(subjectId > 0, (c, cl) => cl.SubjectId == subjectId).Select(c => c.ClassId).ToList();
            string sql = @"(select wk.*,contracU.Student_Name,ta.User_Name as TAUserName,tc.User_Name as TeacherName,rm.RoomName,sub.SubjectName from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id left join C_Subject sub on wk.SubjectId=sub.SubjectId where 1=1";

            if (classIds != null && classIds.Count > 0)
            {
                sql += " and (wk.ClasssId in(" + string.Join(",", classIds) + ") or wk.StudentUid=" + studentUid + ")";
            }
            else
            {
                sql += " and wk.StudentUid=" + studentUid;
            }
            if (!string.IsNullOrEmpty(startStr))
            {
                sql += " and wk.AT_Date>=CAST(@startStr AS date)";
            }
            if (!string.IsNullOrEmpty(endStr))
            {
                sql += " AND wk.AT_Date<CAST(@endStr AS date)";
            }
            if (subjectId > 0)
            {
                sql += " and wk.SubjectId=" + subjectId;
            }
            if (projectId > 0)
            {
                sql += " and wk.ProjectId=" + projectId;
            }
            if (mockOut.HasValue && mockOut.Value > 0)
            {
                sql += " and wk.StudyMode<>5 and wk.StudyMode<>6";
            }
            sql += " and wk.CampusId=" + campusId;
            sql += ")";
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            List<CourseWorkModel> list = _currencyService.DbAccess().Queryable(sql, "orginSql")
            .AddParameters(new { startStr = startStr, endStr = endStr })
            .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToList();
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
            var sheet = workbook.CreateSheet(u.Student_Name + "课程明细");
            sheet.DefaultColumnWidth = 45;
            var row0= sheet.CreateRow(0);
            string[] cellNames = {"上课科目","上课日期","时间段","课时","教师","教室"};
            for (var i = 0; i < cellNames.Length; i++) {
                var cell = row0.CreateCell(i);
                cell.SetCellValue(cellNames[i]);
                cell.CellStyle = headStyle;
            }
            for (var j = 0; j < list.Count; j++) {
                var row = sheet.CreateRow(j+1);
                for (var y = 0; y < 6; y++) {
                    var cell = row.CreateCell(y);
                    if (y == 0) {
                        var title = list[j].ClasssId > 0 ? (list[j].Work_Title + " - " + list[j].SubjectName) : list[j].Work_Title;
                        cell.SetCellValue(title);
                        cell.CellStyle = headStyle;
                    }
                    if (y ==1)
                    {
                        cell.SetCellValue(list[j].AT_Date.ToString("yyyy-MM-dd"));
                        cell.CellStyle = headStyle;
                    }
                    if (y == 2)
                    {
                        cell.SetCellValue(list[j].StartTime+" - "+ list[j].EndTime);
                        cell.CellStyle = headStyle;
                    }
                    if (y == 3)
                    {
                        cell.SetCellValue(list[j].CourseTime);
                        cell.CellStyle = headStyle;
                    }
                    if (y ==4)
                    {
                        cell.SetCellValue(list[j].TeacherName);
                        cell.CellStyle = headStyle;
                    }
                    if (y == 5)
                    {
                        cell.SetCellValue(list[j].RoomName);
                        cell.CellStyle = headStyle;
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
            return File(streamArr, "application/vnd.ms-excel", u.Student_Name + "课程明细" + DateTime.Now.ToString("yyyyMMddHHmmssss") + ".xls");
        }

        /// <summary>
        /// 获取Cr
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryCR()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where((u, ur, r) => r.Role_Name.Contains("销售")&&u.CampusId==Convert.ToInt32(campusId)).Select((u, ur, r) => u).ToList();
            return Json(rsg);
        }

        /// <summary>
        /// 保存签约用户
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveContracUser(C_Contrac_User vmodel) {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            vmodel.CreateUid = userId;
            reg = _contrac.SaveContracUser(vmodel);
            return Json(reg);
        }


        [UsersRoleAuthFilter("H-150", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                C_Contrac_User vmodel = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(it => it.StudentUid == ID).First();
                list = vmodel;
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }



        /// <summary>
        /// 创建合同页面
        /// </summary>
        /// <param name="clueId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public IActionResult CreateContrc(int clueId, int studentId)
        {
            ViewBag.ClueId = clueId;
            ViewBag.StudentUid = studentId;
            ContracInfmotion motion = new ContracInfmotion();
            if (studentId > 0)
            {
                var user = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(item => item.StudentUid == studentId).First();
                motion.StudentNo = user.Student_No;
                motion.StudentName = user.Student_Name;
                motion.Amount = user.Amount;
                motion.Student_Phone = user.Student_Phone;
                motion.Student_Email = user.Student_Email;
                motion.Student_Wechat = user.Student_Wechat;
                motion.CampusId = user.CampusId;
                motion.Birthday = user.Birthday;
                motion.Elder_Email = user.Elder_Email;
                motion.Elder_Name = user.Elder_Name;
                motion.Elder_Phone = user.Elder_Phone;
                motion.Elder_Wechat = user.Elder_Wechat;
                motion.Elder2_Email = user.Elder2_Email;
                motion.Elder2_Name = user.Elder2_Name;
                motion.Elder2_Phone = user.Elder2_Phone;
                motion.Elder2_Wechat = user.Elder2_Wechat;
                motion.Grade = user.Grade;
                motion.InSchool = user.InSchool;
                motion.Sex = user.Sex;
                motion.Soure = user.Soure;
            }
            else
            {
                C_ClueUser clue = _currencyService.DbAccess().Queryable<C_ClueUser>().Where(item => item.ClueId == clueId).First();
                var user=_currencyService.DbAccess().Queryable<C_Contrac_User>().Where(item => item.Student_Name == clue.Student_Name).First();
                if (user != null)
                {
                    motion.StudentNo = user.Student_No;
                    motion.StudentName = user.Student_Name;
                    motion.Amount = user.Amount;
                    motion.Student_Phone = user.Student_Phone;
                    motion.Student_Email = user.Student_Email;
                    motion.Student_Wechat = user.Student_Wechat;
                    motion.CampusId = user.CampusId;
                    motion.Birthday = user.Birthday;
                    motion.Elder_Email = user.Elder_Email;
                    motion.Elder_Name = user.Elder_Name;
                    motion.Elder_Phone = user.Elder_Phone;
                    motion.Elder_Wechat = user.Elder_Wechat;
                    motion.Elder2_Email = clue.Elder_Email2;
                    motion.Elder2_Name = clue.Elder_Name2;
                    motion.Elder2_Phone = clue.Elder_Phone2;
                    motion.Grade = user.Grade;
                    motion.InSchool = user.InSchool;
                    motion.Sex = user.Sex;
                    motion.Soure = user.Soure;
                }
                else {
                    C_SequenceCode sequence = new C_SequenceCode();
                    long sequenceNoLast = _currencyService.DbAccess().Queryable<C_SequenceCode>().Where(c => c.type == 2).Max<long>(c => c.SequenceNo);//最新学号
                    if (sequenceNoLast > 0)
                    {
                        sequence.SequenceNo = sequenceNoLast + 1;
                    }
                    else
                    {
                        sequence.SequenceNo = 10000;
                    }
                    sequence.type = 2;
                    sequence.Remarks = "学生编号";
                    _currencyService.DbAccess().Insertable<C_SequenceCode>(sequence).ExecuteCommand();
                    motion.StudentNo = "CC" + sequence.SequenceNo;
                    motion.StudentName = clue.Student_Name;
                    motion.Student_Phone = clue.Student_Phone;
                    motion.Student_Email = clue.Student_Email;
                    motion.Student_Wechat = clue.Student_Wechat;
                    motion.CampusId = clue.CampusId;
                    motion.Birthday = clue.Birthday;
                    motion.Elder_Email = clue.Elder_Email;
                    motion.Elder_Name = clue.Elder_Name;
                    motion.Elder_Phone = clue.Elder_Phone;
                    motion.Elder_Wechat = clue.Elder_Wechat;
                    motion.Elder2_Email = clue.Elder_Email2;
                    motion.Elder2_Name = clue.Elder_Name2;
                    motion.Elder2_Phone = clue.Elder_Phone2;
                    motion.Grade = clue.Grade;
                    motion.InSchool = clue.InSchool;
                    motion.Sex = clue.Sex;
                    motion.Soure = clue.Soure;
                }
            }
            C_SequenceCode sequence2 = new C_SequenceCode();
            long contrcNoLast = _currencyService.DbAccess().Queryable<C_SequenceCode>().Where(c => c.type == 1).Max<long>(c => c.SequenceNo);//最新合同号
            if (contrcNoLast < 1)
                sequence2.SequenceNo = 20000;
            else
                sequence2.SequenceNo = contrcNoLast + 1;
            sequence2.type = 1;
            sequence2.Remarks = "合同编号";
            _currencyService.DbAccess().Insertable<C_SequenceCode>(sequence2).ExecuteCommand();
            motion.ContraNo = "SH" + sequence2.SequenceNo;
            return View("CreateContrc", motion);
        }


        /// <summary>
        /// 创建合同
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveContrc(ContracInput vmodel) {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            vmodel.CreateUid = userId;
            reg = _contrac.AddUserContrac(vmodel);
            return Json(reg);
        }

        /// <summary>
        /// 获取科目列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> QueryProject(int subjectId)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = await _currencyService.DbAccess().Queryable<C_Project>().Where(n => n.Status < 1&&n.SubjectId==subjectId).ToListAsync();
            return Json(rsg);
        }


        /// <summary>
        /// 获取合同中心
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> QueryContraCenter()
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = await _currencyService.DbAccess().Queryable<C_ContraCenter>().Where(n => n.Status < 1).ToListAsync();
            return Json(rsg);
        }

        /// <summary>
        /// 根据考试类型id集合获取
        /// </summary>
        /// <param name="subjectIds"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryProjects(int[] subjectIds)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = await _currencyService.DbAccess().Queryable<C_Project>().In(n=>n.SubjectId,subjectIds).Where(n => n.Status < 1).ToListAsync();
            return Json(rsg);
        }


        public IActionResult QuerySubject()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<C_Subject>().Where(n => n.Status < 1&&n.CampusId==Convert.ToInt32(campusId)).ToList();
            return Json(rsg);
        }

        

        /// <summary>
        /// 获取校区列表
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryCampus() {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<C_Campus>().Where(n => n.Status < 1).ToList();
            return Json(rsg);
        }

        /// <summary>
        /// 绑定班级
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryClass(int typeId,string className=null) {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<C_Class> list = _currencyService.DbAccess().Queryable<C_Class>().Where(n => n.Status < 1 && n.End_Course_Date > DateTime.Now && n.CampusId == Convert.ToInt32(campusId))
                .WhereIF(typeId > 0, n => n.TypeId == typeId).WhereIF(!string.IsNullOrEmpty(className), n => (n.Class_Name.Contains(className) || n.Class_No.Contains(className))).ToPageList(1,50);
            list.Add(new C_Class { ClassId=0,Class_Name="—选择—"});
            list = list.OrderBy(it => it.ClassId).ToList();
            rsg.data = list;
            return Json(rsg);

        }


        /// <summary>
        /// 根据线索id查询合同是否创建成功
        /// </summary>
        /// <param name="clueId"></param>
        /// <returns></returns>
        public IActionResult QuerYContrcByClueId(int clueId) {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<C_Contrac>().Where(n => n.ClueId == clueId).First();
            return Json(rsg);
        }





        /// <summary>
        /// 添加学员抽词账号
        /// </summary>
        /// <returns></returns>
        public IActionResult SaveSettingAccount(C_Contrac_User vmodel) {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            vmodel.CreateUid = userId;
            if (string.IsNullOrEmpty(vmodel.Student_Account))
            {
                reg.code = 0;
                reg.msg = "学员账号不能为空";
                return Json(reg);
            }
            if (!string.IsNullOrEmpty(vmodel.Student_Pwd))
            {
                var vuser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(n => n.StudentUid == vmodel.StudentUid).First();
                if (vuser.Student_Pwd!=vmodel.Student_Pwd)
                {
                    vmodel.Student_Pwd = Tools.MD5Encryption(vmodel.Student_Pwd);
                }
            }
            else {
                reg.code = 0;
                reg.msg = "请输入学员密码";
                return Json(reg);
            }
            reg = _contrac.SettingAccount(vmodel);
            return Json(reg);
        }


        /// <summary>
        /// 添加学员考试账号
        /// </summary>
        /// <returns></returns>
        public IActionResult SaveSettingExam(C_Contrac_User vmodel)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (string.IsNullOrEmpty(vmodel.ExamAccount))
            {
                reg.code = 0;
                reg.msg = "考试账号不能为空";
                return Json(reg);
            }
            if (string.IsNullOrEmpty(vmodel.ExamPassword))
            {
                reg.code = 0;
                reg.msg = "请输入考试密码";
                return Json(reg);
            }
            var vuser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(n => n.StudentUid == vmodel.StudentUid).First();
            if (vuser != null)
            {
                vuser.ExamAccount = vmodel.ExamAccount;
                vuser.ExamPassword = vmodel.ExamPassword;
                _currencyService.DbAccess().Updateable<C_Contrac_User>(vuser).ExecuteCommand();
                reg.code = 200;
                reg.msg = "保存考试账号成功";
            }
            return Json(reg);
        }



        /// <summary>
        /// 分配CC
        /// </summary>
        /// <returns></returns>
        public IActionResult AssignmentCC(C_Contrac_User vmodel)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            vmodel.CreateUid = userId;
            if (string.IsNullOrEmpty(vmodel.CC_Uid))
            {
                reg.code = 0;
                reg.msg = "顾问CC不能为空";
                return Json(reg);
            }
            var vuser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(n => n.StudentUid == vmodel.StudentUid).First();
            if (vuser !=null)
            {
                vuser.CC_Uid = vmodel.CC_Uid;
                vuser.UpdateTime = DateTime.Now;
                vuser.UpdateUid = userId;
                _currencyService.DbAccess().Updateable<C_Contrac_User>(vuser).ExecuteCommand();
                reg.code = 200;
                reg.msg = "分配CC成功";
            }
            return Json(reg);
        }


        /// <summary>
        /// 分配CR
        /// </summary>
        /// <returns></returns>
        public IActionResult AssignmentCR(C_Contrac_User vmodel)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            vmodel.CreateUid = userId;
            if (string.IsNullOrEmpty(vmodel.CR_Uid))
            {
                reg.code = 0;
                reg.msg = "销售CR不能为空";
                return Json(reg);
            }
            var vuser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(n => n.StudentUid == vmodel.StudentUid).First();
            if (vuser != null)
            {
                vuser.CR_Uid = vmodel.CR_Uid;
                vuser.UpdateTime = DateTime.Now;
                vuser.UpdateUid = userId;
                _currencyService.DbAccess().Updateable<C_Contrac_User>(vuser).ExecuteCommand();
                reg.code = 200;
                reg.msg = "分配CR成功";
            }
            return Json(reg);
        }






        //学员签约合同列表
        [UsersRoleAuthFilter("H-150", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            var ccUse = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where(u => u.User_ID == userId).Select<sys_role>().First();
            PageList<C_ContracUserModel> pageModel = new PageList<C_ContracUserModel>();
            var list = _currencyService.DbAccess().Queryable("C_Contrac_User", "contracU").AddJoinInfo("Sys_User", "cc", "contracU.CC_Uid=cc.User_ID", SqlSugar.JoinType.Left)
            .AddJoinInfo("Sys_User", "cr", "contracU.CR_Uid=cr.User_ID", SqlSugar.JoinType.Left).
            AddJoinInfo("C_Campus", "camp", "contracU.CampusId=camp.CampusId", SqlSugar.JoinType.Left)
            .Where("contracU.CampusId="+campusId)
            .WhereIF(!string.IsNullOrEmpty(title)," charindex(@title,contracU.Student_Name)>0 or charindex(@title,contracU.Student_Phone)>0").AddParameters(new { title = title })
            .WhereIF(ccUse != null && ccUse.Role_Name == "顾问", "contracU.CC_Uid=@CCuid").AddParameters(new { CCuid = userId })
            .Select<C_ContracUserModel>(@"contracU.*,cc.User_Name as CC_UserName,cr.User_Name as CR_UserName,
                camp.CampusName").OrderBy("contracU.CreateTime desc").ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

    }
}
