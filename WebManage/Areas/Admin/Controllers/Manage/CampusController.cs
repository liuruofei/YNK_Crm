using ADT.Models;
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
    public class CampusController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public CampusController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "C-154";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("C-154", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("C-154", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            PageList<C_CampusModel> pageModel = new PageList<C_CampusModel>();
            var list = _currencyService.DbAccess().Queryable<C_Campus>().OrderBy(c => c.CreateTime).Select<C_CampusModel>().ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("C-154", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("C-154", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("C-154", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_Campus>().Where(f => f.CampusId == ID).First();
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("C-154", "Add,Edit")]
        public IActionResult SaveInfo(C_Campus vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.CampusName))
                    return Json(new { code = 0, msg = "科目名称不能为空" });
                if (vmodel.CampusId > 0)
                {
                    C_Campus caps = _currencyService.DbAccess().Queryable<C_Campus>().Where(f => f.CampusId == vmodel.CampusId).First();
                    caps.CampusName = vmodel.CampusName;
                    caps.UpdateUid = userId;
                    var result = _currencyService.DbAccess().Updateable<C_Campus>(caps).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
                else
                {
                    vmodel.CreateTime = DateTime.Now;
                    vmodel.CreateUid = userId;
                    var result = _currencyService.DbAccess().Insertable<C_Campus>(vmodel).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "保存成功";
                    }
                }
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }

        /// <summary>
        /// 获取校长
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryMaster() {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where((u, ur, r) => r.Role_Name.Contains("校长")).Select((u, ur, r) => u).ToList();
            return Json(rsg);
        }

        public IActionResult SetSort(C_Campus vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.CampusId > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<C_Campus>().SetColumns(n => new C_Campus { Sort = vmodel.Sort }).Where(n => n.CampusId == vmodel.CampusId).ExecuteCommand();
                    if (result > 0)
                    {
                        return Json(new { code = 200, msg = "更新成功" });
                    }
                    else
                    {
                        return Json(new { code = code, msg = "更新失败" });
                    }
                }
            }
            catch (Exception er)
            {

            }
            return Json(new { code = code, msg = "缺少参数" });
        }


        /// <summary>
        /// 删除校区
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("C-154", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var result = _currencyService.DbAccess().Deleteable<C_Campus>().Where(p => p.CampusId == Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }
}
