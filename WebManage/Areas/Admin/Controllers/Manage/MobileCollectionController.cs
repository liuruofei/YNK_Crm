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
using WebManage.Models.Req;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class MobileCollectionController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public MobileCollectionController(ICurrencyService currencyService, IC_ContracService contrac)
        {
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "L-151";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("L-151", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetDataSource(CollectionQuery query)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            var ccUse = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where(u => u.User_ID == userId).Select<sys_role>().First();
            PageList<C_CollectionModel> pageModel = new PageList<C_CollectionModel>();
            var list = _currencyService.DbAccess().Queryable<C_Collection, C_Contrac_User, C_Campus, sys_user>((c, cu, ca, sy) => new Object[] { JoinType.Inner, c.StudentUid == cu.StudentUid, JoinType.Left, cu.CampusId == ca.CampusId, JoinType.Left, c.CreateUid == sy.User_ID }).WhereIF(!string.IsNullOrEmpty(query.title), (c, cu) => cu.Student_Name.Contains(query.title))
                .Where(c => c.CampusId == Convert.ToInt32(campusId))
                .WhereIF(query.startTime != null, (c, cu) => c.Collection_Time >= query.startTime).WhereIF(query.endTime != null, (c, cu) => c.Collection_Time <= query.endTime)
                //.WhereIF(ccUse != null && ccUse.Role_Name == "顾问", (c, cu, ca, sy)=>c.CreateUid==userId).AddParameters(new { CCuid = userId })
                .OrderBy(c => c.Collection_Time, OrderByType.Desc)
                .Select<C_CollectionModel>((c, cu, ca, sy) => new C_CollectionModel
                {
                    CampusId = c.CampusId,
                    CampusName = ca.CampusName,
                    StudentName = c.StudentName,
                    Amount = c.Amount,
                    AddedAmount = c.AddedAmount,
                    DeductAmount = c.DeductAmount,
                    StudentUid = c.StudentUid,
                    PayStatus = c.PayStatus,
                    RelationShip_Contras = c.RelationShip_Contras,
                    Collection_Time = c.Collection_Time,
                    PayMothed = c.PayMothed,
                    PayImg = c.PayImg,
                    Id = c.Id,
                    Registration_Time = c.Registration_Time,
                    User_Name = sy.User_Name
                }).ToPageList(query.page, query.limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }
    }
}
