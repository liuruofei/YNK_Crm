﻿using ADT.Models;
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
    public class MockUserController : BaseController
    {
        private ICurrencyService _currencyService;

        public MockUserController(ICurrencyService currencyService) {
            _currencyService = currencyService;
        }

        [UsersRoleAuthFilter("H-152", FunctionEnum.Have)]
        public IActionResult Index(int studentUid)
        {
            C_Contrac_User u = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(it => it.StudentUid == studentUid).First();
            ViewBag.studentUid = studentUid;
            ViewBag.studentName = u.Student_Name;
            return View();
        }
        protected override void Init()
        {
            this.MenuID = "H-152";
        }

        [UsersRoleAuthFilter("H-152", FunctionEnum.Have)]
        public IActionResult GetDataSource(int studentUid,int subjectId,int projectId,int unitId, string title,int page = 1, int limit = 10)
        {
            int total = 0;
            PageList<MockUserModel> pageModel = new PageList<MockUserModel>();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Contrac_User,C_Subject, C_Project,C_Project_Unit,C_Unit_Paper>((wk,u,sub,pro,unt,pp)=>new object[]{JoinType.Left,wk.StudentUid== u.StudentUid, JoinType.Left,wk.SubjectId==sub.SubjectId,JoinType.Left,wk.ProjectId==pro.ProjectId,JoinType.Left,wk.UnitId==unt.UnitId, JoinType.Left,wk.PaperId==pp.PaperId })
                .Where((wk, u, sub, pro, unt) => wk.StudyMode==5&&wk.StudentUid==studentUid)
                .WhereIF(subjectId>0,(wk, u, sub, pro, unt)=>wk.SubjectId==subjectId)
                .WhereIF(projectId>0, (wk, u, sub, pro, unt) => wk.ProjectId == projectId)
                .WhereIF(unitId > 0, (wk, u, sub, pro, unt) => wk.UnitId == unitId)
                .WhereIF(!string.IsNullOrEmpty(title), (wk, u, sub, pro, unt) => wk.Work_Title.Contains(title))
                .Select((wk, u, sub, pro, unt,pp) => new  MockUserModel{Id=wk.Id,StudentUid=u.StudentUid,Student_Name=u.Student_Name,StudyMode=wk.StudyMode,SubjectId=wk.SubjectId,ProjectId=wk.ProjectId,UnitId=wk.UnitId,SubjectName=sub.SubjectName,
                    ProjectName=pro.ProjectName,UnitName=unt.UnitName,Score=wk.Score,AT_Date=wk.AT_Date,EndTime=wk.EndTime,StartTime=wk.StartTime,MockLevel=wk.MockLevel,PaperCode=pp.PaperCode}
                ).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("H-152", FunctionEnum.Have)]
        public IActionResult GetDataSourceByUid(int studentUid, int subjectId, int projectId, int unitId,DateTime? startTime,DateTime? endTime)
        {
            if (!endTime.HasValue) {
                endTime =DateTime.Now;
            }
            ResResult rsg = new ResResult() { code = 200, msg = "获取数据成功" };
            List<int> unitIds = new List<int>();
            List<MockUnitPartModel> unitPart = new List<MockUnitPartModel>();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work,  C_Project_Unit, C_Unit_Paper,C_Subject,C_Project>((wk, unt, pp,sub,pro) => new object[] { JoinType.Left, wk.UnitId == unt.UnitId, JoinType.Left, wk.PaperId == pp.PaperId,JoinType.Left,wk.SubjectId==sub.SubjectId,JoinType.Left,wk.ProjectId==pro.ProjectId })
                .Where((wk, unt, pp) => wk.StudyMode == 5 && wk.StudentUid == studentUid)
                .WhereIF(subjectId > 0, (wk) => wk.SubjectId == subjectId)
                .WhereIF(projectId > 0, (wk) => wk.ProjectId == projectId)
                .WhereIF(unitId > 0, (wk) => wk.UnitId == unitId)
                .WhereIF(startTime.HasValue, (wk) => wk.AT_Date>=startTime.Value)
                .WhereIF(endTime.HasValue, (wk) => wk.AT_Date<= endTime.Value)
                .Select((wk,unt,pp,sub,pro) => new MockUserModel
                {
                    Id = wk.Id,
                    StudentUid=wk.StudentUid,
                    StudyMode = wk.StudyMode,
                    UnitId = wk.UnitId,
                    PaperId=wk.PaperId,
                    UnitName = unt.UnitName,
                    Score = wk.Score,
                    AT_Date = wk.AT_Date,
                    MockLevel = wk.MockLevel,
                    PaperCode = pp.PaperCode,
                    SubjectName=sub.SubjectName,
                    ProjectName=pro.ProjectName
                }
                ).ToList();
            //查询未来3个月需要实考的单元
            var shikUnitPart= _currencyService.DbAccess().Queryable<C_Course_Work,C_Subject,C_Project,C_Project_Unit>((sk,sub,pro,unt)=>new Object[]{ JoinType.Left,sk.SubjectId==sub.SubjectId,JoinType.Left,sk.ProjectId==pro.ProjectId,JoinType.Left,sk.UnitId==unt.UnitId })
                .Where(sk => sk.StudyMode == 6 && sk.StudentUid == studentUid && sk.AT_Date > endTime && sk.AT_Date < DateTime.Now.AddMonths(3)).Select((sk, sub, pro, unt)=>new MockUserModel {
                    Id = sk.Id,
                    StudentUid = sk.StudentUid,
                    StudyMode = sk.StudyMode,
                    UnitId = sk.UnitId,
                    PaperId = sk.PaperId,
                    UnitName = unt.UnitName,
                    AT_Date = sk.AT_Date,
                    SubjectName = sub.SubjectName,
                    ProjectName = pro.ProjectName
                }).ToList();
            if (list != null&& shikUnitPart!=null&& shikUnitPart.Count>0) {
                shikUnitPart.ForEach(it =>
                {
                    list.Add(it);
                });
            }
            List<int> shikUitId = new List<int>();
            if (shikUnitPart != null && shikUnitPart.Count > 0) {
                shikUitId = shikUnitPart.Select(y => y.UnitId).Distinct().ToList();
            }
            if (list != null && list.Count > 0) {
                unitIds = list.Select(n => n.UnitId).Distinct().ToList();
                //查找单元下面相关的试卷
                var paperList = _currencyService.DbAccess().Queryable<C_Unit_Paper>().Where(n => unitIds.Contains(n.UnitId)).ToList();
                foreach (var crtUnt in unitIds) {
                    var untPaperScoreArr = list.Where(n => n.UnitId == crtUnt).ToList();
                    var untPaperArr = paperList.Where(n => n.UnitId == crtUnt).ToList();
                    MockUnitPartModel parUnt = new MockUnitPartModel();
                    parUnt.UnitId = crtUnt;
                    if(shikUitId!=null&& shikUitId.Contains(crtUnt)) {
                        parUnt.isRed = 1;
                    }
                    parUnt.UnitName = (untPaperScoreArr[0].UnitName.Length < 8 ? untPaperScoreArr[0].SubjectName + "_" : "") + untPaperScoreArr[0].ProjectName + "_" + untPaperScoreArr[0].UnitName;
                    parUnt.PaPerArr = new List<MockUnitPaperArr>();
                    if (untPaperArr != null && untPaperArr.Count > 0) {
                        foreach (var crtPaper in untPaperArr)
                        {
                            MockUnitPaperArr arrInfo = new MockUnitPaperArr();
                            arrInfo.PaperCode = crtPaper.PaperCode;
                            //如果当前试卷的成绩有重考
                            var hasMorePaper = untPaperScoreArr.Where(n => n.PaperId == crtPaper.PaperId).ToList();
                            if (hasMorePaper != null && hasMorePaper.Count > 0)
                            {
                                var i = 0;
                                foreach (var rstScore in hasMorePaper)
                                {
                                    if (i == 0)
                                    {
                                        arrInfo.MockLevelAppend = rstScore.MockLevel;
                                        arrInfo.ScoreAppend = rstScore.Score;
                                        arrInfo.WKDateAppend = rstScore.AT_Date.ToString("yyyy/MM/dd");
                                    }
                                    else
                                    {
                                        arrInfo.MockLevelAppend += " | " + rstScore.MockLevel;
                                        arrInfo.ScoreAppend+= " ，" + rstScore.Score;
                                        arrInfo.WKDateAppend+= " ，" + rstScore.AT_Date.ToString("yyyy/MM/dd");
                                    }
                                    i++;
                                }

                            }
                            parUnt.PaPerArr.Add(arrInfo);
                        }
                    }
                    unitPart.Add(parUnt);
                }
            }
            rsg.data = unitPart;
            return Json(rsg);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="studentUid"></param>
        /// <param name="endtime"></param>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public virtual IActionResult ExportPlan(int studentUid, int subjectId, int projectId, int unitId, string title)
        {
            var student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(n => n.StudentUid == studentUid).First();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Contrac_User, C_Subject, C_Project, C_Project_Unit, C_Unit_Paper>((wk, u, sub, pro, unt, pp) => new object[] { JoinType.Left, wk.StudentUid == u.StudentUid, JoinType.Left, wk.SubjectId == sub.SubjectId, JoinType.Left, wk.ProjectId == pro.ProjectId, JoinType.Left, wk.UnitId == unt.UnitId, JoinType.Left, wk.PaperId==pp.PaperId })
                   .Where((wk, u, sub, pro, unt) => wk.StudyMode == 5 && wk.StudentUid == studentUid)
                   .WhereIF(subjectId > 0, (wk, u, sub, pro, unt) => wk.SubjectId == subjectId)
                   .WhereIF(projectId > 0, (wk, u, sub, pro, unt) => wk.ProjectId == projectId)
                   .WhereIF(unitId > 0, (wk, u, sub, pro, unt) => wk.UnitId == unitId)
                   .WhereIF(!string.IsNullOrEmpty(title), (wk, u, sub, pro, unt) => wk.Work_Title.Contains(title))
                   .Select((wk, u, sub, pro, unt,pp) => new MockUserModel
                   {
                       Id = wk.Id,
                       StudentUid = u.StudentUid,
                       Student_Name = u.Student_Name,
                       StudyMode = wk.StudyMode,
                       SubjectId = wk.SubjectId,
                       ProjectId = wk.ProjectId,
                       UnitId = wk.UnitId,
                       SubjectName = sub.SubjectName,
                       ProjectName = pro.ProjectName,
                       UnitName = unt.UnitName,
                       Score = wk.Score,
                       AT_Date = wk.AT_Date,
                       EndTime = wk.EndTime,
                       StartTime = wk.StartTime,
                       MockLevel=wk.MockLevel,
                       PaperCode=pp.PaperCode
                   }
                   ).ToList();
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
            var sheet = workbook.CreateSheet(student.Student_Name + "模考统计");
            sheet.DefaultColumnWidth = 25;
            string[] cellNames = new string[] {"模式","姓名","日期", "类别", "科目", "单元", "成绩","等级","试卷编号"};
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
                    var row = sheet.CreateRow(y + 1);
                    var cell0 = row.CreateCell(0);
                    cell0.SetCellValue("模考");
                    cell0.CellStyle = headStyle;
                    var cell1 = row.CreateCell(1);
                    cell1.SetCellValue(list[y].Student_Name);
                    cell1.CellStyle = headStyle;
                    var cell2 = row.CreateCell(2);
                    cell2.SetCellValue(list[y].AT_Date.ToString("yyyy-MM-dd"));
                    cell2.CellStyle = headStyle;
                    var cell3 = row.CreateCell(3);
                    cell3.SetCellValue(list[y].SubjectName);
                    cell3.CellStyle = headStyle;
                    var cell4 = row.CreateCell(4);
                    cell4.SetCellValue(list[y].ProjectName);
                    cell4.CellStyle = headStyle;
                    var cell5 = row.CreateCell(5);
                    cell5.SetCellValue(list[y].UnitName);
                    cell5.CellStyle = headStyle;
                    var cell6 = row.CreateCell(6);
                    cell6.SetCellValue(list[y].Score);
                    cell6.CellStyle = headStyle;
                    var cell7 = row.CreateCell(7);
                    cell7.SetCellValue(list[y].MockLevel);
                    cell7.CellStyle = headStyle;
                    var cell8 = row.CreateCell(8);
                    cell8.SetCellValue(list[y].PaperCode);
                    cell8.CellStyle = headStyle;
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
            return File(streamArr, "application/vnd.ms-excel", student.Student_Name + "模考详情" + DateTime.Now.ToString("yyyyMMddHHmmssss") + ".xls");
        }
    }
}
