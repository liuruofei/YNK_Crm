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
    public class ClueTaskController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public ClueTaskController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "A-105";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("A-105", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("A-105", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<C_ClueTaskModel> pageModel = new PageList<C_ClueTaskModel>();
            var list = _currencyService.DbAccess().Queryable<C_ClueTask,C_ClueUser, sys_user>((c,clue, cc) => new Object[] { JoinType.Left, c.ClueId == clue.ClueId,JoinType.Left, c.CreateUid == cc.User_ID })
                .Where(c => c.CampusId == Convert.ToInt32(campusId)&&c.TaskStutas<1)
                .WhereIF(!string.IsNullOrEmpty(title), (c, clue, cc) => c.TaskContent.Contains(title)).OrderBy(c => c.ImportLevel).OrderBy(c => c.TaskDate).Select<C_ClueTaskModel>((c, clue, cc) => new C_ClueTaskModel
                {
                    Id = c.Id,
                    CampusId = c.CampusId,
                    TaskContent = c.TaskContent,
                    TaskDate = c.TaskDate,
                    ImportLevel=c.ImportLevel,
                    Student_Name=clue.Student_Name,
                    CCUserName = cc.User_Name,
                    TaskStutas=c.TaskStutas
                }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        public IActionResult Add(int clueId)
        {
            ViewBag.ID = 0;
            ViewBag.ClueId = clueId;
            if(clueId>0)
            ViewBag.ClueName = _currencyService.DbAccess().Queryable<C_ClueUser>().Where(f => f.ClueId == clueId).First().Student_Name;
            return View();
        }

        [UsersRoleAuthFilter("A-105", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            ViewBag.ClueId = 0;
            var clueTask=_currencyService.DbAccess().Queryable<C_ClueTask>().Where(f => f.Id == ID).First();
            if (clueTask != null) {
                ViewBag.ClueName = _currencyService.DbAccess().Queryable<C_ClueUser>().Where(f => f.ClueId == clueTask.ClueId).First().Student_Name;
            }
            return View("Add");
        }

        [UsersRoleAuthFilter("A-105", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_ClueTask>().Where(f => f.Id == ID).First();
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        /// <summary>
        /// 添加待办任务
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("A-105", "Add,Edit")]
        public IActionResult SaveInfo(C_ClueTask vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
                if (string.IsNullOrEmpty(vmodel.TaskContent))
                    return Json(new { code = 0, msg = "待办任务不能为空" });
                if (vmodel.Id > 0)
                {
                    C_ClueTask caps = _currencyService.DbAccess().Queryable<C_ClueTask>().Where(f => f.Id == vmodel.Id).First();
                    caps.TaskContent = vmodel.TaskContent;
                    caps.TaskDate = vmodel.TaskDate;
                    caps.ImportLevel = vmodel.ImportLevel;
                    caps.TaskStutas = vmodel.TaskStutas;
                    var result = _currencyService.DbAccess().Updateable<C_ClueTask>(caps).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
                else
                {
                    vmodel.CreateUid = userId;
                    vmodel.CC_Uid = userId;
                    vmodel.CreateTime = DateTime.Now;
                    vmodel.CampusId =int.Parse(campusId);
                    var result = _currencyService.DbAccess().Insertable<C_ClueTask>(vmodel).ExecuteCommand();
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


        public IActionResult SetSort(C_ClueTask vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.Id > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<C_ClueTask>().SetColumns(n => new C_ClueTask { ImportLevel = vmodel.ImportLevel }).Where(n => n.Id == vmodel.Id).ExecuteCommand();
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
        /// 删除待办任务
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("A-105", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var result = _currencyService.DbAccess().Deleteable<C_ClueTask>().Where(p => p.Id == Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }
}
