using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models.Res;
using ADT.Models;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class TaHouserReportController : BaseController
    {
        private ICurrencyService _currencyService;
        public TaHouserReportController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "R-154";
        }

        [UsersRoleAuthFilter("R-154", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 统计督学工时
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <param name="dateTime"></param>
        /// <param name="userName"></param>
        /// <param name="subjectId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public IActionResult QueryWorkSource(string startTime, string endTime, int monthStatu)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult reg = new ResResult();
            StringBuilder manwhere = new StringBuilder("");
            //本月
            if (monthStatu == 1 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and convert(varchar(6),pln.WorkDate,112)=convert(varchar(6),getdate(),112) and pln.WorkDate<getdate()");
            }
            //上月
            if (monthStatu == 2 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and convert(varchar(6),pln.WorkDate,112)=convert(varchar(6),dateadd(mm,-1,getdate()),112)");
            }
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and pln.WorkDate>=CAST(@startStr AS date) AND pln.WorkDate<CAST(@endStr AS date)");
            }
            StringBuilder sql=new StringBuilder ("(select count(Id)as wkScore,sysU.[User_ID],sysU.[User_Name] from  C_Student_Work_Plan pln left join Sys_User sysU  on pln .TaUid=sysU.[User_ID] ");
            sql.Append(" where  pln.HomeWorkComent!='' and pln.HomeWorkComent is not null and pln.TaUid!='' and pln.TaUid is not null and pln.CampusId=" + campusId+manwhere.ToString());
            sql.Append(" group by sysU.[User_ID],sysU.[User_Name])");
            dynamic list = _currencyService.DbAccess().Queryable(sql.ToString(),"orginSql")
            .AddParameters(new { startStr = startTime, endStr = endTime })
            .Select("*").ToList();
            totalRow total = _currencyService.DbAccess().Queryable("(select sum(wkScore)as totalCourseTime from " + sql.ToString()+ " as b)", "orginSql").AddParameters(new { startStr = startTime, endStr = endTime })
            .Select<totalRow>("*").First();
            reg.data = list;
            reg.totalRow = total;
            reg.code = 0;
            reg.msg = "获取成功";
            return Json(reg);
        }

        /// <summary>
        /// 获取督学工分详情
        /// </summary>
        /// <param name="taUid"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="monthStatu"></param>
        /// <returns></returns>
        public IActionResult QueryTaScoreInfo(string taUid, string startTime, string endTime, int monthStatu,int page=1,int limit=10) {
            int total = 0;
            ResResult reg = new ResResult();
            PageList<StudentWorkPlanModel> pageModel = new PageList<StudentWorkPlanModel>();
            StringBuilder manwhere = new StringBuilder("");
            //本月
            if (monthStatu == 1 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and convert(varchar(6),pln.WorkDate,112)=convert(varchar(6),getdate(),112) and pln.WorkDate<getdate()");
            }
            //上月
            if (monthStatu == 2 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and convert(varchar(6),pln.WorkDate,112)=convert(varchar(6),dateadd(mm,-1,getdate()),112)");
            }
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and pln.WorkDate>=CAST(@startStr AS date) AND pln.WorkDate<CAST(@endStr AS date)");
            }
            StringBuilder sql = new StringBuilder("(select sysU.[User_Name] as TaUseName,stu.Student_Name,pln.WorkDate,pln.HomeWorkComent from  C_Student_Work_Plan pln left join Sys_User sysU  on pln.TaUid=sysU.[User_ID]");
            sql.Append(" left join C_Contrac_User stu on pln.StudentUid=stu.StudentUid ");
            sql.Append(" where  pln.HomeWorkComent!='' and pln.HomeWorkComent is not null and pln.TaUid!='' and pln.TaUid is not null "  + manwhere.ToString());
            sql.Append(" and pln.TaUid=@taUid)");
            List<StudentWorkPlanModel> list = _currencyService.DbAccess().Queryable(sql.ToString(), "orginSql")
            .AddParameters(new { startStr = startTime, endStr = endTime,taUid =taUid})
            .Select<StudentWorkPlanModel>("*").OrderBy("orginSql.WorkDate desc").ToPageList(page, limit, ref total);
            pageModel.data = list;
            pageModel.code = 0;
            pageModel.msg = "获取成功";
            pageModel.count = total;
            return Json(pageModel);
        }

        public IActionResult ExportTaScore(string taUid, string startTime, string endTime, int monthStatu)
        {
            var taUser = _currencyService.DbAccess().Queryable<sys_user>().Where(it => it.User_ID.Equals(taUid)).First();
            StringBuilder manwhere = new StringBuilder("");
            //本月
            if (monthStatu == 1 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and convert(varchar(6),pln.WorkDate,112)=convert(varchar(6),getdate(),112) and pln.WorkDate<getdate()");
            }
            //上月
            if (monthStatu == 2 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and convert(varchar(6),pln.WorkDate,112)=convert(varchar(6),dateadd(mm,-1,getdate()),112)");
            }
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and pln.WorkDate>=CAST(@startStr AS date) AND pln.WorkDate<CAST(@endStr AS date)");
            }
            StringBuilder sql = new StringBuilder("(select  sysU.[User_ID],sysU.[User_Name],stu.Student_Name,pln.WorkDate,pln.HomeWorkComent from  C_Student_Work_Plan pln left join Sys_User sysU  on pln.TaUid=sysU.[User_ID]");
            sql.Append(" left join C_Contrac_User stu on pln.StudentUid=stu.StudentUid ");
            sql.Append(" where  pln.HomeWorkComent!='' and pln.HomeWorkComent is not null and pln.TaUid!='' and pln.TaUid is not null " + manwhere.ToString());
            sql.Append(" and pln.TaUid=@taUid)");
            List<TaScoreModel> list = _currencyService.DbAccess().Queryable(sql.ToString(), "orginSql")
            .AddParameters(new { startStr = startTime, endStr = endTime, taUid = taUid })
            .Select<TaScoreModel>("*").ToList();
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
            var sheet = workbook.CreateSheet("工分详情");
            sheet.DefaultColumnWidth = 25;
            string[] cellNames = new string[] { "学生姓名", "完成作业", "督学", "日期" };
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
                    cell0.SetCellValue(list[y].Student_Name);
                    cell0.CellStyle = headStyle;
                    var cell1 = row.CreateCell(1);
                    cell1.SetCellValue(list[y].HomeWorkComent);
                    cell1.CellStyle = headStyle;
                    var cell2 = row.CreateCell(2);
                    cell2.SetCellValue(list[y].User_Name);
                    cell2.CellStyle = headStyle;
                    var cell3 = row.CreateCell(3);
                    cell3.SetCellValue(list[y].WorkDate.Value.ToString("yyyy-MM-dd"));
                    cell3.CellStyle = headStyle;
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
            return File(streamArr, "application/vnd.ms-excel", taUser.User_Name+"工分详情" + DateTime.Now.ToString("yyyyMMddHHmmssss") + ".xls");
        }
    }
}
