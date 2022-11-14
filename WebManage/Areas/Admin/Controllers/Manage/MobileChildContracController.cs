using ADT.Models;
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
    public class MobileChildContracController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public MobileChildContracController(ICurrencyService currencyService, IC_ContracService contrac)
        {
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "T-358";
        }

        [UsersRoleAuthFilter("T-358", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetDataSource(string title, int contraProperty, int studymode, DateTime? startime, DateTime? endtime, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            var ccUse = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where(u => u.User_ID == userId).Select<sys_role>().First();
            PageList<UserChildContracModel> pageModel = new PageList<UserChildContracModel>();
            var list = _currencyService.DbAccess().Queryable(@"(select c.*,contracU.Student_Name,contracU.Student_Phone,cc.User_Name as CCUserName,contracU.Amount,cl.Class_Name from C_Contrac_Child c left join C_Contrac_User contracU on c.StudentUid=contracU.StudentUid
                left join Sys_User cc on c.CC_Uid=cc.User_ID left join C_Class cl on c.ClassId=cl.ClassId where c.CampusId=@CampusId)", "orginSql").AddParameters(new { CampusId = campusId })
                .WhereIF(contraProperty > -1, "orginSql.Contra_Property=@property").AddParameters(new { property = contraProperty })
                .WhereIF(studymode > 0, "orginSql.StudyMode=@studyModes").AddParameters(new { studyModes = studymode })
                .WhereIF(!string.IsNullOrEmpty(title), "(charindex(@title,orginSql.Student_Name)>0 or charindex(@title,orginSql.Class_Name)>0 or charindex(@title,orginSql.Contra_ChildNo)>0)").AddParameters(new { title = title })
                .WhereIF(startime != null && !string.IsNullOrEmpty(startime.ToString()), "orginSql.Pay_Time>=@startime").AddParameters(new { startime = startime })
                .WhereIF(endtime != null && !string.IsNullOrEmpty(endtime.ToString()), "orginSql.Pay_Time<=@endtime").AddParameters(new { endtime = endtime })
                .Select<UserChildContracModel>("*").OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }
    }
}
