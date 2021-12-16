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
    public class ClueUserRecordController : BaseController
    {
        private ICurrencyService _currencyService;
        public ClueUserRecordController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "A-103";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("A-103", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }



        [UsersRoleAuthFilter("A-103", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            string roleName = "";
            var role = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID }).Where((u, ur, r) => u.User_ID == userId && r.Role_Name == "顾问").Select<sys_role>((u, ur, r) => r).First();
            if (role != null)
            {
                roleName = role.Role_Name;
            }
            PageList<C_ClueRecordModel> pageModel = new PageList<C_ClueRecordModel>();
            var list = _currencyService.DbAccess().Queryable<C_ClueUser_Record,C_ClueUser,sys_user>((record,clue,u)=>new object[] { JoinType.Inner, record.ClueId == clue.ClueId,JoinType.Left,record.CC_Uid==u.User_ID})
                .Where(record=>record.CampusId==Convert.ToInt32(campusId))
                .WhereIF(!string.IsNullOrEmpty(title),(record, clue)=>clue.Student_Name.Contains(title))
                .WhereIF(!string.IsNullOrEmpty(roleName), (record, clue) => clue.CC_Uid== userId)
                .OrderBy(record => record.CreateTime)
                .Select<C_ClueRecordModel>((record, clue,u)=>new C_ClueRecordModel{Id=record.Id,CCUserName=u.User_Name, Student_Name=clue.Student_Name, Follow_Content=record.Follow_Content, Follow_Plan=record.Follow_Plan,
                    Follow_Date=record.Follow_Date,Is_Visit=record.Is_Visit,Visit_Date=record.Visit_Date,CreateTime=record.CreateTime}).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("A-103", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("A-103", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("A-103", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_ClueUser_Record>().Where(f => f.Id == ID).First();
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }


        /// <summary>
        /// 删除banner
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("A-103", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var result = _currencyService.DbAccess().Deleteable<C_ClueUser_Record>().Where(p => p.Id == Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }

}
