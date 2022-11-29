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
    public class RangeTimeController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public RangeTimeController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "C-157";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("C-157", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("C-157", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<C_RangeTimeModel> pageModel = new PageList<C_RangeTimeModel>();
            var list = _currencyService.DbAccess().Queryable<C_Range_Time, C_Campus>((c, ca) => new Object[] { JoinType.Left, c.CampusId ==ca.CampusId })
                .Where(c=>c.CampusId==Convert.ToInt32(campusId))
                .WhereIF(!string.IsNullOrEmpty(title), (c, ca) => c.TimeName.Contains(title)).OrderBy(c => c.StartTime).Select<C_RangeTimeModel>((c, ca) => new C_RangeTimeModel
            {
                Id=c.Id,
                CampusId = c.CampusId,
                CampusName = ca.CampusName,
                TimeName =c.TimeName,
                StartTime = c.StartTime,
                EndTime=c.EndTime,
                CreateTime=c.CreateTime,
                CreateUid=c.CreateUid
            }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("C-157", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("C-157", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("C-157", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_Range_Time>().Where(f => f.Id == ID).First();
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        /// <summary>
        /// 添加时间区间
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("C-157", "Add,Edit")]
        public IActionResult SaveInfo(C_Range_Time vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.TimeName))
                    return Json(new { code = 0, msg = "时间段名称不能为空" });
                if (string.IsNullOrEmpty(vmodel.StartTime))
                    return Json(new { code = 0, msg = "开始时间不能为空" });
                if (string.IsNullOrEmpty(vmodel.EndTime))
                    return Json(new { code = 0, msg = "截止时间不能为空" });
                if (vmodel.Id > 0)
                {
                    C_Range_Time caps = _currencyService.DbAccess().Queryable<C_Range_Time>().Where(f => f.Id == vmodel.Id).First();
                    caps.TimeName = vmodel.TimeName;
                    caps.CampusId = vmodel.CampusId;
                    caps.StartTime = vmodel.StartTime;
                    caps.EndTime = vmodel.EndTime;
                    caps.CampusId = vmodel.CampusId;
                    var result = _currencyService.DbAccess().Updateable<C_Range_Time>(caps).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
                else
                {
                    var result = _currencyService.DbAccess().Insertable<C_Range_Time>(vmodel).ExecuteCommand();
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


        public IActionResult SetSort(C_Range_Time vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.Id > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<C_Range_Time>().SetColumns(n => new C_Range_Time { Sort = vmodel.Sort }).Where(n => n.Id == vmodel.Id).ExecuteCommand();
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
        /// 删除时间区间
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("C-157", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var hasRang = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(ay => ay.RangTimeId == Id).First();
            if (hasRang != null)
            {
                return Json(new { code = 0, msg = "删除失败,该区间有课程已使用" });
            }
            else {
                var result = _currencyService.DbAccess().Deleteable<C_Range_Time>().Where(p => p.Id == Id).ExecuteCommand();
                if (result > 0)
                    return Json(new { code = 200, msg = "删除成功" });
                else
                    return Json(new { code = 0, msg = "删除失败" });
            }
        }
    }
}
