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
    public class MobileStudentController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;

        public MobileStudentController(ICurrencyService currencyService, IC_ContracService contrac)
        {
            _currencyService = currencyService;
            _contrac = contrac;
        }

        protected override void Init()
        {
            this.MenuID = "H-156";
        }

        [UsersRoleAuthFilter("H-156", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 保存签约用户
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveContracUser(C_Contrac_User vmodel)
        {
            if (string.IsNullOrEmpty(vmodel.Student_No)) {
                C_SequenceCode sequence = new C_SequenceCode();
                long sequenceNoLast = _currencyService.DbAccess().Queryable<C_SequenceCode>().Where(c => c.type == 2).Max<long>(c => c.SequenceNo);//最新学号
                if (sequenceNoLast > 0)
                {
                    sequence.SequenceNo = sequenceNoLast + 1;
                }
                else
                {
                    sequence.SequenceNo = 10000;
                }
                sequence.type = 2;
                sequence.Remarks = "学生编号";
                _currencyService.DbAccess().Insertable<C_SequenceCode>(sequence).ExecuteCommand();
                vmodel.Student_No= "CC" + sequence.SequenceNo;
            }
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            vmodel.CreateUid = userId;
            vmodel.CampusId = 1;
            reg = _contrac.SaveContracUser(vmodel);
            return Json(reg);
        }


        [HttpPost]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            var ccUse = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where(u => u.User_ID == userId).Select<sys_role>().First();
            PageList<C_ContracUserModel> pageModel = new PageList<C_ContracUserModel>();
            var list = _currencyService.DbAccess().Queryable("C_Contrac_User", "contracU").AddJoinInfo("Sys_User", "cc", "contracU.CC_Uid=cc.User_ID", SqlSugar.JoinType.Left)
            .AddJoinInfo("Sys_User", "cr", "contracU.CR_Uid=cr.User_ID", SqlSugar.JoinType.Left).
            AddJoinInfo("C_Campus", "camp", "contracU.CampusId=camp.CampusId", SqlSugar.JoinType.Left)
            .Where("contracU.CampusId=" + campusId)
            .WhereIF(!string.IsNullOrEmpty(title), " charindex(@title,contracU.Student_Name)>0 or charindex(@title,contracU.Student_Phone)>0").AddParameters(new { title = title })
            .WhereIF(ccUse != null && ccUse.Role_Name == "顾问", "contracU.CC_Uid=@CCuid").AddParameters(new { CCuid = userId })
            .Select<C_ContracUserModel>(@"contracU.*,cc.User_Name as CC_UserName,cr.User_Name as CR_UserName,
                camp.CampusName").OrderBy("contracU.CreateTime desc").ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }



        [HttpPost]
        public IActionResult SetContrc() {
            ResResult reg = new ResResult();
            C_SequenceCode sequence2 = new C_SequenceCode();
            long contrcNoLast = _currencyService.DbAccess().Queryable<C_SequenceCode>().Where(c => c.type == 1).Max<long>(c => c.SequenceNo);//最新合同号
            if (contrcNoLast < 1)
                sequence2.SequenceNo = 20000;
            else
                sequence2.SequenceNo = contrcNoLast + 1;
            sequence2.type = 1;
            sequence2.Remarks ="合同编号";
            var result=_currencyService.DbAccess().Insertable<C_SequenceCode>(sequence2).ExecuteCommand();
            if (result > 0) {
                reg.data = "SH"+sequence2.SequenceNo;
            }
            return Json(reg);
        }
    }
}
