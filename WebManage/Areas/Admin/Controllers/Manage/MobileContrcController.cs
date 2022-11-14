using ADT.Models;
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
    public class MobileContrcController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public MobileContrcController(ICurrencyService currencyService, IC_ContracService contrac) {
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "T-357";
        }

        [UsersRoleAuthFilter("T-357", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetDataSource(string title, int studymode, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            var ccUse = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where(u => u.User_ID == userId).Select<sys_role>().First();
            PageList<UserContracModel> pageModel = new PageList<UserContracModel>();
            string where = "";
            if (!string.IsNullOrEmpty(title))
            {
                where += " AND  (charindex(@title,u.Student_Name)>0 or charindex(@title,cl.Class_Name)>0 or charindex(@title,child.ContraNo)>0)";
            }
            var list = _currencyService.DbAccess().Queryable("(select c.*,stuff((select ','+convert(varchar(25), child.StudyMode) FROM C_Contrac_Child child left join C_Class cl on child.ClassId=cl.ClassId left join C_Contrac_User u on child.StudentUid=u.StudentUid  WHERE child.ContraNo=c.ContraNo AND child.StudentUid =c.StudentUid " + where +
                "FOR XML PATH('')), 1, 1, '') as StudyModes,camp.CampusName,contracU.Student_Name,contracU.Student_Phone,cc.User_Name as CCUserName,contracU.Amount from C_Contrac c left join C_Contrac_User contracU on contracU.StudentUid=c.StudentUid" +
                " left join Sys_User cc on c.CC_Uid=cc.User_ID left join C_Campus camp on c.CampusId=camp.CampusId  where c.CampusId=@CampusId)", "orginSql").AddParameters(new { CampusId = campusId })
                .WhereIF(!string.IsNullOrEmpty(title), " charindex(@title,orginSql.Student_Name)>0 or charindex(@title,orginSql.Student_Phone)>0 or charindex(@title,orginSql.ContraNo)>0").AddParameters(new { title = title })
                .WhereIF(studymode > 0, "charindex('" + studymode + "',orginSql.StudyModes)>0")
                .WhereIF(ccUse != null && ccUse.Role_Name == "顾问", "orginSql.CC_Uid=@CCuid").AddParameters(new { CCuid = userId })
                .Select<UserContracModel>("*").OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

    }
}
