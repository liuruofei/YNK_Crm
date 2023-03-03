using ADT.Models;
using ADT.Models.Enum;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
    public class UserAmountApplyController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public UserAmountApplyController(ICurrencyService currencyService, IC_ContracService contrac)
        {
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "T-356";
        }

        [UsersRoleAuthFilter("T-356", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }


        //学员余额展示列表
        [UsersRoleAuthFilter("T-356", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<C_ContracUserModel> pageModel = new PageList<C_ContracUserModel>();
            var list = _currencyService.DbAccess().Queryable<C_Contrac_User, C_Campus, sys_user, sys_user>((u, cp, cc, cr) => new Object[] {
              JoinType.Left,u.CampusId ==cp.CampusId, JoinType.Left,u.CC_Uid==cc.User_ID, JoinType.Left,u.CR_Uid==cr.User_ID
            }).WhereIF(!string.IsNullOrEmpty(title), (u, cp, cc, cr) => u.Student_Name.Contains(title)).Where(u=>u.CampusId==Convert.ToInt32(campusId))
            .Select<C_ContracUserModel>((u, cp, cc, cr) => new C_ContracUserModel
            {
                StudentUid = u.StudentUid,
                Student_Name = u.Student_Name,
                Amount = u.Amount,
                IsBackAmount = u.IsBackAmount,
                ApplyBackAmount=u.ApplyBackAmount,
                Sex = u.Sex,
                Grade = u.Grade,
                ContactFamily = u.ContactFamily,
                Student_Phone = u.Student_Phone,
                CampusName = cp.CampusName,
                CR_UserName = cr.User_Name,
                CC_UserName = cc.User_Name,
                CreateTime = u.CreateTime
            }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        /// <summary>
        /// 用户余额退款申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult UserAmountBackApply(int uid,decimal applyBackAmount)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (uid > 0)
            {
                var model = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(v => v.StudentUid == uid).First();
                if (applyBackAmount < 0) {
                    reg.msg = "申请退费不能小于0";
                    reg.code = 200;
                    return Json(reg);
                }
                if (applyBackAmount>model.Amount)
                {
                    reg.msg = "申请退费大于学生余额,无法申请!";
                    reg.code = 200;
                    return Json(reg);
                }
                model.IsBackAmount = 1;
                model.ApplyBackAmount = applyBackAmount;
                var result = _currencyService.DbAccess().Updateable<C_Contrac_User>(model).ExecuteCommand();
                if (result > 0)
                {
                    reg.msg = "已申请用户退款";
                    reg.code = 200;
                }
            }
            else
            {
                reg.msg = "缺少参数";
                reg.code = 300;
            }
            return Json(reg);
        }

        /// <summary>
        ///余额申请退款审核
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult UserAmountBackApplyAudit(int uid, bool through)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (uid > 0)
            {
                var model = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(v => v.StudentUid == uid).First();
                if (through)
                {
                    if (model.ApplyBackAmount > model.Amount)
                    {
                        reg.msg = "学生目前余额小于申请退费金额,无法通过!";
                        reg.code = 200;
                        return Json(reg);
                    }
                    model.IsBackAmount = 2;
                    model.Amount = model.Amount- model.ApplyBackAmount;
                }
                else
                    model.IsBackAmount = 3;
                var result = _currencyService.DbAccess().Updateable<C_Contrac_User>(model).ExecuteCommand();
                if (result > 0)
                {
                    reg.msg = through?"审核成功":"驳回成功";
                    reg.code = 200;
                }

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
