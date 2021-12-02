using ADT.Models;
using ADT.Models.Enum;
using ADT.Models.InputModel;
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
    public class ContrcChangeClassController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public ContrcChangeClassController(ICurrencyService currencyService, IC_ContracService contrac)
        {
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "T-355";
        }

        [UsersRoleAuthFilter("T-355", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }


        //学员签约合同列表
        [UsersRoleAuthFilter("T-355", FunctionEnum.Have)]
        public IActionResult GetDataSource(int contraProperty, int studymode, DateTime? startime, DateTime? endtime, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<ChangeClassModel> pageModel = new PageList<ChangeClassModel>();
            var list = _currencyService.DbAccess().Queryable(@"(select cls.*,c.Pay_Amount,c.Contrac_Child_Status,c.Pay_Time,contracU.Student_Name,contracU.Student_Phone,cc.User_Name as CCUserName,cs1.Class_Name as ClassName,cs2.Class_Name as ChangeClassName from C_Contrac_Child c inner join C_Contrac_Child_ChangeClass cls on c.Contra_ChildNo=cls.Contra_ChildNo left join C_Contrac_User contracU on c.StudentUid=contracU.StudentUid
               left join C_Class cs1 on cls.OldClassId=cs1.ClassId  left join C_Class cs2 on cls.ChangeClassId=cs2.ClassId left join Sys_User cc on c.CC_Uid=cc.User_ID where c.CampusId=@CampusId)", "orginSql").AddParameters(new { CampusId = campusId })
                .WhereIF(contraProperty > -1, "orginSql.Contra_Property=@property").AddParameters(new { property = contraProperty })
                .WhereIF(studymode > 0, "orginSql.StudyMode=@studyModes").AddParameters(new { studyModes = studymode })
                .WhereIF(startime != null && !string.IsNullOrEmpty(startime.ToString()), "orginSql.Pay_Time>=@startime").AddParameters(new { startime = startime })
                .WhereIF(endtime != null && !string.IsNullOrEmpty(endtime.ToString()), "orginSql.Pay_Time<=@endtime").AddParameters(new { endtime = endtime })
                .Where("(orginSql.Contrac_Child_Status=" + (int)ConstraChild_Status.ChangeClass + " or orginSql.Contrac_Child_Status=" + (int)ConstraChild_Status.ChangeClassOk + " or orginSql.Contrac_Child_Status=" + (int)ConstraChild_Status.ChangeClassReject + ")")
                .Select<ChangeClassModel>("*").OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        /// <summary>
        /// 子合同转班审核
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult ChildContrcChnageClassAudit(string Contra_ChildNo, bool through)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (!string.IsNullOrEmpty(Contra_ChildNo))
            {
                reg = _contrac.ContrcChnageClassAudit(Contra_ChildNo, through, userId);
            }
            else
            {
                reg.msg = "缺少参数";
                reg.code = 300;
            }
            return Json(reg);
        }
    }
}
