using ADT.Common;
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
    public class TeacherController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public TeacherController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "C-158";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("C-158", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("C-158", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            PageList<C_TeacherModel> pageModel = new PageList<C_TeacherModel>();
            var list = _currencyService.DbAccess().Queryable<sys_user>().WhereIF(!string.IsNullOrEmpty(title),t=>t.User_Name.Contains(title)).OrderBy(t => t.User_CreateTime).Select<C_TeacherModel>(t=>new C_TeacherModel
            { 
            User_Name=t.User_Name,User_LoginName=t.User_LoginName,CreateTime=t.User_CreateTime.Value,TeacherUid=t.User_ID
            }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("C-158", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("C-158", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("C-158", "Add,Edit")]
        public IActionResult Find(string ID)
        {
            dynamic list;
            if (string.IsNullOrEmpty(ID))
            {
                list = _currencyService.DbAccess().Queryable<sys_user>().Where(f => f.User_ID == ID).First();
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
        [UsersRoleAuthFilter("C-158", "Add,Edit")]
        public IActionResult SaveInfo(sys_user vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.User_Name))
                    return Json(new { code = 0, msg = "老师名称不能为空" });
                if (string.IsNullOrEmpty(vmodel.User_LoginName))
                    return Json(new { code = 0, msg = "登录账号不能为空" });
                if (string.IsNullOrEmpty(vmodel.User_Pwd))
                    return Json(new { code = 0, msg = "登录密码不能为空" });
                if (!string.IsNullOrEmpty(vmodel.User_ID))
                {
                    sys_user ta = _currencyService.DbAccess().Queryable<sys_user>().Where(f => f.User_ID == vmodel.User_ID).First();
                    ta.User_Name = vmodel.User_Name;
                    ta.User_LoginName = vmodel.User_LoginName;
                    if(!ta.User_Pwd.Equals(vmodel.User_Pwd))
                        ta.User_Pwd= Tools.MD5Encryption(ta.User_Pwd);
                    ta.User_CreateTime = DateTime.Now;
                    var result = _currencyService.DbAccess().Updateable<sys_user>(ta).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
                else
                {
                    vmodel.User_CreateTime = DateTime.Now;
                    vmodel.User_Pwd= Tools.MD5Encryption(vmodel.User_Pwd);
                    var result = _currencyService.DbAccess().Insertable<sys_user>(vmodel).ExecuteCommand();
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
        /// 删除老师
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("C-158", FunctionEnum.Delete)]
        public IActionResult Delete(string Id)
        {
            var result = _currencyService.DbAccess().Deleteable<sys_user>().Where(p => p.User_ID == Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }
}
