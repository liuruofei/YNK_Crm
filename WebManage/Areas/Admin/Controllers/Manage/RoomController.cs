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
    public class RoomController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public RoomController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "C-156";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("C-156", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("C-156", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<C_RoomModel> pageModel = new PageList<C_RoomModel>();
            var list = _currencyService.DbAccess().Queryable<C_Room, C_Campus>((c, ca) => new Object[] { JoinType.Left, c.CampusId ==ca.CampusId })
                .Where(c=>c.CampusId==Convert.ToInt32(campusId))
                .WhereIF(!string.IsNullOrEmpty(title), (c, ca) => c.RoomName.Contains(title)).OrderBy(c => c.Sort).Select<C_RoomModel>((c, ca) => new C_RoomModel
            {
                Id=c.Id,
                CampusId = c.CampusId,
                CampusName = ca.CampusName,
                RoomName = c.RoomName,
                Sort = c.Sort
            }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("C-156", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("C-156", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("C-156", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_Room>().Where(f => f.Id == ID).First();
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
        [UsersRoleAuthFilter("C-156", "Add,Edit")]
        public IActionResult SaveInfo(C_Room vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
                if (string.IsNullOrEmpty(vmodel.RoomName))
                    return Json(new { code = 0, msg = "教室名称不能为空" });
                if (vmodel.Id > 0)
                {
                    C_Room caps = _currencyService.DbAccess().Queryable<C_Room>().Where(f => f.Id == vmodel.Id).First();
                    caps.RoomName = vmodel.RoomName;
                    caps.CampusId = vmodel.CampusId;
                    var result = _currencyService.DbAccess().Updateable<C_Room>(caps).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
                else
                {
                    var result = _currencyService.DbAccess().Insertable<C_Room>(vmodel).ExecuteCommand();
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


        public IActionResult SetSort(C_Room vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.Id > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<C_Room>().SetColumns(n => new C_Room { Sort = vmodel.Sort }).Where(n => n.Id == vmodel.Id).ExecuteCommand();
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
        /// 删除教室
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("C-156", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var result = _currencyService.DbAccess().Deleteable<C_Room>().Where(p => p.Id == Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }
}
