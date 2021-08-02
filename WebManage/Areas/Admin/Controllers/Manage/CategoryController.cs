using ADT.Models;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class CategoryController : BaseController
    {
        private ICurrencyService _currencyService;
        public CategoryController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "A-108";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("A-108", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("A-108", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            PageList<YNK_Category> pageModel = new PageList<YNK_Category>();
            var list = _currencyService.DbAccess().Queryable<YNK_Category>().WhereIF(!string.IsNullOrEmpty(title), n => n.Title.Contains(title)).OrderBy(n=>n.Sort).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("A-108", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("A-108", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("A-108", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<YNK_Category>().Where(f => f.Id == ID).First();
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
        [UsersRoleAuthFilter("A-108", "Add,Edit")]
        public IActionResult SaveInfo(YNK_Category vmodel)
        {
            int code = 0;
            string msg = string.Empty;
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.CategoryName))
                    return Json(new { code = 0, msg = "菜单标题不能为空" });
                if (vmodel.Id > 0)
                {
                    YNK_Category banner = _currencyService.DbAccess().Queryable<YNK_Category>().Where(f => f.Id == vmodel.Id).First();
                    banner.CategoryName = vmodel.CategoryName;
                    banner.ImgUrl = vmodel.ImgUrl;
                    banner.Title = vmodel.Title;
                    banner.Subtitle = vmodel.Subtitle;
                    banner.Remarks = vmodel.Remarks;
                    banner.KeyName = vmodel.KeyName;
                    var result = _currencyService.DbAccess().Updateable<YNK_Category>(banner).ExecuteCommand();
                    if (result > 0)
                    {
                        code = 200;
                        msg = "更新成功";
                    }
                }
                else
                {
                    vmodel.ParenId = 0;
                    var result = _currencyService.DbAccess().Insertable<YNK_Category>(vmodel).ExecuteCommand();
                    if (result > 0)
                    {
                        code = 200;
                        msg = "保存成功";
                    }

                }
            }
            else
            {
                return Json(new { code = code, msg = "缺少参数" });
            }
            return Json(new { code = code, msg = msg });
        }


        public IActionResult SetSort(YNK_Category vmodel){
            int code = 0;
            try
            {
                if (vmodel.Id > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<YNK_Category>().SetColumns(n=>new YNK_Category{Sort=vmodel.Sort}).Where(n=>n.Id==vmodel.Id).ExecuteCommand();
                    if (result > 0)
                    {
                        return Json(new { code = 200, msg = "更新成功" });
                    }
                    else {
                        return Json(new { code = code, msg = "更新失败" });
                    }
                }
            }
            catch (Exception er) { 
            
            }
            return Json(new { code = code, msg = "缺少参数" });
        }


        /// <summary>
        /// 删除banner
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("A-108", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var result = _currencyService.DbAccess().Deleteable<YNK_Category>().Where(p => p.Id == Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }

}
