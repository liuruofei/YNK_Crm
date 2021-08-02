using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ADT.Models;
using Newtonsoft.Json;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers.Sys
{
    [Authorize]
    public class FunctionController : BaseController
    {
        private ISys_FunctionService _Sys_FunctionService;
        public FunctionController(ISys_FunctionService sys_FunctionService)
        {
            _Sys_FunctionService = sys_FunctionService;
        }
        protected override void Init()
        {
            this.MenuID = "Z-120";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("Z-120", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("Z-120", FunctionEnum.Have)]
        public IActionResult GetDataSource(int page = 1, int limit = 10)
        {
            var list = _Sys_FunctionService.GetPageList(null, page, limit, "Function_Num asc,Function_CreateTime asc");
            return Json(list);
        }
        [UsersRoleAuthFilter("Z-120", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = "";
            return View();
        }

        [UsersRoleAuthFilter("Z-120", FunctionEnum.Edit)]
        public IActionResult Edit(string ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("Z-120", "Add,Edit")]
        public IActionResult Find(string ID)
        {
            dynamic list;
            if (!string.IsNullOrEmpty(ID))
            {
                list = _Sys_FunctionService.FindByClause(p => p.Function_ID == ID);
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
        [UsersRoleAuthFilter("Z-120", FunctionEnum.Add)]
        public IActionResult AddSave(sys_function vmodel)
        {
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.Function_ID))
                {
                    if (string.IsNullOrEmpty(vmodel.Function_Name))
                        return Json(new { code = 100, msg = "请输入功能名" });
                    vmodel.Function_ID = Guid.NewGuid().ToString();
                    vmodel.Function_CreateTime = DateTime.Now;
                    _Sys_FunctionService.Insert(vmodel);
                }
                else
                {
                    return Json(new { code = 100, msg = "无效参数" });
                }
            }
            else
            {
                return Json(new { code = 100, msg = "无效参数" });
            }
            return Json(new { code = 200, msg = "操作成功" });
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("Z-120", FunctionEnum.Edit)]
        public IActionResult EditSave(sys_function vmodel)
        {
            if (!string.IsNullOrEmpty(vmodel.Function_ID))
            {
                if (string.IsNullOrEmpty(vmodel.Function_Name))
                    return Json(new { code = 100, msg = "请输入功能名" });
                var model = _Sys_FunctionService.FindByClause(p => p.Function_ID == vmodel.Function_ID);
                if (model != null)
                {
                    _Sys_FunctionService.UpdateWhere(model, p => new { p.Function_Num, p.Function_Name, p.Function_ByName }, p => p.Function_ID == vmodel.Function_ID);
                }
                else
                {
                    return Json(new { code = 100, msg = "没有该功能" });
                }
            }
            else
            {
                return Json(new { code = 100, msg = "参数无效" });
            }
            return Json(new { code = 200, msg = "操作成功" });
        }

        /// <summary>
        /// 根据ID集合删除信息
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("Z-120", FunctionEnum.Delete)]
        public IActionResult Delete(string id)
        {
            var idList = JsonConvert.DeserializeObject<List<string>>(id);
            var result = _Sys_FunctionService.Delete(p => idList.Contains(p.Function_ID));
            return Json(new { code = 200, msg = "删除成功" });
        }
    }
}
