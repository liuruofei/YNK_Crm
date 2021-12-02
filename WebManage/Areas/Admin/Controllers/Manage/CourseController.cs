using ADT.Models;
using ADT.Models.ResModel;
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
    public class CourseController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public CourseController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "C-152";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("C-152", FunctionEnum.Have)]
        public IActionResult Index() 
        {
           
            return View();
        }

        [UsersRoleAuthFilter("C-152", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            PageList<C_Course> pageModel = new PageList<C_Course>();
            var list = _currencyService.DbAccess().Queryable<C_Course>().WhereIF(!string.IsNullOrEmpty(title), n => n.CourseName.Contains(title)).OrderBy(n=>n.Sort).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("C-152", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("C-152", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("C-152", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_Course>().Where(f => f.CourseId == ID).First();
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
        [UsersRoleAuthFilter("C-152", "Add,Edit")]
        public IActionResult SaveInfo(C_Course vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.CourseName))
                    return Json(new { code = 0, msg = "课程名称不能为空" });
                if (vmodel.CourseId > 0)
                {
                    C_Course course = _currencyService.DbAccess().Queryable<C_Course>().Where(f => f.CourseId == vmodel.CourseId).First();
                    course.CourseName = vmodel.CourseName;
                    course.Description = vmodel.Description;
                    var result = _currencyService.DbAccess().Updateable<C_Course>(course).ExecuteCommand();
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
                    var result = _currencyService.DbAccess().Insertable<C_Course>(vmodel).ExecuteCommand();
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

        public IActionResult SetSort(C_Course vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.CourseId > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<C_Course>().SetColumns(n => new C_Course { Sort = vmodel.Sort }).Where(n => n.CourseId == vmodel.CourseId).ExecuteCommand();
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
        /// 删除课程
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("C-152", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var clueCouseModel=_currencyService.DbAccess().Queryable<C_ClueUser_Subject>().Where(item => item.SubjectId == Id).First();
            if (clueCouseModel != null)
            {
                var result = _currencyService.DbAccess().Updateable<C_Course>().SetColumns(p => new C_Course { Status = 1 }).Where(p => p.CourseId == Id).ExecuteCommand();
                if (result > 0)
                    return Json(new { code = 200, msg = "禁用成功" });
                else
                    return Json(new { code = 0, msg = "禁用失败" });
            }
            else {
                var result = _currencyService.DbAccess().Deleteable<C_Course>().Where(p => p.CourseId == Id).ExecuteCommand();
                if (result > 0)
                    return Json(new { code = 200, msg = "删除成功" });
                else
                    return Json(new { code = 0, msg = "删除失败" });
            }
        }
    }
}
