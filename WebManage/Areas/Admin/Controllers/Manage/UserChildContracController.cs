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
using ADT.Models.InputModel;
using SqlSugar;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class UserChildContracController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public UserChildContracController(ICurrencyService currencyService, IC_ContracService contrac)
        {
            _currencyService = currencyService;
            _contrac = contrac;
        }

        protected override void Init()
        {
            this.MenuID = "T-351";
        }

        [UsersRoleAuthFilter("T-351", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        //学员签约合同列表
        [UsersRoleAuthFilter("T-351", FunctionEnum.Have)]
        public IActionResult GetDataSource(int contraProperty, int studymode,DateTime? startime,DateTime? endtime, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            var ccUse = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where(u => u.User_ID == userId).Select<sys_role>().First();
            PageList<UserChildContracModel> pageModel = new PageList<UserChildContracModel>();
            var list = _currencyService.DbAccess().Queryable(@"(select c.*,contracU.Student_Name,contracU.Student_Phone,cc.User_Name as CCUserName from C_Contrac_Child c left join C_Contrac_User contracU on c.StudentUid=contracU.StudentUid
                left join Sys_User cc on c.CC_Uid=cc.User_ID where c.CampusId=@CampusId)", "orginSql").AddParameters(new { CampusId = campusId })
                .WhereIF(contraProperty>-1, "orginSql.Contra_Property=@property").AddParameters(new { property=contraProperty })
                .WhereIF(studymode > 0, "orginSql.StudyMode=@studyModes").AddParameters(new { studyModes=studymode })
                .WhereIF(startime!=null&&!string.IsNullOrEmpty(startime.ToString()), "orginSql.Pay_Time>=@startime").AddParameters(new { startime = startime })
                .WhereIF(endtime != null && !string.IsNullOrEmpty(endtime.ToString()), "orginSql.Pay_Time<=@endtime").AddParameters(new { endtime = endtime })
                .WhereIF(ccUse != null && ccUse.Role_Name == "顾问", "orginSql.CC_Uid=@CCuid").AddParameters(new { CCuid = userId })
                .Select<UserChildContracModel>("*").OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

        /// <summary>
        /// 子合同退款
        /// </summary>
        /// <param name="childContrcNo"></param>
        /// <returns></returns>
        public IActionResult BackCost(string childContrcNo)
        {
            ViewBag.ChildContrcNo = childContrcNo;
            return View();
        }

        /// <summary>
        /// 子合同退班
        /// </summary>
        /// <param name="childContrcNo"></param>
        /// <returns></returns>
        public IActionResult BackClass(string childContrcNo)
        {
            ViewBag.ChildContrcNo = childContrcNo;
            return View();
        }


        /// <summary>
        /// 子合同转班
        /// </summary>
        /// <param name="childContrcNo"></param>
        /// <returns></returns>
        public IActionResult ChangeClass(string childContrcNo)
        {
            ViewBag.ChildContrcNo = childContrcNo;
            return View();
        }

        public IActionResult FindChangeClass(string childcontraNo)
        {
            C_ContracChildModel vmodel =_currencyService.DbAccess().Queryable<C_Contrac_Child_ChangeClass>().Where(c => c.Contra_ChildNo == childcontraNo).Select<C_ContracChildModel>(c=>new C_ContracChildModel {
                StudyStatus = c.StudyStatus,
                StudentUid = c.StudentUid,
                ClassId = c.OldClassId,
                Class_Course_Time = c.Class_Course_Time,
                Cycle = c.Cycle,
                Remarks = c.Remarks,
                StudyMode = c.StudyMode,
                Saler_Amount = c.Saler_Amount,
                ContraRate = c.ContraRate,
                Contra_ChildNo = c.Contra_ChildNo,
                StartTime=c.StartTime,
                Contra_Property = c.Contra_Property,
                Discount_Amount = c.Discount_Amount,
                IsPreferential = c.IsPreferential,
                Original_Amount = c.Original_Amount,
                ChangeClassId=c.ChangeClassId
            }).First();
            if (vmodel==null)
            {
                vmodel = _currencyService.DbAccess().Queryable<C_Contrac_Child>().Where(c => c.Contra_ChildNo == childcontraNo)
                 .Select<C_ContracChildModel>(c => new C_ContracChildModel
                 {
                     Id = c.Id,
                     StudyStatus = c.StudyStatus,
                     StudentUid = c.StudentUid,
                     StartTime = c.StartTime,
                     SignIn_Data = c.SignIn_Data,
                     ClassId = c.ClassId,
                     Class_Course_Time = c.Class_Course_Time,
                     Cycle = c.Cycle,
                     Remarks = c.Remarks,
                     StudyMode = c.StudyMode,
                     Saler_Amount = c.Saler_Amount,
                     ContraNo = c.ContraNo,
                     ContraRate = c.ContraRate,
                     Contra_ChildNo = c.Contra_ChildNo,
                     Contra_Property = c.Contra_Property,
                     Discount_Amount = c.Discount_Amount,
                     IsPreferential = c.IsPreferential,
                     Pay_Amount = c.Pay_Amount,
                     Original_Amount = c.Original_Amount
                 }).First();
            }
            return Json(vmodel);
        }


        /// <summary>
        /// 子合同申请退款
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult ChildContrcbackCost(C_Contrac_Child_ConstBack input)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (!string.IsNullOrEmpty(input.Contra_ChildNo))
            {
                reg = _contrac.ContrcBackCostPart(input);
            }
            else
            {
                reg.msg = "缺少参数";
                reg.code = 300;
            }
            return Json(reg);
        }


        /// <summary>
        /// 子合同申请退班
        /// </summary>
        /// <param name="childContrcNo"></param>
        /// <returns></returns>
        public IActionResult ChildContrcRetrunClass(C_Contrac_Child_RetrunClass input)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (!string.IsNullOrEmpty(input.Contra_ChildNo))
            {
                reg = _contrac.ContrcBackClass(input);
            }
            else
            {
                reg.msg = "缺少参数";
                reg.code = 300;
            }
            return Json(reg);
        }

        /// <summary>
        /// 子合同撤销
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <returns></returns>
        public IActionResult ChildContrcCancel(string childContrcNo)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (!string.IsNullOrEmpty(childContrcNo))
            {
                reg = _contrac.ContracCancel(childContrcNo, userId);
            }
            else
            {
                reg.msg = "缺少参数";
                reg.code = 300;
            }
            return Json(reg);
        }

        /// <summary>
        /// 子合同确认
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <returns></returns>
        public IActionResult ChildContrcConfig(string childContrcNo)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (!string.IsNullOrEmpty(childContrcNo))
            {
                reg = _contrac.ContracChildConfig(childContrcNo, userId);
            }
            else
            {
                reg.msg = "缺少参数";
                reg.code = 300;
            }
            return Json(reg);
        }

        /// <summary>
        /// 转班申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult ChildContrcChangeClass(C_Contrac_Child_ChangeClass input) {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (!string.IsNullOrEmpty(input.Contra_ChildNo))
            {
                reg = _contrac.ContrcChangeClass(input);
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
