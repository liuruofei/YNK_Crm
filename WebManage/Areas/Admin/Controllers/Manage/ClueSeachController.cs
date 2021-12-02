using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
    public class ClueSeachController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ClueUserService _clueUserService;
        public ClueSeachController(ICurrencyService currencyService, IC_ClueUserService clueUserService)
        {
            _currencyService = currencyService;
            _clueUserService = clueUserService;
        }
        protected override void Init()
        {
            this.MenuID = "A-104";
        }
        [UsersRoleAuthFilter("A-104", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("A-104", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<ResClueUserModel> pageModel = new PageList<ResClueUserModel>();
            var list = _currencyService.DbAccess().Queryable("C_ClueUser", "clue").AddJoinInfo("Sys_User", "cc", "clue.CC_Uid=cc.User_ID", SqlSugar.JoinType.Left).AddJoinInfo("C_Campus", "camp", "clue.CampusId=camp.CampusId", SqlSugar.JoinType.Left)
                                .AddJoinInfo("C_ClueUser", "clueDefault", "clue.Defualt_ClueId=clueDefault.ClueId", SqlSugar.JoinType.Left).AddJoinInfo(@"Sys_User", "cc2", "clueDefault.CC_Uid=cc2.User_ID", SqlSugar.JoinType.Left)
                .Where("(charindex(@title,clue.Student_Name)>0 or charindex(@title,clue.Student_Phone)>0)").AddParameters(new { title = title })
                .Where("clue.CampusId=@campusId and clue.Status<1").AddParameters(new { campusId = campusId })
                .Select<ResClueUserModel>(@"clue.*,cc.User_Name as CC_UserName,cc2.User_Name as Default_CC_UserName,(select Count(*) from C_ClueUser_Record record where record.ClueId=clue.ClueId) as Follow_Count,
                 camp.CampusName").OrderBy("clue.CreateTime desc").ToPageList(page, limit, ref total);
            if (list != null && list.Count > 0) {
                list.ForEach(ite => {
                    if (ite.CC_Uid != userId) {
                        ite.Student_Phone = !string.IsNullOrEmpty(ite.Student_Phone) ? ite.Student_Phone.Substring(0, 3) + "********" : "";
                    }
                });
            }
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }
    }
}
