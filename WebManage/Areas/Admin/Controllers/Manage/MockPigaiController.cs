using ADT.Models;
using ADT.Models.ResModel;
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

        [UsersRoleAuthFilter("V-356", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("V-356", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("V-356", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                CourseWorkModel vmodel = _currencyService.DbAccess().Queryable<C_Course_Work,C_Unit_Paper,C_Contrac_User>((wk,pper,u)=>new Object[]{JoinType.Left,wk.PaperId==pper.PaperId, JoinType.Left, wk.StudentUid == u.StudentUid}).Where(wk=>wk.Id == ID).Select<CourseWorkModel>((wk, pper,u)=>new CourseWorkModel{
                  Id=wk.Id,
                  Work_Title=wk.Work_Title,
                  SubjectId=wk.SubjectId,
                  ProjectId=wk.ProjectId,
                  PaperId=wk.PaperId,
                  AT_Date=wk.AT_Date,
                  StartTime=wk.StartTime,
                  EndTime=wk.EndTime,
                  PaperCode=pper.PaperCode,
                  Score=wk.Score,
                  Student_Name=u.Student_Name,
                  MockLevel=wk.MockLevel,
                  AvgScore=pper.AvgScore
                }).First();
                list = vmodel;
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }


        [UsersRoleAuthFilter("V-356", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int studymode,int subjectId,int projectId,int unitId,DateTime? startTime = null, DateTime? endTime = null, int page = 1, int limit = 10)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            sys_user teacher = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
                .Where((u, ur, r) => u.User_ID == userId && r.Role_Name.Contains("教师")).First();
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Campus, sys_user, C_Contrac_User,C_Unit_Paper>((c, ca, ta, u,pa) => new Object[] { JoinType.Left, c.CampusId == ca.CampusId, JoinType.Left, c.TeacherUid == ta.User_ID, JoinType.Left, c.StudentUid == u.StudentUid,JoinType.Left,c.PaperId==pa.PaperId})
                .Where(c => c.CampusId == Convert.ToInt32(campusId) && c.StudyMode != 3 && c.StudyMode != 7)
                .WhereIF(studymode > 0, c => c.StudyMode == studymode)
                .WhereIF(teacher != null, c => c.TeacherUid == userId)
                .WhereIF(startTime.HasValue, c => c.AT_Date >= startTime.Value)
                .WhereIF(endTime.HasValue, c => c.AT_Date < endTime.Value)
                .WhereIF(subjectId>0,c=>c.SubjectId==subjectId)
                .WhereIF(projectId > 0, c => c.ProjectId == projectId)
                .WhereIF(unitId > 0, c => c.UnitId ==unitId)
                .WhereIF(!string.IsNullOrEmpty(title), (c, ca, ta, u) => c.Work_Title.Contains(title) || ta.User_Name.Contains(title) || u.Student_Name.Contains(title)).OrderBy(c => c.AT_Date, OrderByType.Desc).Select<CourseWorkModel>((c, ca, ta, u,pa) => new CourseWorkModel
                {
                    Id = c.Id,
                    Work_Title = c.Work_Title,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    TeacherName = ta.User_Name,
                    Student_Name = u.Student_Name,
                    ListeningName = c.ListeningName,
                    Score = c.Score,
                    PaperId=c.PaperId,
                    PaperCode=pa.PaperCode,
                    MockLevel=c.MockLevel,
                    AvgScore=pa.AvgScore,
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
        /// 查询出未考试卷列表
        /// </summary>
        /// <param name="title"></param>
        /// <param name="studymode"></param>
        /// <param name="subjectId"></param>
        /// <param name="projectId"></param>
        /// <param name="unitId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IActionResult QuerPaperNoTest(string title, int studymode, int subjectId, int projectId, int unitId, DateTime? startTime = null, DateTime? endTime = null) {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            var studentModel = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(u => u.Student_Name.Equals(title)).First();
            if (studentModel != null && subjectId > 0 && projectId > 0 && unitId > 0) {
                List<C_Course_Work> list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Campus, C_Unit_Paper>((c, ca, pa) => new Object[] { JoinType.Left, c.CampusId == ca.CampusId, JoinType.Left, c.PaperId == pa.PaperId })
                     .Where(c => c.CampusId == Convert.ToInt32(campusId) && c.StudyMode == studymode && c.SubjectId == subjectId && c.ProjectId == projectId && c.UnitId == unitId).WhereIF(startTime.HasValue, c => c.AT_Date >= startTime.Value)
                .WhereIF(endTime.HasValue, c => c.AT_Date < endTime.Value).Select<C_Course_Work>().ToList();
                //已考试卷编号
                var PaperList_ykao = list.Where(wk => wk.PaperId > 0).Select(wk => wk.PaperId).ToList();
                HashSet<int> hax = new HashSet<int>(PaperList_ykao);
                PaperList_ykao.Clear();
                PaperList_ykao.AddRange(hax);
                //查询当前科目单元下并且未考的试卷
                var PaperList_wkao = _currencyService.DbAccess().Queryable<C_Unit_Paper,C_Subject,C_Project,C_Project_Unit>((pp, sub,prj,ut) => new Object[] { JoinType.Left,pp.SubjectId==sub.SubjectId, JoinType.Left, pp.ProjectId==prj.ProjectId, JoinType.Left, pp.UnitId==ut.UnitId }).Where(pp => pp.SubjectId == subjectId && pp.ProjectId == projectId&&pp.UnitId==unitId).WhereIF(hax.Count > 0, pp => !PaperList_ykao.Contains(pp.PaperId)).Select<UnitPaperModel>((pp, sub, prj, ut) => new UnitPaperModel { 
                 PaperId=pp.PaperId,
                 PaperCode=pp.PaperCode,
                 SubjectName=sub.SubjectName,
                 ProjectName=prj.ProjectName,
                 UnitName=ut.UnitName
                }).ToList();
                rsg.data = PaperList_wkao;
            }
            return Json(rsg);
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="studentUid"></param>
        /// <param name="endtime"></param>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public virtual IActionResult ExportPlan(string title,int studymode,int subjectId, int projectId, int unitId, DateTime? startTime = null, DateTime? endTime = null)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            sys_user teacher = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
                .Where((u, ur, r) => u.User_ID == userId && r.Role_Name.Contains("教师")).First();
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Campus, sys_user, C_Contrac_User,C_Unit_Paper>((c, ca, ta, u,pp) => new Object[] { JoinType.Left, c.CampusId == ca.CampusId, JoinType.Left, c.TeacherUid == ta.User_ID, JoinType.Left, c.StudentUid == u.StudentUid, JoinType.Left, c.PaperId ==pp.PaperId })
                .Where(c => c.CampusId == Convert.ToInt32(campusId)&& c.StudyMode != 3 && c.StudyMode != 7)
                .WhereIF(studymode > 0, c => c.StudyMode == studymode)
                .WhereIF(teacher != null, c => c.TeacherUid == userId)
                .WhereIF(startTime.HasValue, c => c.AT_Date >= startTime.Value)
                .WhereIF(endTime.HasValue, c => c.AT_Date < endTime.Value)
                .WhereIF(subjectId > 0, c => c.SubjectId == subjectId)
                .WhereIF(projectId > 0, c => c.ProjectId == projectId)
                .WhereIF(unitId > 0, c => c.UnitId == unitId)
                .WhereIF(!string.IsNullOrEmpty(title), (c, ca, ta, u) => c.Work_Title.Contains(title) || ta.User_Name.Contains(title) || u.Student_Name.Contains(title)).OrderBy(c => c.AT_Date, OrderByType.Desc).Select<CourseWorkModel>((c, ca, ta, u,pp) => new CourseWorkModel
                {
                    Id = c.Id,
                    Work_Title = c.Work_Title,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    TeacherName = ta.User_Name,
                    Student_Name = u.Student_Name,
                    ListeningName = c.ListeningName,
                    Score = c.Score,
                    PaperId=c.PaperId,
                    StudyMode = c.StudyMode,
                    AT_Date = c.AT_Date,
                    Comment = c.Comment,
                    Comment_Time = c.Comment_Time,
                    MockLevel=c.MockLevel,
                    AvgScore=pp.AvgScore,
                    PaperCode=pp.PaperCode,
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
            string[] cellNames = new string[] { "课程名称", "上课日期", "开始时间", "结束时间", "教师", "成绩", "等级", "试卷编号","分数线"};
            //循环行
            var row0 = sheet.CreateRow(0);
            for (var i = 0; i < cellNames.Length; i++)
            {
                var cell = row0.CreateCell(i);
                cell.SetCellValue(cellNames[i]);
                cell.CellStyle = headStyle;
            }
            //判断是否查询单个学生
            var studentModel = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(u => u.Student_Name.Equals(title)).First();
            if (list != null && list.Count > 0)
            {
                for (var y = 0; y < list.Count; y++)
                {
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
                    cell5.SetCellValue(list[y].Score);
                    cell5.CellStyle = headStyle;
                    var cell6 = row.CreateCell(6);
                    cell6.SetCellValue(list[y].MockLevel);
                    cell6.CellStyle = headStyle;
                    var cell7 = row.CreateCell(7);
                    cell7.SetCellValue(list[y].PaperCode);
                    cell7.CellStyle = headStyle;
                    var cell8 = row.CreateCell(8);
                    cell8.SetCellValue(list[y].AvgScore);
                    cell8.CellStyle = headStyle;
                }
                if (studentModel != null && subjectId > 0 && projectId > 0 && unitId > 0) {
                    //已考试卷编号
                    var PaperList_ykao = list.Where(wk => wk.PaperId > 0).Select(wk => wk.PaperId).ToList();
                    HashSet<int> hax = new HashSet<int>(PaperList_ykao);
                    PaperList_ykao.Clear();
                    PaperList_ykao.AddRange(hax);
                    //查询当前科目下并且未考的试卷
                    var PaperList_wkao = _currencyService.DbAccess().Queryable<C_Unit_Paper>().Where(pp => pp.SubjectId == subjectId && pp.ProjectId == projectId&&pp.UnitId== unitId).WhereIF(hax.Count>0,pp=>!PaperList_ykao.Contains(pp.PaperId)).ToList();
                    var row2 = sheet.CreateRow(list.Count + 2);
                    var cell11 = row2.CreateCell(0);
                    cell11.SetCellValue("未考试卷编号");
                    cell11.CellStyle = headStyle;
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(list.Count + 2, list.Count + 2,0,7));
                    if (PaperList_wkao != null && PaperList_wkao.Count > 0) {
                        var row3 = sheet.CreateRow(list.Count +3);
                        var cell12 = row3.CreateCell(0);
                        var paperCodeGroup =string.Join('，',PaperList_wkao.Select(p => p.PaperCode).ToList());
                        cell12.SetCellValue(paperCodeGroup);
                        cell12.CellStyle = headStyle;
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(list.Count +3, list.Count+3,0,7));
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
            var exportTitle = "考试批量列表";
            if (studentModel != null) {
                exportTitle=studentModel.Student_Name+"考试测试列表";
            }
            return File(streamArr, "application/vnd.ms-excel",exportTitle + DateTime.Now.ToString("yyyyMMddHHmmssss") + ".xls");
        }

        //更新分数
        public IActionResult SaveInfo(CourseWorkModel vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.Id > 0)
                {
                    if (string.IsNullOrEmpty(vmodel.PaperCode))
                        vmodel.PaperId=0;
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(n => new C_Course_Work { Score = vmodel.Score,MockLevel=vmodel.MockLevel,PaperId=vmodel.PaperId }).Where(n => n.Id == vmodel.Id).ExecuteCommand();
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

        /// <summary>
        /// 获取试卷编码
        /// </summary>
        /// <param name="wkId"></param>
        /// <param name="paperCode"></param>
        /// <returns></returns>
        public IActionResult QueryPaper(int wkId,string paperCode) {
            ResResult rsg = new ResResult() { code =200, msg = "获取成功" };
            List<C_Unit_Paper> list = new List<C_Unit_Paper>();
            var work = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(wk => wk.Id == wkId).First();
            if (!string.IsNullOrEmpty(paperCode))
            {
                list = _currencyService.DbAccess().Queryable<C_Unit_Paper>().Where(pp => pp.SubjectId == work.SubjectId && pp.ProjectId == work.ProjectId&&pp.UnitId == work.UnitId && pp.PaperCode.Contains(paperCode)).Select<C_Unit_Paper>(pp=>new C_Unit_Paper{ 
                 PaperId=pp.PaperId,
                 PaperCode=pp.PaperCode,
                 AvgScore=pp.AvgScore
                }).ToList();
            }
            else {
                list = _currencyService.DbAccess().Queryable<C_Unit_Paper>().Where(pp => pp.SubjectId == work.SubjectId && pp.ProjectId == work.ProjectId && pp.UnitId == work.UnitId).WhereIF(work.UnitId > 0, pp => pp.UnitId == work.UnitId).Select<C_Unit_Paper>(pp => new C_Unit_Paper
                {
                    PaperId = pp.PaperId,
                    PaperCode = pp.PaperCode,
                    AvgScore = pp.AvgScore
                }).ToPageList(1, 20);
            }
            rsg.data = list;
            return Json(rsg);
        }
    }
}
