using ADT.Models;
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
    [Authorize]
    public class NewsController : BaseController
    {
        private ICurrencyService _currencyService;
        private RedisConfig redisConfig;
        public NewsController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "A-104";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("A-104", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("A-104", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            PageList<YNK_News> pageModel = new PageList<YNK_News>();
            var list = _currencyService.DbAccess().Queryable<YNK_News>().WhereIF(!string.IsNullOrEmpty(title), n => n.Title.Contains(title)).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("A-104", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("A-104", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("A-104", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<YNK_News>().Where(f => f.Id == ID).First();
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
        [UsersRoleAuthFilter("A-104", "Add,Edit")]
        public IActionResult SaveInfo(YNK_News vmodel)
        {
            int code = 0;
            string msg = string.Empty;
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.Title))
                    return Json(new { code = 0, msg = "案例标题不能为空" });
                if (vmodel.Id > 0)
                {
                    YNK_News banner = _currencyService.DbAccess().Queryable<YNK_News>().Where(f => f.Id == vmodel.Id).First();
                    banner.ImgUrl = vmodel.ImgUrl;
                    banner.MobileImgUrl = vmodel.MobileImgUrl;
                    banner.Title = vmodel.Title;
                    banner.Subtitle = vmodel.Subtitle;
                    banner.Content = vmodel.Content;
                    banner.CreateTime = DateTime.Now;
                    var result = _currencyService.DbAccess().Updateable<YNK_News>(banner).ExecuteCommand();
                    if (result > 0)
                    {
                        code = 200;
                        msg = "更新成功";
                    }
                }
                else
                {
                    vmodel.CreateTime = DateTime.Now;
                    var result = _currencyService.DbAccess().Insertable<YNK_News>(vmodel).ExecuteCommand();
                    if (result > 0)
                    {
                        code = 200;
                        msg = "保存成功";
                    }

                }
                if (RedisLock.KeyExists("YNK_News4", redisConfig.RedisCon))
                {
                    RedisLock.KeyDelete("YNK_News4", redisConfig.RedisCon);
                    List<YNK_News> list = _currencyService.DbAccess().Queryable<YNK_News>().OrderBy(n => n.CreateTime, SqlSugar.OrderByType.Desc).ToPageList(1, 4);
                    RedisLock.SetStringKey<List<YNK_News>>("YNK_News4", list, 3600000, redisConfig.RedisCon);
                }
                if (RedisLock.KeyExists("YNK_News50", redisConfig.RedisCon))
                {
                    RedisLock.KeyDelete("YNK_News50", redisConfig.RedisCon);
                    List<YNK_News> list = _currencyService.DbAccess().Queryable<YNK_News>().OrderBy(n => n.CreateTime, SqlSugar.OrderByType.Desc).ToPageList(1, 50);
                    RedisLock.SetStringKey<List<YNK_News>>("YNK_News50", list, 3600000, redisConfig.RedisCon);
                }
            }
            else
            {
                return Json(new { code = code, msg = "缺少参数" });
            }
            return Json(new { code = code, msg = msg });
        }


        /// <summary>
        /// 删除banner
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("A-104", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var result = _currencyService.DbAccess().Deleteable<YNK_News>().Where(p => p.Id == Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }
}
