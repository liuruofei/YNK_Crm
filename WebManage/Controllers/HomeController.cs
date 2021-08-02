using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ADT.Models;
using ADT.Service.IService;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebManage.Models.Req;
using WebManage.Models.Res;

namespace WebManage.Controllers
{
    public class HomeController : Controller
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public HomeController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            _currencyService = currencyService;
            redisConfig = _redisConfig.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id,string type)
        {
            ViewBag.Id =id;
            ViewBag.type = type;
            return View();
        }
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IActionResult FindDetail(int Id,string type) {
            ArcleModel vmodel = new ArcleModel();
            if (Id > 0 && type == "first")
            {
                vmodel = _currencyService.DbAccess().Queryable<YNK_Banner>().Where(n => n.Id == Id).Select<ArcleModel>(n => new ArcleModel
                {
                    Id = n.Id,
                    Content = n.Remarks,
                    ImgUrl = n.BannerImage,
                    Title = n.Title,
                    Subtitle = n.Subtitle
                }).First();
            }
            else if (Id > 0 && type == "course")
            {
                vmodel = _currencyService.DbAccess().Queryable<YNK_Course>().Where(n => n.Id == Id).Select<ArcleModel>(n => new ArcleModel
                {
                    Id = n.Id,
                    Content = n.Content,
                    ImgUrl = n.ImgUrl,
                    Title = n.CourseName,
                    Subtitle = n.CourseTitle
                }).First();
            }
            else if (Id > 0 && type =="case")
            {
                vmodel = _currencyService.DbAccess().Queryable<YNK_Case>().Where(n => n.Id == Id&&n.Type==1).Select<ArcleModel>(n => new ArcleModel
                {
                    Id = n.Id,
                    Content = n.Content,
                    ImgUrl = n.ImgUrl,
                    Title = n.Title,
                    Subtitle = n.Subtitle
                }).First();
            }
            else if (Id > 0 && type == "yashi")
            {
                vmodel = _currencyService.DbAccess().Queryable<YNK_Case>().Where(n => n.Id == Id && n.Type ==2).Select<ArcleModel>(n => new ArcleModel
                {
                    Id = n.Id,
                    Content = n.Content,
                    ImgUrl = n.ImgUrl,
                    Title = n.Title,
                    Subtitle = n.Subtitle
                }).First();
            }
            else if (Id > 0 && type == "item")
            {
                vmodel = _currencyService.DbAccess().Queryable<YNK_Case>().Where(n => n.Id == Id && n.Type == 3).Select<ArcleModel>(n => new ArcleModel
                {
                    Id = n.Id,
                    Content = n.Content,
                    ImgUrl = n.ImgUrl,
                    Title = n.Title,
                    Subtitle = n.Subtitle
                }).First();
            }
            else if (Id > 0 && type =="news")
            {
                vmodel = _currencyService.DbAccess().Queryable<YNK_News>().Where(n => n.Id == Id).Select<ArcleModel>(n => new ArcleModel
                {
                    Id = n.Id,
                    Content = n.Content,
                    ImgUrl = n.ImgUrl,
                    Title = n.Title,
                    Subtitle = n.Subtitle
                }).First();
            }
            else {
                vmodel = _currencyService.DbAccess().Queryable<YNK_Category>().Where(n => n.KeyName== "about").Select<ArcleModel>(n => new ArcleModel
                {
                    Id = n.Id,
                    Content = n.Remarks,
                    ImgUrl = n.ImgUrl,
                    Title = n.Title,
                    Subtitle = n.Subtitle
                }).First();
            }
            return Json(new { code = 200, msg = "获取成功", data = vmodel });
        }

        /// <summary>
        /// 获取牛津详情
        /// </summary>
        /// <returns></returns>
        public IActionResult GetNiujinDetail() {
            ArcleModel vmodel = new ArcleModel();
            vmodel = _currencyService.DbAccess().Queryable<YNK_Category>().Where(n => n.KeyName == "niujin").Select<ArcleModel>(n => new ArcleModel
            {
                Id = n.Id,
                Content = n.Remarks,
                ImgUrl = n.ImgUrl,
                Title = n.Title,
                Subtitle = n.Subtitle
            }).First();
            return Json(new { code = 200, msg = "获取成功", data = vmodel });
        }

        /// <summary>
        /// 获取Alevel详情
        /// </summary>
        /// <returns></returns>
        public IActionResult GetAlevelDetail()
        {
            ArcleModel vmodel = new ArcleModel();
            vmodel = _currencyService.DbAccess().Queryable<YNK_Category>().Where(n => n.KeyName == "alevel").Select<ArcleModel>(n => new ArcleModel
            {
                Id = n.Id,
                Content = n.Remarks,
                ImgUrl = n.ImgUrl,
                Title = n.Title,
                Subtitle = n.Subtitle
            }).First();
            return Json(new { code = 200, msg = "获取成功", data = vmodel });
        }
        /// <summary>
        /// 获取栏目雅思内容
        /// </summary>
        /// <returns></returns>
        public IActionResult GetYashiDetail()
        {
            ArcleModel vmodel = new ArcleModel();
            vmodel = _currencyService.DbAccess().Queryable<YNK_Category>().Where(n => n.KeyName == "yashi").Select<ArcleModel>(n => new ArcleModel
            {
                Id = n.Id,
                Content = n.Remarks,
                ImgUrl = n.ImgUrl,
                Title = n.Title,
                Subtitle = n.Subtitle
            }).First();
            return Json(new { code = 200, msg = "获取成功", data = vmodel });
        }
        


        /// <summary>
        /// 课程页面
        /// </summary>
        /// <returns></returns>
        public IActionResult CourseList() {
            return View();
        }

        /// <summary>
        /// 案例页面
        /// </summary>
        /// <returns></returns>
        public IActionResult CaseList() {
            return View();
        }

        /// <summary>
        /// 雅思培训
        /// </summary>
        /// <returns></returns>
        public IActionResult YasiList()
        {
            return View();
        }

        /// <summary>
        /// Alevel页面
        /// </summary>
        /// <returns></returns>
        public IActionResult ALevel(string  type) {
            ViewBag.type = type;
            return View();
        }

        /// <summary>
        /// G5/牛津页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Niujin(string type)
        {
            ViewBag.type = type;
            return View();
        }

        public IActionResult About() {
            return View();
        }

        /// <summary>
        ///获取首页菜单
        /// </summary>
        /// <returns></returns>
        public IActionResult GetFirstMuen() {
            int code = 200;
            var list = _currencyService.DbAccess().Queryable<YNK_Category>().OrderBy(n=>n.Sort).ToList();
            return Json(new { code = code, msg = "获取成功",data=list});
        }
        /// <summary>
        /// 绑定资料
        /// </summary>
        /// <returns></returns>
        public IActionResult BindMateria()
        {
            int code = 200;
            var list = _currencyService.DbAccess().Queryable<YNK_Materia>().OrderBy(n => n.Sort).ToList();
            return Json(new { code = code, msg = "获取成功", data = list });
        }

        /// <summary>
        /// 绑定课程
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BindCourse(BaseSearch vmodel) {
            int code = 200;
            List<YNK_Course> list = null;
            if (RedisLock.KeyExists("YNK_Course" + vmodel.pagesize, redisConfig.RedisCon))
            {
                list = RedisLock.GetStringKey<List<YNK_Course>>("YNK_Course" + vmodel.pagesize, redisConfig.RedisCon);
            }
            else
            {
                list=_currencyService.DbAccess().Queryable<YNK_Course>().OrderBy(n => n.CreateTime, SqlSugar.OrderByType.Desc).ToPageList(vmodel.page, vmodel.pagesize);
                RedisLock.SetStringKey<List<YNK_Course>>("YNK_Course" + vmodel.pagesize, list, 3600000, redisConfig.RedisCon);
            }
            return Json(new { code = code, msg = "获取成功", data = list });
        }

        /// <summary>
        /// 绑定案例
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BindCase(BaseSearch vmodel) {
            int code = 200;
            List<YNK_Case> list = null;
            if (RedisLock.KeyExists("YNK_Case" + vmodel.pagesize, redisConfig.RedisCon))
            {
                list = RedisLock.GetStringKey<List<YNK_Case>>("YNK_Case" + vmodel.pagesize, redisConfig.RedisCon);
            }
            else
            {
                list = _currencyService.DbAccess().Queryable<YNK_Case>().Where(n => n.Type == 1).OrderBy(n => n.CreateTime, SqlSugar.OrderByType.Desc).ToPageList(vmodel.page, vmodel.pagesize);
                RedisLock.SetStringKey<List<YNK_Case>>("YNK_Case" + vmodel.pagesize, list, 3600000, redisConfig.RedisCon);
            }
            return Json(new { code = code, msg = "获取成功", data = list });
        }


        /// <summary>
        /// 绑定雅思培训
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BindYase(BaseSearch vmodel)
        {
            int code = 200;
            var list = _currencyService.DbAccess().Queryable<YNK_Case>().Where(n => n.Type ==2).OrderBy(n => n.CreateTime, SqlSugar.OrderByType.Desc).ToPageList(vmodel.page, vmodel.pagesize);
            return Json(new { code = code, msg = "获取成功", data = list });
        }

        /// <summary>
        /// 绑定Banner图
        /// </summary>
        /// <returns></returns>
        public IActionResult BindBanner()
        {
            int code = 200;
            var list = _currencyService.DbAccess().Queryable<YNK_Banner>().OrderBy(n => n.Sort).ToList();
            return Json(new { code = code, msg = "获取成功", data = list });
        }

        /// <summary>
        /// 绑定新闻
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BindNews(BaseSearch vmodel) {
            int code = 200;
            List<YNK_News> list = null;
            if (RedisLock.KeyExists("YNK_News"+vmodel.pagesize, redisConfig.RedisCon))
            {
                list = RedisLock.GetStringKey<List<YNK_News>>("YNK_News"+vmodel.pagesize, redisConfig.RedisCon);
            }
            else
            {
                list = _currencyService.DbAccess().Queryable<YNK_News>().OrderBy(n => n.CreateTime, SqlSugar.OrderByType.Desc).ToPageList(vmodel.page, vmodel.pagesize);
                RedisLock.SetStringKey<List<YNK_News>>("YNK_News" + vmodel.pagesize, list, 3600000, redisConfig.RedisCon);
            }
            return Json(new { code = code, msg = "获取成功", data = list });
        }




        /// <summary>
        /// 领取资料
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public async Task<IActionResult> Region(YNK_Region vmodel) { 
            int code = 200;
            try
            {
                vmodel.CreateTime = DateTime.Now;
                var realModel = await _currencyService.DbAccess().Queryable<YNK_Region>().Where(n => n.Mobile.Equals(vmodel.Mobile.Trim())).FirstAsync();
                if (realModel != null) {
                    return Json(new { code = code, msg = "你已经领取过！" });
                }
                var result = _currencyService.DbAccess().Insertable<YNK_Region>(vmodel).ExecuteCommand();
                if (result > 0)
                {
                    return Json(new { code = code, msg = "领取成功" });
                }
                else {
                    return Json(new { code = 300, msg = "领取失败" });
                }
            }
            catch (Exception er)
            {
                return Json(new { code = 300, msg = "程序异常"});
            }
        }
    }
}
