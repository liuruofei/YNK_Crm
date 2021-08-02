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
    public class MateriaController : BaseController
    {
        private ICurrencyService _currencyService;
        public MateriaController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "A-106";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("A-106", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("A-106", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            PageList<YNK_Materia> pageModel = new PageList<YNK_Materia>();
            var list = _currencyService.DbAccess().Queryable<YNK_Materia>().WhereIF(!string.IsNullOrEmpty(title), n => n.MateriaName.Contains(title)).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("A-106", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("A-106", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("A-106", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<YNK_Materia>().Where(f => f.MaterialId == ID).First();
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
        [UsersRoleAuthFilter("A-106", "Add,Edit")]
        public IActionResult SaveInfo(YNK_Materia vmodel)
        {
            int code = 0;
            string msg = string.Empty;
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.MateriaName))
                    return Json(new { code = 0, msg = "资料不能为空" });
                if (vmodel.MaterialId > 0)
                {
                    YNK_Materia banner = _currencyService.DbAccess().Queryable<YNK_Materia>().Where(f => f.MaterialId == vmodel.MaterialId).First();
                    banner.MateriaName = vmodel.MateriaName;
                    banner.IsEnble = vmodel.IsEnble;
                    var result = _currencyService.DbAccess().Updateable<YNK_Materia>(banner).ExecuteCommand();
                    if (result > 0)
                    {
                        code = 200;
                        msg = "更新成功";
                    }
                }
                else
                {
                    var result = _currencyService.DbAccess().Insertable<YNK_Materia>(vmodel).ExecuteCommand();
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


        /// <summary>
        /// 删除banner
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("A-106", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            YNK_Region region = _currencyService.DbAccess().Queryable<YNK_Region>().Where(n => n.MaterialId == Id).First();
            if (region != null) {
                return Json(new { code = 200, msg = "该资料已被用户领取，无法删除" });
            }
            var result = _currencyService.DbAccess().Deleteable<YNK_Materia>().Where(p => p.MaterialId == Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }
}
