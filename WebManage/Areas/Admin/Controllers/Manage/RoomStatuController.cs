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
    public class RoomStatuController : BaseController
    {
        private ICurrencyService _currencyService;
        public RoomStatuController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "V-359";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("V-359", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult Find(int ID) {
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

        public IActionResult Add(int ID)
        {
            ViewBag.ID =ID;
            C_Course_Work work = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(f => f.Id ==ID).First();
            return View(work);
        }


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("V-359", "Add,Edit")]
        public IActionResult SaveInfo(C_Course_Work vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel.Id>0)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
                if (vmodel.RoomId<1)
                    return Json(new { code = 0, msg = "教室不能为空" });
                C_Course_Work work = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(f => f.Id == vmodel.Id).First();
                work.RoomId = vmodel.RoomId;
                var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(f=>new C_Course_Work {RoomId=vmodel.RoomId}).Where(f=>f.Id==vmodel.Id).ExecuteCommand();
                if (result > 0)
                {
                    rsg.code = 200;
                    rsg.msg = "更新教室成功";
                }
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }


        public IActionResult QueryWorkSource(string startStr, string endStr, string userName,int page = 1, int limit = 10)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult reg = new ResResult();
            var students = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(t => t.Student_Name.Contains(userName)).ToList();
            List<int> classIds = new List<int>();
            if (students != null && students.Count > 0)
            {
                var studentids = students.Select(v => v.StudentUid).ToList();
                //排除退班的班级
                int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((c, cl) => new object[] { JoinType.Left, c.ClassId == cl.ClassId }).Where(c => studentids.Contains(c.StudentUid) && c.ClassId > 0 && c.Contrac_Child_Status != contracStatus).Select(c => c.ClassId).ToList();
            }
            string sql = @"(select wk.*,contracU.Student_Name,ta.User_Name as TaUserName,tc.User_Name as TeacherName,rm.RoomName from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.StudyMode<>3  and wk.StudyMode<>5 and wk.StudyMode<>6 and wk.StudyMode<>9";
            if (!string.IsNullOrEmpty(startStr))
            {
                sql += " AND wk.AT_Date>=CAST(@startStr AS date)";
            }
            if (!string.IsNullOrEmpty(endStr)) {
                sql+= "  AND wk.AT_Date<CAST(@endStr AS date)";
            }
            if (classIds != null && classIds.Count > 0)
            {
                sql += " and (wk.ClasssId in(" + string.Join(",", classIds) + ") or charindex(@userName,contracU.Student_Name)>0) ";
            }
            else
            {
                if (!string.IsNullOrEmpty(userName))
                    sql += " and (charindex(@userName,tc.User_Name)>0 or charindex(@userName,contracU.Student_Name)>0 or charindex(@userName,wk.ListeningName)>0 or charindex(@userName,cl.Class_Name)>0) ";
            }
            sql += " and wk.CampusId=" + campusId;
            sql += ")";
            int total = 0;
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            List<CourseWorkModel> list = _currencyService.DbAccess().Queryable(sql, "orginSql")
            .AddParameters(new { startStr = startStr, endStr = endStr, userName = userName })
            .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            reg.data = list;
            reg.code = 0;
            reg.count = total;
            reg.msg = "获取成功";
            return Json(reg);
        }

    }
}
