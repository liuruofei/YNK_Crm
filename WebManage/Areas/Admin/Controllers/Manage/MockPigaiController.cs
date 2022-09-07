using ADT.Models;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class MockPigaiController : BaseController
    {
        private ICurrencyService _currencyService;
        public MockPigaiController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        protected override void Init()
        {
            this.MenuID = "V-356";
        }
        [UsersRoleAuthFilter("V-356", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }


        [UsersRoleAuthFilter("V-356", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int studymode, DateTime? startTime = null, DateTime? endTime = null, int page = 1, int limit = 10)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            sys_user teacher = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
                .Where((u, ur, r) => u.User_ID == userId && r.Role_Name.Contains("教师")).First();
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Campus, sys_user, C_Contrac_User>((c, ca, ta, u) => new Object[] { JoinType.Left, c.CampusId == ca.CampusId, JoinType.Left, c.TeacherUid == ta.User_ID, JoinType.Left, c.StudentUid == u.StudentUid })
                .Where(c => c.CampusId == Convert.ToInt32(campusId) && c.StudyMode != 3 && c.StudyMode != 7)
                .WhereIF(studymode > 0, c => c.StudyMode == studymode)
                .WhereIF(teacher != null, c => c.TeacherUid == userId)
                .WhereIF(startTime.HasValue, c => c.AT_Date >= startTime.Value)
                .WhereIF(endTime.HasValue, c => c.AT_Date < endTime.Value)
                .WhereIF(!string.IsNullOrEmpty(title), (c, ca, ta, u) => c.Work_Title.Contains(title) || ta.User_Name.Contains(title) || u.Student_Name.Contains(title)).OrderBy(c => c.AT_Date, OrderByType.Desc).Select<CourseWorkModel>((c, ca, ta, u) => new CourseWorkModel
                {
                    Id = c.Id,
                    Work_Title = c.Work_Title,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    TeacherName = ta.User_Name,
                    Student_Name = u.Student_Name,
                    ListeningName = c.ListeningName,
                    Score = c.Score,
                    StudyMode = c.StudyMode,
                    AT_Date = c.AT_Date,
                    Comment = c.Comment,
                    Comment_Time = c.Comment_Time,
                    CreateTime = c.CreateTime,
                    CreateUid = c.CreateUid,
                    Work_Stutas = c.Work_Stutas
                }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="studentUid"></param>
        /// <param name="endtime"></param>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public virtual IActionResult ExportPlan(string title,int studymode, DateTime? startTime = null, DateTime? endTime = null)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            sys_user teacher = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
                .Where((u, ur, r) => u.User_ID == userId && r.Role_Name.Contains("教师")).First();
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Campus, sys_user, C_Contrac_User>((c, ca, ta, u) => new Object[] { JoinType.Left, c.CampusId == ca.CampusId, JoinType.Left, c.TeacherUid == ta.User_ID, JoinType.Left, c.StudentUid == u.StudentUid })
                .Where(c => c.CampusId == Convert.ToInt32(campusId)&& c.StudyMode != 3 && c.StudyMode != 7)
                .WhereIF(studymode > 0, c => c.StudyMode == studymode)
                .WhereIF(teacher != null, c => c.TeacherUid == userId)
                .WhereIF(startTime.HasValue, c => c.AT_Date >= startTime.Value)
                .WhereIF(endTime.HasValue, c => c.AT_Date < endTime.Value)
                .WhereIF(!string.IsNullOrEmpty(title), (c, ca, ta, u) => c.Work_Title.Contains(title) || ta.User_Name.Contains(title) || u.Student_Name.Contains(title)).OrderBy(c => c.AT_Date, OrderByType.Desc).Select<CourseWorkModel>((c, ca, ta, u) => new CourseWorkModel
                {
                    Id = c.Id,
                    Work_Title = c.Work_Title,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    TeacherName = ta.User_Name,
                    Student_Name = u.Student_Name,
                    ListeningName = c.ListeningName,
                    Score = c.Score,
                    StudyMode = c.StudyMode,
                    AT_Date = c.AT_Date,
                    Comment = c.Comment,
                    Comment_Time = c.Comment_Time,
                    CreateTime = c.CreateTime,
                    CreateUid = c.CreateUid
                }).ToList();
            //导出代码
            var workbook = new HSSFWorkbook();
            //标题列样式
            var headFont = workbook.CreateFont();
            headFont.IsBold = true;
            var headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            headStyle.BorderBottom = BorderStyle.Thin;
            headStyle.BorderLeft = BorderStyle.Thin;
            headStyle.BorderRight = BorderStyle.Thin;
            headStyle.BorderTop = BorderStyle.Thin;
            headStyle.WrapText = true;//自动换行
            headStyle.SetFont(headFont);
            var sheet = workbook.CreateSheet("点评列表");
            sheet.DefaultColumnWidth = 25;
            string[] cellNames = new string[] { "课程名称", "上课日期", "开始时间", "结束时间", "教师", "上课模式", "点评内容", "点评时间" };
            //循环行
            var row0 = sheet.CreateRow(0);
            for (var i = 0; i < cellNames.Length; i++)
            {
                var cell = row0.CreateCell(i);
                cell.SetCellValue(cellNames[i]);
                cell.CellStyle = headStyle;
            }
            if (list != null && list.Count > 0)
            {
                for (var y = 0; y < list.Count; y++)
                {
                    string studyModelName = "";
                    string comment = "";
                    if (list[y].StudyMode == 1)
                    {
                        studyModelName = "1对1";
                    }
                    else if (list[y].StudyMode == 2)
                    {
                        studyModelName = "小班";
                    }
                    else if (list[y].StudyMode == 4)
                    {
                        studyModelName = "试听";
                    }
                    else if (list[y].StudyMode == 5)
                    {
                        studyModelName = "模考";
                    }
                    else if (list[y].StudyMode == 6)
                    {
                        studyModelName = "实考";
                    }
                    if (!string.IsNullOrEmpty(list[y].Comment))
                    {

                        comment = list[y].Comment;
                    }
                    var row = sheet.CreateRow(y + 1);
                    var cell0 = row.CreateCell(0);
                    cell0.SetCellValue(list[y].Work_Title);
                    cell0.CellStyle = headStyle;
                    var cell1 = row.CreateCell(1);
                    cell1.SetCellValue(list[y].AT_Date.ToString("yyyy-MM-dd"));
                    cell1.CellStyle = headStyle;
                    var cell2 = row.CreateCell(2);
                    cell2.SetCellValue(list[y].StartTime);
                    cell2.CellStyle = headStyle;
                    var cell3 = row.CreateCell(3);
                    cell3.SetCellValue(list[y].EndTime);
                    cell3.CellStyle = headStyle;
                    var cell4 = row.CreateCell(4);
                    cell4.SetCellValue(list[y].TeacherName);
                    cell4.CellStyle = headStyle;
                    var cell5 = row.CreateCell(5);
                    cell5.SetCellValue(studyModelName);
                    cell5.CellStyle = headStyle;
                    var cell6 = row.CreateCell(6);
                    cell6.SetCellValue(comment);
                    cell6.CellStyle = headStyle;
                    var cell7 = row.CreateCell(7);
                    cell7.SetCellValue(list[y].Comment_Time.HasValue ? list[y].Comment_Time.Value.ToString("yyyy-MM-dd") : "");
                    cell7.CellStyle = headStyle;
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
            return File(streamArr, "application/vnd.ms-excel", "考试批量列表" + DateTime.Now.ToString("yyyyMMddHHmmssss") + ".xls");
        }

        //更新分数
        public IActionResult SetScore(C_Course_Work vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.Id > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(n => new C_Course_Work { Score = vmodel.Score }).Where(n => n.Id == vmodel.Id).ExecuteCommand();
                    if (result > 0)
                    {
                        return Json(new { code = 200, msg = "更新成功" });
                    }
                    else
                    {
                        return Json(new { code = code, msg = "更新失败" });
                    }
                }
            }
            catch (Exception er)
            {

            }
            return Json(new { code = code, msg = "缺少参数" });
        }
    }
}
