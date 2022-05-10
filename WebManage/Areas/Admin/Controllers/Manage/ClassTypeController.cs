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
    public class ClassTypeController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public ClassTypeController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
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
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<C_ClassTypeModel> pageModel = new PageList<C_ClassTypeModel>();
            var list = _currencyService.DbAccess().Queryable<C_ClassType,C_Campus>((c, ca) => new Object[] { JoinType.Inner,c.CampusId ==ca.CampusId}).Where(c=>c.Status<1&&c.CampusId==Convert.ToInt32(campusId))
                .WhereIF(!string.IsNullOrEmpty(title),c => c.TypeName.Contains(title)).OrderBy(c => c.CreateTime,OrderByType.Desc).Select<C_ClassTypeModel>((c,ca)=>new C_ClassTypeModel
            { 
              CampusId=c.CampusId,CampusName=ca.CampusName,TypeName=c.TypeName,CreateTime=c.CreateTime,CreateUid=c.CreateUid,UpdateTime=c.UpdateTime,UpdateUid=c.UpdateUid,Id=c.Id
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
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_ClassType>().Where(f => f.Id == ID).First();
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
        public IActionResult SaveInfo(C_ClassType vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.TypeName))
                    return Json(new { code = 0, msg = "类型名称不能为空" });
                if (vmodel.CampusId<1)
                    return Json(new { code = 0, msg = "请选择校区" });
                if (vmodel.Id > 0)
                {
                    C_ClassType type = _currencyService.DbAccess().Queryable<C_ClassType>().Where(f => f.Id == vmodel.Id).First();
                    type.TypeName = vmodel.TypeName;
                    type.CampusId = vmodel.CampusId;
                    type.UpdateTime = DateTime.Now;
                    type.UpdateUid = userId;
                    var result = _currencyService.DbAccess().Updateable<C_ClassType>(type).ExecuteCommand();
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
                    vmodel.Status =0;
                    var result = _currencyService.DbAccess().Insertable<C_ClassType>(vmodel).ExecuteCommand();
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
        /// 获取校区列表
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryCampus()
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<C_Campus>().Where(n => n.Status < 1).ToList();
            return Json(rsg);
        }




        /// <summary>
        /// 删除科目
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("C-158", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var anyClas = _currencyService.DbAccess().Queryable<C_Class>().Where(n => n.TypeId == Id).First();
            if (anyClas == null)
            {
                var result = _currencyService.DbAccess().Deleteable<C_ClassType>().Where(p => p.Id == Id).ExecuteCommand();
                if (result > 0)
                    return Json(new { code = 200, msg = "删除成功" });
                else
                    return Json(new { code = 0, msg = "删除失败" });
            }
            else {
                return Json(new { code = 0, msg = "该类型有班级，无法删除" });
            }
        }
    }
}
