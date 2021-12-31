using ADT.Models;
using ADT.Models.Enum;
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
    public class UserChildContracChangeController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public UserChildContracChangeController(ICurrencyService currencyService, IC_ContracService contrac)
        {
            _currencyService = currencyService;
        }

        protected override void Init()
        {
            this.MenuID = "T-352";
        }

        [UsersRoleAuthFilter("T-352", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("T-352", FunctionEnum.Have)]
        public IActionResult ChildDtail(string childContraNo, string contraNo)
        {
            ViewBag.ChildContraNo = childContraNo;
            ViewBag.ContraNo = contraNo;
            return View();
        }



        //学员签约合同列表
        [UsersRoleAuthFilter("T-352", FunctionEnum.Have)]
        public IActionResult GetDataSource(int contraProperty, int studymode, DateTime? startime, DateTime? endtime, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<UserChildContracModel> pageModel = new PageList<UserChildContracModel>();
            var list = _currencyService.DbAccess().Queryable(@"(select c.*,contracU.Student_Name,contracU.Student_Phone,cc.User_Name as CCUserName,contracU.Amount from C_Contrac_Child c left join C_Contrac_User contracU on c.StudentUid=contracU.StudentUid
                left join Sys_User cc on c.CC_Uid=cc.User_ID where c.CampusId=@CampusId)", "orginSql").AddParameters(new { CampusId = campusId })
                .WhereIF(contraProperty > -1, "orginSql.Contra_Property=@property").AddParameters(new { property = contraProperty })
                .WhereIF(studymode > 0, "orginSql.StudyMode=@studyModes").AddParameters(new { studyModes = studymode })
                .WhereIF(startime != null && !string.IsNullOrEmpty(startime.ToString()), "orginSql.Pay_Time>=@startime").AddParameters(new { startime = startime })
                .WhereIF(endtime != null && !string.IsNullOrEmpty(endtime.ToString()), "orginSql.Pay_Time<=@endtime").AddParameters(new { endtime = endtime })
                .Where("(orginSql.Contrac_Child_Status="+ (int)ConstraChild_Status.Change+ " or orginSql.Contrac_Child_Status="+ (int)ConstraChild_Status.ChangeOk+ ")")
                .Select<UserChildContracModel>("*").OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


       /// <summary>
       /// 校长确认变更
       /// </summary>
       /// <returns></returns>
        public IActionResult ChildContrcChangeSave(string childContrcNo) {
            ResResult rsg = new ResResult() { code = 0, msg = "缺少参数合同编号" };
            if (!string.IsNullOrEmpty(childContrcNo))
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                C_Contrac_Child model = _currencyService.DbAccess().Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(childContrcNo)).First();
                model.UpdateUid = userId;
                model.UpdateTime = DateTime.Now;
                model.Contrac_Child_Status = (int)ConstraChild_Status.ChangeOk;
                _currencyService.DbAccess().Updateable<C_Contrac_Child>(model).ExecuteCommand();
                rsg.code = 200;
                rsg.msg = "保存成功";
            }
            return Json(rsg);
        }
    }
}
